# Changelog — (DX11)Armor Thrusters

Format based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/).
Categories: **Fixed**, **Changed**, **Removed**, **Added**.

## [1.1] — 2026-10-30

### Changed
- Folder switched to the modern mod layout: `data/`, `models/`, `textures/` renamed to
  `Data/`, `Models/`, `Textures/`. It can now be republished as a non-legacy Workshop item that
  dedicated servers can download (the original legacy item makes the server fail to start with
  `k_EResultTimeout`).
- Repo folder renamed from `(DX11)Armor Thrusters` to `(DX11)Armor_Thrusters` (repo rule: no spaces).
  If you use it as a local mod, enable it again in your world after updating.

### Added
- `metadata.mod` (`ModVersion` 1.1).
- `<ThrusterType>Ion</ThrusterType>` and `<PCU>12</PCU>` on the 4 blocks (values of the vanilla
  small ion thruster).
- `Data/BlockVariantGroups.sbc`: the 4 thrusters grouped into a single "Armor Thrusters" block
  variant group (G menu). Requested improvement, beyond the minimal fix.
- `docs/WORKSHOP_DESCRIPTION.txt`: Steam Workshop description (BBCode), to paste when republishing.
- `README.md`, `CHANGELOG.md` and `IN_GAME_TESTS.md` for the mod, with the analysis.

### Fixed
- Path casing in `CubeBlocks.sbc` aligned with the files:
  `Textures\darth_biomech\…` → `Textures\Darth_Biomech\…`,
  `small_armor_thruster.mwm` → `Small_armor_thruster.mwm`.

No `TypeId`/`SubtypeId` changed. The cosmetic issues (old categories, obsolete tags, "Small" name
of the large block) are documented in the README but not changed.

**Tested on:** Space Engineers 1.210.014 (build 24675677) — local single player, creative (2026-10-30).
Loads without error (log), tab and group visible in the G menu, 4 blocks placed (large and small
grid), thrust, blue flame and PCU 12 checked by the tester, blocks kept after saving and reloading.
Not tested: dedicated server (republishing needed), migration of a v1.0 world, welding stages in detail.

## [1.0] — 2026-10-30

Original version imported from the Workshop (295393216), author Darth Biomech. No changes.
