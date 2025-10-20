# Grid & View Integration Notes

## Current Direction
- Keep `ShipData` as the single source of truth for decks, tiles, rooms, and occupancy.
- Use custom renderers (`ShipView`, `ShipView3D`) as thin views that subscribe to data events or are refreshed by systems that mutate `ShipData`.
- Borrow ideas from SoulGames Easy Grid Builder (ghost previews, heatmaps, per-cell visuals) but avoid embedding the third-party grid runtime directly.

## Functional Zone Architecture
- **Goals**  
  - Treat zones as functional spaces (stateroom, dining, leisure, engine, storage, corridor) that drive capacity, staffing, and economy metrics.  
  - Allow high-level simulation to run on zones even before rooms exist, while still supporting detailed room/furniture authoring where needed.
- **Data Model Changes**  
  - Extend `ShipData` with `ZoneData` records (id, deck, function type, adjacency, state) that index the tiles they own.  
  - Track `RoomData` as children of a zone; store footprint tiles, door connectors, nav-mesh chunk references, and capacity stats.  
  - Register furniture/interaction nodes to their owning room (or directly to the zone for roomless archetypes) with affordance tags and occupancy slots.  
  - Each tile stores `ZoneId` and optional `RoomId` for fast lookup, heatmaps, and hierarchical path queries.
- **Zone Typing Rules**  
  - Define zone archetypes with tunable constraints (min/max area, required service hookups, default room blueprints, staffing needs).  
  - Flag roomless archetypes (engine, storage, utilities) that rely on data-only metrics and spawn functional props without intermediate rooms.  
  - Introduce connector archetypes (corridor, stair, elevator, gangway/exit) with explicit rules for deck links, transfer costs, and occupancy.  
  - Allow archetypes to demand a single room (e.g. engine core, bridge) or lock the room archetype list to a single option so tooling prevents extra rooms.  
  - Capture footprint constraints per archetype (rectangular only, min/max bounds, aspect ratios) so zone validators can reject invalid brush shapes.  
  - Support themed variants (e.g. luxury vs. standard stateroom) by swapping blueprint and furnishing sets without changing simulation logic.
- **Player & Tooling Flow**  
  - Step 1: player paints a zone footprint; validation enforces archetype constraints and registers adjacency.  
  - Step 2: zone optionally instantiates rooms using authored blueprints or procedural kits; designers can override or hand-place rooms.  
  - Step 3: room-level furnishing spawns from modular kits, with manual placement available for customization.  
  - Provide default auto-generation for fast prototyping while keeping edit tools for advanced layouts.
- **Simulation, AI, and Pathfinding**  
  - Maintain a zone graph for coarse routing and high-level scheduling (capacity, staffing, resource flow).  
  - Maintain room graphs per zone for fine navigation; rooms expose door/connectivity data for nav-mesh stitching.  
  - Furniture nodes advertise interaction affordances, enabling agents to resolve tasks down from zone -> room -> furniture.  
  - Roomless zones expose direct interaction nodes or scripted behaviours so AI can still engage with functional props.
- **Visual Dressing**  
  - Zones drive overlays (colour, heatmaps) and influence which modular kits spawn.  
  - Rooms assemble from kit-of-parts prefabs sized to their footprint; corridor spawning follows adjacency rules.  
  - Furnishing spawn points derive from tile metadata (wall, center, corner) to reduce manual hook setup.
- **Implementation Notes**  
  - ✅ Prototype zone authoring UI changes (brushes, selection, validation feedback).  
  - ✅ Define initial `ZoneData`/`RoomData` structs and integration into save/load.  
  - ✅ Update build pipelines and ShipView layers to react to zone/room events before introducing AI behaviours.  
  - ☐ Document which existing prefabs convert into modular kits versus data-only props.  
  - ✅ New scaffolding types (`ShipIdentifiers`, `ShipSpatialData`, `ShipEditCommands`) provide ids, data containers, and command hooks—wire gameplay logic against these as features land.  
  - ✅ Default ship content now seeds `ZoneData`/`RoomData` directly via `CreateRoomFromDefinition`, removing the legacy `Room.Room` layer entirely.
  - ✅ Deck offsets are recorded per deck so multi-level rooms/zones align in ship-space even when deck footprints differ.
  - ✅ Ship class data supports `DefaultZonePlacement` entries so exits/elevators/engine connectors can be authored as pure zones via `CreateZoneArea`.
- **Versioning and State Flow**  
  - Maintain a lightweight memento/command log for zone/room edits so undo/redo and save checkpoints capture the exact operation (brush stroke, door move) rather than raw data blobs.  
  - Store schema version in `ShipData`; on load run migration steps (e.g. legacy `AccessTag` -> new connector/functional zones) before snapshots are replayed.
  - Each edit command generates a memento (before/after, affected ids) enabling quick revert without deep copies of the entire ship.  
  - Partial builds (in-progress rooms/zones) keep their data objects but mark themselves as `IsOperational=false`; nav/AI ignores them until construction completes.  
  - Expose diagnostics hooks so tooling can diff live state vs. last serialized snapshot.

