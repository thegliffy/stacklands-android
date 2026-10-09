package com.gliffy.stacklands

import androidx.compose.runtime.mutableStateListOf
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.mutableIntStateOf
import androidx.compose.runtime.mutableLongStateOf
import androidx.compose.runtime.getValue
import androidx.compose.runtime.setValue
import androidx.compose.ui.geometry.Offset
import kotlin.math.ceil
import kotlin.math.max
import kotlin.math.min
import kotlin.math.roundToInt
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
    // --- parity state (PC-mechanics port) ---
    var hp: Int = -1,                 // runtime HP for combatables (-1 = not a combatant)
    var age: Int = -1,                // villager age in moons (-1 = doesn't age)
    var foodValue: Int = -1,          // runtime food value (-1 = not food)
    var spoilSeconds: Double = -1.0,  // time alive as food, for spoiling
    var creationMonth: Int = 0,
    var conflictId: Int = -1,
    var equip: MutableList<PlacedCard> = mutableListOf(),   // equipables attached
    val statuses: MutableMap<String, Double> = mutableMapOf(), // statusId -> seconds elapsed
    val statusTick: MutableMap<String, Double> = mutableMapOf(),
    var attackTimer: Double = 0.0,
    var stunTimer: Double = 0.0,
) {
    var poisonTicks by mutableStateOf(0)
    var depletedUntil by mutableLongStateOf(0L)  // harvestable resting until this epoch ms
    var faceUp by mutableStateOf(true)           // cards from packs start face-down; first tap flips
    val def: CardDef get() = GameData.cards[id] ?: GameData.fallback
    val isCombatable: Boolean get() = def.combat != null
    val team: String get() = def.team
}

class ConflictState(val id: Int) {
    val participants = mutableListOf<PlacedCard>()
    var timeSinceLastAttack: Double = 1.0
}

class Engine {
    val stacks = mutableStateListOf<PlacedCard>()          // root cards
    val selected = mutableStateOf<PlacedCard?>(null)       // card being dragged
    val dragPos = mutableStateOf(Offset.Zero)
    val message = mutableStateOf<String?>(null)
    val inMenu = mutableStateOf(true)                      // main menu until Play is tapped
    var now = System.currentTimeMillis()
    val rng = Random(System.nanoTime())
    var cardsOpened = 0

    // --- moon clock (RunOptions.MoonLength: Short=90s, Normal=120s, Long=200s) ---
    val moonLengths = listOf(90.0, 120.0, 200.0)
    var moonLengthIndex by mutableStateOf(1)   // Normal default
    val monthSeconds: Double get() = moonLengths[moonLengthIndex]
    var monthTimer by mutableStateOf(0.0)
    var month by mutableStateOf(0)

    var tickCount by mutableIntStateOf(0)   // bumped every engine tick so the canvas redraws (HP bars, statuses)

    fun cycleMoonLength() {
        moonLengthIndex = (moonLengthIndex + 1) % moonLengths.size
        // rescale the current progress so the moon doesn't jump
        monthTimer = monthTimer.coerceAtMost(monthSeconds)
        message.value = "Moon length: ${moonLengths[moonLengthIndex].toInt()}s"
    }

    val conflicts = mutableStateListOf<ConflictState>()
    private var nextConflictId = 1

