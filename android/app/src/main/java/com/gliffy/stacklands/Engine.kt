package com.gliffy.stacklands

import androidx.compose.runtime.mutableStateListOf
import androidx.compose.runtime.mutableStateOf
import androidx.compose.ui.geometry.Offset
import kotlin.math.max
import kotlin.random.Random

data class PlacedCard(
    val id: String,
    var x: Float,
    var y: Float,
    val children: MutableList<PlacedCard> = mutableListOf(),
    var timerEnd: Long = 0L,          // epoch ms when action completes
    var timerAction: String = "",     // "harvest","recipe","grow","breed","cook","sell","open"
    var recipeIndex: Int = -1,
    var amountLeft: Int = 0,          // for harvestables
    var isFoil: Boolean = false,
) {
    val def: CardDef get() = GameData.cards[id] ?: GameData.fallback
}

class Engine {
    val stacks = mutableStateListOf<PlacedCard>()          // root cards
    val selected = mutableStateOf<PlacedCard?>(null)       // card being dragged
    val dragPos = mutableStateOf(Offset.Zero)
    val message = mutableStateOf<String?>(null)
    var now = System.currentTimeMillis()
    val rng = Random(System.nanoTime())
    var cardsOpened = 0

    val humanIds: Set<String> by lazy {
        GameData.cards.values.filter { it.behavior == "worker" }.map { it.id }.toSet()
    }
    val breedableIds: Set<String> by lazy {
        GameData.cards.values.filter { it.behavior == "worker" && it.id != "kid" }.map { it.id }.toSet()
    }
    val animalIds: Set<String> by lazy {
        GameData.cards.values.filter { it.behavior == "animal" }.map { it.id }.toSet()
    }
    val foodIds: Set<String> by lazy {
        GameData.cards.values.filter { it.behavior == "food" }.map { it.id }.toSet()
    }

    fun start() {
        stacks.clear()
        val starter = GameData.boosters["starter"] ?: return
        // starter contents are guaranteed (each entry is a fixed card, not a roll)
        val slots = starter.bags.flatMap { it.chances }
        slots.forEachIndexed { j, ch ->
            stacks.add(PlacedCard(ch.card, 0.08f + (j % 4) * 0.24f, 0.18f + (j / 4) * 0.26f))
        }
        // booster packs on the board (tap to open) — like the in-game shop
        val shop = listOf("basic", "farming", "cooking", "idea", "structures", "equipment")
        shop.forEachIndexed { i, id ->
            if (GameData.boosters.containsKey(id)) {
                stacks.add(PlacedCard(id, 0.12f + (i % 3) * 0.3f, 0.72f + (i / 3) * 0.16f))
            }
        }
    }

    // viewport hit test in normalized coords; cw/ch in px
    fun hitTest(pos: Offset, w: Float, h: Float, cw: Float, ch: Float): PlacedCard? {
        for (root in stacks.reversed()) {
            for (child in root.children.reversed()) {
                val cp = nodePos(root, child, w, h, cw, ch)
                if (pos.x in cp.x..(cp.x + cw * 0.8f) && pos.y in cp.y..(cp.y + ch * 0.8f)) return child
            }
            val rp = Offset(root.x * w, root.y * h)
            if (pos.x in rp.x..(rp.x + cw) && pos.y in rp.y..(rp.y + ch)) return root
        }
        return null
    }

    fun drawFromBag(chances: List<BagEntry>): String? {
        if (chances.isEmpty()) return null
        val total = chances.sumOf { it.chance }
        var r = rng.nextInt(total)
        for (c in chances) {
            if (r < c.chance) return c.card
            r -= c.chance
        }
        return chances.last().card
    }

    fun matchesSpecialId(cardId: String, reqId: String): Boolean {
        if (reqId.contains("|")) return reqId.split("|").any { matchesSpecialId(cardId, it) }
        if (reqId == "any_villager") return cardId in humanIds
        if (reqId == "breedable_villager") return cardId in breedableIds
        return cardId == reqId
    }

