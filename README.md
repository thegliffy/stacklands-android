# Stacklands Android (unofficial port)

An Android port of [Stacklands](https://store.steampowered.com/app/1959500/Stacklands/) by Sokpop, built by decompiling the original Unity (Mono) game and reimplementing the core loop in Kotlin/Compose.

**This is a fan port, not affiliated with Sokpop.** Game assets (sprites, card data, localization) belong to Sokpop.

## What works (v0.1)

- ✅ Full card database extracted from the game: **389 cards** with real recipes, harvest bags, and drop chances
- ✅ Original sprites (254) + original card colors from the game's ColorManager
- ✅ Drag & drop stacking, blueprint builds, harvest-on-timer, booster packs (tap to open)
- ✅ Starter deal + shop row of booster packs
- ✅ Kid → villager growth, house breeding
- ❌ Not yet: combat, energy, pollution/wellbeing, saves, DLC cards (not present in source install)

## Try it

Grab the APK from the [latest release](../../releases) — min Android 8.0 (API 26).

## Layout

```
android/       Gradle project (Kotlin + Compose, min SDK 26)
  app/src/main/java/com/gliffy/stacklands/
    MainActivity.kt   Compose UI: canvas, drag gestures, card rendering
    Engine.kt         game loop: recipes, harvest timers, boosters, breeding
    GameData.kt       JSON loader for extracted data
  app/src/main/assets/
    gamedata/         cards.json, boosters.json (extracted, see below)
    sprites/          original game sprites (PNG)
scripts/       extraction pipeline (run on a box with the game installed)
decompiled/    ilspycmd output of GameScripts.dll (reference)
```

## How it was made

1. **Decompile** — `ilspycmd -p` on `Stacklands_Data/Managed/GameScripts.dll` (Mono build, fully readable)
2. **Assets** — AssetRipper 2.0 headless (`--headless --port N`, driven via its HTTP API): UnityProject export → prefab YAML + sprite `.meta` GUID map
3. **Data** — `scripts/consolidate.py` parses prefab YAML (PyYAML with a custom loader for Unity `!u!` tags), resolves sprite GUIDs, extracts localization from `LocResources.asset` (Odin `SerializedBytes` hex blob), merges booster/bag data
4. **App** — Kotlin/Compose reimplementation of the core loop driven by the extracted JSON

See `scripts/` for the exact pipeline.

## Build

```bash
cd android
JAVA_HOME=/path/to/jdk17+ ANDROID_HOME=$ANDROID_HOME ~/gradle/gradle-8.7/bin/gradle :app:assembleDebug
```
