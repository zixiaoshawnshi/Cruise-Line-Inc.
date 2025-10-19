using UnityEngine;

using UnityEngine;

namespace CruiseLineInc.Ship.Data
{
    /// <summary>
    /// Defines a deck configuration template
    /// </summary>
    [CreateAssetMenu(fileName = "DeckTypeData", menuName = "Cruise Line Inc/Ship/Deck Type Data")]
    public class DeckTypeData : ScriptableObject
    {
        [Header("Deck Information")]
        [SerializeField] private string _deckName;
        [SerializeField] private DeckType _deckType;
        [SerializeField] private int _defaultLevel;
        
        [Header("Grid Configuration (2D Cross-Section)")]
        [SerializeField] private int _width = 14;  // Horizontal tiles only
        [SerializeField] private int _depth = 8;   // Tiles along Z
        [SerializeField] private float _deckHeight = 3f;
        [SerializeField] private float _tileSize = 1f;
        
        [Header("Tile Type Pattern")]
        [Tooltip("Default tile types for each X position. If empty, all tiles default to Indoor. " +
                 "Array length should match width, or will be padded/trimmed automatically.")]
        [SerializeField] private TileType[] _defaultTilePattern;
        
        [Header("Default Zone")]
        [SerializeField] private ZoneTag _defaultZoneTag = ZoneTag.Public;
        
        [Header("Description")]
        [TextArea(3, 5)]
        [SerializeField] private string _description;
        
        #region Properties
        
        public string DeckName => _deckName;
        public DeckType DeckType => _deckType;
        public int DefaultLevel => _defaultLevel;
        public int Width => Mathf.Max(1, _width);
        public int Depth => Mathf.Max(1, _depth);
        public float DeckHeight => Mathf.Max(1f, _deckHeight);
        public float TileSize => _tileSize;
        public ZoneTag DefaultZoneTag => _defaultZoneTag;
        public string Description => _description;
        public TileType[] DefaultTilePattern => _defaultTilePattern;
        
        #endregion
        
        /// <summary>
        /// Gets the tile type for a specific X position based on the default pattern.
        /// Returns TileType.Indoor if pattern is not set or position is out of range.
        /// </summary>
        /// <param name="xPosition">Horizontal position on the deck</param>
        /// <returns>TileType for this position</returns>
        public TileType GetTileTypeForPosition(int xPosition)
        {
            if (_defaultTilePattern == null || _defaultTilePattern.Length == 0)
                return TileType.Indoor;
            
            if (xPosition < 0 || xPosition >= _defaultTilePattern.Length)
                return TileType.Indoor;
            
            return _defaultTilePattern[xPosition];
        }
        
        /// <summary>
        /// Checks if the tile pattern is defined and matches the deck width
        /// </summary>
        public bool HasValidTilePattern()
        {
            return _defaultTilePattern != null && _defaultTilePattern.Length == Width;
        }
    }
}
