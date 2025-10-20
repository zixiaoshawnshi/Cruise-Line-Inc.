using System;
using System.Collections.Generic;
using System.Linq;
using CruiseLineInc.Ship.Data;
using CruiseLineInc.Room.Data;
using UnityEngine;

namespace CruiseLineInc.Ship
{
    /// <summary>
    /// Pure data class representing the entire ship structure.
    /// No MonoBehaviour - just data that can be queried, modified, and serialized.
    /// </summary>
    [Serializable]
    public class ShipData
    {
        #region Fields
        
        public const int CurrentSchemaVersion = 1;

        public string ShipName;
        public string ClassName;
        public Deck[] Decks;

        public int SchemaVersion { get; private set; } = CurrentSchemaVersion;
        
        // Capacity
        public int MaxPassengers;
        public int MaxCrew;
        public int TotalTiles;
        
        // Resources
        public float WaterCapacity;
        public float FoodCapacity;
        public float WasteCapacity;
        public float FuelCapacity;
        
        // Hierarchical zone data (new system scaffolding)
        public Dictionary<ZoneId, ZoneData> Zones { get; } = new Dictionary<ZoneId, ZoneData>();
        public Dictionary<RoomId, RoomData> ZoneRooms { get; } = new Dictionary<RoomId, RoomData>();
        public Dictionary<FurnitureNodeId, FurnitureNode> FurnitureNodes { get; } = new Dictionary<FurnitureNodeId, FurnitureNode>();
        public Dictionary<PortalId, ZonePortal> ZonePortals { get; } = new Dictionary<PortalId, ZonePortal>();
        public Dictionary<AgentId, AgentRuntimeState> AgentStates { get; } = new Dictionary<AgentId, AgentRuntimeState>();
        public Dictionary<int, DeckZoneIndex> DeckZoneIndices { get; } = new Dictionary<int, DeckZoneIndex>();
        public ZoneGraphData ZoneGraph { get; } = new ZoneGraphData();
        public PortalDistanceCache PortalDistanceCache { get; } = new PortalDistanceCache();
        public List<ShipEditMemento> CommandLog { get; } = new List<ShipEditMemento>();

