# (DX11)Cryopods

| | |
|---|---|
| **Original author** | Darth Biomech |
| **Original Workshop page** | https://steamcommunity.com/sharedfiles/filedetails/?id=294618647 |
| **Last original update** | 2015-12-17 |
| **Last repo update** | 2026-10-30 |
| **Status** | `Functional` (local single player; dedicated server to test after republishing; 4 non-blocking `MOD_ERROR`, see issue 11) |
| **Current version** | 1.1 |
| **Tested on Space Engineers** | 1.210.014 (build 24675677), local single player, creative (2026-10-30) |
| **Republished on the Workshop** | no |

## Description

Cryogenic pods for large and small grids: functional pod (`CryoChamber`), open pod (cockpit), decorative pod and pod pack (8 blocks).

## Contents

- `Data/`: `CubeBlocks.sbc` (8 blocks: 2 `CryoChamberDefinition`, 2 `CockpitDefinition`, 4 decorative `CubeBlock`), `BlockCategories.sbc`, `BlockVariantGroups.sbc` (added in this repo, v1.1), `Animations.sbc` (character pose `CryopodPostureSolo`), `TransparentMaterials.sbc` (`CryopodGlass`)
- `Models/Darth_Biomech/`: 8 `.mwm` models
- `Textures/Darth_Biomech/`: model textures; `gui/`: 4 icons
- `metadata.mod`: added in this repo (v1.1)
- `docs/WORKSHOP_DESCRIPTION.txt`: Steam Workshop description (BBCode), added in this repo (v1.1)
- `README.txt`, `thumb.jpg`: original files from the author
- Dependencies: none. Sounds and the glass texture come from the base game (all still present in the current game).

The blocks appear in the "Darth Biomech Mods" tab of the G menu, shared with
[(DX11)Armor Thrusters](../%28DX11%29Armor_Thrusters/) and [(DX11)Docking Ring](../%28DX11%29Docking_Ring/)
(same `<Name>darthbiomech</Name>` in all three mods). Since v1.1, the 8 pods (functional, open,
decorative and pack, large and small grid) form a single block variant group there, "Cryogenic Pods".

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
uses the original ID 294618647: copy the `_legacy.bin` from the Steam client into
`content/244850/294618647/` on the server and declare the item in `appworkshop_244850.acf`
(`WorkshopItemsInstalled` and `WorkshopItemDetails` sections, `manifest` set to `-1`, same
`ugchandle`, `size` and `timeupdated` as the client). The server then sees it as "Up to date" and loads it.

## Analysis

Static analysis of 2026-10-30, compared with the vanilla content of the installed game version.
Valid XML, no duplicate `SubtypeId`, every path in the `.sbc` files points to an existing file.
Line numbers refer to v1.1.

