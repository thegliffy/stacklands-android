#!/usr/bin/env python3
"""Merge combat/food/equip/drop data from cards_raw.json into the app's cards.json.
Adds per-card: combat{}, foodValue, canSpoil, equipType, attackType, possibleEquip,
isBreedable, drops[], team. Enums per decompile:
AttackType: None=0 Melee=1 Ranged=2 Magic=3 Foot=4 Armour=5 Air=6
SpecialHitType: None Poison Stun Heal HealLowest LifeSteal Bleeding Frenzy Damage Invulnerable Crit Sick Anxious
SpecialHitTarget: Self Target RandomFriendly RandomEnemy AllFriendly AllEnemy
EquipableType: Head=0 Torso=1 Weapon=2
"""
import json, os
BASE = os.path.expanduser("~/stacklands-port")
raw = json.load(open(f"{BASE}/gamedata/cards_raw.json"))
app_path = f"{BASE}/android/app/src/main/assets/gamedata/cards.json"
app = json.load(open(app_path))

raw_by_id = {}
for c in raw:
    cid = c.get("Id")
    if cid:
        raw_by_id[cid] = c

CLASS_TEAM = {"Enemy": "enemy"}  # everything else with combat stats is player team

def combat_of(c):
    cs = c.get("BaseCombatStats")
    if not cs or not cs.get("MaxHealth"):
        return None
    return {
        "maxHealth": cs.get("MaxHealth", 0),
        "attackSpeed": cs.get("AttackSpeed", 3.5),
        "hitChance": cs.get("HitChance", 0.5),
        "attackDamage": cs.get("AttackDamage", 1),
        "defence": cs.get("Defence", 1),
        "asInc": cs.get("AttackSpeedIncrement", 0),
        "hcInc": cs.get("HitChanceIncrement", 0),
        "adInc": cs.get("AttackDamageIncrement", 0),
        "defInc": cs.get("DefenceIncrement", 0),
        "specialHits": [
            {"chance": sh.get("Chance", 0), "type": sh.get("HitType", 0), "target": sh.get("Target", 1)}
            for sh in (cs.get("SpecialHits") or [])
        ],
    }

def drops_of(c):
    d = c.get("Drops")
    if not isinstance(d, dict):
        return []
    out = []
    for ch in (d.get("Chances") or []):
        if isinstance(ch, dict) and ch.get("Id"):
            out.append({"card": ch["Id"], "chance": ch.get("Chance", 1)})
    return out

added = {"combat": 0, "food": 0, "equip": 0, "drops": 0, "team": 0}
for cid, entry in app.items():
    r = raw_by_id.get(cid)
    if not r:
        continue
    cls = str(r.get("_class", ""))
    cb = combat_of(r)
    if cb:
        entry["combat"] = cb
        added["combat"] += 1
    if "FoodValue" in r and r.get("FoodValue") is not None:
        entry["foodValue"] = r.get("FoodValue", 0)
        entry["canSpoil"] = bool(r.get("CanSpoil"))
        added["food"] += 1
    if cls.startswith("Equipable") or cls.startswith("Equipment") or entry.get("behavior") == "equipable":
        entry["equipType"] = r.get("EquipableType", 0)
        entry["attackType"] = r.get("AttackType", 0)
        ms = r.get("MyStats")
        if isinstance(ms, dict):
            entry["equipStats"] = {
                "maxHealth": ms.get("MaxHealth", 0),
                "asInc": ms.get("AttackSpeedIncrement", 0),
                "hcInc": ms.get("HitChanceIncrement", 0),
                "adInc": ms.get("AttackDamageIncrement", 0),
                "defInc": ms.get("DefenceIncrement", 0),
                "specialHits": [
                    {"chance": sh.get("Chance", 0), "type": sh.get("HitType", 0), "target": sh.get("Target", 1)}
                    for sh in (ms.get("SpecialHits") or [])
                ],
            }
        added["equip"] += 1
    pe = r.get("PossibleEquipableIds")
    if pe:
        entry["possibleEquip"] = pe
    dr = drops_of(r)
    if dr:
        entry["drops"] = dr
        added["drops"] += 1
    if r.get("IsBreedable"):
        entry["isBreedable"] = True
    if r.get("IsCookedFood"):
        entry["cookedFood"] = True
    if r.get("SickChance"):
        entry["sickChance"] = r.get("SickChance")
    if r.get("CanMakeSick"):
        entry["canMakeSick"] = True
    team = CLASS_TEAM.get(cls.split("_")[0].rstrip("."), "")
    if cls.startswith("Enemy") or cls.startswith("Mob") or cls.startswith("Boss"):
        team = "enemy"
    elif entry.get("behavior") in ("worker", "animal") or cb:
        team = "player"
    if team:
        entry["team"] = team
        added["team"] += 1

json.dump(app, open(app_path, "w"), indent=1)
print("merged into", app_path, added)