    // can `child` be placed on root `root`?
    fun canDrop(root: PlacedCard, child: PlacedCard): Boolean {
        if (root.children.size >= 30) return false
        val rd = root.def; val cd = child.def
        // blueprint accepts its recipe cards
        if (rd.recipes.isNotEmpty()) {
            return rd.recipes.any { r -> r.req.any { matchesSpecialId(cd.id, it) } }
        }
        // harvestable: villagers/workers on top
        if (rd.bag.isNotEmpty()) {
            return cd.behavior == "worker"
        }
        // house: villagers, kids, houses
        if (rd.special?.breed == true) {
            return cd.behavior == "worker" || cd.id == "house"
        }
        // sawmill/composter/brickyard/flourmill etc
        rd.special?.let { sp ->
            if (sp.accept.isNotEmpty()) return cd.id in sp.accept
            if (sp.feed.isNotEmpty()) return cd.id == sp.feed
            if (sp.chest) return true
        }
        // market sells anything
        if (rd.id == "market") return true
        // animal pen: animals + feed
        if (rd.id == "animal_pen" || rd.id == "breeding_pen") return cd.behavior == "animal" || (rd.special?.feed?.let { cd.id == it } == true)
        // worker slots: buildings with WorkerAmount accept workers
        if (rd.workers > 0 && cd.behavior == "worker") return true
        return false
    }

    fun dropOn(root: PlacedCard, child: PlacedCard): Boolean {
        if (!canDrop(root, child)) return false
        root.children.add(child)
        recomputeTimer(root)
        return true
    }

    fun recomputeTimer(root: PlacedCard) {
        root.timerEnd = 0; root.timerAction = ""; root.recipeIndex = -1
        val rd = root.def
        // blueprint subprint exact match
        if (rd.recipes.isNotEmpty()) {
            val stackIds = (listOf(root) + root.children).map { it.id }
            rd.recipes.forEachIndexed { idx, r ->
                if (matchMultiset(stackIds, r.req)) {
                    root.recipeIndex = idx
                    root.timerAction = "recipe"
                    root.timerEnd = now + r.time * 1000L
                }
            }
            if (root.timerEnd > 0) return
        }
        // harvestable: worker on top
        if (rd.bag.isNotEmpty() && root.children.isNotEmpty()) {
            val top = root.children.first()
            if (top.def.behavior == "worker") {
                val left = if (rd.canDeplete) (if (root.amountLeft == 0) rd.amount else root.amountLeft) else Int.MAX_VALUE
                if (left > 0) {
                    root.timerAction = "harvest"
                    root.timerEnd = now + rd.harvestTime * 1000L
                    if (root.amountLeft == 0) root.amountLeft = rd.amount
                }
                return
            }
        }
        // house: kid on top grows
        if (rd.special?.breed == true) {
            val kid = root.children.firstOrNull { it.id == "kid" }
            if (kid != null) { root.timerAction = "grow"; root.timerEnd = now + 120_000L; return }
        }
        // hardcoded production
        rd.special?.let { sp ->
            if (sp.accept.isNotEmpty() && sp.result.isNotEmpty()) {
                val have = root.children.count { it.id in sp.accept }
                if (have >= max(1, sp.need)) {
                    root.timerAction = "recipe"
                    root.recipeIndex = -2 // hardcoded
                    root.timerEnd = now + sp.time * 1000L
                    return
                }
            }
            if (sp.feed.isNotEmpty() && sp.time > 0) {
                // animal pen: animal + feed
                val animal = root.children.firstOrNull { it.id in animalIds }
                val feed = root.children.firstOrNull { it.id == sp.feed }
                if (animal != null && feed != null) {
                    root.timerAction = "recipe"; root.recipeIndex = -2
                    root.timerEnd = now + sp.time * 1000L
                    return
                }
            }
            if (sp.chest && root.children.isNotEmpty()) {
                // storage: nothing happens, cards stay
            }
        }
        if (rd.id == "market" && root.children.isNotEmpty()) {
            root.timerAction = "sell"; root.timerEnd = now + 60_000L
        }
    }

