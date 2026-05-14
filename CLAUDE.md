# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**deliverrr** is a 2D top-down delivery game built in Unity 6 (6000.3.8f1), targeting WebGL. The player drives a vehicle to pick up packages and deliver them to matching dropzones.

## Development Workflow

This is a Unity project — there are no CLI build/test commands. All development is done through the Unity Editor:

- **Run:** Open project in Unity 6, press Play in the Editor
- **Build:** File → Build Settings → WebGL → Build
- **Tests:** Window → General → Test Runner (Unity Test Framework 1.6.0 is installed)
- **Hot reload:** FastScriptReload plugin is installed for iterative development without full recompile

Scripts are compiled to `Temp/bin/Debug/Assembly-CSharp.dll`. All source is C# 9.0 targeting .NET Standard 2.1.

## Architecture

### Procedural Level Generation
Levels are generated at runtime by `CityGenerator.cs` (execution order -100). The seed flows from `LevelSeedConfig` (static, scene-persistent) → `CityGenerator.Awake()` → `PackageManager.InitPackages()` → `PackageManager.Start()`.

Generation pipeline:
1. **Graph** — DFS spanning tree over a `gridWidth × gridHeight` grid, then ~30% of remaining edges re-added for loops. Dead-end nodes (degree 1) become package spawns; non-dead-ends become dropzones.
2. **Tilemap painting** — `stride = blockSize + roadWidth` (default 9). Road tilemap gets intersections + active corridors. Ground tilemap gets 8-way sidewalk border around road, then grass fill for all remaining interior and exterior tiles. A solid fence `boundsTileThickness` tiles thick is painted on a separate Bounds tilemap just outside the map.
3. **Obstacles** — BoostPads then Barriers are placed at shuffled corridor midpoints (guaranteed on-road, no overlap with packages/dropzones).
4. **Camera confiner** — `PolygonCollider2D` path is set in local space using `InverseTransformPoint`, extended `boundsTileThickness + confinerPadding` tiles beyond the map edge. `CinemachineConfiner2D.Damping` is forced to `0` to prevent fighting the position composer.

**Script execution order** (Edit → Project Settings → Script Execution Order):
- `CityGenerator`: `-100`
- `PackageManager`: `-50`

`PackageManager.InitPackages()` must be called before `PackageManager.Start()`. `Package.cs` fetches its components in `Awake()` (not `Start()`) so they are available when `PackageManager.Start()` configures them.

### Game Loop
`PackageManager.cs` owns the game state: it tracks all `Package` and dropzone objects (counts must match), runs the delivery timer, and signals completion. The player vehicle uses `Delivery.cs` to detect collisions with tagged objects (`Packages` tag, `Dropzone` tag), notifying PackageManager on pickup/dropoff.

### Two Vehicle Systems
There are two separate vehicle implementations — only one should be active at a time:
- **`VehicleController.cs`** — Keyboard-driven (W/A/S/D), smooth acceleration curves, wheel angle visualization
- **`PhysicsVehicle.cs`** — Rigidbody2D physics, mouse-aim rotation, left-click to accelerate, right-click/space to reverse

Both read from the tilemap to apply a 50% speed penalty on certain ground tiles.

### Key Components

| Script | Role |
|---|---|
| `CityGenerator.cs` | Procedural road graph + tilemap painter + pickup/obstacle spawner |
| `LevelSeedConfig.cs` | Static seed carrier; persists across scene loads |
| `LevelSelectManager.cs` | Level select UI — reads seed input or preset, loads game scene |
| `PackageManager.cs` | Core game state, timer (MM:SS.MS), package/dropzone tracking |
| `Delivery.cs` | Vehicle collision handler for pickups and dropoffs |
| `Package.cs` | Per-package state (pickupable, collider, sprite, ID matching) |
| `VehicleInteractable.cs` | Speed boost pads — 1.5× multiplier for 5s, one-time use |
| `ScreenSpaceIndicator.cs` | Off-screen arrow indicators clamped to screen edges |
| `PointerIndicatorController.cs` | Instantiates indicators for packages |
| `Speedometer.cs` | HUD gauge with speed-based color (white → red) |
| `SharpUtils.cs` | Shared utilities; contains `Remap()` helper |

### Scene & Prefab Layout
- Main scene: `Assets/Bruh.unity`
- Generation scripts: `Assets/Scripts/Generation/` (`CityGenerator.cs`, `LevelSeedConfig.cs`)
- Prefabs organized under `Assets/Prefabs/`: `Gameplay/`, `Managers/`, `UI/`, `Vehicle/`, `Levels/`
- Sprites sourced from kenney_roguelike-modern-city asset pack

## Key Packages
- **Cinemachine 3.1.5** — camera follow
- **Input System 1.18.0** — input handling
- **Universal Render Pipeline (URP) 17.3.0** — rendering
- **2D Tilemap + Extras** — level ground with speed zones
- **Aseprite Importer 3.0.1** — sprite import pipeline