## Data Backbone Sketch
- **Identifiers & Utilities**  
  - `ZoneId`, `RoomId`, `FurnitureNodeId`, `PortalId` as lightweight structs (wrapping ints) generated by `ShipData` so references stay stable across reloads.  
  - `TileCoord` value type (`Deck`, `X`, `Z`) used everywhere to avoid coupling to `ShipTile` instances during batch edits or async jobs.  
  - `ZoneFunctionType`, `RoomArchetypeId`, `ConnectorType` enums describe behaviour expectations and drive validation.
- **ShipData Additions**  
  - `Dictionary<ZoneId, ZoneData> Zones`, `Dictionary<RoomId, RoomData> Rooms`, `Dictionary<FurnitureNodeId, FurnitureNode>` for fast look-ups.  
  - `DeckZoneIndex` per deck caches tile→zone/room mappings for painting tools and renderer overlays.  
  - `ZoneGraphData` stores zone adjacency (including corridor connectors) and invalidates derived caches when zones mutate.  
  - `Dictionary<AgentId, AgentRuntimeState>` holds passengers/crew metadata (home room, current task, cached portal chain, movement flag) so schedulers can tick moving vs. idle agents at different rates.
- **ZoneData**  
  - `ZoneId Id`, `ZoneFunctionType Function`, `int Deck`, `ConnectorType? Connector`, `HashSet<TileCoord> Tiles`.  
  - `List<PortalId> Portals`, `HashSet<ZoneId> AdjacentZones`, occupancy/capacity metrics, optional `ZoneBlueprintId`.  
  - For connector zones: `List<DeckLink>` (deck target, portal, traversal cost, capacity).  
  - Serialization block contains author metadata, visual theme overrides, and build state (under construction, active).
- **RoomData**  
  - `RoomId Id`, `ZoneId Zone`, `RoomArchetypeId Archetype`, `BoundsInt LocalBounds`, `HashSet<TileCoord> Tiles`.  
  - `List<RoomDoor>` (tile coord, facing, linked `PortalId` or aisle room), `Dictionary<PortalId, float> CachedPortalCosts`.  
  - `List<FurnitureNodeId> FurnitureNodes`, capacity stats (beds, seats, service points), service hookups.  
  - Flags for auto-generated vs. player-authored to support regeneration and undo.
- **FurnitureNode**  
  - `FurnitureNodeId Id`, `RoomId Room`, `FurnitureType Type`, `string PrefabId`, `Vector3 LocalPosition`.  
  - `List<Vector3> ApproachOffsets`, `List<string> InteractionTags`, occupancy slots/state.  
  - Optional `ZoneId` reference for roomless zones (engine consoles, cargo cranes).
- **Graph & Cache Containers**  
  - `DeckGraph`: nodes keyed by deck level, edges referencing connector portal costs.  
  - `CorridorZoneGraph`: adjacency list of corridor zones + portal references used by macro BFS.  
  - `PortalRegistry`: `Dictionary<PortalId, ZonePortal>` where portal stores `ZoneId OwnerZone`, `ZoneId LinkedCorridor`, `TileCoord Entry`, optional `PortalId CrossDeck`.  
  - `PortalDistanceCache`: per-zone dictionary of `PortalId -> Dictionary<RoomId, float>`, plus timestamp/hash for invalidation.  
  - `AgentHomeCache`: per-agent data pointing to `RoomId`, preferred `PortalId` chain, last validated tick.
- **Events & Dirty Flags**  
  - `ShipZoneChanged`, `ShipRoomChanged`, `PortalGraphChanged` events fire with diff payloads for renderers, AI, and UI.  
  - Dirty flags per deck and per zone trigger incremental cache rebuilds during end-of-frame sync.
- **Persistence Hooks**  
  - `ShipDataSerializer` writes zone/room/furniture dictionaries; ensure ids survive binary/json saves.  
  - Migration helpers translate legacy access-based `AccessTag`s into functional zones on load.
- **Concurrency Readiness**  
  - Keep core collections (`Zones`, `Rooms`, `FurnitureNodes`) in immutable snapshots during frame execution; mutating commands build a new snapshot at sync points to avoid readers seeing partial states.  
  - Provide `ZoneDataView` structs for Burst/jobs that copy only the fields needed for AI/pathfinding; author jobs against these POD views, not the mutable dictionaries.  
  - Mark derived caches (portal distances, graphs) as job-friendly (contiguous arrays) so future Burst compilations can consume them without GC pressure.  
  - Guard mutations with a scheduler/command queue so edit-time and simulation-time changes serialize deterministically.