    // shop fixtures (BuyBoosterBox / SellBox in the original)
    data class ShopBox(val boosterId: String, val label: String, val cost: Int, var stored: Int, val x: Float, val y: Float)
    val boxes = mutableStateListOf<ShopBox>()
    var sellBoxX = 0.015f
    var sellBoxY = 0.16f

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
        conflicts.clear()
        boxes.clear()
        month = 0; monthTimer = 0.0
        // board starts with just the starter pack on it — tap it to open
        stacks.add(spawnInit("starter", 0.42f, 0.48f))
        // shop row along the top (CreatePackLine): sell box first, then boxes; they accumulate gold, spawn packs when paid
        val shop = listOf("basic" to 3, "idea" to 4, "combat_intro" to 3, "farming" to 10,
            "cooking" to 10, "equipment" to 15, "structures" to 25, "locations" to 20)
        shop.forEachIndexed { i, (id, cost) ->
            if (GameData.boosters.containsKey(id))
                boxes.add(ShopBox(id, id.replace('_', ' ').replaceFirstChar { it.uppercase() }, cost, 0,
                    0.015f + (i + 1) * 0.111f, 0.16f))
        }
    }

    private fun spawnInit(id: String, x: Float, y: Float): PlacedCard {
        val c = PlacedCard(id, x, y)
        initRuntime(c)
        return c
    }

    /** Initialize parity runtime state for a freshly spawned card. */
    private fun initRuntime(c: PlacedCard) {
        val d = c.def
        c.creationMonth = month
        if (d.combat != null) c.hp = d.combat.maxHealth
        if (d.id == "villager") c.age = 2            // BaseVillager prefab Age=2 (Adult)
        if (d.foodValue > 0) { c.foodValue = d.foodValue; c.spoilSeconds = 0.0 }
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
        // combatable stacks: equipables + same-team combatables + opposing team (starts conflict)
        if (rd.combat != null) {
            if (cd.equipType >= 0) return true
            if (cd.combat != null) return true   // same team stacks, opposing team starts conflict
            if (cd.behavior == "food") return true // CanBePlacedOnVillager foods (approximate)
            return false
        }
        // equipable on equipable? no.
        if (cd.equipType >= 0 && rd.combat == null) return false
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
        // equipable onto combatable -> attach as equipment, apply increments
        if (root.isCombatable && child.def.equipType >= 0) {
            root.equip.add(child)
            child.conflictId = root.conflictId
            val es = child.def.equipStats
            if (root.hp >= 0 && es != null && es.maxHealth > 0) root.hp += es.maxHealth   // equipping caps current HP at new max
            recomputeTimer(root)
            return true
        }
        root.children.add(child)
        // opposing combatables in a stack start a conflict
        if (root.isCombatable && child.isCombatable && root.team != child.team) {
            startConflict(root)
        }
        recomputeTimer(root)
        return true
    }

    private fun startConflict(root: PlacedCard) {
        if (root.conflictId >= 0) return
        val c = ConflictState(nextConflictId++)
        conflicts.add(c)
        // all combatables in the stack join
        (listOf(root) + root.children).forEach { if (it.isCombatable) joinConflict(c, it) }
    }

    private fun joinConflict(c: ConflictState, card: PlacedCard) {
        if (card.conflictId >= 0) return
        card.conflictId = c.id
        card.hp = card.def.combat!!.maxHealth
        card.attackTimer = 0.0
        c.participants.add(card)
    }

    private fun conflictOf(card: PlacedCard): ConflictState? =
        conflicts.firstOrNull { it.id == card.conflictId }

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
                    root.timerEnd = now + (r.time * actionTimeMultiplier(root) * 1000L).toLong()
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
                    root.timerEnd = now + (rd.harvestTime * actionTimeMultiplier(root) * 1000L).toLong()
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
                    root.timerEnd = now + (sp.time * actionTimeMultiplier(root) * 1000L).toLong()
                    return
                }
            }
            if (sp.feed.isNotEmpty() && sp.time > 0) {
                // animal pen: animal + feed
                val animal = root.children.firstOrNull { it.id in animalIds }
                val feed = root.children.firstOrNull { it.id == sp.feed }
                if (animal != null && feed != null) {
                    root.timerAction = "recipe"; root.recipeIndex = -2
                    root.timerEnd = now + (sp.time * actionTimeMultiplier(root) * 1000L).toLong()
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

    /** Anxious villagers make actions take 2.5x as long (StatusEffect_Anxious). */
    private fun actionTimeMultiplier(root: PlacedCard): Double {
        val anxious = (listOf(root) + root.children).any { it.statuses.containsKey("anxious") }
        return if (anxious) 2.5 else 1.0
    }

    private fun matchMultiset(stackIds: List<String>, req: List<String>): Boolean {
        // original StackMatchesSubprint: stack must CONTAIN every required card
        // (the blueprint/idea root itself can satisfy a req; extras are fine)
        val pool = stackIds.toMutableList()
        for (r in req) {
            val it = pool.firstOrNull { matchesSpecialId(it, r) } ?: return false
            pool.remove(it)
        }
        return true
    }

    fun tick() {
        now = System.currentTimeMillis()
        tickCount++
        for (root in stacks.toList()) {
            // depleted harvestables rest, then refill (StatusEffect_Depleted)
            if (root.depletedUntil > now) continue
            if (root.depletedUntil in 1..now) {
                root.depletedUntil = 0
                root.amountLeft = root.def.amount
                recomputeTimer(root)
            }
            tickNode(root)
        }
        tickCombat(0.25)
        // moon clock
        monthTimer += 0.25
        if (monthTimer >= monthSeconds) {
            monthTimer -= monthSeconds
            endOfMonth()
        }
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
        initRuntime(c)
        stacks.add(c)
        return c
    }

    private fun completeRecipe(root: PlacedCard) {
        val rd = root.def
        if (root.recipeIndex >= 0) {
            val r = rd.recipes[root.recipeIndex]
            // consume exactly the cards in the recipe's remove list (workers/ideas stay on the stack)
            for (rid in r.remove) {
                val it = root.children.firstOrNull { matchesSpecialId(it.id, rid) } ?: continue
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
            if (root.amountLeft <= 0) {
                // depleted: rest, then refill
                root.depletedUntil = now + rd.depletedTime * 1000L
                root.timerEnd = 0; root.timerAction = ""
                return
            }
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
                val c = spawn(id, node.x + (rng.nextFloat() - 0.5f) * 0.3f, node.y + (rng.nextFloat() - 0.5f) * 0.3f)
                c?.faceUp = false   // cards from packs start face-down; tap to flip
            }
        }
        cardsOpened++
        message.value = "Booster opened!"
    }

    /** SellBox: drop a card, get gold (value in gold pieces). */
    fun sellCard(node: PlacedCard, atX: Float, atY: Float) {
        val value = max(1, node.def.value)
        detach(node)
        repeat(value) { spawn("gold", atX + (rng.nextFloat() - 0.5f) * 0.08f, atY + (rng.nextFloat() - 0.5f) * 0.08f) }
        message.value = "Sold ${node.def.name}"
    }

    /** BuyBoosterBox: drop gold on a box; when enough is stored, it spawns the pack.
     *  Returns true if the box consumed the card; false = caller puts the card back. */
    fun dropOnBox(box: ShopBox, node: PlacedCard): Boolean {
        if (node.id != "gold" && node.id != "gold_bar") return false
        detach(node)
        box.stored += max(1, node.def.value)
        if (box.stored >= box.cost) {
            box.stored = 0
            val pack = spawn(box.boosterId, box.x, box.y - 0.2f)
            if (pack != null) message.value = "${box.label} pack available!"
        }
        return true
    }

    /** Tap a face-down card to flip it up. */
    fun flipCard(node: PlacedCard) {
        if (!node.faceUp) node.faceUp = true
    }

    // drag & drop: detach node from its parent
    fun detach(node: PlacedCard) {
        for (s in stacks) {
            if (s === node) {
                stacks.remove(node)
                // pulling a participant out of a conflict leaves the conflict
                if (node.conflictId >= 0) {
                    conflictOf(node)?.participants?.remove(node)
                    node.conflictId = -1
                }
                return
            }
            if (node in s.children) { s.children.remove(node); recomputeTimer(s); return }
        }
    }

    // ===================== COMBAT =====================
    // All rules verified against decompiled Combatable.cs / Conflict.cs / CombatStats.cs.

    private data class EffStats(
        val maxHealth: Int, val attackSpeed: Double, val hitChance: Double,
        val attackDamage: Int, val defence: Int, val attackType: Int,
        val specialHits: List<SpecialHitDef>,
    )

    private fun processedStats(c: PlacedCard): EffStats {
        val b = c.def.combat!!
        var as_ = b.attackSpeed; var hc = b.hitChance; var ad = b.attackDamage; var df = b.defence; var mh = b.maxHealth
        var at = 1 // Melee default when combatable
        val specials = b.specialHits.toMutableList()
        for (e in c.equip) {
            val es = e.def.equipStats ?: continue
            as_ = (as_ - es.asInc * 0.6).coerceIn(0.5, 3.5)
            hc = (hc + es.hcInc * 0.09).coerceIn(0.5, 0.95)
            ad += es.adInc
            df += es.defInc
            mh += es.maxHealth
            specials.addAll(es.specialHits)
            if (e.def.attackType > 0) at = e.def.attackType
        }
        if (c.statuses.containsKey("frenzy")) as_ = (as_ - 0.6).coerceIn(0.5, 3.5)
        return EffStats(mh, as_, hc, ad, df, at, specials)
    }

    // rock-paper-scissors: melee > magic > ranged > melee (IsVeryEffective)
    private fun isVeryEffective(self: Int, target: Int): Boolean =
        (self == 1 && target == 3) || (self == 3 && target == 2) || (self == 2 && target == 1)

    private fun damageMultiplier(c: PlacedCard): Double = if (c.statuses.containsKey("drunk")) 2.0 else 1.0
    private fun hitChanceOf(c: PlacedCard): Double {
        var hc = processedStats(c).hitChance
        if (c.statuses.containsKey("drunk")) hc *= 0.6
        return hc
    }

    private fun getDamage(attacker: PlacedCard, target: PlacedCard): Int {
        val a = processedStats(attacker); val t = processedStats(target)
        if (target.statuses.containsKey("invulnerable")) return 0
        var dmg = a.attackDamage
        if (rng.nextDouble() < 0.5) dmg += 1                       // 50% +1 roll
        dmg -= ceil(t.defence * 0.5).toInt()
        dmg = (dmg * (if (isVeryEffective(a.attackType, t.attackType)) 1.4 else 1.0) * damageMultiplier(attacker)).roundToInt()
        if (dmg > 0) return dmg
        return rng.nextInt(2)                                      // fully blocked: 50% chance of 1
    }

    private fun pickSpecialHit(c: PlacedCard): SpecialHitDef? {
        val specials = processedStats(c).specialHits
        if (specials.isEmpty()) return null
        // WeightedRandomBag over chance out of 100; remainder = plain hit
        var r = rng.nextDouble() * 100.0
        for (s in specials) {
            if (r < s.chance) return s
            r -= s.chance
        }
        return null
    }

    private fun teamOf(c: PlacedCard) = if (c.team == "enemy") "enemy" else "player"

    private fun teammates(conflict: ConflictState, team: String) =
        conflict.participants.filter { teamOf(it) == team }

    /** Conflict.GetTarget: proportional range; player team 50% picks lowest-HP in range. */
    private fun getTarget(conflict: ConflictState, attacker: PlacedCard): PlacedCard? {
        val myTeam = teamOf(attacker)
        val foeTeam = if (myTeam == "player") "enemy" else "player"
        val mine = teammates(conflict, myTeam)
        val foes = teammates(conflict, foeTeam)
        if (foes.isEmpty()) return null
        val i = mine.indexOf(attacker)
        val ratio = foes.size.toDouble() / mine.size
        val minI = floor0(i * ratio)
        val maxI = ceil0((i + 1) * ratio)
        val range = foes.filterIndexed { idx, _ -> idx >= minI && idx < maxI }.ifEmpty { foes }
        return if (myTeam == "player" && rng.nextDouble() < 0.5) range.minByOrNull { it.hp } else range[rng.nextInt(range.size)]
    }

    private fun floor0(v: Double) = kotlin.math.floor(v).toInt()
    private fun ceil0(v: Double) = kotlin.math.ceil(v).toInt()

    private fun tickCombat(dt: Double) {
        for (conflict in conflicts.toList()) {
            conflict.timeSinceLastAttack += dt
            val players = teammates(conflict, "player")
            val enemies = teammates(conflict, "enemy")
            if (players.isEmpty() || enemies.isEmpty()) {
                if (conflict.participants.isEmpty()) conflicts.remove(conflict)
                else if (players.isEmpty() || enemies.isEmpty()) {
                    // one side gone: conflict ends, survivors stay put
                    conflict.participants.forEach { it.conflictId = -1 }
                    conflicts.remove(conflict)
                }
                continue
            }
            for (attacker in conflict.participants.toList()) {
                if (attacker.stunTimer > 0) { attacker.stunTimer -= dt; continue }
                if (conflict.timeSinceLastAttack <= 0.3) continue   // global 0.3s attack gate
                if (attacker.attackTimer < processedStats(attacker).attackSpeed) continue
                attacker.attackTimer = 0.0
                conflict.timeSinceLastAttack = 0.0
                performAttack(conflict, attacker)
                if (conflict.participants.isEmpty()) break
            }
        }
        // standalone status ticks (bleed/poison/sick on cards not in a conflict)
        for (root in stacks.toList()) tickStatuses(root, dt)
    }

    private fun performAttack(conflict: ConflictState, attacker: PlacedCard) {
        val target = getTarget(conflict, attacker) ?: return
        if (rng.nextDouble() > hitChanceOf(attacker)) return        // miss
        val dmg = getDamage(attacker, target).coerceIn(0, 100)
        val special = pickSpecialHit(attacker)
        if (special == null) {
            applyDamage(attacker, target, dmg)
            return
        }
        // PerformSpecialHit: effect on the chosen target set, then damage on enemy-side targets
        // (Self counts for damage only for Crit/Stun/Bleeding; HealLowest never damages)
        val myTeam = teamOf(attacker)
        val foeTeam = if (myTeam == "player") "enemy" else "player"
        val targets: List<PlacedCard> = when (special.target) {
            0 -> listOf(attacker)                                   // Self
            1 -> listOf(target)                                     // Target
            2 -> teammates(conflict, myTeam).shuffled(rng).take(1)  // RandomFriendly
            3 -> teammates(conflict, foeTeam).shuffled(rng).take(1) // RandomEnemy
            4 -> teammates(conflict, myTeam)                        // AllFriendly
            5 -> teammates(conflict, foeTeam)                       // AllEnemy
            else -> listOf(target)
        }
        val dmgFlagBase = special.target in setOf(1, 3, 5)
        for (t in targets) {
            val dmgFlag = dmgFlagBase || (special.target == 0 && special.type in setOf(10, 2, 6)) // Self+Crit/Stun/Bleeding
            val d = if (special.type == 10) dmg * 2 else dmg        // Crit doubles
            when (special.type) {
                1 -> if (!t.statuses.containsKey("poison")) addStatus(t, "poison")
                2 -> { t.statuses.remove("stunned"); addStatus(t, "stunned") }
                3, 4 -> heal(t, 2)                                  // Heal / HealLowest
                5 -> heal(attacker, d)                               // LifeSteal heals attacker
                6 -> if (!t.statuses.containsKey("bleeding")) addStatus(t, "bleeding")
                7 -> { t.statuses.remove("frenzy"); addStatus(t, "frenzy") }
                11 -> addStatus(t, "sick")                           // plague_mask block not modeled (no plague_mask card in data)
                12 -> if (!t.statuses.containsKey("anxious")) addStatus(t, "anxious")
                9 -> if (!t.statuses.containsKey("invulnerable")) addStatus(t, "invulnerable")
            }
            if (dmgFlag) applyDamage(attacker, t, d)
        }
    }

    private fun addStatus(c: PlacedCard, id: String) {
        c.statuses[id] = 0.0     // re-applying resets the timer
        c.statusTick[id] = 0.0
    }

    private fun heal(c: PlacedCard, amount: Int) {
        val max = processedStats(c).maxHealth
        c.hp = min(max, c.hp + amount)
    }

    private fun applyDamage(attacker: PlacedCard, target: PlacedCard, dmg: Int) {
        if (dmg <= 0) return
        target.hp -= dmg
        target.stunTimer = 0.05          // can't attack for 0.05s after taking damage
        if (target.hp <= 0) {
            target.hp = 0
            onDeath(target)
        }
    }

    private fun onDeath(c: PlacedCard) {
        conflictOf(c)?.participants?.remove(c)
        c.conflictId = -1
        // drops: draw one card from the drop bag (WeightedRandomBag in the original)
        val dropId = if (c.def.drops.isNotEmpty()) drawFromBag(c.def.drops) else null
        // remove card from board
        detach(c)
        if (dropId != null) spawn(dropId, c.x, c.y)
        if (c.id == "villager") {
            spawn("corpse", c.x, c.y)
            message.value = "${c.def.name} died!"
        }
    }

    private fun tickStatuses(c: PlacedCard, dt: Double) {
        if (c.hp >= 0) {
            // bleeding: 1 dmg every 2s for 10s
            c.statuses["bleeding"]?.let { t ->
                c.statuses["bleeding"] = t + dt
                val tk = (c.statusTick.getOrPut("bleeding") { 0.0 }) + dt
                c.statusTick["bleeding"] = tk
                if (tk >= 2.0) { c.statusTick["bleeding"] = 0.0; applyDamage(c, c, 1) }
                if (c.statuses["bleeding"]!! >= 10.0) c.statuses.remove("bleeding")
            }
            // poison: 3 dmg every 60s (30s on enemies, max 3 ticks)
            c.statuses["poison"]?.let { t ->
                val isEnemy = teamOf(c) == "enemy"
                val period = if (isEnemy) 30.0 else 60.0
                val tk = (c.statusTick.getOrPut("poison") { 0.0 }) + dt
                c.statusTick["poison"] = tk
                if (tk >= period) {
                    c.statusTick["poison"] = 0.0
                    c.poisonTicks++
                    applyDamage(c, c, 3)
                    if (isEnemy && c.poisonTicks >= 3) c.statuses.remove("poison")
                }
            }
            // sick: 2 dmg every 30s until dead
            c.statuses["sick"]?.let { t ->
                val tk = (c.statusTick.getOrPut("sick") { 0.0 }) + dt
                c.statusTick["sick"] = tk
                if (tk >= 30.0) { c.statusTick["sick"] = 0.0; applyDamage(c, c, 2) }
            }
            // stun blocks attacks for 5s
            c.statuses["stunned"]?.let { t ->
                c.statuses["stunned"] = t + dt
                if (c.statuses["stunned"]!! >= 5.0) c.statuses.remove("stunned")
            }
            // frenzy: +1 attack speed level for 10s
            c.statuses["frenzy"]?.let { t ->
                c.statuses["frenzy"] = t + dt
                if (c.statuses["frenzy"]!! >= 10.0) c.statuses.remove("frenzy")
            }
            // invulnerable: 5s
            c.statuses["invulnerable"]?.let { t ->
                c.statuses["invulnerable"] = t + dt
                if (c.statuses["invulnerable"]!! >= 5.0) c.statuses.remove("invulnerable")
            }
            // drunk/anxious: last a moon
            for (id in listOf("drunk", "anxious")) {
                c.statuses[id]?.let { t ->
                    c.statuses[id] = t + dt
                    if (c.statuses[id]!! >= monthSeconds) c.statuses.remove(id)
                }
            }
        }
        c.children.forEach { tickStatuses(it, dt) }
    }

    // ===================== END OF MONTH =====================
    // WorldManager.EndOfMonth: feed villagers -> age -> sick check -> demands.

    private fun allCards(): List<PlacedCard> = stacks.flatMap { listOf(it) + it.children + it.equip }

    private fun endOfMonth() {
        month++
        feedVillagers()
        ageVillagers()
        spoilFood()
        sickCheck()
        message.value = "Moon $month"
    }

    /** BaseVillager.GetRequiredFoodCount: villagers 2, dog 1, trained monkey 0. */
    private fun feedVillagers() {
        val eaters = allCards().filter { it.def.id == "villager" || it.def.id == "dog" || it.def.id == "trained_monkey" }
        val pool = allCards().filter { it.foodValue > 0 }.toMutableList()
        for (eater in eaters) {
            var need = when (eater.def.id) {
                "dog" -> 1
                "trained_monkey" -> 0
                else -> 2
            }
            while (need > 0) {
                val food = pool.firstOrNull { it.foodValue > 0 } ?: break
                val take = min(need, food.foodValue)
                food.foodValue -= take
                need -= take
                if (food.foodValue <= 0) {
                    // food fully consumed -> becomes goop (StatusEffect_Spoiling rule)
                    pool.remove(food)
                    detach(food)
                    spawn("goop", food.x, food.y)
                }
            }
            if (need > 0) {
                // starved
                detach(eater)
                spawn("corpse", eater.x, eater.y)
                message.value = "Someone starved!"
            }
        }
    }

    /** DetermineLifeStageFromAge: <2 teen, 2-6 adult, 7-8 elderly, >=9 dead. */
    private fun ageVillagers() {
        for (c in allCards().filter { it.age >= 0 }.toList()) {
            c.age++
            if (c.age >= 9) {
                detach(c)
                spawn("corpse", c.x, c.y)
                message.value = "A villager died of old age"
            }
        }
        // animals: old >= 3 moons, die >= 5 (WorldManager EndOfMonth)
        for (c in allCards().filter { it.def.behavior == "animal" }.toList()) {
            if (month - c.creationMonth >= 5) {
                detach(c)
                message.value = "An animal died of old age"
            }
        }
    }

    /** Food spoils after 1 moon (2 if cooked); spoiling food loses 2 value per moon -> goop. */
    private fun spoilFood() {
        for (c in allCards().filter { it.spoilSeconds >= 0 }.toList()) {
            c.spoilSeconds += monthSeconds
            val limit = if (c.def.cookedFood) monthSeconds * 2 else monthSeconds
            if (c.spoilSeconds >= limit) {
                c.foodValue -= 2
                if (c.foodValue <= 0) {
                    detach(c)
                    spawn("goop", c.x, c.y)
                }
            }
        }
    }

    /** Poop cards roll SickChance% per moon to sicken a random villager. */
    private fun sickCheck() {
        val victims = allCards().filter { it.def.id == "villager" }.toMutableList()
        for (c in allCards().filter { it.def.canMakeSick && it.def.sickChance > 0 }) {
            if (victims.isEmpty()) break
            if (rng.nextDouble() * 100.0 < c.def.sickChance) {
                val v = victims[rng.nextInt(victims.size)]
                addStatus(v, "sick")
                message.value = "Someone got sick!"
            }
        }
    }
}
