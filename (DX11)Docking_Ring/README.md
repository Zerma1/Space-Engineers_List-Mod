# (DX11)Docking Ring

| | |
|---|---|
| **Original author** | Darth Biomech |
| **Original Workshop page** | https://steamcommunity.com/sharedfiles/filedetails/?id=294619787 |
| **Last original update** | 2015-10-02 |
| **Last repo update** | 2026-10-30 |
| **Status** | `Functional` (local single player; dedicated server to test after republishing) |
| **Current version** | 1.1 |
| **Tested on Space Engineers** | 1.210.014 (build 24675677), local single player, creative (2026-10-30) |
| **Republished on the Workshop** | no |

## Description

Modular docking ring: sectors, corners, docking clamps (merge blocks) and airlocks (doors), mostly for large grids (10 blocks, 2 of them hidden).

## Contents

- `Data/`: `CubeBlocks.sbc` (10 blocks: 4 `DoorDefinition`, 2 `MergeBlockDefinition`, 4 `CubeBlock`; `DockingTubeSector` and `DockingRing` are hidden, `<Public>false</Public>`), `BlockCategories.sbc`, `BlockVariantGroups.sbc` (added in this repo, v1.1)
- `Models/Darth_Biomech/`: 16 `.mwm` models (including the door subparts `Docking_ring_door_left/right.mwm`)
- `Textures/Darth_Biomech/`: model textures; `gui/`: 4 icons
- `metadata.mod`: added in this repo (v1.1)
- `docs/WORKSHOP_DESCRIPTION.txt`: Steam Workshop description (BBCode), added in this repo (v1.1)
- `README.txt`, `thumb.jpg`: original files from the author
- Dependencies: none. Door sounds and construction textures come from the base game.

The blocks appear in the "Darth Biomech Mods" tab of the G menu, shared with
[(DX11)Armor Thrusters](../%28DX11%29Armor_Thrusters/) and [(DX11)Cryopods](../%28DX11%29Cryopods/)
(same `<Name>darthbiomech</Name>` in all three mods). Since v1.1, the 8 public blocks are sorted
there into 3 block variant groups (one icon per group):

| Group | Blocks |
|-------|--------|
| Docking Ring Sector | `DockingRingSectorPart`, `DockingRingSectorCorner` |
| Docking Ring Clamp | `DockingRingPart` (merge block), `DockingRingCorner` |
| Docking Ring Airlock | `DockingTubeDoor`, `DockingTubeDoorReverse`, `DockingTubeDoorCenter`, `DockingTubeDoorCenterS` (small grid) |

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
uses the original ID 294619787: copy the `_legacy.bin` from the Steam client into
`content/244850/294619787/` on the server and declare the item in `appworkshop_244850.acf`
(`WorkshopItemsInstalled` and `WorkshopItemDetails` sections, `manifest` set to `-1`, same
`ugchandle`, `size` and `timeupdated` as the client). The server then sees it as "Up to date" and loads it.

## Analysis

Static analysis of 2026-10-30, compared with the vanilla content of the installed game version.
Valid XML, no duplicate `SubtypeId`, the 18 paths in the `.sbc` files point to existing files,
door sounds and mirroring axes (`ZMinusHalfX`, `MinusHalfX`, `HalfX`) still used by the game.
Line numbers refer to v1.1.

