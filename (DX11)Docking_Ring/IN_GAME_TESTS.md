# In-game tests - (DX11)Docking Ring

**Original author:** Darth Biomech  
**Workshop:** https://steamcommunity.com/sharedfiles/filedetails/?id=294619787  
**Mod folder:** `(DX11)Docking_Ring`  
**Tested on build:** 24675677 (Space Engineers 1.210.014)  
**Date of tests:** 2026-10-30  
**Tester:** repository maintainer

## Objective
Verify that v1.1 (modern layout, path casing, PCU, categories, variant groups) loads and works,
without removing blocks from existing worlds.

## Setup
- Local creative world, single player, with the 3 (DX11) Darth Biomech mods at v1.1 as local mods.

## Test checklist
| ID | Test | Expected result | Status (OK/Fail/N/A) | Notes |
|----|------|-----------------|---------------------|-------|
| T01 | Load a world with blocks placed with v1.0 | No blocks disappear, no errors on load | not tested | |
| T02 | Load a world with the mod | No error from this mod in `SpaceEngineers.log` | OK | log of 2026-10-30, 09:38 and 09:58 |
| T03 | G menu | "Darth Biomech Mods" tab separate from the rest, Sector / Clamp / Airlock groups show | OK | tester |
| T04 | Place the 8 public blocks | Icons and models show, blocks place correctly | OK | no missing texture in the render log |
| T05 | Save and reload | The 8 blocks are still there | OK | save of 2026-10-30, 10:09 |
| T06 | Airlocks (4 doors, large and small grid) | Open and close, subparts move | OK | tester |
| T07 | Airlocks: airtightness and sounds | Room stays pressurized when closed, door sounds play | not tested | not checked in detail |
| T08 | Clamps: merge two large grids | Grids merge | OK | tester |
| T09 | Clamps: undock | Turning the clamps off separates the grids | OK | one clamp per grid, then a full ring of 4 clamps + 4 corners. Two earlier test ships stayed merged; not reproduced (see README) |
| T10 | PCU | 115 on the doors, 100 on the clamps | not tested | |
| T11 | Welding / grinding stages | Construction stages show correctly | not tested | not checked in detail |
| T12 | Dedicated server | Server downloads the mod and starts | not tested | needs the republished Workshop item |

## Known issues to watch for
- If a junction stays merged with all clamps off, check whether another block touches the other ship.
- Hidden legacy blocks `DockingTubeSector` and `DockingRing` may render incorrectly in old worlds (README issue 7).

## Results
**Overall:** Passed (local single player)  
**Functional:** Yes (confirmed by a human in-game test, 2026-10-30)  
**Comments:** airtightness, PCU display, dedicated server and v1.0 world migration still to test.
