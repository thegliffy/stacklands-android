# Stacklands Combat Spec (from decompiled GameScripts.dll)

Source of truth: `decompiled/Combatable.cs`, `Conflict.cs`, `CombatStats.cs`, `SpecialHit.cs`,
`SpecialHitType.cs`, `SpecialHitTarget.cs`, `AttackType.cs`, `Equipable.cs`, `StatusEffect_*.cs`,
cross-checked with stacklands.fandom.com/wiki/Combat_Mechanics. Card-level stats live in
`gamedata/cards_raw.json` (`BaseCombatStats`, `PossibleEquipableIds`, `Drops`).

## Attack resolution order (per attack)
1. **Hit roll** — `Random < HitChance` (base ladder 0.50/0.59/0.68/0.77/0.86/0.95; villager = 0.68).
   Drunk status multiplies attacker hit chance ×0.6. Miss → "miss", no damage.
2. **Special hit pick** — weighted random bag over `SpecialHits` (Chance = % out of 100;
   remainder = plain hit). Only one special hit can fire per attack.
3. **Target selection** — proportional position: attacker at index i of team size n targets
   enemy indices [floor(i·m/n), ceil((i+1)·m/n)). Player team has 50% chance to instead pick the
   *lowest-health* target in that range (`Conflict.GetTarget`).
4. **Damage** (`Combatable.GetDamage`):
   - invulnerable target → 0
   - `dmg = AttackDamage`; 50% chance `dmg+1`
   - `dmg -= ceil(targetDefence × 0.5)`
   - `dmg = round(dmg × effectiveness × damageMultiplier)`
     - effectiveness 1.4× when very effective: **melee > magic, magic > ranged, ranged > melee**
     - damageMultiplier 2× when attacker is drunk
   - if result ≤ 0 (fully blocked): `round(Random)` → 50% chance of 1 damage anyway
5. **Crit** doubles damage *after* block/rounding (SpecialHitType.Crit).

## Attack timing
- Attack cooldown = AttackSpeed seconds (ladder 3.5/2.9/2.3/1.7/1.1/0.5; villager base 2.9 = "Slow").
- Cooldown starts after the attack animation fully finishes (melee 0.33s per target; ranged ~0.2s+travel; magic +0.3s hold).
- Only one attack may *start* per conflict per 0.3s; cooldowns pause during that window.
- A unit can't attack for 0.05s after receiving damage (incl. bleed/poison ticks).
- Cooldown doesn't tick while the card is being dragged.
- Frenzy: attack speed +1 ladder level for 10s (re-applying resets to 10s).
- Stunned: can't *start* attacks for 5s (re-applying resets); cooldown still ticks.

## Status effects (combat)
| Effect | Rule | Source |
|---|---|---|
| Bleeding | 1 dmg every 2s for 10s (5 ticks); no stack | StatusEffect_Bleeding.cs |
| Poison | 3 dmg every 60s (30s vs enemies, max 3 ticks) until dead or cured by Charcoal | StatusEffect_Poison.cs |
| Stunned | 5s, re-apply resets | StatusEffect_Stunned.cs |
| Frenzy | +1 attack-speed level, 10s, re-apply resets | StatusEffect_Frenzy.cs |
| Invulnerable | 5s, no damage taken | StatusEffect_Invulnerable.cs |
| Sick | 2 dmg every 30s; blocked by plague_mask | StatusEffect_Sick.cs |
| Anxious | action time ×2.5 (slower); lasts a moon | StatusEffect_Anxious.cs, WorldManager.cs:398 |
| Drunk | hit chance ×0.6, damage ×2; lasts a moon | Combatable.cs |

## Equipment (Equipable.cs / CombatStats.cs)
- Types: Head / Torso / Weapon (weapon can change AttackType).
- Increments map: attack speed +0.6s faster per level (clamp 0.5–3.5); hit chance +0.09 per level
  (clamp 0.5–0.95); damage +1 per level; defence +1 per level; max health additive.
- Equipping caps current HP at new max. Dropping an equipable onto a unit with inventory auto-equips.

## Conflicts (Conflict.cs)
- Start: drop a Combatable onto an opposing-team Combatable (player team = villagers/workers/cities;
  everything else = enemy). All combatables in the same stack join.
- Overlapping conflicts auto-merge.
- End: when one team has no participants; survivors stay where they are.
- Player-initiated fights: units can be pulled out (drag out of the fight rectangle); enemy-initiated: locked in.
- Death (HP ≤ 0): leave conflict, destroy card; enemies spawn their `Drops` bag (data-driven);
  villagers just die (graveyard/corpse chain is data); animals die → unhappiness (non-aggressive).

## Verified base stats (from cards_raw.json)
| Card | HP | Spd | Hit | Dmg | Def | Type | Special |
|---|---|---|---|---|---|---|---|
| Villager | 15 | 2.9 | .68 | 2 | 2 | Melee | — |
| Kid | — (no combat) | | | | | | |
| Dog | 9 | 2.3 | .68 | 2 | 1 | Melee | (inherits villager flag) |
| Wolf | 20 | 2.3 | .59 | 2 | 3 | Melee | — |
| Goblin | 10 | 2.9 | .68 | 2 | 1 | Melee | — |
| Skeleton | 12 | 2.9 | .68 | 2 | 2 | Melee | 5% Sick→target |
| Dark Elf | 15 | 2.9 | .68 | 2 | 2 | Magic | 5% Crit→target |
| Ent | 30 | 2.9 | .59 | 3 | 3 | Melee | — |
| Ghost | 12 | 2.9 | .68 | 3 | 2 | Magic | — |

Full per-card stats for all 34 enemies + equipables are in `gamedata/cards_raw.json`
(`BaseCombatStats`, `BaseAttackType`, `PossibleEquipableIds`, `Drops`) — the Android data pipeline
must carry these into `cards.json`.
