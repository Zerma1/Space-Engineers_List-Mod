# Contributing

> 🇫🇷 Version française : [CONTRIBUTING.fr.md](CONTRIBUTING.fr.md). This English version is the reference.

Thank you for helping keep these mods alive! The full process is described in [AGENTS.md](AGENTS.md).
It applies to humans and AI agents alike.

## Rules (mandatory)

- **English only.** All documentation meant for people (READMEs, changelogs, pull requests) and all
  commit messages are in English. A French copy (`*.fr.md`) may exist, but the English file is the reference.
  C# code and original identifiers stay as in the original mod.
- **No direct commits to `main`.** Every change goes through its own branch and a pull request.
- **No push and no Workshop publication without explicit approval** from the repository owner.
  This applies to humans and AI agents alike. Before republishing a mod, try to contact the original
  author and follow [NOTICE.md](NOTICE.md).
- **Never change an existing `TypeId` / `SubtypeId`.** It removes the blocks from players' worlds.
- **Do not modify binary files** (`.mwm`, `.dds`, `.jpg`) unless explicitly asked.
- **Keep the original author's credits** (see [NOTICE.md](NOTICE.md)).
- **Mod folder names: no spaces, use underscores (`_`).** Name the folder after the original mod and
  replace every space with `_`, e.g. `(DX11)Docking Ring` → `(DX11)Docking_Ring`. Renaming a folder
  does not change the mod's blocks, but a world that uses it as a local mod must enable it again.
- **Steam Workshop description in BBCode.** Every mod has a `docs/WORKSHOP_DESCRIPTION.txt` inside
  its folder: the player-facing parts of its `README.md` (credits and original Workshop link,
  description, how to use, changes, known limitations, versions, dates) in Steam BBCode, ready to paste
  as the Workshop description. Steam does not render Markdown, and the description is limited to
  8000 characters. Keep it in sync with `README.md`.
- **`Functional` only after a human has tested the mod in game.**
- **No secrets or personal paths** (Steam IDs, user names, tokens) in any file.
- By contributing, you agree that your contribution is published under the [MIT License](LICENSE).

## Quick workflow

1. **Fork** the repository and create a branch:
   - `fix/<mod-name>-<topic>`
   - `import/<mod-name>`
   - `docs/<topic>`
   - `chore/<topic>`
2. **Import a new mod.** The first commit is the mod exactly as downloaded, with no changes.
   Then add `README.md` and `CHANGELOG.md` from [`docs/templates/`](docs/templates/), the
   `docs/WORKSHOP_DESCRIPTION.txt` file, and a row in the table of the root [README](README.md).
3. **Fix a mod.** Start by filling in the "Analysis" section of the mod's README, then make
   **minimal** fixes, one topic per commit.
4. **Document every change.** Add an entry to the mod's `CHANGELOG.md`, update
   `docs/WORKSHOP_DESCRIPTION.txt`, and set the **"Last repo update"** date in the mod's README and
   in the root README table. This is mandatory, even though git already records the date.
5. **Open a pull request** that states:
   - the Space Engineers version used;
   - what was tested in game (single player, dedicated server, multiplayer);
   - what was not tested.

## Reporting a broken mod

Open an issue with the mod name, the Workshop link, the symptom, and an excerpt of
`%AppData%\SpaceEngineers\SpaceEngineers.log` (or of the dedicated server log).

## Commit message format

```text
<type>(<mod>): <short description in English>
```

Types: `import`, `fix`, `chore` (cleanup), `docs`, `feat` (only when requested).
Example: `chore(ForceField): remove the duplicate CubeBlocks - Copy.sbc`.
