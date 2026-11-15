# Zerma - Docking Light

| | |
|---|---|
| **Original author** | Zerma1 (own mod, inspired by Sektan's S - Docking position lights, see Credits) |
| **Workshop page** | [3839922017](https://steamcommunity.com/sharedfiles/filedetails/?id=3839922017) |
| **Last original update** | unknown (Workshop page not checked) |
| **Last repo update** | 2026-11-15 |
| **Status** | `To test` |
| **Current version** | 1.0 (`metadata.mod`) |
| **Tested on Space Engineers** | not tested |
| **Republished on the Workshop** | already published by Zerma1 ([3839922017](https://steamcommunity.com/sharedfiles/filedetails/?id=3839922017)) |

## Credits

- **Idea and globe model:** **S - Docking position lights** by
  [Sektan](https://steamcommunity.com/id/sektan/myworkshopfiles/)
  ([Workshop 379096179](https://steamcommunity.com/sharedfiles/filedetails/?id=379096179), released in 2015).
  That mod used `Reactor` blocks with fixed models to show docking lights.
  The globe model (`Models/Zerma_DockingLight_Globe.mwm`) is taken from that mod.
- **`thumb.jpg`** is also Sektan's image (identical file to the original mod's thumbnail), see Analysis #1.
- **Spotlight models:** Zerma1, based on the vanilla Space Engineers spotlight (Keen Software House).
- **Definitions, scripts, icons and the rest of the mod:** Zerma1.

## Description

A spotlight (large and small grid) that projects a **line of glowing globes** along its beam, to guide
ships to a docking port.

- Adjustable from the terminal: distance of the last globe, spacing, globe size, vertical and horizontal offset.
- Blinking: none, global, outgoing wave (from the lamp) or incoming wave (towards the lamp), with period,
  on-time / wave width and minimum level.
- Toolbar actions: increase / decrease distance, spacing and offsets, with a selectable step.
- The native blink controls of the spotlight are hidden on these blocks (replaced by the mod's ones).
- Power use grows with the number of globes. At most 100 globes (large) / 50 (small) per lamp,
  1500 per client in total.
- Settings are saved in the block and synced in multiplayer (the server checks the player's access).
- Unlocked in progression by building a blueprint projector.

## Contents

- `Data/CubeBlocks.sbc`: `ReflectorLight/Zerma_DockingLight_Large` and `ReflectorLight/Zerma_DockingLight_Small`
- `Data/BlockCategories.sbc`: "Zerma - Docking Light" category
- `Data/EntityComponents.sbc`: `ModStorageComponent/Zerma_DockingLight` (block settings storage)
- `Data/ResearchBlocks.sbc`, `Data/ResearchGroups.sbc`: unlocked by `LargeProjector` / `SmallProjector`
- `Data/Scripts/Zerma_DockingLight/`: `ZdlSession.cs` (session, network channel 47231),
  `ZdlLogic.cs` (block logic, globes, power), `ZdlControls.cs` (terminal controls and actions),
  `ZdlSettings.cs` (limits and settings serialization)
- `Models/`: large and small block (with LOD1-3), globe model; build stages use the game's `LightConstruction_*`
- `Textures/`: 2 icons, globe textures
- Dependencies: none

## Original license

MIT license of this repository for Zerma1's own work (scripts, definitions, icons).
The globe model and `thumb.jpg` belong to Sektan: no license was stated (Workshop page not readable,
to check). The spotlight models are derived from Keen Software House assets. See [NOTICE.md](../NOTICE.md).
Contact with Sektan: not attempted yet.

## Analysis

| # | File:line | Problem | Severity | Confirmed? |
|---|-----------|---------|----------|------------|
| 1 | `thumb.jpg` | Same file as the thumbnail of S - Docking position lights: it is Sektan's image, not Zerma1's | major (rights) | confirmed (identical file) |
| 1b | `Models/Zerma_DockingLight_Globe.mwm` | Globe model taken from Sektan's mod (stated in the Workshop description); Sektan's permission not documented | major (rights) | confirmed by Zerma1 |
| 2 | `Data/CubeBlocks.sbc:13`, `:74` | Comment `[AUTEUR ORIGINAL À COMPLÉTER]`: original author (Sektan) not filled in | cosmetic | confirmed |
| 3 | `Data/CubeBlocks.sbc`, `ZdlControls.cs` | In-game texts (description, terminal labels, tooltips) are in French only | minor | confirmed |

The Workshop descriptions are in `docs/` (`WORKSHOP_DESCRIPTION.en.bbc` is the reference, `WORKSHOP_DESCRIPTION.fr.bbc` the French copy).

Static checks (2026-11-15): the 5 `.sbc` files are valid XML, no duplicate `SubtypeId`, every `<Icon>` and
`<Model>` of the mod exists. Not checked against the ModAPI whitelist in game.

## To test in game

- [ ] The world loads without a mod error
- [ ] Large and small block: build, globe line visible along the beam
- [ ] Distance, spacing, size and offset sliders; the 4 blink modes
- [ ] Toolbar actions and step selection
- [ ] Settings kept after save / reload
- [ ] Multiplayer / dedicated server: settings synced, refused without access

## History

See [CHANGELOG.md](CHANGELOG.md).
