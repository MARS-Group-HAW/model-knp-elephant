# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project overview

KrugerNationalPark Elephant Model: an agent-based simulation of South African elephants in the Kruger National
Park savannah, built on the [MARS](https://www.mars-group.org/) simulation framework (`Mars.Life.Simulations`
NuGet package). It models elephant herd movement, feeding, drinking and reproduction against GIS layers for
vegetation biomass, water, temperature, shade and park fencing, to study effects of tourism, resource
availability and climate on elephant population development over time.

## Build & run

Requires the .NET SDK (targets `net6.0` for the runner, `netstandard2.0` for the model library).

```bash
# Build the whole solution
dotnet build KNPElephant.sln

# Run the simulation (from Models/KrugerNationalParkBox)
cd Models/KrugerNationalParkBox
dotnet run --sm config.json
```

`--sm config.json` (or `-sm`) is required and points at the simulation config; without it, `Program.cs` falls
back to reading `config.json` from the current directory. Pass `-l` to enable console logging (off by default).

All simulation inputs (GIS layers as zipped rasters/GeoJSON, agent init CSVs) live in
`Models/KrugerNationalParkBox/resources/` and are referenced by relative path from `config.json`. There is no
test project in this repository.

### Building a distributable "box"

```bash
cd Models/KrugerNationalParkBox
sh ./build.sh
```

Publishes self-contained builds for macOS/Windows/Linux into `KrugerNationalParkBase/`, zips each, and (per the
script) copies a `model_input/` directory alongside the binary — note the actual resources folder is named
`resources`, not `model_input`, so this copy step needs a matching directory or adjustment before it will work
as-is. On macOS, the resulting `KrugerNationalParkBox` binary and its `*.dylib`/`*.dll` files may need
`xattr -d com.apple.quarantine` before they'll run (Gatekeeper quarantine).

## Architecture

Two projects, wired together via a `ProjectReference`:

- **`Models/KrugerNationalPark`** — the model library (`netstandard2.0`). All simulation logic: agents,
  layers, interactions, and output adapters. Depends only on `Mars.Life.Simulations`.
- **`Models/KrugerNationalParkBox`** — the executable runner (`net6.0`, `OutputType=Exe`,
  `AssemblyName=KrugerNationalParkBox`). `Program.cs` is the entry point: it registers layers and agent types
  with a MARS `ModelDescription`, loads `config.json` via `SimulationConfig.Deserialize`, and runs the
  simulation through `SimulationStarter`.

### MARS layer/agent model

The simulation is composed of **layers** (spatial/environmental data or active behavior) and **agents** that
read from and act on those layers. Everything is registered in `Program.cs` in this order:

1. `RasterTempLayer`, `RasterFenceLayer`, `RasterShadeLayer`, `RasterVegetationLayer` — raster GIS layers
   (temperature time series, park fence boundary, shade potential, vegetation/DGVM biomass), loaded from the
   zip files referenced in `config.json`.
2. `VectorWaterLayer` — vector GIS layer of water sources (GeoJSON).
3. `ElephantLayer` (`AbstractActiveLayer`/`ISteppedActiveLayer`) — the active layer that owns the
   `GeoHashEnvironment<Elephant>` spatial index, spawns/registers `Elephant` agents from the CSV configured in
   `config.json`, groups them into `ElephantHerd`s (tracking a `Leading` elephant per herd), and exposes
   `SpawnCalf`/`GetLeadingElephantByHerd` used by elephants at runtime. Historical culling logic lives in
   `PostTick()` but is commented out.
4. `Elephant` (`Agents/Elephant.cs`) is the core agent (`Agent`, `IPositionable`, `ITripSavingAgent`). Its
   `Reason()` method is the per-tick behavior loop: hydration/satiety decay, death conditions (72h without
   water, 24h without food while starving, old age via `YearlyRoutine`), pregnancy/calving every ~730 ticks
   (hours), and hour-of-day-dependent behavior (sleep, seek shade, eat/drink + move). Leading elephants
   (`LeadingElephantAction`) navigate toward remembered water sources or the best nearby vegetation cell and
   respect `RasterFenceLayer` boundaries; following elephants (`ElephantAction`) move toward their herd's
   leader. All distances/movement go through `Position.CalculateRelativePosition`/`GetBearing` and are checked
   against the fence layer before being applied.
5. `WaterSources` gives each leading elephant a simple memory of nearby/known water points, used by
   `TryToDrinkFromWaterhole` and `LeadingElephantAction`.
6. Elephant type/life-stage progression (`ElephantType`, `ElephantLifePeriod`) and cause of death
   (`MattersOfDeath`) are plain enums driving age-dependent food/water needs and behavior branches.

The `Interactions/` folder (`EatLeavesAction`, `EatMarulaFruitsAction`, `EatSeedlingAction`, `PoopAction`,
`PushTreeAction`) contains tree-interaction actions from an earlier Marula-tree feeding model; they are
currently unused (the corresponding call sites in `Elephant.cs` are commented out) but kept for reference.

### Output

`Output/TripsOutputAdapter.PrintTripResult` runs once at the end of `Program.Main`, collecting every agent's
`TripsCollection` (accumulated each tick in `Reason()`) and writing a timestamped `trips_<timestamp>.geojson`
file via a custom NetTopologySuite `GeoJsonWriter` setup (`TripPositionCoordinateConverter`,
`TripsLineConverter`). This is separate from MARS's own CSV output configured via `config.json`
(`"output": "csv"`, per-agent `outputFrequency`/`outputtype`).

### Simulation config (`config.json`)

Declares simulation time bounds/step size (`deltaT`, `startTime`, `endTime`), the GIS layer files to load, and
the agent populations to spawn (file, count, tick frequency, output frequency) — including a `Tourist` agent
type referenced in `config.json` that has no corresponding implementation in this codebase.

## Relationship to the `model-knp` repo

This repo is a `netstandard2.0`/`Mars.Life.Simulations 4.2.1` port of the sibling `model-knp` repo (`net10.0`/
`5.0.0`, C# 12 primary constructors, nullable enabled). The elephant behavior model itself (`Elephant.cs`,
`ElephantHerd`, `WaterSources`, life-stage/death enums, the unused `Interactions/*` files) is unchanged between
the two — differences there are purely syntactic (primary constructors expanded, nullable annotations dropped,
explicit `using`s), a byproduct of the older target framework. A few spots differ for real, all traceable to the
Mars API surface changing between library versions:

- **`VectorWaterLayer.ExploreClosestFullPotentialField`** resolves "nearest water" differently: `model-knp` takes
  the explored feature's geometry centroid, this repo takes the spatial-index node's position. Elephants here can
  resolve to a slightly different water point than in `model-knp` — worth checking first if simulation outputs
  ever need to be compared across the two repos.
- **`TripPositionCoordinateConverter`** inherits from `CoordinateConverter` here vs. `GeometryConverter` in
  `model-knp`, with a matching adjustment in `TripsOutputAdapter`'s converter-removal filter — a serializer
  compatibility fix, output shape should stay equivalent.
- **`Program.cs`** starts the simulation via the simpler `SimulationStarter.Start(...).Run()` here (and honors
  `-l` by actually calling `ActivateConsoleLogging()`), vs. `model-knp`'s manual `Build`/`PrepareSimulation`/
  `StartSimulation` sequence (which leaves console logging commented out even with `-l`).
