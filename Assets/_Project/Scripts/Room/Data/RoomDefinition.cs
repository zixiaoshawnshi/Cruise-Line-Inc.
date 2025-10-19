using System.Collections.Generic;
using UnityEngine;

namespace CruiseLineInc.Room.Data
{
    /// <summary>
    /// ScriptableObject defining a room type template.
    /// Contains all the properties and rules for a specific room type (e.g., Cabin, Restaurant, Pool).
    /// </summary>
    [CreateAssetMenu(fileName = "RoomDefinition", menuName = "Cruise Line Inc/Room/Room Definition", order = 1)]
    public class RoomDefinition : ScriptableObject
    {
        #region Identification
        
        [Header("Identity")]
        [SerializeField] private string _roomId = "cabin_standard";
        [SerializeField] private string _displayName = "Standard Cabin";
        [TextArea(2, 4)]
        [SerializeField] private string _description = "Basic passenger cabin with bed and bathroom.";
        
        #endregion
        
        #region Visual
        
        [Header("Visual")]
        [SerializeField] private Sprite _roomSprite;
        [SerializeField] private Color _tintColor = Color.white;
        
        #endregion
        
        #region Size & Placement
        
        [Header("Size & Placement")]
        [Tooltip("How many tiles wide the room is")]
        [SerializeField] private int _width = 2;
        
        [Tooltip("How many tiles deep the room is")]
        [SerializeField] private int _length = 1;
        
        [Tooltip("How many decks tall the room is (1 = single deck)")]
        [SerializeField] private int _height = 1;
        
        [Tooltip("Which tile types this room can be placed on")]
        [SerializeField] private List<TileType> _allowedTileTypes = new List<TileType> { TileType.Indoor };
        
        [Tooltip("Which deck types this room can be placed on")]
        [SerializeField] private List<DeckType> _allowedDeckTypes = new List<DeckType>();
        
        #endregion
        
        #region Economics
        
        [Header("Economics")]
        [SerializeField] private float _buildCost = 500f;
        [SerializeField] private float _maintenanceCost = 10f;  // Per day
        [SerializeField] private float _refundPercentage = 1.0f;  // 100% during planning, 50% during voyage
        
        #endregion
        
        #region Resource Consumption
        
        [Header("Resource Consumption (per day)")]
        [SerializeField] private float _waterConsumption = 5f;
        [SerializeField] private float _foodConsumption = 0f;
        [SerializeField] private float _powerConsumption = 2f;
        [SerializeField] private float _wasteProduction = 3f;
        
        #endregion
        
        #region Attributes
        
        [Header("Attributes")]
        [Tooltip("Noise level produced (affects nearby rooms)")]
        [SerializeField] private float _noiseLevel = 0.2f;
        
        [Tooltip("Comfort rating for passengers")]
        [SerializeField] private float _comfortRating = 0.5f;
        
        [Tooltip("Maximum occupancy (passengers)")]
        [SerializeField] private int _capacity = 2;
        
        #endregion
        
        #region Properties
        
        public string RoomId => _roomId;
        public string DisplayName => _displayName;
        public string Description => _description;
        public Sprite RoomSprite => _roomSprite;
        public Color TintColor => _tintColor;
        public int Width => _width;
        public int Length => _length;
        public int Height => _height;
        public List<TileType> AllowedTileTypes => _allowedTileTypes;
        public List<DeckType> AllowedDeckTypes => _allowedDeckTypes;
        public float BuildCost => _buildCost;
        public float MaintenanceCost => _maintenanceCost;
        public float RefundPercentage => _refundPercentage;
        public float WaterConsumption => _waterConsumption;
        public float FoodConsumption => _foodConsumption;
        public float PowerConsumption => _powerConsumption;
        public float WasteProduction => _wasteProduction;
        public float NoiseLevel => _noiseLevel;
        public float ComfortRating => _comfortRating;
        public int Capacity => _capacity;
        
        #endregion
        
        #region Validation
        
        /// <summary>
        /// Can this room be placed on the given tile type?
        /// </summary>
        public bool IsValidTileType(TileType tileType)
        {
            return _allowedTileTypes.Count == 0 || _allowedTileTypes.Contains(tileType);
        }
        
        /// <summary>
        /// Can this room be placed on the given deck type?
        /// </summary>
        public bool IsValidDeckType(DeckType deckType)
        {
            return _allowedDeckTypes.Count == 0 || _allowedDeckTypes.Contains(deckType);
        }
        
        /// <summary>
        /// Get refund amount based on current phase
        /// </summary>
        public float GetRefundAmount(GamePhase currentPhase)
        {
            float percentage = currentPhase == GamePhase.Planning ? 1.0f : _refundPercentage;
            return _buildCost * percentage;
        }
        
        #endregion
        
        #region Debug
        
        private void OnValidate()
        {
            if (_width < 1) _width = 1;
            if (_length < 1) _length = 1;
            if (_height < 1) _height = 1;
            if (_capacity < 0) _capacity = 0;
        }
        
        #endregion
    }
}