## Navigation Hierarchy
- **Graph Layers**  
  - `DeckGraph`: deck nodes with transfer costs for stairs/elevators to bias horizontal travel over vertical hops.  
  - `CorridorZoneGraph`: corridor zones form the global walkable network and expose portals into functional zones.  
  - `ZonePortals`: each corridor-to-zone interface stores corridor zone id, local exit id, and tile footprint so high-level routing can pick the best entry.  
  - `RoomGraph`: rooms (including aisle rooms) attach to one or more portals and cache door connectivity plus shortest distances back to each portal.  
  - `FurnitureNodes`: interaction points register with their owning room, declaring approach positions and affordance tags for AI.
- **Special Connectors**  
  - Stairs and elevators exist as connector zones flagged with vertical links; each connector stores per-deck portal ids plus traversal costs (time, capacity, wait).  
  - Elevators can be modeled as dynamic nodes that queue agents; corridor portals connect to an elevator lobby node which in turn links across decks.  
  - Ship exits/gangways are connector zones that expose portals to world entry/exit points (port terminals, tenders) so AI can leave/board using the same graph.
- **Path Request Flow**  
  1. Resolve target furniture/room/zone candidates based on the task.  
  2. Run BFS/Dijkstra over `DeckGraph` + `CorridorZoneGraph` to select an optimal portal (tie-break on direction, congestion, deck changes).  
  3. Use the target zone's cached portal-distance map to choose the best room doorway; recompute incrementally when rooms or doors change.  
  4. Execute local nav-mesh/A* inside the room from the chosen doorway to the furniture interaction point.  
  5. Cache successful routes per agent/task and invalidate on layout edits; agents with persistent locations (e.g. passengers' staterooms) store their preferred deck/corridor/portal chain so "return home" behaviours skip fresh searches unless topology changes.
- **Large Zone Strategies**  
  - Precompute portal distance maps for large zones; limit recomputation to affected rooms/doors.  
  - Allow sprawling spaces to spawn aisle rooms that subdivide the room graph while still anchoring to corridor portals.  
  - Roomless zones expose portal-linked interaction nodes so they share the same routing pipeline without extra room data.

## ShipView3D Roadmap (High Level)
1. **Deck Grids**  
   - Build a lightweight `DeckVisual` per deck mirroring width/depth.  
   - Maintain tile nodes (position, renderer reference, flags) for fast lookup.
2. **Tile Rendering**  
   - Switch from single quad meshes to per-tile prefabs (already in progress).  
   - Support zone colours, buildable overlays, and selection highlights via material instances or decals.
3. **Interaction Layer**  
   - Introduce a grid interaction controller that handles raycasts, hover info, and placement previews.  
   - Emit events so build tools can request placement/validation without referencing rendering internals.
4. **Visual Overlays**  
   - Implement ghost objects, area highlights, and navigation heatmaps using pooled quads or render textures.
5. **Performance**  
   - Batch or instance tile meshes where possible.  
   - Allow hiding/inactivating decks above/below the active focus deck to keep scene light.

## Renderer Refresh Flow
- `ShipData` raises a coarse `ShipChanged` notification once per edit scope. Payload carries change type (create/update/delete), affected zone/room ids, and a deck dirty set so renderers can batch refresh work.
- Low-level notifications (`ZoneChanged`, `RoomChanged`, `PortalsChanged`) still fire, but are suppressed while a `ShipSpatialEditScope` is active. The scope ends with a single aggregated `ShipChanged`.
- A lightweight `ShipUpdateDispatcher` queues change messages on the main thread and fans them out to subscribers (`ShipView3D`, tooling, future AI observers). This keeps ShipData decoupled from Unity update order and allows batching/deferral if edits arrive off-thread later.
- Deck-level dirty flags live inside `ShipData`; renderers query them to decide which decks to repaint. Decks above/below the active focus can stay dormant until they become visible.
- `ShipView3D` and interaction controllers subscribe to the dispatcher and consume the aggregated change payload, enabling future overlays (ghost previews, heatmaps) to hook in without tight coupling.

## Open Items
- ☐ Decide on furniture data storage; current plan is to introduce a new data layer before visuals depend on it.  
- ☐ Establish undo/redo story once grid interaction tooling is in place.  
- ☐ Lock down zone/room data schemas, archetype definitions, and migration path from access-based zones.  
- ☐ Outline AI task routing requirements so hierarchical pathfinding can be validated early.  
- ☐ Evaluate tooling scope for procedural room generation versus manual prefab placement per zone type.

Keep this document lightweight—update it as systems evolve so the team has a quick reference to current grid/view architecture decisions.