| # | File:line | Issue | Severity | Confirmed? | State |
|---|-----------|-------|----------|------------|-------|
| 1 | (mod layout) | Legacy Workshop item (`_legacy.bin`): downloading it fails on a dedicated server (`k_EResultTimeout`) and the server does not start | blocking (dedicated server) | confirmed (Pterodactyl server, 2026-10-29) | folder switched to the modern layout in v1.1; the Workshop item must be republished to solve it |
| 2 | (mod layout) | Lowercase `data/`, `models/`, `textures/` folders, no `metadata.mod` | minor | confirmed | **fixed** in v1.1 |
| 3 | `Data/CubeBlocks.sbc` (18 `<Icon>`, `<Model>`, `BuildProgressModels` paths) | Path casing differs from the files (`darth_biomech`, `docking_ring_Part.mwm` for `Docking_Ring_part.mwm`, `docking_ring_Sector.dds`…) | minor | assumed (no effect on Windows) | **fixed** in v1.1. The `.mwm` files themselves reference `Textures\darth_biomech\…` in lowercase: cannot be changed without the model sources |
| 4 | `Data/CubeBlocks.sbc:128,218,268,319,369,439` | No `<PCU>` (field added to the game after 2015) | minor | assumed (default not checked) | **fixed** in v1.1 (`115` on the doors, `100` on the merge blocks, like vanilla); `CubeBlock` blocks left at the default |
| 5 | `Data/BlockCategories.sbc:43-54` | The small-grid door `DockingTubeDoorCenterS` was missing from the "Darth Biomech Mods" category | minor | confirmed (missing from the list) | **fixed** in v1.1 |
| 6 | `Models/Darth_Biomech/Docking_ring_door*.mwm`, `*_progress_*.mwm` | The models reference missing `_de` textures (`docking_ring_door_de.dds` from the mod, `default_grey_construction_de.dds` from the game). In DX11 the game uses the `_cm`/`_ng`/`_add` textures, which are present | cosmetic | assumed; the render log reports no missing texture for this mod (2026-10-30) | cannot be changed (binary) |
| 7 | `Data/CubeBlocks.sbc:376-439` | Hidden blocks `DockingTubeSector` and `DockingRing` (3×3, old models): their models only use DX9 textures (`_de`, `_metal_ns`), so probably wrong rendering. Kept so that these blocks are not removed from existing worlds | cosmetic | assumed, to verify in game | not changed |
| 8 | `Data/BlockCategories.sbc:10,29` | The `LargeBlocks` and `SmallBlocks` categories no longer exist in the game: new categories with these names may appear in the G menu | cosmetic | assumed, to verify in game | not changed |
| 9 | `Data/CubeBlocks.sbc:127,169` | `<DamageEffectId>` replaced by `<DamageEffectName>` in the current game; still used by a few vanilla files, so probably accepted | cosmetic | assumed | not changed |
| 10 | `Textures/Darth_Biomech/*_ns.dds`, `*_me.dds` | DX9 textures not referenced by the DX11 models | cosmetic | assumed (string search in the `.mwm` files) | kept (original binaries) |

### In-game load result (2026-10-30, local single player)

Local creative world with the 3 (DX11) mods at v1.1 (`SpaceEngineers.log` of 2026-10-30, 09:38):
the 10 block definitions are created, without any error or warning for this mod.
Second load at 09:58, with the variant groups: `BlockVariantGroups.sbc` is loaded and
post-processed without error or warning.

Seen in game by the tester (2026-10-30): the "Darth Biomech Mods" tab is separate from the rest of
the G menu and the 3 groups show. The 8 public blocks (including the small-grid airlock) were placed,
then the world was saved and reloaded (10:09): all 8 are in the save (16 of each ring piece, 2 to 6
of each airlock). The render log reports no missing texture for the models of this mod, so the
missing `_de` textures (issue 6) have no visible effect in the log.

Docking clamps, tested in game on 2026-10-30 (large grids): merging works, and turning the clamps
off separates the two grids, with a single clamp per grid as well as with a full ring (4 clamps +
4 clamp corners at the junction). On two earlier test ships, the grids stayed merged even with all
clamps off; the problem did not happen again on a new identical ring and most likely came from those
ships (cause not identified). If a junction stays stuck, check that no other block touches the other
ship: grids only separate once no mount point connects them any more.

## To test in game

Details, procedure and results: [IN_GAME_TESTS.md](IN_GAME_TESTS.md).

## History

See [CHANGELOG.md](CHANGELOG.md).

## Steam Workshop description

The player-facing parts of this README are kept in BBCode in [docs/WORKSHOP_DESCRIPTION.txt](docs/WORKSHOP_DESCRIPTION.txt).
