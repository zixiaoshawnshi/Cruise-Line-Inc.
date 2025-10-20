using System.Collections.Generic;
using UnityEngine;
using CruiseLineInc.Ship;

namespace CruiseLineInc.Ship.Data
{
    /// <summary>
    /// Defines a complete ship class configuration (e.g., Mistral - Coastal Class)
    /// </summary>
    [CreateAssetMenu(fileName = "ShipClassData", menuName = "Cruise Line Inc/Ship/Ship Class Data")]
    public class ShipClassData : ScriptableObject
    {
        [Header("Ship Identity")]
        [SerializeField] private string _shipName = "Mistral";
        [SerializeField] private string _className = "Coastal Cruiser";
        [SerializeField] private string _shipId = "mistral_coastal";
        
        [Header("Capacity")]
        [SerializeField] private int _maxPassengers = 40;
        [SerializeField] private int _maxCrew = 10;
        [SerializeField] private int _totalTiles = 64;
        
        [Header("Resource Capacities")]
        [SerializeField] private float _waterCapacity = 150f;
        [SerializeField] private float _foodCapacity = 200f;
        [SerializeField] private float _wasteCapacity = 180f;
        [SerializeField] private float _fuelCapacity = 1000f;
        
        [Header("Starting Resources")]
        [SerializeField] private float _startingMoney = 5000f;
        [SerializeField] private float _startingWater = 150f;
        [SerializeField] private float _startingFood = 200f;
        [SerializeField] private float _startingFuel = 800f;
        
        [Header("Deck Configuration")]
        [SerializeField] private List<DeckConfig> _deckConfigs = new List<DeckConfig>();
        
        [Header("Default Rooms")]
        [Tooltip("Required/default rooms that are pre-placed on the ship (Bridge, Engine Room, etc.)")]
        [SerializeField] private List<DefaultRoomPlacement> _defaultRooms = new List<DefaultRoomPlacement>();
        
        [Header("Default Zones")]
        [Tooltip("Zones that should exist when the ship is first created (corridors, connectors, exits, etc.)")]
        [SerializeField] private List<DefaultZonePlacement> _defaultZones = new List<DefaultZonePlacement>();
        
        [Header("Voyage Parameters")]
        [SerializeField] private int _defaultVoyageDays = 2;
        [SerializeField] private float _cruiseSpeed = 20f; // knots
        
        [Header("Economic")]
        [SerializeField] private float _dailyUpkeep = 1200f;
        [SerializeField] private float _standardTicketPrice = 120f;
        [SerializeField] private float _premiumTicketPrice = 300f;
        
        [Header("Visual")]
        [SerializeField] private GameObject _shipPrefab;
        [SerializeField] private Sprite _shipIcon;
        
        [Header("Description")]
        [TextArea(3, 5)]
        [SerializeField] private string _description;
        
        #region Properties
        
        public string ShipName => _shipName;
        public string ClassName => _className;
        public string ShipId => _shipId;
        public int MaxPassengers => _maxPassengers;
        public int MaxCrew => _maxCrew;
        public int TotalTiles => _totalTiles;
        public float WaterCapacity => _waterCapacity;
        public float FoodCapacity => _foodCapacity;
        public float WasteCapacity => _wasteCapacity;
        public float FuelCapacity => _fuelCapacity;
        public float StartingMoney => _startingMoney;
        public float StartingWater => _startingWater;
        public float StartingFood => _startingFood;
        public float StartingFuel => _startingFuel;
        public List<DeckConfig> DeckConfigs => _deckConfigs;
        public List<DefaultRoomPlacement> DefaultRooms => _defaultRooms;
        public List<DefaultZonePlacement> DefaultZones => _defaultZones;
        public int DefaultVoyageDays => _defaultVoyageDays;
        public float CruiseSpeed => _cruiseSpeed;
        public float DailyUpkeep => _dailyUpkeep;
        public float StandardTicketPrice => _standardTicketPrice;
        public float PremiumTicketPrice => _premiumTicketPrice;
        public GameObject ShipPrefab => _shipPrefab;
        public Sprite ShipIcon => _shipIcon;
        public string Description => _description;
        