        private static readonly Vector2Int[] TileNeighbourOffsets =
        {
            new Vector2Int(1, 0),
            new Vector2Int(-1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(0, -1)
        };

        private int _nextZoneId = 1;
        private int _nextRoomId = 1;
        private int _nextFurnitureNodeId = 1;
        private int _nextPortalId = 1;
        private int _nextAgentId = 1;
        private int _commandSequence;

        private readonly Stack<IShipEditCommand> _undoStack = new Stack<IShipEditCommand>();
        private readonly Stack<IShipEditCommand> _redoStack = new Stack<IShipEditCommand>();

        #endregion

        #region Events

        public event Action<ZoneId, ZoneData> ZoneChanged;
        public event Action<RoomId, RoomData> RoomChanged;
        public event Action PortalsChanged;

        #endregion
        
        #region Constructor
        
        public ShipData(string shipName, string className, int maxPassengers, int maxCrew)
        {
            ShipName = shipName;
            ClassName = className;
            MaxPassengers = maxPassengers;
            MaxCrew = maxCrew;
            Decks = new Deck[0];
            SchemaVersion = CurrentSchemaVersion;
        }
        
        #endregion
        
        #region Queries
        
        /// <summary>
        /// Gets a deck by level
        /// </summary>
        public Deck GetDeck(int deckLevel)
        {
            foreach (Deck deck in Decks)
            {
                if (deck.DeckLevel == deckLevel)
                    return deck;
            }
            return null;
        }
        
        /// <summary>
        /// Gets a ShipTile at specific position (x, z, deck).
        /// </summary>
        public ShipTile GetTile(int xPosition, int zPosition, int deckLevel)
        {
            Deck deck = GetDeck(deckLevel);
            return deck?.GetTile(xPosition, zPosition);
        }

        /// <summary>
        /// Legacy helper for 1D decks (z = 0).
        /// </summary>
        public ShipTile GetTile(int xPosition, int deckLevel) => GetTile(xPosition, 0, deckLevel);
        
        /// <summary>
        /// Gets all tiles in the ship
        /// </summary>
        public List<ShipTile> GetAllTiles()
        {
            List<ShipTile> allTiles = new List<ShipTile>();
            foreach (Deck deck in Decks)
            {
                foreach (ShipTile tile in deck.Tiles)
                    allTiles.Add(tile);
            }
            return allTiles;
        }
        
        #endregion
        
        #region Room Placement
        
        /// <summary>
        /// Checks if a single-deck room can be placed
        /// </summary>
        public bool CanPlaceRoom(int xPosition, int zPosition, int width, int depth, int deckLevel, TileType[] requiredTileTypes = null)
        {
            Deck deck = GetDeck(deckLevel);
            if (deck == null)
                return false;

            return deck.CanPlaceRoom(xPosition, zPosition, width, depth, requiredTileTypes);
        }

        public bool CanPlaceRoom(int xPosition, int width, int deckLevel, TileType[] requiredTileTypes = null) =>
            CanPlaceRoom(xPosition, 0, width, 1, deckLevel, requiredTileTypes);
        
        /// <summary>
        /// Checks if a multi-level room can be placed
        /// </summary>
        public bool CanPlaceMultiLevelRoom(int xPosition, int zPosition, int width, int depth, int startDeckLevel, int deckHeight, TileType[] requiredTileTypes = null)
        {
            for (int d = 0; d < deckHeight; d++)
            {
                int deckLevel = startDeckLevel + d;
                Deck deck = GetDeck(deckLevel);
                
                if (deck == null)
                    return false;
                
                if (!deck.CanPlaceRoom(xPosition, zPosition, width, depth, requiredTileTypes))
                    return false;
            }
            
            return true;
        }

        public bool CanPlaceMultiLevelRoom(int xPosition, int width, int startDeckLevel, int deckHeight, TileType[] requiredTileTypes = null) =>
            CanPlaceMultiLevelRoom(xPosition, 0, width, 1, startDeckLevel, deckHeight, requiredTileTypes);
        
        #endregion
        
        #region Room Management

        /// <summary>
        /// Creates a zone and room pair from a room definition and footprint.
        /// </summary>
        public RoomData CreateRoomFromDefinition(RoomDefinition roomDefinition, int xPosition, int zPosition, int deckLevel, bool isOperational = true, bool isDefaultPlacement = false, bool markAutoGenerated = true, bool isDeletable = true)
        {
            if (roomDefinition == null)
                throw new ArgumentNullException(nameof(roomDefinition));

            int width = Mathf.Max(1, roomDefinition.Width);
            int length = Mathf.Max(1, roomDefinition.Length);
            int height = Mathf.Max(1, roomDefinition.Height);

            TileType[] requiredTiles = (roomDefinition.AllowedTileTypes != null && roomDefinition.AllowedTileTypes.Count > 0)
                ? roomDefinition.AllowedTileTypes.ToArray()
                : null;

            if (!CanPlaceMultiLevelRoom(xPosition, zPosition, width, length, deckLevel, height, requiredTiles))
            {
                Debug.LogWarning($"Cannot place room '{roomDefinition.DisplayName}' at ({xPosition},{zPosition}) deck {deckLevel} - footprint blocked.");
                return null;
            }

            ZoneData zone = CreateZone(roomDefinition.ZoneFunction, deckLevel);
            zone.IsOperational = isOperational;
            zone.ZoneBlueprintId = roomDefinition.RoomId;
            zone.IsDefaultPlacement = isDefaultPlacement;
            zone.Origin = new Vector3Int(xPosition, deckLevel, zPosition);
            zone.Size = new Vector2Int(width, length);

            RoomData room = CreateRoom(zone.Id, RoomArchetypeId.Invalid);
            room.IsOperational = isOperational;
            room.IsAutoGenerated = markAutoGenerated;
            room.IsDeletable = isDeletable;
            room.Footprint = new BoundsInt(
                new Vector3Int(xPosition, deckLevel, zPosition),
                new Vector3Int(width, height, length));
            room.CapacityStats["Occupancy"] = roomDefinition.Capacity;
            room.CapacityStats["Comfort"] = roomDefinition.ComfortRating;
            room.CapacityStats["Noise"] = roomDefinition.NoiseLevel;

            zone.Metrics["NoiseLevel"] = roomDefinition.NoiseLevel;
            zone.Metrics["MaintenanceCost"] = roomDefinition.MaintenanceCost;
            zone.Metrics["BuildCost"] = roomDefinition.BuildCost;
            if (height > 1)
            {
                zone.Metrics["StackHeight"] = height;
                zone.Metrics["StackIndex"] = 0f;
            }

            OccupyRoomTiles(zone, room);

            RaiseRoomChanged(room.Id, room);
            RaiseZoneChanged(zone.Id, zone);
            PortalDistanceCache.Clear();

            return room;
        }

        private void OccupyRoomTiles(ZoneData zone, RoomData room)
        {
            room.Tiles.Clear();
            BoundsInt footprint = room.Footprint;
            bool isMultiLevel = footprint.size.y > 1;
            int rootDeck = footprint.position.y;

            for (int y = 0; y < footprint.size.y; y++)
            {
                int deckLevel = rootDeck + y;
                Deck deck = GetDeck(deckLevel);
                if (deck == null)
                    continue;

                deck.OccupyTiles(footprint.position.x, footprint.position.z, footprint.size.x, footprint.size.z, room.Id, isMultiLevel, rootDeck);

                DeckZoneIndex index = GetOrCreateDeckZoneIndex(deckLevel);

                for (int x = 0; x < footprint.size.x; x++)
                {
                    for (int z = 0; z < footprint.size.z; z++)
                    {
                        TileCoord coord = new TileCoord(deckLevel, footprint.position.x + x, footprint.position.z + z);
                        zone.Tiles.Add(coord);
                        room.Tiles.Add(coord);
                        index.SetZone(coord, zone.Id);
                        index.SetRoom(coord, room.Id);
                    }
                }
            }

            RebuildZoneAdjacency(zone);
        }

        private void ReleaseRoomTiles(RoomData room)
        {
            BoundsInt footprint = room.Footprint;
            int rootDeck = footprint.position.y;

            for (int y = 0; y < footprint.size.y; y++)
            {
                int deckLevel = rootDeck + y;
                Deck deck = GetDeck(deckLevel);
                if (deck != null)
                {
                    deck.ClearTiles(footprint.position.x, footprint.position.z, footprint.size.x, footprint.size.z);
                }

                if (DeckZoneIndices.TryGetValue(deckLevel, out DeckZoneIndex index))
                {
                    for (int x = 0; x < footprint.size.x; x++)
                    {
                        for (int z = 0; z < footprint.size.z; z++)
                        {
                            TileCoord coord = new TileCoord(deckLevel, footprint.position.x + x, footprint.position.z + z);
                            index.RemoveZone(coord);
                            index.RemoveRoom(coord);
                        }
                    }
                }
            }

            if (Zones.TryGetValue(room.ZoneId, out ZoneData zone))
            {
                foreach (TileCoord coord in room.Tiles)
                {
                    zone.Tiles.Remove(coord);
                }

                RebuildZoneAdjacency(zone);
            }

            room.Tiles.Clear();
        }

        public ZoneData CreateZoneArea(
            ZoneFunctionType functionType,
            int deckLevel,
            int xPosition,
            int zPosition,
            int width,
            int length,
            bool isOperational = true,
            bool isDefaultPlacement = false,
            bool isDeletable = true,
            ConnectorType? connector = null,
            string blueprintId = null)
        {
            width = Mathf.Max(1, width);
            length = Mathf.Max(1, length);

            Deck deck = GetDeck(deckLevel);
            if (deck == null)
            {
                Debug.LogWarning($"Cannot create zone {functionType} on deck {deckLevel}: deck not found.");
                return null;
            }

            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < length; z++)
                {
                    int tileX = xPosition + x;
                    int tileZ = zPosition + z;
                    if (!deck.IsValidPosition(tileX, tileZ) || !deck.IsActiveTile(tileX, tileZ))
                    {
                        Debug.LogWarning($"Cannot create zone {functionType} at ({tileX},{tileZ}) on deck {deckLevel}: out of bounds or inactive.");
                        return null;
                    }
                }
            }

