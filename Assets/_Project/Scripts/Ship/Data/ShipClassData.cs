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
            List<Deck> decks = new List<Deck>();
            foreach (DeckConfig deckConfig in _deckConfigs)
            {
                if (deckConfig.DeckTypeData == null)
                {
                    Debug.LogWarning($"DeckConfig at level {deckConfig.DeckLevel} has no DeckTypeData!");
                    continue;
                }
                
                Deck deck = new Deck(
                    deckConfig.DeckLevel,
                    deckConfig.DeckTypeData.DeckType,
                    deckConfig.Width,
                    deckConfig.Depth,
                    deckConfig.Height
                );
                
                if (deckConfig.DeckTypeData.HasValidTilePattern())
                {
                    for (int x = 0; x < deck.Width; x++)
                    {
                        TileType tileType = deckConfig.DeckTypeData.GetTileTypeForPosition(x);
                        for (int z = 0; z < deck.Depth; z++)
                        {
                            ShipTile tile = deck.GetTile(x, z);
                            if (tile != null)
                                tile.TileType = tileType;
                        }
                    }
                }
                
                decks.Add(deck);
            }
            
            shipData.Decks = decks.ToArray();
            
            // Place default rooms (Bridge, Engine Room, etc.)
            PlaceDefaultRooms(shipData);
            
            Debug.Log($"Created ShipData '{shipData.ShipName}': {shipData.Decks.Length} decks, {shipData.TotalTiles} tiles, {_defaultRooms.Count} default rooms");
            
            return shipData;
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
                
                // Create room instance
                string roomId = System.Guid.NewGuid().ToString();
                Room.Room room = new Room.Room(
                    roomId,
                    roomDef.RoomId,
                    defaultRoom.XPosition,
                    defaultRoom.ZPosition,
                    defaultRoom.DeckLevel,
                    roomDef.Width,
                    roomDef.Length,
                    roomDef.Height,
                    defaultRoom.IsFree ? 0f : roomDef.BuildCost,  // Free rooms cost $0
                    defaultRoom.IsDeletable  // Pass deletable flag
                );
                
                // Default rooms are always active
                room.IsActive = true;
                room.ConstructionProgress = 1f;
                
                // Place room tiles
                if (roomDef.Height > 1)
                {
                    shipData.PlaceMultiLevelRoom(
                        defaultRoom.XPosition,
                        defaultRoom.ZPosition,
                        roomDef.Width,
                        roomDef.Length,
                        defaultRoom.DeckLevel,
                        roomDef.Height,
                        roomId
                    );
                }
                else
                {
                    shipData.PlaceRoom(
                        defaultRoom.XPosition,
                        defaultRoom.ZPosition,
                        roomDef.Width,
                        roomDef.Length,
                        defaultRoom.DeckLevel,
                        roomId
                    );
                }
                
                // Add to room list
                shipData.AddRoom(room);
                
                Debug.Log($"鉁?Placed default room: {roomDef.DisplayName} at ({defaultRoom.XPosition}, {defaultRoom.DeckLevel}) - Size: {roomDef.Width}x{roomDef.Height}");
            }
        }
    }
}



