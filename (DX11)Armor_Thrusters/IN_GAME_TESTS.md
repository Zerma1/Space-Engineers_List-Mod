# In-game tests - (DX11)Armor Thrusters

**Original author:** Darth Biomech  
**Workshop:** https://steamcommunity.com/sharedfiles/filedetails/?id=295393216  
**Mod folder:** `(DX11)Armor_Thrusters`  
**Tested on build:** 24675677 (Space Engineers 1.210.014)  
**Date of tests:** 2026-10-30  
**Tester:** repository maintainer

## Objective
Verify that v1.1 (modern layout, path casing, thruster type, PCU, variant group) loads and works,
without removing blocks from existing worlds.

## Setup
- Local creative world, single player, with the 3 (DX11) Darth Biomech mods at v1.1 as local mods.

## Test checklist
| ID | Test | Expected result | Status (OK/Fail/N/A) | Notes |
|----|------|-----------------|---------------------|-------|
| T01 | Load a world with blocks placed with v1.0 | No blocks disappear, no errors on load | not tested | |
| T02 | Load a world with the mod | No error from this mod in `SpaceEngineers.log` | OK | log of 2026-10-30, 09:38, 09:58, 10:05 |
| T03 | G menu | "Darth Biomech Mods" tab separate from the rest, "Armor Thrusters" group shows | OK | tester |
| T04 | Place the 4 blocks (large and small grid) | Icons and models show, blocks place correctly | OK | no missing texture in the render log |
| T05 | Thrust | Thrusters push the grid, blue (ion) flame, PCU 12 | OK | tester |
| T06 | Save and reload | The 4 blocks are still there | OK | save of 2026-10-30, 10:12 |
| T07 | Welding / grinding stages | Construction stages show correctly | not tested | not checked in detail |
| T08 | Dedicated server | Server downloads the mod and starts | not tested | needs the republished Workshop item |

## Known issues to watch for
- Old `LargeBlocks` / `SmallBlocks` / `ShipThrusters` categories may add extra tabs in the G menu (README issue 5).
- The large blocks are named "Small Armor Thrust" / "Small Armor Sloped Thrust" (README issue 7).

## Results
**Overall:** Passed (local single player)  
**Functional:** Yes (confirmed by a human in-game test, 2026-10-30)  
**Comments:** dedicated server and v1.0 world migration still to test.