        #endregion
        
        /// <summary>
        /// Nested class for deck configuration
        /// </summary>
        [System.Serializable]
        public class DeckConfig
        {
            [SerializeField] private int _deckLevel;
            [SerializeField] private DeckTypeData _deckTypeData;
            [SerializeField] private int _widthOverride; // Leave 0 to use DeckTypeData default
            [SerializeField] private int _depthOverride; // Leave 0 to use DeckTypeData default
            [SerializeField] private float _heightOverride; // Leave 0 to use DeckTypeData default
            [SerializeField] private int _startX; // Global grid starting X position
            [SerializeField] private int _startZ; // Global grid starting Z position
            
            public int DeckLevel => _deckLevel;
            public DeckTypeData DeckTypeData => _deckTypeData;
            public int Width => _widthOverride > 0
                ? _widthOverride
                : (_deckTypeData != null ? _deckTypeData.Width : 1);
            public int Depth => _depthOverride > 0
                ? _depthOverride
                : (_deckTypeData != null ? _deckTypeData.Depth : 1);
            public float Height => _heightOverride > 0f
                ? _heightOverride
                : (_deckTypeData != null ? _deckTypeData.DeckHeight : 3f);
            public int StartX => _startX;
            public int StartZ => _startZ;
        }
        
        /// <summary>
        /// Nested class for default room placement
        /// </summary>
        [System.Serializable]
        public class DefaultRoomPlacement
        {
            [SerializeField] private Room.Data.RoomDefinition _roomDefinition;
            [SerializeField] private int _xPosition;
            [SerializeField] private int _zPosition;
            [SerializeField] private int _deckLevel;
            [Tooltip("If true, room is free and doesn't cost money")]
            [SerializeField] private bool _isFree = true;
            [Tooltip("If false, room cannot be deleted (e.g., Bridge, Engine Room)")]
            [SerializeField] private bool _isDeletable = false;
            
            public Room.Data.RoomDefinition RoomDefinition => _roomDefinition;
            public int XPosition => _xPosition;
            public int ZPosition => _zPosition;
            public int DeckLevel => _deckLevel;
            public bool IsFree => _isFree;
            public bool IsDeletable => _isDeletable;
        }
        
        /// <summary>
        /// Nested class for default zone placement
        /// </summary>
        [System.Serializable]
        public class DefaultZonePlacement
        {
            [SerializeField] private ZoneFunctionType _functionType = ZoneFunctionType.Unknown;
            [SerializeField] private ConnectorType _connector = ConnectorType.None;
            [SerializeField] private string _blueprintId;
            [SerializeField] private int _deckLevel;
            [SerializeField] private int _xPosition;
            [SerializeField] private int _zPosition;
            [SerializeField] private int _width = 1;
            [SerializeField] private int _length = 1;
            [SerializeField] private int _height = 1;
            [SerializeField] private bool _isOperational = true;
            [SerializeField] private bool _isDeletable = true;
            [SerializeField] private float _verticalTraversalCost = 1f;
            [SerializeField] private int _verticalCapacity = 1;

            public ZoneFunctionType FunctionType => _functionType;
            public ConnectorType Connector => _connector;
            public string BlueprintId => _blueprintId;
            public int DeckLevel => _deckLevel;
            public int XPosition => _xPosition;
            public int ZPosition => _zPosition;
            public int Width => Mathf.Max(1, _width);
            public int Length => Mathf.Max(1, _length);
            public int Height => Mathf.Max(1, _height);
            public bool IsOperational => _isOperational;
            public bool IsDeletable => _isDeletable;
            public float VerticalTraversalCost => Mathf.Max(0f, _verticalTraversalCost);
            public int VerticalCapacity => Mathf.Max(0, _verticalCapacity);

            public bool HasConnector => _connector != ConnectorType.None;
        }
        
        /// <summary>
        /// Gets total deck count
        /// </summary>
        public int GetDeckCount()
        {
            return _deckConfigs.Count;
        }
        
