# Changelog — (DX11)Cryopods

Format based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/).
Categories: **Fixed**, **Changed**, **Removed**, **Added**.

## [1.1] — 2026-10-30

### Changed
- Folder switched to the modern mod layout: `data/`, `models/`, `textures/` renamed to
  `Data/`, `Models/`, `Textures/`. It can now be republished as a non-legacy Workshop item that
  dedicated servers can download (the original legacy item makes the server fail to start with
  `k_EResultTimeout`).

### Added
- `metadata.mod` (`ModVersion` 1.1).
- `<PCU>15</PCU>` on the 2 functional pods and the 2 cockpits (value of the vanilla cryo chamber
  and passenger seat).
- The 4 small-grid variants in the "Darth Biomech Mods" category (G menu), shared with
  Armor Thrusters and Docking Ring.
- `Data/BlockVariantGroups.sbc`: the 8 pods grouped into a single "Cryogenic Pods" block variant
  group (G menu). Requested improvement, beyond the minimal fix.
- `docs/WORKSHOP_DESCRIPTION.txt`: Steam Workshop description (BBCode), to paste when republishing.
- `README.md`, `CHANGELOG.md` and `IN_GAME_TESTS.md` for the mod, with the analysis.

### Fixed
- Path casing in `CubeBlocks.sbc` and `Animations.sbc` aligned with the folders:
  `models\` → `Models\`, `textures\` → `Textures\`, `darth_biomech` → `Darth_Biomech`.

No `TypeId`/`SubtypeId` changed. The issues that cannot be fixed (construction textures removed
from the game) and the cosmetic issues are documented in the README.

**Tested on:** Space Engineers 1.210.014 (build 24675677) — local single player, creative (2026-10-30).
Loads with 4 non-blocking `MOD_ERROR` already present in the original version (`Computer` component
in the decorative pods, see README issue 11). Tab and group visible in the G menu, 8 blocks placed,
kept after saving and reloading. Tester: entered the 4 pods (functional and open, large and small
grid), character pose correct, pod pressurized. Not tested: respawn, displayed PCU, dedicated server
(republishing needed), migration of a v1.0 world, welding stages in detail.

## [1.0] — 2026-10-30

Original version imported from the Workshop (294618647), author Darth Biomech. No changes.
