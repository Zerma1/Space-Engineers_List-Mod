# Full First Spawn

> 🇫🇷 Version française : [README.fr.md](README.fr.md). This English version is the reference.

| | |
|---|---|
| **Original author** | Zerma1 (created in this repository) |
| **Original Workshop page** | none (created in this repository) |
| **Last original update** | 2026-10-30 (creation) |
| **Last repo update** | 2026-10-30 |
| **Status** | `To test` |
| **Current version** | 1.0 (`metadata.mod`) |
| **Tested on Space Engineers** | not tested |
| **Republished on the Workshop** | no |

## Description

On each player's **first spawn** in the world, fills to the maximum: health, hunger (food),
suit energy, oxygen (O²) and hydrogen (H²).

- Once per identity: a respawn after death is not refilled.
- The identities already served are stored in the world (session variable
  `Zerma_FullFirstSpawn_Identities`), so they are kept across server restarts.
- Players who already spawned before the mod was added are served on their next spawn.
- The script runs on the server (or on the host in single player); the game syncs the values.

## Contents

- `Data/Scripts/FullFirstSpawn/FullFirstSpawn.cs`: session component
- No `.sbc`, models or textures
- Dependencies: none

## How it works

- `MyVisualScriptLogicProvider.PlayerSpawned` event, then a wait of about 2 s (120 ticks) so that
  the values set by the game at spawn are applied first.
- Energy, O² and H²: `SetPlayersEnergyLevel` / `SetPlayersOxygenLevel` / `SetPlayersHydrogenLevel`
  with the value `1` (0..1 ratio; checked in the game code: the energy value is multiplied by the
  suit battery capacity).
- Health and food: `Health` and `Food` stats of the character's `MyEntityStatComponent`, set to their maximum.

## Original license

MIT license of this repository.

## Analysis

| # | File:line | Problem | Severity | Confirmed? |
|---|-----------|---------|----------|------------|
| 1 | `FullFirstSpawn.cs` | The types used (`MyVisualScriptLogicProvider`, `MyEntityStatComponent`, `MyEntityStat`) must be allowed by the ModAPI whitelist | blocking if refused | to check in game |
| 2 | `FullFirstSpawn.cs` | The 2 s delay must be enough to run after the spawn values (medical room, survival kit) | major | to check in game |

Static check: the script compiles against the game DLLs (2026-10-30). This does not prove that it
passes the whitelist.

## To test in game

- [ ] The world loads without a mod error
- [ ] New player: health, food, energy, O², H² at 100 % about 2 s after spawning
- [ ] The log contains `Full First Spawn: stats filled for identity …`
- [ ] Death then respawn: no refill
- [ ] Server restart: a player already served is not refilled
- [ ] Single player
- [ ] Multiplayer / dedicated server

## History

See [CHANGELOG.md](CHANGELOG.md).
