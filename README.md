# Space Engineers — Abandoned Mods, Repaired by the Community

> 🇫🇷 Version française : [README.fr.md](README.fr.md). This English version is the reference.

This repository collects **Space Engineers Steam Workshop mods** that are broken or no longer
updated by their original authors. The goal is to:

1. **Analyze** each mod and document why it is broken;
2. **Fix** it with minimal changes that do not break existing player worlds;
3. **Track** every update in the mod's changelog;
4. Let **the community keep** maintaining them.

## Mods

| Mod | Original author | Workshop | Last original update | Last repo update | Status | Tested on |
|-----|-----------------|----------|----------------------|------------------|--------|-----------|
| [Airtight Force Field Hangar Doors](Airtight%20Force%20Field%20Hangar%20Doors/) | Hannobal | [901800144](https://steamcommunity.com/sharedfiles/filedetails/?id=901800144) | 2019-02-07 | 2026-10-27 | `To test` | — |
| [(DX11)Armor Thrusters](%28DX11%29Armor_Thrusters/) | Darth Biomech | [295393216](https://steamcommunity.com/sharedfiles/filedetails/?id=295393216) | 2015-12-10 | 2026-10-30 | `Functional` | SE 1.210.014 (build 24675677), single player |
| [(DX11)Cryopods](%28DX11%29Cryopods/) | Darth Biomech | [294618647](https://steamcommunity.com/sharedfiles/filedetails/?id=294618647) | 2015-12-17 | 2026-10-30 | `Functional` | SE 1.210.014 (build 24675677), single player |
| [(DX11)Docking Ring](%28DX11%29Docking_Ring/) | Darth Biomech | [294619787](https://steamcommunity.com/sharedfiles/filedetails/?id=294619787) | 2015-10-02 | 2026-10-30 | `Functional` | SE 1.210.014 (build 24675677), single player |
| [Mod Block Tabs](Mod%20Block%20Tabs/) | Zerma1 (created here) | — | 2026-10-30 | 2026-10-30 | `To test` | — |

Statuses: `To analyze` → `Analyzed` → `In repair` → `To test` → `Functional` (or `Blocked`).
Each mod's details (analysis, issues, tests) are in its `README.md`, and its history is in its `CHANGELOG.md`.

## Key rules

The full rules are in [CONTRIBUTING.md](CONTRIBUTING.md) and [AGENTS.md](AGENTS.md). The most important ones:

- **English only** for all documentation meant for people (READMEs, changelogs, commit messages, code comments).
  A French copy (`*.fr.md`) may exist, but the English file is the reference.
- **No direct commits to `main`.** Every change goes through its own branch and a pull request.
- **No push and no Workshop publication without explicit approval** from the repository owner.
  This applies to humans and AI agents alike. Before republishing a mod, also read [NOTICE.md](NOTICE.md).
- **Never change an existing `TypeId` / `SubtypeId`.** It removes the blocks from players' worlds.
- **Minimal fixes only**, one topic per commit. Do not refactor, add features or rebalance unless asked.
- **Do not modify binary files** (`.mwm`, `.dds`, `.jpg`) unless explicitly asked.
- **Keep the original author's credits** and the original Workshop link in every mod README.
- **Update the dates.** "Last repo update" in the mod README and in the table above must change on every change to a mod.
- **`Functional` only after a human has tested the mod in game.** Static checks are not enough.
- **Steam Workshop description in BBCode.** Each mod has a `docs/WORKSHOP_DESCRIPTION.txt` (BBCode, ready to paste
  on the Workshop page, max 8000 characters). It must stay in sync with the player-facing parts of the mod's `README.md`.
- **Link to this repository on the Workshop page.** Once a mod update is pushed, its Steam description
  (`docs/WORKSHOP_DESCRIPTION.txt` and the published Workshop page) must contain a link to this repository:
  https://github.com/Zerma1/Space-Engineers_List-Mod.
- **Mod folder names have no spaces.** Use underscores instead (e.g. `(DX11)Docking_Ring`).
- **No secrets or personal paths** (Steam IDs, user names, tokens) in any file.

## Installing a mod from this repository

1. Download the repository (**Code → Download ZIP**) or clone it.
2. Copy the folder of the mod you want into `%AppData%\SpaceEngineers\Mods\`.
3. In game: **Load Game → Edit Settings → Mods**, then add the mod from the local tab.

> ⚠️ Back up your world before replacing the Workshop version of a mod with a version from this repository.

## Contributing

All help is welcome: reporting a broken mod, proposing a fix, testing in game.

- [CONTRIBUTING.md](CONTRIBUTING.md): how to propose a change
- [AGENTS.md](AGENTS.md): the detailed process (analysis → fix → changelog), also for AI agents
- [docs/GUIDE_MODDING_SE.md](docs/GUIDE_MODDING_SE.md): common breakages and where to read the errors
- [docs/templates/](docs/templates/): README and CHANGELOG templates for a new mod

## License

- The work done in this repository (fixes, documentation, tools) is under the **[MIT License](LICENSE)**.
  Anyone can reuse, modify and republish it, as long as they keep the copyright notice.
- **Original mods remain the property of their authors**, credited in each mod folder.
  See [NOTICE.md](NOTICE.md) before any republication on the Workshop.

Space Engineers is a trademark of Keen Software House. This project is not affiliated with Keen Software House.
