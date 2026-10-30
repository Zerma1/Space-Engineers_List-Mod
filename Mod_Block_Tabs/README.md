# Mod Block Tabs

> 🇫🇷 Version française : [README.fr.md](README.fr.md). This English version is the reference.

| | |
|---|---|
| **Original author** | Zerma1 (created in this repository) |
| **Original Workshop page** | none (created in this repository) |
| **Last original update** | 2026-10-30 (creation) |
| **Last repo update** | 2026-10-30 |
| **Status** | `To test` |
| **Current version** | 1.0 (`metadata.mod`) |
| **Tested on Space Engineers** | not tested |
| **Republished on the Workshop** | no |

## Description

Adds **one tab per loaded mod** to the build menu (**G**), named `[MOD] <mod name>`, listing every
block of that mod. No more searching the general categories for modded blocks; they also stay in
their original tabs.

- Tabs are created automatically when the world loads, from the world's mod list.
- The tab name is the mod name from the mod list (`FriendlyName`), otherwise its folder name.
- Tabs are sorted alphabetically with the other categories.
- Client side only: on a dedicated server the script does nothing, but the mod can be added to the
  server's mod list so that every player gets it.

## Contents

- `Data/Scripts/ModBlockTabs/ModBlockTabs.cs`: session component (`BeforeStart` creates the
  categories, `UnloadData` removes them)
- No `.sbc`, models or textures
- Dependencies: none

## How it works

The G menu (`MyGuiScreenToolbarConfigBase.RecreateControls`) reads
`MyDefinitionManager.Static.GetCategories()` every time it opens. The script adds one
`MyGuiBlockCategoryDefinition` per mod to that dictionary, with `ItemIds` in the `TypeId/SubtypeId`
form used by `BlockCategories.sbc`. Only `Public` blocks are listed; the game then applies its usual
filters (survival, research, DLC).

## Original license

MIT license of this repository.

## Analysis

| # | File:line | Problem | Severity | Confirmed? |
|---|-----------|---------|----------|------------|
| 1 | `ModBlockTabs.cs` | `MyGuiBlockCategoryDefinition` and `GetCategories()` (namespace `Sandbox.Definitions`) must be allowed by the ModAPI whitelist | blocking if refused | to check in game |
| 2 | `ModBlockTabs.cs` | Display order of the `[MOD]` tabs among the vanilla categories (sorted by display name) | cosmetic | to check in game |

Static check: the script compiles against the game DLLs (2026-10-30). This does not prove that it
passes the whitelist.

## To test in game

- [ ] The world loads without a mod error (no "Prohibited type" in the log)
- [ ] The log contains `Mod Block Tabs: N mod tab(s) added to the G menu`
- [ ] The `[MOD] …` tabs appear in the G menu with the right blocks
- [ ] Blocks also stay in their original categories
- [ ] Single player
- [ ] Multiplayer / dedicated server
- [ ] Loading another world (with other mods): no tab of a missing mod

## History

See [CHANGELOG.md](CHANGELOG.md).