        /// <summary>
        /// Gets deck config at specified level
        /// </summary>
        public DeckConfig GetDeckConfig(int deckLevel)
        {
            return _deckConfigs.Find(d => d.DeckLevel == deckLevel);
        }
        
        /// <summary>
        /// Creates ShipData instance from this configuration
        /// </summary>
        public ShipData CreateShipData()
        {
            ShipData shipData = new ShipData(_shipName, _className, _maxPassengers, _maxCrew)
            {
                WaterCapacity = _waterCapacity,
                FoodCapacity = _foodCapacity,
                WasteCapacity = _wasteCapacity,
                FuelCapacity = _fuelCapacity,
                TotalTiles = _totalTiles
            };
            
            // Create decks
            int minStartX = int.MaxValue;
            int minStartZ = int.MaxValue;
            int maxEndX = int.MinValue;
            int maxEndZ = int.MinValue;

            foreach (DeckConfig deckConfig in _deckConfigs)
            {
                if (deckConfig.DeckTypeData == null)
                {
                    continue;
                }

                int startX = deckConfig.StartX;
                int startZ = deckConfig.StartZ;
                int width = Mathf.Max(1, deckConfig.Width);
                int depth = Mathf.Max(1, deckConfig.Depth);

                if (startX < minStartX) minStartX = startX;
                if (startZ < minStartZ) minStartZ = startZ;
                if (startX + width > maxEndX) maxEndX = startX + width;
                if (startZ + depth > maxEndZ) maxEndZ = startZ + depth;
            }

            if (maxEndX == int.MinValue || maxEndZ == int.MinValue)
            {
                minStartX = 0;
                minStartZ = 0;
                maxEndX = 1;
                maxEndZ = 1;
            }

            int globalWidth = Mathf.Max(1, maxEndX - minStartX);
            int globalDepth = Mathf.Max(1, maxEndZ - minStartZ);

            List<Deck> decks = new List<Deck>();
            foreach (DeckConfig deckConfig in _deckConfigs)
            {
                if (deckConfig.DeckTypeData == null)
                {
                    Debug.LogWarning($"DeckConfig at level {deckConfig.DeckLevel} has no DeckTypeData!");
                    continue;
                }

                int adjustedStartX = deckConfig.StartX - minStartX;
                int adjustedStartZ = deckConfig.StartZ - minStartZ;
                int deckWidth = Mathf.Max(1, deckConfig.Width);
                int deckDepth = Mathf.Max(1, deckConfig.Depth);
                RectInt activeBounds = new RectInt(adjustedStartX, adjustedStartZ, deckWidth, deckDepth);

                Deck deck = new Deck(
                    deckConfig.DeckLevel,
                    deckConfig.DeckTypeData.DeckType,
                    globalWidth,
                    globalDepth,
                    activeBounds,
                    deckConfig.Height
                );
                
                if (deckConfig.DeckTypeData.HasValidTilePattern())
                {
                    for (int x = 0; x < deckWidth; x++)
                    {
                        TileType tileType = deckConfig.DeckTypeData.GetTileTypeForPosition(x);
                        int globalX = activeBounds.x + x;
                        for (int z = 0; z < deckDepth; z++)
                        {
                            int globalZ = activeBounds.y + z;
                            ShipTile tile = deck.GetTile(globalX, globalZ);
                            if (tile != null)
                                tile.TileType = tileType;
                        }
                    }
                }
                
                decks.Add(deck);
            }
            
            shipData.Decks = decks.ToArray();
            
            // Place default zones first so rooms can reference them
            PlaceDefaultZones(shipData);

            // Place default rooms (Bridge, Engine Room, etc.)
            PlaceDefaultRooms(shipData);
            
            Debug.Log($"Created ShipData '{shipData.ShipName}': {shipData.Decks.Length} decks, {shipData.TotalTiles} tiles, {_defaultRooms.Count} default rooms");
            
            return shipData;
        }
        
