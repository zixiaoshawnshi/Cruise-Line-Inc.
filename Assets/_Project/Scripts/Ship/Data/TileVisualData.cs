using UnityEngine;
using UnityEngine.Tilemaps;

namespace CruiseLineInc.Ship.Data
{
    /// <summary>
    /// ScriptableObject that defines the visual appearance of tiles in the Tilemap.
    /// Maps TileTypes to their corresponding TileBase assets (sprites, rules, etc.).
    /// </summary>
    [CreateAssetMenu(fileName = "TileVisualData", menuName = "Cruise Line Inc/Ship/Tile Visual Data", order = 2)]
    public class TileVisualData : ScriptableObject
    {
        #region Serialized Fields

        [Header("Tile Visuals by Type")]
        [Tooltip("Visual representation for Indoor tiles (cabins, restaurants, etc.)")]
        [SerializeField] private TileBase _indoorTile;

        [Tooltip("Visual representation for Outdoor tiles (pools, sun decks, etc.)")]
        [SerializeField] private TileBase _outdoorTile;

        [Tooltip("Visual representation for Entrance tiles (doors, gangways, etc.)")]
        [SerializeField] private TileBase _entranceTile;

        [Tooltip("Visual representation for Corridor tiles (hallways, passages, etc.)")]
        [SerializeField] private TileBase _corridorTile;

        [Tooltip("Visual representation for Utility tiles (engines, storage, etc.)")]
        [SerializeField] private TileBase _utilityTile;

        [Tooltip("Visual representation for Restricted tiles (crew only, bridge, etc.)")]
        [SerializeField] private TileBase _restrictedTile;

        [Tooltip("Visual representation for Special tiles (elevators, stairs, etc.)")]
        [SerializeField] private TileBase _specialTile;

        [Header("Debug Settings")]
        [Tooltip("Fallback tile for unassigned types")]
        [SerializeField] private TileBase _defaultTile;

        [Tooltip("Tile used for visualizing buildable empty spaces")]
        [SerializeField] private TileBase _emptyBuildableTile;

        [Tooltip("Tile used for visualizing non-buildable spaces")]
        [SerializeField] private TileBase _emptyNonBuildableTile;

        #endregion

        #region Properties

        public TileBase IndoorTile => _indoorTile;
        public TileBase OutdoorTile => _outdoorTile;
        public TileBase EntranceTile => _entranceTile;
        public TileBase CorridorTile => _corridorTile;
        public TileBase UtilityTile => _utilityTile;
        public TileBase RestrictedTile => _restrictedTile;
        public TileBase SpecialTile => _specialTile;
        public TileBase DefaultTile => _defaultTile;
        public TileBase EmptyBuildableTile => _emptyBuildableTile;
        public TileBase EmptyNonBuildableTile => _emptyNonBuildableTile;

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the appropriate TileBase for the given TileType.
        /// Returns DefaultTile if no specific tile is assigned.
        /// </summary>
        /// <param name="tileType">The type of tile to get visuals for</param>
        /// <returns>TileBase asset for rendering in Tilemap</returns>
        public TileBase GetTileBaseForType(TileType tileType)
        {
            return tileType switch
            {
                TileType.Indoor => _indoorTile ?? _defaultTile,
                TileType.Outdoor => _outdoorTile ?? _defaultTile,
                TileType.Entrance => _entranceTile ?? _defaultTile,
                TileType.Corridor => _corridorTile ?? _defaultTile,
                TileType.Utility => _utilityTile ?? _defaultTile,
                TileType.Restricted => _restrictedTile ?? _defaultTile,
                TileType.Special => _specialTile ?? _defaultTile,
                _ => _defaultTile
            };
        }

        /// <summary>
        /// Gets the tile to use when a tile is empty (not occupied by a room).
        /// </summary>
        /// <param name="isBuildable">Whether the empty tile allows construction</param>
        /// <returns>TileBase for empty space visualization</returns>
        public TileBase GetEmptyTile(bool isBuildable)
        {
            return isBuildable ? _emptyBuildableTile : _emptyNonBuildableTile;
        }

        #endregion

        #region Validation

        private void OnValidate()
        {
            // Warn if default tile is missing
            if (_defaultTile == null)
            {
                Debug.LogWarning($"[TileVisualData] {name}: Default tile is not assigned. " +
                                 "This may cause rendering issues if specific tiles are missing.");
            }

            // Warn about missing tile types
            if (_indoorTile == null) Debug.LogWarning($"[TileVisualData] {name}: Indoor tile not assigned.");
            if (_outdoorTile == null) Debug.LogWarning($"[TileVisualData] {name}: Outdoor tile not assigned.");
            if (_entranceTile == null) Debug.LogWarning($"[TileVisualData] {name}: Entrance tile not assigned.");
            if (_corridorTile == null) Debug.LogWarning($"[TileVisualData] {name}: Corridor tile not assigned.");
            if (_utilityTile == null) Debug.LogWarning($"[TileVisualData] {name}: Utility tile not assigned.");
            if (_restrictedTile == null) Debug.LogWarning($"[TileVisualData] {name}: Restricted tile not assigned.");
            if (_specialTile == null) Debug.LogWarning($"[TileVisualData] {name}: Special tile not assigned.");
        }

        #endregion
    }
}
