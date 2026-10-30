# (DX11)Armor Thrusters

| | |
|---|---|
| **Original author** | Darth Biomech |
| **Original Workshop page** | https://steamcommunity.com/sharedfiles/filedetails/?id=295393216 |
| **Last original update** | 2015-12-10 |
| **Last repo update** | 2026-10-30 |
| **Status** | `Functional` (local single player; dedicated server to test after republishing) |
| **Current version** | 1.1 |
| **Tested on Space Engineers** | 1.210.014 (build 24675677), local single player, creative (2026-10-30) |
| **Republished on the Workshop** | no |

## Description

Thrusters built into an armor block, for large and small grids, in straight and sloped versions (4 blocks).

## Contents

- `Data/`: `CubeBlocks.sbc` (4 `ThrustDefinition`: `LargeBlockArmorThrust`, `SmallBlockArmorThrust`, `LargeBlockArmorSloppedThrust`, `SmallBlockArmorSloppedThrust`), `BlockCategories.sbc`, `BlockVariantGroups.sbc` (added in this repo, v1.1)
- `Models/Darth_Biomech/`: 4 `.mwm` models
- `Textures/Darth_Biomech/GUI/`: 2 `.dds` icons
- `metadata.mod`: added in this repo (v1.1)
- `docs/WORKSHOP_DESCRIPTION.txt`: Steam Workshop description (BBCode), added in this repo (v1.1)
- `README.txt`, `thumb.jpg`: original files from the author
- Dependencies: none. Construction models, sounds and flame materials come from the base game (all still present in the current game).

The blocks appear in the "Darth Biomech Mods" tab of the G menu, shared with
[(DX11)Cryopods](../%28DX11%29Cryopods/) and [(DX11)Docking Ring](../%28DX11%29Docking_Ring/)
(same `<Name>darthbiomech</Name>` in all three mods). Since v1.1, the 4 thrusters
(straight and sloped, large and small grid) form a single block variant group there, "Armor Thrusters".

## Original license

No license stated by the author. The original `README.txt` only says
"Model made by Darth Biomech" (DeviantArt: http://darth-biomech.deviantart.com).
Contact with the author: not attempted yet.

## Note: legacy Workshop item (2015)

The Workshop serves the original version as a single `<ugchandle>_legacy.bin` archive
(a zip whose paths use `\`). Version 1.0 in this repo is that archive extracted as-is
(lowercase `data/`, `models/`, `textures/` folders). Since v1.1 the folder uses the modern layout
(`Data/`, `Models/`, `Textures/`, `metadata.mod`): once **republished**, it becomes a normal
Workshop item that dedicated servers download without trouble.

On a **dedicated server**, downloading the original legacy item fails (`k_EResultTimeout`)
and the server does not start. Workaround found on 2026-10-29, still needed as long as the server
uses the original ID 295393216: copy the `_legacy.bin` from the Steam client into
`content/244850/295393216/` on the server and declare the item in `appworkshop_244850.acf`
(`WorkshopItemsInstalled` and `WorkshopItemDetails` sections, `manifest` set to `-1`, same
`ugchandle`, `size` and `timeupdated` as the client). The server then sees it as "Up to date" and loads it.

## Analysis

Static analysis of 2026-10-30, compared with the vanilla content of the installed game version.
Valid XML, no duplicate `SubtypeId`, all 4 blocks listed in the categories.
Line numbers refer to v1.1.

| # | File:line | Issue | Severity | Confirmed? | State |
|---|-----------|-------|----------|------------|-------|
| 1 | (mod layout) | Legacy Workshop item (`_legacy.bin`): downloading it fails on a dedicated server (`k_EResultTimeout`) and the server does not start | blocking (dedicated server) | confirmed (Pterodactyl server, 2026-10-29) | folder switched to the modern layout in v1.1; the Workshop item must be republished to solve it |
| 2 | (mod layout) | Lowercase `data/`, `models/`, `textures/` folders, no `metadata.mod` | minor | confirmed | **fixed** in v1.1 |
| 3 | `Data/CubeBlocks.sbc:11,95,169,254,98` | Path casing differs from the files (`darth_biomech`, `small_armor_thruster.mwm`): no effect on Windows, file possibly not found on a case-sensitive system | minor | assumed (no effect on Windows) | **fixed** in v1.1 |
| 4 | `Data/CubeBlocks.sbc:84-85` (and equivalents) | No `<ThrusterType>` or `<PCU>` (fields added to the game after 2015): thruster type and PCU left at their defaults | minor | assumed (defaults not checked) | **fixed** in v1.1 (`Ion`, `12`, like the vanilla small ion thruster) |
| 5 | `Data/BlockCategories.sbc:10,24,38` | The `LargeBlocks`, `SmallBlocks` and `ShipThrusters` categories no longer exist in the game: new categories with these names may appear in the G menu. The blocks stay available in the "Darth Biomech Mods" category (line 56) | cosmetic | assumed, to verify in game | not changed |
| 6 | `Data/CubeBlocks.sbc:70,75,33` (and equivalents) | Old tags: `<FlameGlareMaterial>` (replaced by `<FlameFlare>`), `<DamageEffectId>` (replaced by `<DamageEffectName>`), `<Variants>`. They still appear in a few vanilla files, so they are probably accepted; at worst, no flame glare or damage effect | cosmetic | assumed; the game loads them without warning (log of 2026-10-30) | not changed |
| 7 | `Data/CubeBlocks.sbc:10` | The large block is named "Small Armor Thrust" (same for the sloped version, line 168): same name as the small-grid version | cosmetic | confirmed | not changed (original name) |

### In-game load result (2026-10-30, local single player)

Local creative world with the 3 (DX11) mods at v1.1 (`SpaceEngineers.log` of 2026-10-30, 09:38 and 09:58):
the 4 definitions are created without any error or warning, so the old tags (issue 6) are accepted
by the game. Third load at 10:05, with the variant group: `BlockVariantGroups.sbc` is loaded and
post-processed without error or warning.

Seen in game by the tester (2026-10-30): the "Darth Biomech Mods" tab is separate from the rest of
the G menu and the "Armor Thrusters" group shows. All 4 blocks (large and small grid) were placed,
the world was saved and reloaded: the 4 blocks are in the save (10:12), and the render log reports
no missing texture for the models of this mod. Thrust, blue flame and PCU 12 checked by the tester.

## To test in game

Details, procedure and results: [IN_GAME_TESTS.md](IN_GAME_TESTS.md).

## History

See [CHANGELOG.md](CHANGELOG.md).

## Steam Workshop description

The player-facing parts of this README are kept in BBCode in [docs/WORKSHOP_DESCRIPTION.txt](docs/WORKSHOP_DESCRIPTION.txt).
