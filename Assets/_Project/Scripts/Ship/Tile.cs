using System;
using CruiseLineInc.Ship.Data;

namespace CruiseLineInc.Ship
{
    /// <summary>
    /// Pure data class representing a single tile in the ship grid.
    /// Tiles are positioned at (XPosition, ZPosition) within a deck and
    /// can participate in multi-deck rooms via RootDeckLevel/LayerOffset.
    /// No MonoBehaviour - just data that can be easily serialized and queried.
    /// </summary>
    [Serializable]
    public class ShipTile
    {
        #region Position
        
        public int XPosition;
        public int ZPosition;
        
        #endregion
        
        #region Type & Properties
        
        public DeckType DeckType;
        public TileType TileType;
        public bool IsBuildable;
        public bool IsEntrance;
        public bool IsSelected;
        public bool IsHighlighted;
        
        #endregion
        
        #region Occupancy
        
        public bool IsOccupied;
        public RoomId RoomId;
        
        #endregion
        
        #region Multi-Level Room Support
        
        public bool IsMultiLevelTile;  // Part of a room spanning multiple decks
        public int RootDeckLevel;      // Which deck is the "floor" of the multi-level room
        public int LayerOffset;        // Offset from the root deck (0 = root deck)
        
        #endregion
        
        #region Navigation
        
        public bool IsNavigable;  // Can agents walk on this tile?
        
        #endregion
        
        #region Access Control
        
        public AccessTag AccessTag;
        
        #endregion
        
        #region Constructor
        
        public ShipTile(int xPosition, int zPosition, int deckLevel, DeckType deckType, TileType tileType = TileType.Indoor)
        {
            XPosition = xPosition;
            ZPosition = zPosition;
            DeckType = deckType;
            TileType = tileType;
            
            IsBuildable = true;
            IsEntrance = false;
            IsSelected = false;
            IsHighlighted = false;
            IsOccupied = false;
            RoomId = RoomId.Invalid;
            IsMultiLevelTile = false;
            RootDeckLevel = deckLevel;
            LayerOffset = 0;
            IsNavigable = true;
            AccessTag = AccessTag.Public;
        }
        
        #endregion
        
        #region Query Methods
        
        /// <summary>
        /// Can a room be built on this tile?
        /// </summary>
        public bool CanBuild()
        {
            return IsBuildable && !IsOccupied;
        }
        
        /// <summary>
        /// Can an agent walk on this tile?
        /// </summary>
        public bool CanWalkOn()
        {
            return IsNavigable;
        }
        
        #endregion
        
        #region Debug
        
        public int ActualDeckLevel => RootDeckLevel + LayerOffset;
        
        public override string ToString()
        {
            string pos = $"({XPosition}, {ZPosition})";
            string type = $"{DeckType}/{TileType}";
            string status = IsOccupied ? $"Occupied:{RoomId.Value}" : "Empty";
            string nav = IsNavigable ? "Nav" : "NoNav";
            
            if (IsMultiLevelTile)
            {
                status += $" [Multi-level root {RootDeckLevel} offset {LayerOffset}]";
            }
            
            return $"Tile {pos} deck:{ActualDeckLevel} {type} {status} {nav}";
        }
        
        #endregion
    }
}
