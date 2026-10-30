# In-game tests - (DX11)Cryopods

**Original author:** Darth Biomech  
**Workshop:** https://steamcommunity.com/sharedfiles/filedetails/?id=294618647  
**Mod folder:** `(DX11)Cryopods`  
**Tested on build:** 24675677 (Space Engineers 1.210.014)  
**Date of tests:** 2026-10-30  
**Tester:** repository maintainer

## Objective
Verify that v1.1 (modern layout, path casing, PCU, categories, variant group) loads and works,
without removing blocks from existing worlds.

## Setup
- Local creative world, single player, with the 3 (DX11) Darth Biomech mods at v1.1 as local mods.

## Test checklist
| ID | Test | Expected result | Status (OK/Fail/N/A) | Notes |
|----|------|-----------------|---------------------|-------|
| T01 | Load a world with blocks placed with v1.0 | No blocks disappear, no errors on load | not tested | |
| T02 | Load a world with the mod | No new error from this mod in `SpaceEngineers.log` | OK | only the 4 known `MOD_ERROR` (README issue 11) |
| T03 | G menu | "Darth Biomech Mods" tab separate from the rest, "Cryogenic Pods" group shows | OK | tester |
| T04 | Place the 8 blocks (large and small grid) | Icons and models show, blocks place correctly | OK | render log: missing texture only on `cryopod_progress_01.mwm` (README issue 6) |
| T05 | Functional pods (large and small): enter | Character inside, pod pressurized | OK | tester |
| T06 | Open pods (cockpit, large and small): sit | Character pose correct, first-person view works | OK | tester |
| T07 | Save and reload | The 8 blocks are still there | OK | save of 2026-10-30, 10:09 |
| T08 | Functional pod: respawn | The pod is offered as a respawn point | not tested | |
| T09 | PCU | 15 on the functional and open pods | not tested | not checked explicitly |
| T10 | Welding / grinding stages | Construction stages show correctly | not tested | not checked in detail |
| T11 | Dedicated server | Server downloads the mod and starts | not tested | needs the republished Workshop item |

## Known issues to watch for
- 4 non-blocking `MOD_ERROR` on load for the decorative pods (README issue 11).
- Missing texture on the construction stage of the pod pack (README issue 6).

## Results
**Overall:** Passed (local single player)  
**Functional:** Yes (confirmed by a human in-game test, 2026-10-30)  
**Comments:** respawn, PCU display, dedicated server and v1.0 world migration still to test.
