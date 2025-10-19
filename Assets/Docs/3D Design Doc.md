Design Goals

Upgrade presentation to an isometric 3D view while retaining grid/deck/room data.
Support future multi-floor rooms and consistent deck heights (default per deck).
Keep existing systems: resources, placement, validation, agents.
Lay groundwork for large-scale navigation: graph-based pathfinding, flow fields, vertical connectors.
Maintain extensibility for props, décor layers, crowd/LOD systems.
World Structure

Tile Grid: Each deck remains a 2D grid (Tile.x, Tile.deckLevel). Tiles retain DeckType, TileType, IsBuildable, etc.
Decks: Every deck stored with a heightIndex (int). Default deck height H (e.g., 3 world units). World Y-position for deck d = d * H.
Rooms: RoomDefinition adds:
IsoPrefab (3D model) or IsoSpriteAtlas (isometric sprite)
PropSet (optional list for décor)
EntrancePoints (relative coordinates for agent portals)
VerticalSpan (read from Height; future multi-floor support)
Multi-Floor Rooms:
Still placed via the grid but store RootDeckLevel + Height tiles.
Future rendering: instantiate a prefab that spans multiple deck heights.
Tiles for decks above the root flagged IsMultiLevelTile, referencing the root room for navigation/rendering.
Rendering Pipeline

Base Deck Tiles

For each deck, instantiate floor meshes/sprites using TileVisualData.
Parent under Deck_[level]/Floor.
Room Prefabs / Sprites

For each Room instance:
Calculate world footprint from grid coordinates and deck height.
Instantiate IsoPrefab once at the root deck (handles multi-floor geometry internally) or
instantiate layered iso sprites per tile with correct sorting order.
Parent under Deck_[level]/Rooms.
Props Layer

If PropSet defined, spawn interior props relative to room origin.
Future: allow procedural placement or artist-authored anchor points.
Agents

Agents positioned on deck surfaces (deckLevel * H).
Use animator or billboard sprite aligned to isometric camera.
Parent under Deck_[level]/Agents.
Declutter Tools

Deck visibility toggles: hide decks above selected level or render them transparent.
Ghost mode for multi-floor rooms when editing other decks.
Preview Layer

Placement controller spawns a ghost prefab at candidate location.
Tint green/red using current validation logic; render above rooms but below UI.
Navigation & Interaction

Graph Nodes:

Rooms and corridor segments registered as nodes; edges represent portals (doorways).
Elevators/stairs add edges between decks; metadata stores travel cost, capacity.
Tile Mapping:

Each node references the tiles it occupies, enabling local A* when necessary.
Flow Fields (future):

For common destinations, compute flow fields per deck or per corridor and cache them.
Large crowds follow the shared flow, reducing per-agent pathfinding cost.
Agent Controllers:

High-level path: node graph (rooms/corridors).
Local steering: handle collision avoidance, queueing in narrow corridors, elevator wait logic.
Elevators & Stairs

Dedicated ConnectorDefinition with:
Type (Staircase, Elevator).
Connected decks list.
Entry tile positions per deck.
Visual prefab & animation data.
Capacity/throughput constraints (for future traffic management).
Rendering: multi-level prefab anchored at lowest connected deck.
Navigation: add edges in graph with travel time, queue cost.
Data Additions Summary

class Deck {
    int Level;
    float HeightOffset; // default Level * DefaultDeckHeight
    Tile[] Tiles;
}

class RoomDefinition {
    string RoomId;
    string DisplayName;
    int Width, Height; // Height = number of decks
    GameObject IsoPrefab; // optional
    Sprite[] IsoSprites;  // fallback
    PropSet Props;
    Vector2Int[] EntranceOffsets;
}

class Room {
    string RoomId;
    string RoomDefinitionId;
    int XPosition;
    int DeckLevel;      // root deck
    int Width, Height;  // tiles wide/decks tall
    // existing fields…
}
Camera & UI

Orthographic camera tilted (e.g., 30° x-axis, 45° y-axis) locked to isometric orientation.
Controls: pan, zoom, rotate 90° increments (optional).
Deck selector UI to quickly focus on specific level.
In build mode, highlight valid tiles when hovering over decks.
Performance / Scalability Considerations

Pool reused prefabs (rooms/props/agents).
Batch static geometry per deck via static batching or GPU instancing.
Use LODs: far-away decks/rooms swap to low-poly or sprite representations.
For thousands of agents:
Use hierarchical pathfinding + flow fields.
Crowd simulation for local collision avoidance.
AI LOD: distant decks run scripted behaviors.
Workflow & Tooling

2D deck editor remains primary authoring interface.
Provide “Preview Isometric” toggle in editor to visualize final look.
Validate room prefabs (size vs tile footprint) via editor script.
Optional context menu on ShipView to auto-load all RoomDefinition assets (globally or from designated folder).
Future Extensions

Multi-floor room interiors with camera cutaways (e.g., transparent upper decks when focused on lower floors).
Dynamic lighting per deck or per room.
Prop packs per room category (luxury vs. crew).
Integration with resource UI to show room status overlays in isometric view.
Procedural passenger flow events (e.g., muster drills) using flow fields.
