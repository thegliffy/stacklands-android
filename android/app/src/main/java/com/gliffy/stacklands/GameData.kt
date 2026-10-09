package com.gliffy.stacklands

import android.content.res.AssetManager
import kotlinx.serialization.json.Json
import kotlinx.serialization.json.JsonArray
import kotlinx.serialization.json.JsonElement
import kotlinx.serialization.json.JsonObject
import kotlinx.serialization.json.JsonPrimitive
import kotlinx.serialization.json.int
import kotlinx.serialization.json.intOrNull
import kotlinx.serialization.json.jsonArray
import kotlinx.serialization.json.jsonObject
import kotlinx.serialization.json.jsonPrimitive
import kotlinx.serialization.json.longOrNull

data class Recipe(val req: List<String>, val remove: List<String>, val result: String, val extra: List<String>, val time: Int)
data class BagEntry(val card: String, val chance: Int)
data class SpecialHitDef(val chance: Int, val type: Int, val target: Int)
data class CombatDef(
    val maxHealth: Int,
    val attackSpeed: Double,
    val hitChance: Double,
    val attackDamage: Int,
    val defence: Int,
    val asInc: Int = 0,
    val hcInc: Int = 0,
    val adInc: Int = 0,
    val defInc: Int = 0,
    val specialHits: List<SpecialHitDef> = emptyList(),
)
data class EquipStats(
    val maxHealth: Int = 0,
    val asInc: Int = 0,
    val hcInc: Int = 0,
    val adInc: Int = 0,
    val defInc: Int = 0,
    val specialHits: List<SpecialHitDef> = emptyList(),
)
data class Special(
    val accept: List<String> = emptyList(),
    val need: Int = 0,
    val time: Int = 0,
    val result: String = "",
    val worker: Boolean = false,
    val chest: Boolean = false,
    val breed: Boolean = false,
    val feed: String = "",
    val bag: Boolean = false,
)
data class CardDef(
    val id: String,
    val name: String,
    val desc: String,
    val value: Int,
    val icon: String,
    val behavior: String,
    val workers: Int,
    val educated: Boolean,
    val recipes: List<Recipe>,
    val bag: List<BagEntry>,
    val bagCount: Int,
    val special: Special?,
    val harvestTime: Int = 10,
    val canDeplete: Boolean = false,
    val depletedTime: Int = 30,
    val amount: Int = 3,
    val isUnlimited: Boolean = false,
    val combat: CombatDef? = null,
    val foodValue: Int = 0,
    val canSpoil: Boolean = false,
    val equipType: Int = -1,
    val attackType: Int = 0,
    val equipStats: EquipStats? = null,
    val possibleEquip: List<String> = emptyList(),
    val drops: List<BagEntry> = emptyList(),
    val team: String = "",
    val isBreedable: Boolean = false,
    val sickChance: Double = 0.0,
    val canMakeSick: Boolean = false,
    val cookedFood: Boolean = false,
)
data class BoosterBag(val cardsInPack: Int, val chances: List<BagEntry>)
data class BoosterPack(val id: String, val bags: List<BoosterBag>, val cost: Int, val isIntro: Boolean)

object GameData {
    val cards = mutableMapOf<String, CardDef>()
    val boosters = mutableMapOf<String, BoosterPack>()
    val json = Json { ignoreUnknownKeys = true }
    val fallback = CardDef("?", "Unknown", "", 1, "", "other", 0, false, emptyList(), emptyList(), 0, null)

    private fun JsonObject.obj(key: String): JsonObject? = this[key] as? JsonObject
    private fun JsonObject.arr(key: String): List<JsonElement> = (this[key] as? JsonArray) ?: emptyList()
    private fun JsonObject.str(key: String, dflt: String = ""): String {
        val p = this[key] as? JsonPrimitive ?: return dflt
        return if (p.isString) p.content else dflt
    }
    private fun JsonObject.int(key: String, dflt: Int): Int {
        val p = this[key] as? JsonPrimitive ?: return dflt
        return p.content.toIntOrNull() ?: dflt
    }
    private fun JsonObject.bool(key: String): Boolean {
        val p = this[key] as? JsonPrimitive ?: return false
        return p.content.toBooleanStrictOrNull() ?: false
    }
    private fun JsonObject.dbl(key: String, dflt: Double): Double {
        val p = this[key] as? JsonPrimitive ?: return dflt
        return p.content.toDoubleOrNull() ?: dflt
    }
    private fun parseSpecialHits(el: List<JsonElement>): List<SpecialHitDef> = el.mapNotNull {
        val o = it as? JsonObject ?: return@mapNotNull null
        SpecialHitDef(o.int("chance", 0), o.int("type", 0), o.int("target", 1))
    }
    private fun parseCombat(o: JsonObject?): CombatDef? {
        o ?: return null
        return CombatDef(
            maxHealth = o.int("maxHealth", 0),
            attackSpeed = o.dbl("attackSpeed", 3.5),
            hitChance = o.dbl("hitChance", 0.5),
            attackDamage = o.int("attackDamage", 1),
            defence = o.int("defence", 1),
            asInc = o.int("asInc", 0),
            hcInc = o.int("hcInc", 0),
            adInc = o.int("adInc", 0),
            defInc = o.int("defInc", 0),
            specialHits = parseSpecialHits(o.arr("specialHits")),
        )
    }
    private fun parseEquipStats(o: JsonObject?): EquipStats? {
        o ?: return null
        return EquipStats(
            maxHealth = o.int("maxHealth", 0),
            asInc = o.int("asInc", 0),
            hcInc = o.int("hcInc", 0),
            adInc = o.int("adInc", 0),
            defInc = o.int("defInc", 0),
            specialHits = parseSpecialHits(o.arr("specialHits")),
        )
    }