| # | File:line | Issue | Severity | Confirmed? | State |
|---|-----------|-------|----------|------------|-------|
| 1 | (mod layout) | Legacy Workshop item (`_legacy.bin`): downloading it fails on a dedicated server (`k_EResultTimeout`) and the server does not start | blocking (dedicated server) | confirmed (Pterodactyl server, 2026-10-29) | folder switched to the modern layout in v1.1; the Workshop item must be republished to solve it |
| 2 | (mod layout) | Lowercase `data/`, `models/`, `textures/` folders, no `metadata.mod` | minor | confirmed | **fixed** in v1.1 |
| 3 | `Data/CubeBlocks.sbc` (all `<Icon>`, `<Model>`, `<OverlayTexture>`), `Data/Animations.sbc:15` | Path casing differs from the folders (`models\`, `textures\`, `darth_biomech`) | minor | assumed (no effect on Windows) | **fixed** in v1.1. The `.mwm` files themselves reference `textures\darth_biomech\…` in lowercase: cannot be changed without the model sources |
| 4 | `Data/CubeBlocks.sbc:60,118,177,232` | No `<PCU>` (field added to the game after 2015) | minor | assumed (default not checked) | **fixed** in v1.1 (`15` on the functional pods and cockpits, like the vanilla cryo chamber and passenger seat); decorative blocks left at the default |
| 5 | `Data/BlockCategories.sbc:43-54` | The "Darth Biomech Mods" category only listed the 4 large-grid blocks | minor | assumed (the small-grid variants may still be reachable by switching size, `BlockPairName`) | **fixed** in v1.1 (8 blocks listed) |
| 6 | `Models/Darth_Biomech/*_progress_01.mwm` | The construction models use `Textures\Models\Cubes\armor\armor_construction_*.dds`, which no longer exist in the game: missing texture while building | cosmetic | confirmed for `cryopod_progress_01.mwm` (render log of 2026-10-30: "Missing textures for part(s) 03 - Default"); `cryopod_solo_progress_01.mwm` not displayed yet | cannot be changed (binary) |
| 7 | `Data/Animations.sbc:15`, `Data/CubeBlocks.sbc:25,83,143,200` | Character pose (`character_cryopod_solo_pose.mwm`) exported in 2015: the character skeleton has changed since, the pose may be wrong | minor | disproved in game (pose correct, tester 2026-10-30) | not changed |
| 8 | `Data/BlockCategories.sbc:10,26` | The `LargeBlocks` and `SmallBlocks` categories no longer exist in the game: new categories with these names may appear in the G menu | cosmetic | assumed, to verify in game | not changed |
| 9 | `Data/CubeBlocks.sbc:53` (and 7 others) | `<DamageEffectId>` replaced by `<DamageEffectName>` in the current game; still used by a few vanilla files, so probably accepted | cosmetic | assumed | not changed |
| 10 | `Models/Darth_Biomech/cryopod.mwm`, `Textures/Darth_Biomech/*_ns.dds`, `*_glass_clear_*.dds` | Files not referenced by the `.sbc` files (or only by this unused model): leftovers from the DX9 version | cosmetic | confirmed (no reference in the `.sbc` files) | kept (original binaries) |
| 11 | `Data/CubeBlocks.sbc:240,334` (`DB_Cryopod`, `DB_Cryopod_pack`) and `:286,380` (small-grid versions) | Decorative blocks (`CubeBlock`) whose recipe contains a `Computer` component: the game shows 4 `MOD_ERROR` on load ("contains a computer, but the block TypeId is not a terminal": the block has an owner that cannot be changed from the terminal). The blocks are still created. Present since the original version | minor | confirmed (`SpaceEngineers.log` of 2026-10-30) | not changed: changing the `TypeId` is ruled out (removes blocks from worlds); removing the `Computer` from the recipe would change the build cost, decision pending |

### In-game load result (2026-10-30, local single player)

Local creative world with the 3 (DX11) mods at v1.1 (`SpaceEngineers.log` of 2026-10-30, 09:38 and 09:58):
the 8 definitions are created. Only message: the 4 non-blocking `MOD_ERROR` of issue 11.
Third load at 10:05, with the variant group: `BlockVariantGroups.sbc` is loaded and
post-processed without error or warning.

Seen in game by the tester (2026-10-30): the "Darth Biomech Mods" tab is separate from the rest of
the G menu and the "Cryogenic Pods" group shows. The 8 blocks (large and small grid) were placed,
then the world was saved and reloaded (10:09): all 8 are in the save. Render log: only
`cryopod_progress_01.mwm` reports missing textures (issue 6); no message for the other models.
No message about the animation in the game log. The tester entered the 4 functional and open pods
(large and small): character pose correct, functional pods pressurized.

## To test in game

Details, procedure and results: [IN_GAME_TESTS.md](IN_GAME_TESTS.md).

## History

See [CHANGELOG.md](CHANGELOG.md).

## Steam Workshop description

The player-facing parts of this README are kept in BBCode in [docs/WORKSHOP_DESCRIPTION.txt](docs/WORKSHOP_DESCRIPTION.txt).