    private fun matchMultiset(stackIds: List<String>, req: List<String>): Boolean {
        val pool = stackIds.toMutableList()
        for (r in req) {
            val it = pool.firstOrNull { matchesSpecialId(it, r) } ?: return false
            pool.remove(it)
        }
        return pool.isEmpty()
    }

    fun tick() {
        now = System.currentTimeMillis()
        for (root in stacks.toList()) tickNode(root)
    }

    private fun tickNode(node: PlacedCard) {
        if (node.timerEnd in 1..now) {
            when (node.timerAction) {
                "recipe" -> completeRecipe(node)
                "harvest" -> completeHarvest(node)
                "grow" -> completeGrow(node)
                "sell" -> completeSell(node)
                "open" -> openBooster(node)
            }
        }
        node.children.forEach { tickNode(it) }
    }

    private fun spawn(id: String, x: Float, y: Float): PlacedCard? {
        val def = GameData.cards[id] ?: return null
        val c = PlacedCard(id, x, y)
        stacks.add(c)
        return c
    }

    private fun completeRecipe(root: PlacedCard) {
        val rd = root.def
        if (root.recipeIndex >= 0) {
            val r = rd.recipes[root.recipeIndex]
            // remove required cards (they're consumed)
            val pool = root.children.toMutableList()
            for (req in r.req) {
                val it = pool.firstOrNull { matchesSpecialId(it.id, req) } ?: continue
                pool.remove(it)
                root.children.remove(it)
            }
            val result = spawn(r.result, root.x, root.y - 0.18f)
            r.extra.forEach { spawn(it, root.x + 0.1f, root.y - 0.18f) }
            if (result != null) message.value = "Created ${result.def.name}!"
        } else {
            // hardcoded special
            val sp = rd.special ?: return
            val consumed = root.children.filter { it.id in sp.accept }.sortedBy { it.id }.take(sp.need)
            consumed.forEach { root.children.remove(it) }
            val result = spawn(sp.result, root.x, root.y - 0.18f)
            if (result != null) message.value = "Created ${result.def.name}!"
        }
        recomputeTimer(root)
    }

    private fun completeHarvest(root: PlacedCard) {
        val rd = root.def
        if (rd.canDeplete) {
            if (root.amountLeft <= 0) root.amountLeft = rd.amount
            root.amountLeft--
        }
        val id = drawFromBag(rd.bag)
        if (id != null) {
            val c = spawn(id, root.x, root.y - 0.18f)
            if (c != null) message.value = "Harvested ${c.def.name}"
        }
        recomputeTimer(root)
    }

    private fun completeGrow(root: PlacedCard) {
        val kid = root.children.firstOrNull { it.id == "kid" } ?: return
        root.children.remove(kid)
        spawn("villager", root.x, root.y - 0.18f)
        message.value = "A kid grew up into a Villager!"
        recomputeTimer(root)
    }

    private fun completeSell(root: PlacedCard) {
        val child = root.children.firstOrNull() ?: return
        root.children.remove(child)
        val value = max(1, child.def.value)
        repeat(value) { spawn("gold", root.x + (rng.nextFloat() - 0.5f) * 0.1f, root.y - 0.18f) }
        message.value = "Sold ${child.def.name}"
        recomputeTimer(root)
    }

    fun openBooster(node: PlacedCard) {
        val pack = GameData.boosters[node.id] ?: return
        stacks.remove(node)
        pack.bags.forEach { bag ->
            repeat(bag.cardsInPack) {
                val id = drawFromBag(bag.chances) ?: return@repeat
                spawn(id, node.x + (rng.nextFloat() - 0.5f) * 0.3f, node.y + (rng.nextFloat() - 0.5f) * 0.3f)
            }
        }
        cardsOpened++
        message.value = "Booster opened!"
    }

    // drag & drop: detach node from its parent
    fun detach(node: PlacedCard) {
        for (s in stacks) {
            if (s === node) { stacks.remove(node); return }
            if (node in s.children) { s.children.remove(node); recomputeTimer(s); return }
        }
    }
}
