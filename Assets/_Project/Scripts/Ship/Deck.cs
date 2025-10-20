using System;
using System.Collections.Generic;
using CruiseLineInc.Ship.Data;
using UnityEngine;

namespace CruiseLineInc.Ship
{
    /// <summary>
    /// Pure data class representing a deck. Supports 2D grids of tiles.
    /// No MonoBehaviour - just data that can be queried and modified.
    /// </summary>
    [Serializable]
    public class Deck
    {
        #region Fields
        
        public int DeckLevel;
        public DeckType DeckType;
        public int Width;   // Tiles along X
        public int Depth;   // Tiles along Z
        public ShipTile[,] Tiles;
        public float DeckHeight;
        public RectInt ActiveBounds { get; }
        
        #endregion
        
        #region Constructor
        
        public Deck(int deckLevel, DeckType deckType, int globalWidth, int globalDepth, RectInt activeBounds, float deckHeight = 3f)
        {
            DeckLevel = deckLevel;
            DeckType = deckType;
            Width = globalWidth <= 0 ? 1 : globalWidth;
            Depth = globalDepth <= 0 ? 1 : globalDepth;
            DeckHeight = deckHeight > 0f ? deckHeight : 3f;
            ActiveBounds = activeBounds;
            
            // Create tiles
            Tiles = new ShipTile[Width, Depth];
            for (int x = 0; x < Width; x++)
            {
                for (int z = 0; z < Depth; z++)
                {
                    Tiles[x, z] = new ShipTile(x, z, deckLevel, deckType);
                    if (!ActiveBounds.Contains(new Vector2Int(x, z)))
                    {
                        Tiles[x, z].IsBuildable = false;
                        Tiles[x, z].IsNavigable = false;
                    }
                }
            }
        }
        
        #endregion
        
        #region ShipTile Access
        
        public ShipTile GetTile(int xPosition, int zPosition)
        {
            if (IsValidPosition(xPosition, zPosition))
            {
                return Tiles[xPosition, zPosition];
            }
            return null;
        }

        public ShipTile GetTile(int xPosition) => GetTile(xPosition, 0);

        public bool IsValidPosition(int xPosition, int zPosition)
        {
            return xPosition >= 0 && xPosition < Width &&
                   zPosition >= 0 && zPosition < Depth;
        }

        public bool IsValidPosition(int xPosition) => IsValidPosition(xPosition, 0);
        
        /// <summary>
        /// Gets all buildable tiles
        /// </summary>
        public List<ShipTile> GetBuildableTiles()
        {
            List<ShipTile> buildable = new List<ShipTile>();
            foreach (ShipTile tile in Tiles)
            {
                if (tile != null && tile.CanBuild())
                    buildable.Add(tile);
            }
            return buildable;
        }
        
        #endregion
        
        #region Room Placement
        
        /// <summary>
        /// Checks if a room can be placed at position with given footprint.
        /// </summary>
        public bool CanPlaceRoom(int xPosition, int zPosition, int roomWidth, int roomDepth, TileType[] requiredTileTypes = null)
        {
            for (int x = 0; x < roomWidth; x++)
            {
                for (int z = 0; z < roomDepth; z++)
                {
                    int checkX = xPosition + x;
                    int checkZ = zPosition + z;

                    if (!IsValidPosition(checkX, checkZ))
                        return false;

                    if (!IsActiveTile(checkX, checkZ))
                        return false;

                    ShipTile tile = GetTile(checkX, checkZ);
                    if (tile == null || !tile.CanBuild())
                        return false;

                    if (requiredTileTypes != null && requiredTileTypes.Length > 0)
                    {
                        bool typeMatches = false;
                        foreach (TileType requiredType in requiredTileTypes)
                        {
                            if (tile.TileType == requiredType)
                            {
                                typeMatches = true;
                                break;
                            }
                        }

                        if (!typeMatches)
                            return false;
                    }
                }
            }

            return true;
        }
        
        /// <summary>
        /// Occupies tiles for a room
        /// </summary>
        public void OccupyTiles(int xPosition, int zPosition, int roomWidth, int roomDepth, RoomId roomId, bool isMultiLevel = false, int rootDeckLevel = -1)
        {
            for (int x = 0; x < roomWidth; x++)
            {
                for (int z = 0; z < roomDepth; z++)
                {
                    ShipTile tile = GetTile(xPosition + x, zPosition + z);

                    if (tile == null)
                        continue;

                    if (!IsActiveTile(xPosition + x, zPosition + z))
                        continue;

                    tile.IsOccupied = true;
                    tile.RoomId = roomId;
                    tile.IsMultiLevelTile = isMultiLevel;
                    tile.RootDeckLevel = rootDeckLevel >= 0 ? rootDeckLevel : DeckLevel;
                    tile.LayerOffset = DeckLevel - tile.RootDeckLevel;

                    if (isMultiLevel && DeckLevel != tile.RootDeckLevel)
                        tile.IsNavigable = false;
                }
            }
        }
        
        /// <summary>
        /// Clears tiles previously occupied by a room
        /// </summary>
        public void ClearTiles(int xPosition, int zPosition, int roomWidth, int roomDepth)
        {
            for (int x = 0; x < roomWidth; x++)
            {
                for (int z = 0; z < roomDepth; z++)
                {
                    ShipTile tile = GetTile(xPosition + x, zPosition + z);

                    if (tile == null)
                        continue;

                    if (!IsActiveTile(xPosition + x, zPosition + z))
                        continue;

                    tile.IsOccupied = false;
                    tile.RoomId = RoomId.Invalid;
                    tile.IsMultiLevelTile = false;
                    tile.RootDeckLevel = DeckLevel;
                    tile.LayerOffset = 0;
                    tile.IsNavigable = true;
                }
            }
        }

        public bool IsActiveTile(int xPosition, int zPosition) => ActiveBounds.Contains(new Vector2Int(xPosition, zPosition));
        
        #endregion
        
        #region Debug
        
        public override string ToString()
        {
            return $"Deck {DeckLevel} ({DeckType}): {Width}x{Depth} tiles, height {DeckHeight}, {GetBuildableTiles().Count} buildable";
        }
        
        #endregion
    }
}


