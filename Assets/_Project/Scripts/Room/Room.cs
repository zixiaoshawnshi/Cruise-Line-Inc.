using System;
using UnityEngine;

namespace CruiseLineInc.Room
{
    /// <summary>
    /// Runtime instance of a placed room on the ship.
    /// Pure data class - no MonoBehaviour.
    /// </summary>
    [Serializable]
    public class Room
    {
        #region Identification
        
        public string RoomId;  // Unique ID for this room instance
        public string RoomDefinitionId;  // Reference to RoomDefinition ScriptableObject
        
        #endregion
        
        #region Position & Size
        
        public int XPosition;  // Left-most tile position
        public int ZPosition;  // Forward-most tile position
        public int DeckLevel;  // Which deck this room is on
        public int Width;      // How many tiles wide
        public int Length;     // How many tiles deep
        public int Height;     // How many decks tall (for multi-level rooms)
        
        #endregion
        
        #region State
        
        public bool IsActive;  // Is the room functional (completed construction)
        public float ConstructionProgress;  // 0-1, used during construction phase
        public bool IsDeletable;  // Can this room be deleted? (false for Bridge, Engine, etc.)
        
        #endregion
        
        #region Economics
        
        public float BuildCost;  // How much it cost to build
        public float CurrentValue;  // Current resale value (for refunds)
        
        #endregion
        
        #region Constructor
        
        public Room(string roomId, string definitionId, int x, int z, int deck, int width, int length, int height, float cost, bool isDeletable = true)
        {
            RoomId = roomId;
            RoomDefinitionId = definitionId;
            XPosition = x;
            ZPosition = z;
            DeckLevel = deck;
            Width = width;
            Length = length;
            Height = height;
            BuildCost = cost;
            CurrentValue = cost;  // Initially worth what we paid
            IsActive = false;
            ConstructionProgress = 0f;
            IsDeletable = isDeletable;
        }
        
        #endregion
        
        #region Query Methods
        
        /// <summary>
        /// Does this room occupy the given tile position?
        /// </summary>
        public bool OccupiesTile(int x, int z, int deck)
        {
            if (deck < DeckLevel || deck >= DeckLevel + Height)
                return false;
            
            if (x < XPosition || x >= XPosition + Width)
                return false;
            
            if (z < ZPosition || z >= ZPosition + Length)
                return false;
            
            return true;
        }

        public bool OccupiesTile(int x, int deck) => OccupiesTile(x, ZPosition, deck);
        
        /// <summary>
        /// Get the root (bottom) deck level of this room
        /// </summary>
        public int GetRootDeckLevel()
        {
            return DeckLevel;
        }
        
        /// <summary>
        /// Is this a multi-level room?
        /// </summary>
        public bool IsMultiLevel()
        {
            return Height > 1;
        }
        
        #endregion
        
        #region Debug
        
        public override string ToString()
        {
            string size = IsMultiLevel() ? $"{Width}x{Length}x{Height}" : $"{Width}x{Length}";
            string pos = $"({XPosition}, {ZPosition}, {DeckLevel})";
            string state = IsActive ? "Active" : $"Building ({ConstructionProgress:P0})";
            
            return $"Room '{RoomId}' [{RoomDefinitionId}] {size} @ {pos} - {state}";
        }
        
        #endregion
    }
}
