# Changelog — (DX11)Docking Ring

Format based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/).
Categories: **Fixed**, **Changed**, **Removed**, **Added**.

## [1.1] — 2026-10-30

### Changed
- Folder switched to the modern mod layout: `data/`, `models/`, `textures/` renamed to
  `Data/`, `Models/`, `Textures/`. It can now be republished as a non-legacy Workshop item that
  dedicated servers can download (the original legacy item makes the server fail to start with
  `k_EResultTimeout`).
- Repo folder renamed from `(DX11)Docking Ring` to `(DX11)Docking_Ring` (repo rule: no spaces).
  If you use it as a local mod, enable it again in your world after updating.

### Added
- `metadata.mod` (`ModVersion` 1.1).
- `<PCU>115</PCU>` on the 4 doors and `<PCU>100</PCU>` on the 2 merge blocks (vanilla values).
- The small-grid door `DockingTubeDoorCenterS` in the "Darth Biomech Mods" category (G menu),
  shared with Armor Thrusters and Cryopods.
- `Data/BlockVariantGroups.sbc`: 3 block variant groups in the G menu, **Docking Ring Sector**
  (sector + corner), **Docking Ring Clamp** (clamp + corner) and **Docking Ring Airlock** (4 doors).
  Requested improvement, beyond the minimal fix.
- `docs/WORKSHOP_DESCRIPTION.txt`: Steam Workshop description (BBCode), to paste when republishing.
- `README.md`, `CHANGELOG.md` and `IN_GAME_TESTS.md` for the mod, with the analysis.

### Fixed
- Casing of the 18 `<Icon>`, `<Model>` and `BuildProgressModels` paths in `CubeBlocks.sbc`
  aligned with the actual file names.

No `TypeId`/`SubtypeId` changed; the 2 hidden blocks (`DockingTubeSector`, `DockingRing`) are kept.
The issues that cannot be fixed (textures referenced by the models) and the cosmetic issues are
documented in the README.

**Tested on:** Space Engineers 1.210.014 (build 24675677) — local single player, creative (2026-10-30).
Loads without error or warning for this mod. Tab and 3 groups visible in the G menu, 8 public blocks
placed, kept after saving and reloading. Tester: opened and closed the 4 airlocks (large and small
grid), merged two large grids with the clamps, and separated them by turning the clamps off (one clamp
per grid, then a full ring of 4 clamps + 4 corners). Not tested: airtightness and sounds of the
airlocks, welding stages in detail, displayed PCU, dedicated server (republishing needed), migration
of a v1.0 world.

## [1.0] — 2026-10-30

Original version imported from the Workshop (294619787), author Darth Biomech. No changes.
