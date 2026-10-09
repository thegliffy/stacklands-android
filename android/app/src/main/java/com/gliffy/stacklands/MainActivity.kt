package com.gliffy.stacklands

import android.graphics.BitmapFactory
import android.os.Bundle
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.compose.foundation.Canvas
import androidx.compose.foundation.background
import androidx.compose.foundation.gestures.detectDragGestures
import androidx.compose.foundation.gestures.detectTapGestures
import androidx.compose.foundation.layout.*
import androidx.compose.material3.Text
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.geometry.CornerRadius
import androidx.compose.ui.geometry.Offset
import androidx.compose.ui.geometry.Size
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.graphics.ImageBitmap
import androidx.compose.ui.graphics.asImageBitmap
import androidx.compose.ui.graphics.drawscope.DrawScope
import androidx.compose.ui.graphics.drawscope.Stroke
import androidx.compose.ui.input.pointer.pointerInput
import androidx.compose.ui.platform.LocalConfiguration
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import kotlinx.coroutines.delay

class MainActivity : ComponentActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContent { App() }
    }
}

// palette: (card bg, border) per behavior — from ColorManager
val PALETTES = mapOf(
    "blueprint" to (Color(0xFF657EBB) to Color(0xFF7F94C5)),
    "structure" to (Color(0xFF393831) to Color(0xFF5D5C55)),
    "resource" to (Color(0xFF647083) to Color(0xFF9CA4B0)),
    "food" to (Color(0xFFEF994C) to Color(0xFDFAB3)),
    "worker" to (Color(0xFFFFF9E3) to Color(0xFFFAECB2)),
    "animal" to (Color(0xFF95663F) to Color(0xFFAD8461)),
    "enemy" to (Color(0xDF5959) to Color(0xF37878)),
    "equipable" to (Color(0xFF636363) to Color(0xFF424141)),
    "location" to (Color(0xFFBA64BD) to Color(0xFFCED0CE)),
    "fish" to (Color(0xB0E283) to Color(0xFF6B9543)),
    "rumor" to (Color(0xFF657EBB) to Color(0xFF7F94C5)),
    "display" to (Color(0xFFE8B94A) to Color(0xFFD4A017)),
    "other" to (Color(0xFFFFF9E3) to Color(0xFFFAECB2)),
)

class SpriteCache {
    val map = mutableMapOf<String, ImageBitmap>()
    fun get(assets: android.content.res.AssetManager, name: String): ImageBitmap? {
        if (name.isEmpty()) return null
        map[name]?.let { return it }
        return try {
            val bmp = assets.open("sprites/$name.png").use { BitmapFactory.decodeStream(it) }
            bmp.asImageBitmap().also { map[name] = it }
        } catch (e: Exception) { null }
    }
}

