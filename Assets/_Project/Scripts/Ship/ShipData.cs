using System;
using System.Collections.Generic;

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
        
        public string ShipName;
        public string ClassName;
        public Deck[] Decks;
        
        // Capacity
        public int MaxPassengers;
        public int MaxCrew;
        public int TotalTiles;
        
        // Resources
        public float WaterCapacity;
        public float FoodCapacity;
        public float WasteCapacity;
        public float FuelCapacity;
        
        // Rooms
        public List<Room.Room> Rooms = new List<Room.Room>();
        
        #endregion
        
        #region Constructor
        
        public ShipData(string shipName, string className, int maxPassengers, int maxCrew)
        {
            ShipName = shipName;
            ClassName = className;
            MaxPassengers = maxPassengers;
            MaxCrew = maxCrew;
            Decks = new Deck[0];
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
        
        /// <summary>
        /// Gets all tiles belonging to a specific room
        /// </summary>
        public List<ShipTile> GetRoomTiles(string roomId)
        {
            List<ShipTile> roomTiles = new List<ShipTile>();
            foreach (Deck deck in Decks)
            {
                foreach (ShipTile ShipTile in deck.Tiles)
                {
                    if (ShipTile.RoomId == roomId)
                    {
                        roomTiles.Add(ShipTile);
                    }
                }
            }
            return roomTiles;
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
        
        /// <summary>
        /// Places a single-deck room
        /// </summary>
        public void PlaceRoom(int xPosition, int zPosition, int width, int depth, int deckLevel, string roomId)
        {
            Deck deck = GetDeck(deckLevel);
            deck?.OccupyTiles(xPosition, zPosition, width, depth, roomId, false, deckLevel);
        }

        public void PlaceRoom(int xPosition, int width, int deckLevel, string roomId) =>
            PlaceRoom(xPosition, 0, width, 1, deckLevel, roomId);
        
        /// <summary>
        /// Places a multi-level room
        /// </summary>
        public void PlaceMultiLevelRoom(int xPosition, int zPosition, int width, int depth, int startDeckLevel, int deckHeight, string roomId)
        {
            for (int d = 0; d < deckHeight; d++)
            {
                int deckLevel = startDeckLevel + d;
                Deck deck = GetDeck(deckLevel);
                deck?.OccupyTiles(xPosition, zPosition, width, depth, roomId, true, startDeckLevel);
            }
        }

        public void PlaceMultiLevelRoom(int xPosition, int width, int startDeckLevel, int deckHeight, string roomId) =>
            PlaceMultiLevelRoom(xPosition, 0, width, 1, startDeckLevel, deckHeight, roomId);
        
        #endregion
        
        #region Room Management
        
        /// <summary>
        /// Add a room to the ship
        /// </summary>
        public void AddRoom(Room.Room room)
        {
            Rooms.Add(room);
        }
        
        /// <summary>
        /// Remove a room from the ship (clears tiles and removes from list)
        /// </summary>
        public bool RemoveRoom(string roomId)
        {
            Room.Room room = Rooms.Find(r => r.RoomId == roomId);
            if (room == null) return false;
            
            // Clear tiles first
            ClearRoomTiles(roomId);
            
            // Then remove from list
            return Rooms.Remove(room);
        }
        
        /// <summary>
        /// Clear tiles occupied by a room
        /// </summary>
        private void ClearRoomTiles(string roomId)
        {
            List<ShipTile> roomTiles = GetRoomTiles(roomId);
            
            // Group tiles by deck
            Dictionary<int, List<ShipTile>> tilesByDeck = new Dictionary<int, List<ShipTile>>();
            foreach (ShipTile ShipTile in roomTiles)
            {
                int deckLevel = ShipTile.ActualDeckLevel;
                if (!tilesByDeck.ContainsKey(deckLevel))
                {
                    tilesByDeck[deckLevel] = new List<ShipTile>();
                }
                tilesByDeck[deckLevel].Add(ShipTile);
            }
            
            // Clear tiles per deck
            foreach (var kvp in tilesByDeck)
            {
                int deckLevel = kvp.Key;
                List<ShipTile> tiles = kvp.Value;
                
                if (tiles.Count == 0)
                    continue;

                int minX = int.MaxValue;
                int maxX = int.MinValue;
                int minZ = int.MaxValue;
                int maxZ = int.MinValue;

                foreach (ShipTile tile in tiles)
                {
                    if (tile.XPosition < minX) minX = tile.XPosition;
                    if (tile.XPosition > maxX) maxX = tile.XPosition;
                    if (tile.ZPosition < minZ) minZ = tile.ZPosition;
                    if (tile.ZPosition > maxZ) maxZ = tile.ZPosition;
                }

                int width = maxX - minX + 1;
                int depth = maxZ - minZ + 1;

                Deck deck = GetDeck(deckLevel);
                deck?.ClearTiles(minX, minZ, width, depth);
            }
        }
        
        /// <summary>
        /// Get a room by ID
        /// </summary>
        public Room.Room GetRoom(string roomId)
        {
            return Rooms.Find(r => r.RoomId == roomId);
        }
        
        /// <summary>
        /// Get all rooms on a specific deck
        /// </summary>
        public List<Room.Room> GetRoomsOnDeck(int deckLevel)
        {
            return Rooms.FindAll(r => r.DeckLevel == deckLevel || (r.DeckLevel < deckLevel && r.DeckLevel + r.Height > deckLevel));
        }
        
        /// <summary>
        /// Get room at specific ShipTile position
        /// </summary>
        public Room.Room GetRoomAtPosition(int x, int z, int deckLevel)
        {
            return Rooms.Find(r => r.OccupiesTile(x, z, deckLevel));
        }

        public Room.Room GetRoomAtPosition(int x, int deckLevel) => GetRoomAtPosition(x, 0, deckLevel);
        
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
