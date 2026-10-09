package com.gliffy.stacklands

import android.graphics.BitmapFactory
import android.os.Bundle
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.compose.foundation.Canvas
import androidx.compose.foundation.background
import androidx.compose.foundation.gestures.awaitEachGesture
import androidx.compose.foundation.gestures.awaitFirstDown
import androidx.compose.foundation.gestures.detectTapGestures
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material3.Text
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.geometry.CornerRadius
import androidx.compose.ui.geometry.Offset
import androidx.compose.ui.geometry.Size
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.graphics.ImageBitmap
import androidx.compose.ui.graphics.nativeCanvas
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

val labelPaint = android.graphics.Paint().apply {
    color = android.graphics.Color.WHITE
    textSize = 26f
    isFakeBoldText = true
    isAntiAlias = true
}

// card name text: black on the colored header band (real game style)
val namePaint = android.graphics.Paint().apply {
    color = android.graphics.Color.BLACK
    textSize = 22f
    isFakeBoldText = true
    isAntiAlias = true
}

// light variant for dark header bands
val lightNamePaint = android.graphics.Paint().apply {
    color = android.graphics.Color.WHITE
    textSize = 22f
    isFakeBoldText = true
    isAntiAlias = true
}

// black cost badge number: cream text
val badgePaint = android.graphics.Paint().apply {
    color = android.graphics.Color.parseColor("#FFF9E3")
    textSize = 24f
    isFakeBoldText = true
    isAntiAlias = true
    textAlign = android.graphics.Paint.Align.CENTER
}

fun darken(c: Color, f: Float): Color =
    Color(android.graphics.Color.rgb((c.red*255*f).toInt(), (c.green*255*f).toInt(), (c.blue*255*f).toInt()))

@Composable
fun MainMenu(onPlay: () -> Unit) {
    Box(Modifier.fillMaxSize().background(Color(0xFFEFE8D8)), contentAlignment = Alignment.Center) {
        Column(horizontalAlignment = Alignment.CenterHorizontally) {
            Text("Stacklands", fontSize = 42.sp, fontWeight = FontWeight.Bold, color = Color(0xFF3A3630))
            Spacer(Modifier.height(8.dp))
            Text("Android port (unofficial)", fontSize = 14.sp, color = Color(0xFF7A7466))
            Spacer(Modifier.height(40.dp))
            Box(
                Modifier
                    .background(Color(0xFF4CAF50), RoundedCornerShape(12.dp))
                    .pointerInput(Unit) { detectTapGestures { onPlay() } }
                    .padding(horizontal = 48.dp, vertical = 16.dp),
            ) { Text("Play", fontSize = 22.sp, fontWeight = FontWeight.Bold, color = Color.White) }
            Spacer(Modifier.height(16.dp))
            Text("Open the starter pack to begin", fontSize = 13.sp, color = Color(0xFF7A7466))
        }
    }
}

