# Zerma - Docking Light

| | |
|---|---|
| **Original author** | Zerma1 (own mod, inspired by an idea from another mod, see Credits) |
| **Workshop page** | [3839922017](https://steamcommunity.com/sharedfiles/filedetails/?id=3839922017) |
| **Last original update** | unknown (Workshop page not checked) |
| **Last repo update** | 2026-11-15 |
| **Status** | `To test` |
| **Current version** | 1.0 (`metadata.mod`) |
| **Tested on Space Engineers** | not tested |
| **Republished on the Workshop** | already published by Zerma1 ([3839922017](https://steamcommunity.com/sharedfiles/filedetails/?id=3839922017)) |

## Credits

The **idea** comes from **S - Docking position lights**
([Workshop 379096179](https://steamcommunity.com/sharedfiles/filedetails/?id=379096179), author: unknown,
to complete). That mod used `Reactor` blocks with fixed models to show docking lights.

Only the idea is reused. The definitions, scripts, models, icons and globe textures of this mod are new,
made by Zerma1. **Exception: `thumb.jpg` is the original mod's image** (identical file), see Analysis #1.

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

MIT license of this repository (code and assets made by Zerma1). `thumb.jpg` belongs to the author of
S - Docking position lights.

## Analysis

| # | File:line | Problem | Severity | Confirmed? |
|---|-----------|---------|----------|------------|
| 1 | `thumb.jpg` | Same file as the thumbnail of S - Docking position lights: it is the original author's image, not Zerma1's | major (rights) | confirmed (identical file) |
| 2 | `Data/CubeBlocks.sbc:13`, `:74` | Comment `[AUTEUR ORIGINAL À COMPLÉTER]`: original author not filled in | cosmetic | confirmed |
| 3 | `Data/CubeBlocks.sbc`, `ZdlControls.cs` | In-game texts (description, terminal labels, tooltips) are in French only | minor | confirmed |

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
