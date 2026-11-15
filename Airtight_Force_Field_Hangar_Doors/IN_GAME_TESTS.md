# In-game tests - Airtight_Force_Field_Hangar_Doors

**Original author:** Hannobal  
**Workshop:** https://steamcommunity.com/sharedfiles/filedetails/?id=901800144  
**Mod folder:** `Airtight_Force_Field_Hangar_Doors`  
**Tested on build:** Steam build 24675677  
**Date of tests:** 2026-10-28 (tests 1, 2 and 3 — test 3 at 10:44 UTC+1)  
**Tester:** user

## Objective
Verify the airtightness / power / collision rework gives the target behaviour without breaking
existing blocks:

| State | Air | Player / object / ship |
|-------|-----|------------------------|
| Switched off or unpowered | passes | pass |
| Active + passable ("Open" switch on) | blocked | pass |
| Active + solid ("Open" switch off) | blocked | blocked |

## Setup
- [ ] Offline creative world, only this mod enabled
- [ ] A sealed room (air vent producing oxygen) whose only opening is closed by a force field door
- [ ] The field powered by a battery you can switch off; a timer block nearby
- [ ] Add `[ffdebug]` to the force field name: its state is shown on screen

## Test checklist
| ID | Test | Expected result | Status (OK/Fail/N/A) | Notes |
|----|------|-----------------|---------------------|-------|
| T01 | Load an existing world with force fields placed before the fix | No block disappears, no mod error; open fields are passable, closed ones solid; Custom Data gets a `ForceField:` line | OK (test 3) |  |
| T02 | Field on, "Open" switch **off** (solid) | Room pressurized; you cannot walk through; a thrown object / block is stopped | **OK** (test 7) | test 3: intangible; tests 5–6: violent push / intangible after open → closed; fixed in test 7 |
| T03 | Field on, "Open" switch **on** (passable) | Room stays pressurized; you can walk and fly through; color changes | OK (test 3) |  |
| T04 | Toggle with the toolbar action, the use key (F), and a timer block "Open On/Off" | Same as T02/T03 each time; the room never depressurizes | OK (test 7) | |
| T05 | Switch the field **off** (On/Off) | Room depressurizes; field invisible; you can pass; the block stays off | OK (test 3) |  |
| T06 | Switch it back **on** | Field reappears in its previous solid/passable state; room repressurizes | OK (test 3) |  |
| T07 | **Cut the power** (battery off, no other source) | Within ~2 s: room depressurizes; field invisible; you can pass | OK (test 3) | instantaneous |
| T08 | **Restore the power** | Within ~2 s: field back in its previous state; room repressurizes | OK (test 3) | instantaneous |
| T09 | Save, reload the world, repeat T02/T03 | State kept after reload | to retest | test 7: right after loading, base and field panel stayed fixed in space on a moving grid (fixed afterwards in the 2026-10-28 rework, to confirm) |
| T10 | Check `%AppData%\SpaceEngineers\SpaceEngineers.log` | No error mentioning `ForceField` | OK (test 3) | script loaded, no error; only "BlockVariants are obsolete" warnings |
| T11 | Multiplayer / dedicated server (if possible) | Same results on host and clients |  |  |
| T12 | Weapons: shoot bullets, flares and rockets at the field | Passable: they go through; solid: they hit the field | **OK** (test 7) | solid field is damaged by the hits |

When a test fails, note the `[ffdebug]` line shown on screen.

## Known issues to watch for
- A damaged (non-functional) field keeps its last airtightness (game limitation).
- The front/back mount points only cover the top block (removed duplicate `CubeBlocks - Copy.sbc`).
- 242 models reference `Textures\Models\H_ForceField_{add,cm,ng}.dds` (only in `tmp\`): pink or
  checkered fields would point to it.
- The airtight faces are the door's front/back faces: if nothing becomes airtight in T02, report it.

## Results

| Run | Date | Power off: air | Power off: objects | On + open: air | On + open: objects | On + closed: air | On + closed: objects |
|-----|------|----------------|--------------------|----------------|--------------------|------------------|----------------------|
| Target | | passes | pass | blocked | pass | blocked | blocked |
| Test 1 (script compile fix only) | 2026-10-28 | blocked ❌ | pass ✅ | blocked ✅ | pass ✅ | blocked ✅ | blocked ✅ |
| Test 2 (first airtightness fix) | 2026-10-28 | blocked ❌ | pass ✅ | blocked ✅ | pass ✅ | blocked ✅ | pass ❌ |
| Test 3 (rework) | 2026-10-28 | passes ✅ | pass ✅ | blocked ✅ | pass ✅ | blocked ✅ | pass ❌ |
| Test 4 (rework, collision left to the game) | 2026-10-28 | — | — | — | — | — | closed field violently pushes everything away; after closed → open → closed, everything passes ❌ |
| Test 5 (collision re-enabled + resync) | 2026-10-28 | — | — | — | — | — | untouched: violent push; after open → closed: intangible ❌ |
| Test 6 (legacy subpart update skipped for collision) | 2026-10-28 | — | — | — | — | — | no push, but jitter; after open → closed: intangible ❌ |
| Test 7 (subparts rebuilt by the game) | 2026-10-28 | passes ✅ | pass ✅ | blocked ✅ | pass ✅ | blocked ✅ | blocked ✅ — after load, visual left behind on a moving grid |

**Overall:** Test 7 passed (air, collision, power, weapons). Only issue: visual subparts left behind after a world load on a moving grid — fixed afterwards in the 2026-10-28 rework, to confirm  
**Functional:** No (only set to Yes by human confirmation)  
**Comments:** Tests 1–3 from the user's report (`test/test_result_28-10-2026.md`). Tests 4–7 reported in chat; positions read from SpaceEngineers.log (ffdebug). Next: reload a world on a moving grid and check the visual (T09).

## Log excerpts (if any)
```
<paste relevant log lines from SpaceEngineers.log>
```