@Composable
fun App() {
    val context = LocalContext.current
    val sprites = remember { SpriteCache() }
    LaunchedEffect(Unit) { GameData.load(context.assets) }
    val engine = remember { Engine() }
    LaunchedEffect(Unit) {
        while (true) { engine.tick(); delay(250) }
    }
    val selected by engine.selected
    val message by engine.message
    val inMenu by engine.inMenu
    val dragPos by remember { engine.dragPos }
    engine.tickCount   // read in composition so the canvas redraws when per-card state changes

    // ---- main menu ----
    if (inMenu) {
        MainMenu(onPlay = {
            if (GameData.cards.isNotEmpty()) engine.start()
            engine.inMenu.value = false
        })
        return
    }

    val cardW = 92.dp
    val cardH = 122.dp

    Box(Modifier.fillMaxSize().background(Color(0xFFB3E5B2))) {
        Row(Modifier.fillMaxWidth().padding(12.dp), verticalAlignment = Alignment.CenterVertically) {
            Text("Stacklands", fontSize = 20.sp, fontWeight = FontWeight.Bold, color = Color(0xFF2A332A))
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
                    val w = size.width.toFloat(); val h = size.height.toFloat()
                    val cw = cardW.toPx(); val ch = cardH.toPx()
                    val shopWpx = 72.dp.toPx()
                    fun hitTest(pos: Offset): PlacedCard? {
                        for (root in engine.stacks.reversed()) {
                            for (child in root.children.reversed()) {
                                val cp = nodePos(root, child, w, h, cw, ch)
                                if (pos.x in cp.x..(cp.x + cw * 0.8f) && pos.y in cp.y..(cp.y + ch * 0.8f)) return child
                            }
                            val rp = Offset(root.x * w, root.y * h)
                            if (pos.x in rp.x..(rp.x + cw) && pos.y in rp.y..(rp.y + ch)) return root
                        }
                        return null
                    }
                    fun hitBox(pos: Offset): Engine.ShopBox? = engine.boxes.firstOrNull {
                        val bp = Offset(it.x * w, it.y * h)
                        pos.x in bp.x..(bp.x + shopWpx) && pos.y in bp.y..(bp.y + ch * 0.7f)
                    }
                    fun hitSell(pos: Offset): Boolean {
                        val bp = Offset(engine.sellBoxX * w, engine.sellBoxY * h)
                        return pos.x in bp.x..(bp.x + shopWpx) && pos.y in bp.y..(bp.y + ch * 0.7f)
                    }
                    awaitEachGesture {
                        val down = awaitFirstDown(requireUnconsumed = false)
                        var dragNode: PlacedCard? = null
                        var moved = false
                        var pos = down.position
                        // wait a moment: if it lifts fast = tap, if it moves = drag
                        while (true) {
                            val event = awaitPointerEvent()
                            val change = event.changes.firstOrNull() ?: break
                            if (!change.pressed) {
                                // released
                                if (dragNode != null) {
                                    val box = hitBox(pos); val sell = hitSell(pos); val target = hitTest(pos)
                                    when {
                                        box != null && engine.dropOnBox(box, dragNode) -> {}
                                        sell -> engine.sellCard(dragNode, engine.sellBoxX, engine.sellBoxY)
                                        target != null && target !== dragNode && engine.canDrop(target, dragNode) -> engine.dropOn(target, dragNode)
                                        else -> {
                                            dragNode.x = (pos.x / w).coerceIn(0.02f, 0.92f)
                                            dragNode.y = (pos.y / h).coerceIn(0.34f, 0.86f)
                                            engine.stacks.add(dragNode)
                                        }
                                    }
                                    engine.selected.value = null
                                } else if (!moved) {
                                    // tap
                                    val node = hitTest(pos)
                                    if (node != null) {
                                        if (GameData.boosters.containsKey(node.id)) engine.openBooster(node)
                                        else engine.flipCard(node)
                                    }
                                }
                                break
                            }
                            pos = change.position
                            if ((pos - down.position).getDistance() > 30f) {
                                if (!moved && dragNode == null) {
                                    moved = true
                                    dragNode = hitTest(down.position)
                                    if (dragNode != null) {
                                        engine.detach(dragNode)
                                        engine.selected.value = dragNode
                                        engine.dragPos.value = pos
                                    }
                                }
                                if (dragNode != null) engine.dragPos.value = pos
                            }
                        }
                    }
                }
        ) {
            // sell box — near-black like the real shop row
            val shopW = 72.dp
            val sellPos = Offset(engine.sellBoxX * size.width, engine.sellBoxY * size.height)
            val sellW = shopW.toPx(); val sellH = cardH.toPx() * 0.7f
            drawRoundRect(Color(0xFF1A1A1A), sellPos, Size(sellW, sellH), CornerRadius(8f, 8f))
            drawContext.canvas.nativeCanvas.drawText("Sell", sellPos.x + sellW * 0.22f, sellPos.y + sellH * 0.62f, labelPaint)
            // shop boxes
            for (box in engine.boxes) {
                val cw = shopW.toPx(); val ch = cardH.toPx() * 0.7f
                val pos = Offset(box.x * size.width, box.y * size.height)
                drawRoundRect(Color(0xFF1A1A1A), pos, Size(cw, ch), CornerRadius(8f, 8f))
                drawContext.canvas.nativeCanvas.drawText(box.label, pos.x + 8f, pos.y + ch * 0.42f, labelPaint)
                drawContext.canvas.nativeCanvas.drawText("cost ${box.cost}${if (box.stored > 0) "  (${box.stored})" else ""}",
                    pos.x + 8f, pos.y + ch * 0.78f, labelPaint)
            }
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
    // booster packs on the board: near-black box with white label (real game style)
    if (GameData.boosters.containsKey(node.id)) {
        drawRoundRect(Color(0xFF1A1A1A), pos, Size(w, h), CornerRadius(10f, 10f))
        drawContext.canvas.nativeCanvas.drawText("PACK", pos.x + w * 0.28f, pos.y + h * 0.55f, labelPaint)
        return
    }
    val pal = PALETTES[node.def.behavior] ?: PALETTES["other"]!!
    drawRoundRect(pal.first, pos, Size(w, h), CornerRadius(10f, 10f))
    if (!node.faceUp) {
        // face-down: card back, tap to flip
        drawRoundRect(Color(0xFFE8DCC0), pos, Size(w, h), CornerRadius(10f, 10f))
        drawRoundRect(Color(0xFF1A1A1A), pos, Size(w, h), CornerRadius(10f, 10f), style = Stroke(width = 4f))
        return
    }
    // name header band (darker shade of card color, black text) — real game style
    val bandH = h * 0.16f
    drawRoundRect(darken(pal.second, 0.92f), pos, Size(w, bandH), CornerRadius(10f, 10f))
    drawRect(darken(pal.second, 0.92f), Offset(pos.x, pos.y + bandH / 2), Size(w, bandH / 2))
    // dark bands (e.g. charcoal structure cards) need light text
    val bandLum = 0.299*pal.second.red + 0.587*pal.second.green + 0.114*pal.second.blue
    val np = if (bandLum < 0.45f) lightNamePaint else namePaint
    drawContext.canvas.nativeCanvas.drawText(node.def.name, pos.x + 8f, pos.y + bandH * 0.78f, np)
    drawSprite(sprites.get(assets, node.def.icon), pos, w, h)
    // cost/value badge: black blob bottom-left
    if (node.def.value > 0) {
        drawContext.canvas.nativeCanvas.drawCircle(pos.x + 20f, pos.y + h - 20f, 16f, android.graphics.Paint().apply {
            color = android.graphics.Color.BLACK; isAntiAlias = true
        })
        drawContext.canvas.nativeCanvas.drawText(node.def.value.toString(), pos.x + 20f, pos.y + h - 12f, badgePaint)
    }
    // thick black outline on top of everything
    drawRoundRect(Color(0xFF1A1A1A), pos, Size(w, h), CornerRadius(10f, 10f), style = Stroke(width = 4f))
    if (selected) drawRoundRect(Color(0xFFFFC107), pos, Size(w, h), CornerRadius(10f, 10f), style = Stroke(width = 5f))
    // in a conflict: red outline
    if (node.conflictId >= 0)
        drawRoundRect(Color(0xFFE53935), pos, Size(w, h), CornerRadius(10f, 10f), style = Stroke(width = 5f))
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