@Composable
fun App() {
    val context = LocalContext.current
    val sprites = remember { SpriteCache() }
    LaunchedEffect(Unit) { GameData.load(context.assets) }
    val engine = remember { Engine() }
    LaunchedEffect(Unit) { if (GameData.cards.isNotEmpty() && engine.stacks.isEmpty()) engine.start() }
    LaunchedEffect(Unit) {
        while (true) { engine.tick(); delay(250) }
    }
    val selected by engine.selected
    val message by engine.message
    val dragPos by remember { engine.dragPos }

    val cardW = 92.dp
    val cardH = 122.dp

    Box(Modifier.fillMaxSize().background(Color(0xFFEFE8D8))) {
        Row(Modifier.fillMaxWidth().padding(12.dp), verticalAlignment = Alignment.CenterVertically) {
            Text("Stacklands", fontSize = 20.sp, fontWeight = FontWeight.Bold, color = Color(0xFF3A3630))
            Spacer(Modifier.weight(1f))
            // moon clock: tap to cycle moon length (Short 90s / Normal 120s / Long 200s)
            val moonFrac = (engine.monthTimer / engine.monthSeconds).coerceIn(0.0, 1.0)
            Text("Moon ${engine.month} ${"█".repeat((moonFrac * 8).toInt())}${"░".repeat(8 - (moonFrac * 8).toInt())} ${engine.monthSeconds.toInt()}s ▾",
                fontSize = 13.sp, color = Color(0xFF7A7466),
                modifier = Modifier.pointerInput(Unit) {
                    detectTapGestures { engine.cycleMoonLength() }
                })
            Spacer(Modifier.weight(1f))
            Text("cards: ${engine.stacks.size}", fontSize = 13.sp, color = Color(0xFF7A7466))
        }
        message?.let { m ->
            LaunchedEffect(m) { delay(2500); engine.message.value = null }
            Text(
                m,
                fontSize = 15.sp,
                fontWeight = FontWeight.SemiBold,
                color = Color(0xFF2E7D32),
                modifier = Modifier.align(Alignment.TopCenter).padding(top = 48.dp),
            )
        }

        Canvas(
            Modifier
                .fillMaxSize()
                .pointerInput(engine, GameData.cards.size) {
                    var dragNode: PlacedCard? = null
                    var dragPos = Offset.Zero
                    fun hitTest(pos: Offset): PlacedCard? {
                        val w = size.width.toFloat(); val h = size.height.toFloat()
                        for (root in engine.stacks.reversed()) {
                            for (child in root.children.reversed()) {
                                val cp = nodePos(root, child, w, h, cardW.toPx(), cardH.toPx())
                                if (pos.x in cp.x..(cp.x + cardW.toPx() * 0.8f) && pos.y in cp.y..(cp.y + cardH.toPx() * 0.8f)) return child
                            }
                            val rp = Offset(root.x * size.width, root.y * size.height)
                            if (pos.x in rp.x..(rp.x + cardW.toPx()) && pos.y in rp.y..(rp.y + cardH.toPx())) return root
                        }
                        return null
                    }
                    detectDragGestures(
                        onDragStart = { pos ->
                            val node = hitTest(pos)
                            if (node != null) {
                                if (GameData.boosters.containsKey(node.id)) {
                                    engine.openBooster(node)
                                } else {
                                    engine.detach(node)
                                    dragNode = node
                                    engine.selected.value = node
                                    engine.dragPos.value = pos
                                }
                            }
                        },
                        onDrag = { change, _ -> dragPos = change.position; engine.dragPos.value = change.position },
                        onDragEnd = {
                            val node = dragNode
                            if (node != null) {
                                val target = hitTest(dragPos)
                                if (target != null && target !== node && engine.canDrop(target, node)) {
                                    engine.dropOn(target, node)
                                } else {
                                    node.x = (dragPos.x / size.width).coerceIn(0.02f, 0.88f)
                                    node.y = (dragPos.y / size.height).coerceIn(0.10f, 0.86f)
                                    engine.stacks.add(node)
                                }
                            }
                            dragNode = null
                            engine.selected.value = null
                        },
                        onDragCancel = {
                            dragNode?.let { engine.stacks.add(it) }
                            dragNode = null
                            engine.selected.value = null
                        },
                    )
                }
        ) {
            for (root in engine.stacks) {
                drawCard(root, null, sprites, context.assets, cardW.toPx(), cardH.toPx(), false)
                root.children.forEachIndexed { i, child ->
                    drawCard(root, child, sprites, context.assets, cardW.toPx(), cardH.toPx(), false, i)
                }
                if (root.timerEnd > 0) {
                    val total = (root.def.recipes.getOrNull(root.recipeIndex)?.time?.times(1000L)
                        ?: root.def.special?.time?.times(1000L)
                        ?: (root.def.harvestTime * 1000L)).coerceAtLeast(1L)
                    val frac = ((root.timerEnd - System.currentTimeMillis()).coerceAtLeast(0)).toFloat() / total
                    drawRect(
                        Color(0xFF4CAF50),
                        Offset(root.x * size.width, root.y * size.height + cardH.toPx() + 4f),
                        Size(cardW.toPx() * (1f - frac.coerceIn(0f, 1f)), 6f),
                    )
                }
            }
            selected?.let { node ->
                val cw = cardW.toPx(); val ch = cardH.toPx()
                val p = dragPos
                drawRoundRect(Color(0x33000000), p, Size(cw, ch), CornerRadius(10f, 10f))
                drawSprite(sprites.get(context.assets, node.def.icon), p, cw, ch)
            }
        }
    }
}