    fun load(assets: AssetManager) {
        cards.clear(); boosters.clear()
        try {
            val cardsJson = json.parseToJsonElement(assets.open("gamedata/cards.json").reader().readText()) as JsonObject
            for ((_, v) in cardsJson) {
                val o = v as? JsonObject ?: continue
                val id = o.str("id")
                if (id.isEmpty()) continue
                val sp = o.obj("special")
                cards[id] = CardDef(
                    id = id,
                    name = o.str("name").ifEmpty { id },
                    desc = o.str("desc"),
                    value = o.int("value", 1),
                    icon = o.str("icon"),
                    behavior = o.str("behavior", "other"),
                    workers = o.int("workers", 0),
                    educated = o.bool("educated"),
                    recipes = o.arr("recipes").mapNotNull { r ->
                        val ro = r as? JsonObject ?: return@mapNotNull null
                        Recipe(
                            req = ro.arr("req").map { it.jsonPrimitive.content },
                            remove = ro.arr("remove").map { it.jsonPrimitive.content },
                            result = (ro["result"] as? JsonPrimitive)?.takeIf { it.isString }?.content ?: "",
                            extra = ro.arr("extra").map { it.jsonPrimitive.content },
                            time = ro.int("time", 10),
                        )
                    },
                    bag = o.arr("bag").mapNotNull { b ->
                        val bo = b as? JsonObject ?: return@mapNotNull null
                        BagEntry(bo.str("card"), bo.int("chance", 1))
                    },
                    bagCount = o.int("bagCount", 0),
                    special = sp?.let {
                        Special(
                            accept = it.arr("accept").map { x -> x.jsonPrimitive.content },
                            need = it.int("need", 0),
                            time = it.int("time", 0),
                            result = it.str("result"),
                            worker = it.bool("worker"),
                            chest = it.bool("chest"),
                            breed = it.bool("breed"),
                            feed = it.str("feed"),
                            bag = it.bool("bag"),
                        )
                    },
                    harvestTime = o.int("harvesttime", 10),
                    canDeplete = o.bool("candeplete"),
                    depletedTime = o.int("depletedtime", 30),
                    amount = o.int("amount", 3),
                    isUnlimited = o.bool("isunlimited"),
                    combat = parseCombat(o.obj("combat")),
                    foodValue = o.int("foodValue", 0),
                    canSpoil = o.bool("canSpoil"),
                    equipType = o.int("equipType", -1),
                    attackType = o.int("attackType", 0),
                    equipStats = parseEquipStats(o.obj("equipStats")),
                    possibleEquip = o.arr("possibleEquip").map { it.jsonPrimitive.content },
                    drops = o.arr("drops").mapNotNull { b ->
                        val bo = b as? JsonObject ?: return@mapNotNull null
                        BagEntry(bo.str("card"), bo.int("chance", 1))
                    },
                    team = o.str("team"),
                    isBreedable = o.bool("isBreedable"),
                    sickChance = o.dbl("sickChance", 0.0),
                    canMakeSick = o.bool("canMakeSick"),
                    cookedFood = o.bool("cookedFood"),
                )
            }
            val boostersJson = json.parseToJsonElement(assets.open("gamedata/boosters.json").reader().readText()) as JsonObject
            for ((k, v) in boostersJson) {
                val o = v as? JsonObject ?: continue
                boosters[k] = BoosterPack(
                    id = k,
                    bags = o.arr("bags").map { b ->
                        val bo = b as? JsonObject ?: return@map b.let { BoosterBag(0, emptyList()) }
                        BoosterBag(
                            cardsInPack = bo.int("cardsInPack", 5),
                            chances = bo.arr("chances").mapNotNull { c ->
                                val co = c as? JsonObject ?: return@mapNotNull null
                                BagEntry(co.str("card"), co.int("chance", 1))
                            },
                        )
                    },
                    cost = o.int("cost", 0),
                    isIntro = o.bool("isIntro"),
                )
            }
        } catch (e: Exception) {
            android.util.Log.e("GameData", "load failed", e)
        }
    }
}