            ZoneData zone = CreateZone(functionType, deckLevel);
            zone.IsOperational = isOperational;
            zone.IsDefaultPlacement = isDefaultPlacement;
            zone.IsDeletable = isDeletable;
            zone.Connector = connector;
            zone.ZoneBlueprintId = blueprintId;
            zone.Origin = new Vector3Int(xPosition, deckLevel, zPosition);
            zone.Size = new Vector2Int(width, length);

            DeckZoneIndex deckIndex = GetOrCreateDeckZoneIndex(deckLevel);

            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < length; z++)
                {
                    TileCoord coord = new TileCoord(deckLevel, xPosition + x, zPosition + z);
                    zone.Tiles.Add(coord);
                    deckIndex.SetZone(coord, zone.Id);
                }
            }

            RebuildZoneAdjacency(zone);
            RaiseZoneChanged(zone.Id, zone);
            PortalDistanceCache.Clear();

            return zone;
        }

        public IEnumerable<RoomData> GetRoomsOnDeck(int deckLevel)
        {
            foreach (RoomData room in ZoneRooms.Values)
            {
                BoundsInt footprint = room.Footprint;
                int minDeck = footprint.position.y;
                int maxDeck = footprint.position.y + footprint.size.y - 1;
                if (deckLevel >= minDeck && deckLevel <= maxDeck)
                {
                    yield return room;
                }
            }
        }

        public RoomData GetRoomAtPosition(int x, int z, int deckLevel)
        {
            TileCoord coord = new TileCoord(deckLevel, x, z);
            if (DeckZoneIndices.TryGetValue(deckLevel, out DeckZoneIndex index) &&
                index.TryGetRoom(coord, out RoomId roomId) &&
                ZoneRooms.TryGetValue(roomId, out RoomData room))
            {
                return room;
            }

            return null;
        }

        public bool TryGetZoneAtPosition(int deckLevel, int x, int z, out ZoneData zone)
        {
            zone = null;
            TileCoord coord = new TileCoord(deckLevel, x, z);
            if (!DeckZoneIndices.TryGetValue(deckLevel, out DeckZoneIndex index))
                return false;

            if (!index.TryGetZone(coord, out ZoneId zoneId))
                return false;

            if (!Zones.TryGetValue(zoneId, out ZoneData zoneData))
                return false;

            zone = zoneData;
            return true;
        }

        public void LogVerticalConnectorStacks()
        {
            var connectorZones = Zones.Values
                .Where(z => z.Connector.HasValue && (z.Connector.Value == ConnectorType.Elevator || z.Connector.Value == ConnectorType.Stair))
                .ToList();

            if (connectorZones.Count == 0)
            {
                Debug.Log("[ShipData] No vertical connector zones (elevator/stair) found.");
                return;
            }

            foreach (var group in connectorZones.GroupBy(z => string.IsNullOrEmpty(z.ZoneBlueprintId) ? $"Connector_{z.Id.Value}" : z.ZoneBlueprintId))
            {
                List<ZoneData> ordered = group.OrderBy(z => z.Deck).ToList();
                string stackName = group.Key;
                string connectorName = ordered[0].Connector?.ToString() ?? "Connector";

                if (ordered.Count <= 1)
                {
                    Debug.LogWarning($"[ShipData] {connectorName} stack '{stackName}' only has one slice (deck {ordered[0].Deck}).");
                    continue;
                }

                bool allLinked = true;
                for (int i = 1; i < ordered.Count; i++)
                {
                    ZoneData current = ordered[i];
                    ZoneData previous = ordered[i - 1];

                    bool hasDownLink = current.DeckLinks != null && current.DeckLinks.Any(link => link.TargetDeck == previous.Deck);
                    bool hasUpLink = previous.DeckLinks != null && previous.DeckLinks.Any(link => link.TargetDeck == current.Deck);

                    if (!hasDownLink || !hasUpLink)
                    {
                        allLinked = false;
                        Debug.LogWarning($"[ShipData] {connectorName} stack '{stackName}' is missing deck links between decks {previous.Deck} and {current.Deck}.");
                    }
                }

                if (allLinked)
                {
                    string deckList = string.Join(", ", ordered.Select(z => z.Deck));
                    Debug.Log($"[ShipData] {connectorName} stack '{stackName}' connected across decks [{deckList}].");
                }
            }
        }

        #endregion

        #region Zone System (Scaffolding)

        public ZoneId AllocateZoneId() => new ZoneId(_nextZoneId++);
        public RoomId AllocateRoomId() => new RoomId(_nextRoomId++);
        public FurnitureNodeId AllocateFurnitureNodeId() => new FurnitureNodeId(_nextFurnitureNodeId++);
        public PortalId AllocatePortalId() => new PortalId(_nextPortalId++);
        public AgentId AllocateAgentId() => new AgentId(_nextAgentId++);

        public DeckZoneIndex GetOrCreateDeckZoneIndex(int deckLevel)
        {
            if (!DeckZoneIndices.TryGetValue(deckLevel, out DeckZoneIndex index))
            {
                index = new DeckZoneIndex(deckLevel);
                DeckZoneIndices[deckLevel] = index;
            }

            return index;
        }

        public ZoneData CreateZone(ZoneFunctionType functionType, int deckLevel)
        {
            ZoneId id = AllocateZoneId();
            ZoneData zone = new ZoneData
            {
                Id = id,
                FunctionType = functionType,
                Deck = deckLevel
            };

            Zones[id] = zone;
            return zone;
        }


        public bool TryGetZone(ZoneId id, out ZoneData zoneData) => Zones.TryGetValue(id, out zoneData);

        public bool RemoveZone(ZoneId id)
        {
            if (!Zones.TryGetValue(id, out ZoneData zone))
                return false;

            RoomId[] childRooms = zone.Rooms.ToArray();
            foreach (RoomId childRoom in childRooms)
            {
                RemoveRoom(childRoom);
            }

            RemoveZoneAdjacencyLinks(zone, true);

            Zones.Remove(id);
            foreach (TileCoord coord in zone.Tiles)
            {
                if (DeckZoneIndices.TryGetValue(coord.Deck, out DeckZoneIndex index))
                {
                    index.RemoveZone(coord);
                }
            }

            RaiseZoneChanged(id, zone);
            PortalDistanceCache.Clear();
            return true;
        }

        public RoomData CreateRoom(ZoneId zoneId, RoomArchetypeId archetype)
        {
            RoomId roomId = AllocateRoomId();
            RoomData room = new RoomData
            {
                Id = roomId,
                ZoneId = zoneId,
                Archetype = archetype
            };

            ZoneRooms[roomId] = room;

            if (Zones.TryGetValue(zoneId, out ZoneData zone))
            {
                if (!zone.Rooms.Contains(roomId))
                {
                    zone.Rooms.Add(roomId);
                }
            }

            return room;
        }

        public bool TryGetRoom(RoomId id, out RoomData roomData) => ZoneRooms.TryGetValue(id, out roomData);

        public bool RemoveRoom(RoomId id)
        {
            if (!ZoneRooms.TryGetValue(id, out RoomData room))
                return false;

            if (Zones.TryGetValue(room.ZoneId, out ZoneData zone))
            {
                zone.Rooms.Remove(id);
            }

            ReleaseRoomTiles(room);

            ZoneRooms.Remove(id);
            RaiseRoomChanged(id, room);
            if (zone != null)
            {
                RaiseZoneChanged(room.ZoneId, zone);
            }
            PortalDistanceCache.Clear();
            return true;
        }

        public FurnitureNode RegisterFurnitureNode(RoomId roomId, ZoneId zoneId, string prefabId)
        {
            FurnitureNodeId furnitureId = AllocateFurnitureNodeId();
            FurnitureNode node = new FurnitureNode
            {
                Id = furnitureId,
                RoomId = roomId,
                ZoneId = zoneId,
                PrefabId = prefabId
            };

            FurnitureNodes[furnitureId] = node;

            if (ZoneRooms.TryGetValue(roomId, out RoomData room))
            {
                room.FurnitureNodes.Add(furnitureId);
                RaiseRoomChanged(roomId, room);
            }

            return node;
        }

        public ZonePortal RegisterPortal(ZoneId ownerZone, ZoneId corridorZone, TileCoord entryCoord)
        {
            PortalId portalId = AllocatePortalId();
            ZonePortal portal = new ZonePortal
            {
                Id = portalId,
                OwnerZone = ownerZone,
                LinkedCorridor = corridorZone,
                Entry = entryCoord
            };

            ZonePortals[portalId] = portal;

            if (Zones.TryGetValue(ownerZone, out ZoneData zone))
            {
                zone.Portals.Add(portalId);
                RaiseZoneChanged(ownerZone, zone);
            }

            RaisePortalsChanged();
            return portal;
        }

        public bool TryGetPortal(PortalId portalId, out ZonePortal portal) => ZonePortals.TryGetValue(portalId, out portal);

        public bool RemovePortal(PortalId portalId)
        {
            if (!ZonePortals.TryGetValue(portalId, out ZonePortal portal))
                return false;

            ZonePortals.Remove(portalId);

            if (Zones.TryGetValue(portal.OwnerZone, out ZoneData zone))
            {
                zone.Portals.Remove(portalId);
                RaiseZoneChanged(portal.OwnerZone, zone);
            }

            RaisePortalsChanged();
            return true;
        }

        public AgentRuntimeState RegisterAgent()
        {
            AgentId agentId = AllocateAgentId();
            AgentRuntimeState state = new AgentRuntimeState
            {
                Id = agentId
            };

            AgentStates[agentId] = state;
            return state;
        }

        public bool TryGetAgent(AgentId agentId, out AgentRuntimeState agentState) => AgentStates.TryGetValue(agentId, out agentState);

        public bool RemoveAgent(AgentId agentId) => AgentStates.Remove(agentId);

        public ShipEditMemento ExecuteCommand(IShipEditCommand command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            command.Apply(this);
            ShipEditMemento memento = new ShipEditMemento(++_commandSequence, DateTime.UtcNow, command.Description);
            CommandLog.Add(memento);
            _undoStack.Push(command);
            _redoStack.Clear();
            return memento;
        }

        public bool TryUndo()
        {
            if (_undoStack.Count == 0)
                return false;

            IShipEditCommand command = _undoStack.Pop();
            command.Revert(this);
            _redoStack.Push(command);
            return true;
        }

        public bool TryRedo()
        {
            if (_redoStack.Count == 0)
                return false;

            IShipEditCommand command = _redoStack.Pop();
            command.Apply(this);
            _undoStack.Push(command);
            return true;
        }

        private void RaiseZoneChanged(ZoneId id, ZoneData zone) => ZoneChanged?.Invoke(id, zone);
        private void RaiseRoomChanged(RoomId id, RoomData room) => RoomChanged?.Invoke(id, room);
        private void RaisePortalsChanged() => PortalsChanged?.Invoke();

        private void RemoveZoneAdjacencyLinks(ZoneData zone, bool updateGraph)
        {
            if (zone == null)
                return;

            ZoneId[] neighbours = zone.AdjacentZones.ToArray();
            foreach (ZoneId neighbourId in neighbours)
            {
                if (neighbourId.IsValid && Zones.TryGetValue(neighbourId, out ZoneData neighbourZone))
                {
                    neighbourZone.AdjacentZones.Remove(zone.Id);
                }
            }

            zone.AdjacentZones.Clear();

            if (updateGraph)
            {
                ZoneGraph.RemoveZone(zone.Id);
            }
        }

        private void RebuildZoneAdjacency(ZoneData zone)
        {
            if (zone == null)
                return;

            RemoveZoneAdjacencyLinks(zone, true);

            if (zone.Tiles.Count == 0)
                return;

            HashSet<ZoneId> processed = new HashSet<ZoneId>();

            foreach (TileCoord coord in zone.Tiles)
            {
                foreach (Vector2Int offset in TileNeighbourOffsets)
                {
                    TileCoord neighbourCoord = new TileCoord(coord.Deck, coord.X + offset.x, coord.Z + offset.y);

                    if (!DeckZoneIndices.TryGetValue(neighbourCoord.Deck, out DeckZoneIndex deckIndex))
                        continue;

                    if (!deckIndex.TryGetZone(neighbourCoord, out ZoneId neighbourZoneId))
                        continue;

                    if (!neighbourZoneId.IsValid || neighbourZoneId == zone.Id)
                        continue;

                    if (!processed.Add(neighbourZoneId))
                        continue;

                    if (!Zones.TryGetValue(neighbourZoneId, out ZoneData neighbourZone))
                        continue;

                    zone.AdjacentZones.Add(neighbourZoneId);
                    neighbourZone.AdjacentZones.Add(zone.Id);
                    ZoneGraph.AddEdge(zone.Id, neighbourZoneId);
                }
            }
        }

        #endregion
        
        #region Debug
        
        public override string ToString()
        {
            int totalOccupied = 0;
            foreach (Deck deck in Decks)
            {
                foreach (ShipTile ShipTile in deck.Tiles)
                {
                    if (ShipTile.IsOccupied) totalOccupied++;
                }
            }
            
            return $"Ship '{ShipName}' ({ClassName}): {Decks.Length} decks, {TotalTiles} tiles, {totalOccupied} occupied";
        }
        
        #endregion
    }
}