fun nodePos(root: PlacedCard, child: PlacedCard?, w: Float, h: Float, cw: Float, ch: Float): Offset {
    if (child == null) return Offset(root.x * w, root.y * h)
    val i = root.children.indexOf(child)
    return Offset(root.x * w + i * cw * 0.12f, root.y * h - i * ch * 0.10f)
}

fun DrawScope.drawCard(
    root: PlacedCard,
    child: PlacedCard?,
    sprites: SpriteCache,
    assets: android.content.res.AssetManager,
    cw: Float,
    ch: Float,
    selected: Boolean,
    childIndex: Int = 0,
) {
    val node = child ?: root
    val scale = if (child == null) 1f else (0.88f - childIndex * 0.02f).coerceAtLeast(0.6f)
    val w = cw * scale; val h = ch * scale
    val pos = nodePos(root, child, size.width, size.height, cw, ch)
    val pal = PALETTES[node.def.behavior] ?: PALETTES["other"]!!
    drawRoundRect(pal.first, pos, Size(w, h), CornerRadius(10f, 10f))
    drawRoundRect(pal.second, pos, Size(w, h), CornerRadius(10f, 10f), style = Stroke(width = 3f))
    if (selected) drawRoundRect(Color(0xFFFFC107), pos, Size(w, h), CornerRadius(10f, 10f), style = Stroke(width = 5f))
    // in a conflict: red outline
    if (node.conflictId >= 0)
        drawRoundRect(Color(0xFFE53935), pos, Size(w, h), CornerRadius(10f, 10f), style = Stroke(width = 5f))
    drawSprite(sprites.get(assets, node.def.icon), pos, w, h)
    // HP bar for damaged combatables
    val cb = node.def.combat
    if (cb != null && node.hp >= 0 && node.hp < cb.maxHealth) {
        val frac = (node.hp.toFloat() / cb.maxHealth).coerceIn(0f, 1f)
        drawRect(Color(0x66000000), Offset(pos.x + 6f, pos.y + h - 12f), Size(w - 12f, 6f))
        drawRect(Color(0xFFE53935), Offset(pos.x + 6f, pos.y + h - 12f), Size((w - 12f) * frac, 6f))
    }
    // status dots (poison green, bleed red, sick yellow, stun white, frenzy orange, invuln blue, drunk purple, anxious grey)
    if (node.statuses.isNotEmpty()) {
        val colors = mapOf(
            "poison" to Color(0xFF4CAF50), "bleeding" to Color(0xFFD32F2F), "sick" to Color(0xFFFFEB3B),
            "stunned" to Color(0xFFFFFFFF), "frenzy" to Color(0xFFFF9800), "invulnerable" to Color(0xFF2196F3),
            "drunk" to Color(0xFF9C27B0), "anxious" to Color(0xFF9E9E9E),
        )
        node.statuses.keys.toList().forEachIndexed { i, s ->
            val c = colors[s] ?: return@forEachIndexed
            drawCircle(c, radius = 7f, center = Offset(pos.x + 14f + i * 18f, pos.y + 12f))
        }
    }
}

fun DrawScope.drawSprite(bmp: ImageBitmap?, pos: Offset, w: Float, h: Float) {
    bmp ?: return
    val s = minOf(w * 0.72f / bmp.width, h * 0.62f / bmp.height)
    val dw = (bmp.width * s).toInt().coerceAtLeast(1); val dh = (bmp.height * s).toInt().coerceAtLeast(1)
    drawImage(
        image = bmp,
        srcOffset = androidx.compose.ui.unit.IntOffset.Zero,
        srcSize = androidx.compose.ui.unit.IntSize(bmp.width, bmp.height),
        dstOffset = androidx.compose.ui.unit.IntOffset((pos.x + (w - dw) / 2).toInt(), (pos.y + (h - dh) / 2 - h * 0.04f).toInt()),
        dstSize = androidx.compose.ui.unit.IntSize(dw, dh),
    )
}