        /// <summary>
        /// Places default zones defined by the ship class configuration
        /// </summary>
        private void PlaceDefaultZones(ShipData shipData)
        {
            foreach (DefaultZonePlacement defaultZone in _defaultZones)
            {
                if (defaultZone == null)
                    continue;

                ZoneData previousSlice = null;
                int targetHeight = Mathf.Max(1, defaultZone.Height);

                for (int layer = 0; layer < targetHeight; layer++)
                {
                    int deckLevel = defaultZone.DeckLevel + layer;
                    Deck deck = shipData.GetDeck(deckLevel);
                    if (deck == null)
                    {
                        Debug.LogWarning($"Default zone '{defaultZone.BlueprintId ?? defaultZone.FunctionType.ToString()}' references missing deck level {deckLevel}. Skipping.");
                        continue;
                    }

                    ZoneData zone = shipData.CreateZoneArea(
                        defaultZone.FunctionType,
                        deckLevel,
                        defaultZone.XPosition,
                        defaultZone.ZPosition,
                        defaultZone.Width,
                        defaultZone.Length,
                        defaultZone.IsOperational,
                        isDefaultPlacement: true,
                        isDeletable: defaultZone.IsDeletable,
                        connector: defaultZone.HasConnector ? defaultZone.Connector : (ConnectorType?)null,
                        blueprintId: defaultZone.BlueprintId);

                    if (zone == null)
                        continue;

                    if (defaultZone.VerticalTraversalCost > 0f)
                        zone.Metrics["VerticalTraversalCost"] = defaultZone.VerticalTraversalCost;

                    if (defaultZone.VerticalCapacity > 0)
                        zone.Metrics["VerticalCapacity"] = defaultZone.VerticalCapacity;

                    if (previousSlice != null)
                    {
                        DeckLink upwardLink = new DeckLink
                        {
                            TargetDeck = previousSlice.Deck,
                            TraversalCost = defaultZone.VerticalTraversalCost > 0f ? defaultZone.VerticalTraversalCost : 1f,
                            Capacity = defaultZone.VerticalCapacity > 0 ? defaultZone.VerticalCapacity : 1
                        };

                        DeckLink downwardLink = new DeckLink
                        {
                            TargetDeck = zone.Deck,
                            TraversalCost = defaultZone.VerticalTraversalCost > 0f ? defaultZone.VerticalTraversalCost : 1f,
                            Capacity = defaultZone.VerticalCapacity > 0 ? defaultZone.VerticalCapacity : 1
                        };

                        zone.DeckLinks.Add(upwardLink);
                        previousSlice.DeckLinks.Add(downwardLink);
                    }

                    previousSlice = zone;
                }
            }
        }
        
        /// <summary>
        /// Places default/required rooms on the ship
        /// </summary>
        private void PlaceDefaultRooms(ShipData shipData)
        {
            foreach (DefaultRoomPlacement defaultRoom in _defaultRooms)
            {
                if (defaultRoom.RoomDefinition == null)
                {
                    Debug.LogWarning("Default room has no RoomDefinition assigned!");
                    continue;
                }
                
                Room.Data.RoomDefinition roomDef = defaultRoom.RoomDefinition;
                
                RoomData roomData = shipData.CreateRoomFromDefinition(
                    roomDef,
                    defaultRoom.XPosition,
                    defaultRoom.ZPosition,
                    defaultRoom.DeckLevel,
                    isOperational: true,
                    isDefaultPlacement: true,
                    markAutoGenerated: false,
                    isDeletable: defaultRoom.IsDeletable
                );

                if (roomData == null)
                {
                    Debug.LogWarning($"Failed to place default room '{roomDef.DisplayName}' at ({defaultRoom.XPosition}, {defaultRoom.ZPosition}) on deck {defaultRoom.DeckLevel}.");
                    continue;
                }

                if (defaultRoom.IsFree && shipData.TryGetZone(roomData.ZoneId, out ZoneData zone))
                {
                    zone.Metrics["BuildCost"] = 0f;
                }

                Debug.Log($"Placed default room: {roomDef.DisplayName} at ({defaultRoom.XPosition}, {defaultRoom.ZPosition}, deck {defaultRoom.DeckLevel}) - Size: {roomDef.Width}x{roomDef.Height}");
            }
        }
    }
}



