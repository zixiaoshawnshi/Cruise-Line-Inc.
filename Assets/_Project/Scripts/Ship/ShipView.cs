using UnityEngine;
using UnityEngine.Tilemaps;
using CruiseLineInc.Ship.Data;
using CruiseLineInc.Room.Data;
using System.Collections.Generic;

namespace CruiseLineInc.Ship
{
    /// <summary>
    /// MonoBehaviour that renders ship data onto a tilemap.
    /// This is the ONLY MonoBehaviour in the ship system - everything else is pure data.
    /// </summary>
    public class ShipView : MonoBehaviour
    {
        [Header("Rendering Components")]
        [SerializeField] private Grid _grid;
        [SerializeField] private Tilemap _tilemap;
        [SerializeField] private Tilemap _roomTilemap;
        [SerializeField] private TileVisualData _tileVisualData;
        
        [Header("Room Definitions")]
        [Tooltip("Auto-populated from Resources or manually assign room definitions")]
        [SerializeField] private List<CruiseLineInc.Room.Data.RoomDefinition> _roomDefinitions = new List<CruiseLineInc.Room.Data.RoomDefinition>();
        
        [Header("Current Ship")]
        [SerializeField] private ShipData _currentShipData;
        
        #region Properties
        
        public ShipData CurrentShipData => _currentShipData;
        public Tilemap Tilemap => _tilemap;
        public Tilemap RoomTilemap => _roomTilemap;
        public Grid Grid => _grid;
        
        #endregion
        
        #region Setup
        
        private void Awake()
        {
            SetupComponents();
            LoadRoomDefinitions();
        }
        
        private void SetupComponents()
        {
            if (_grid == null)
            {
                _grid = GetComponentInChildren<Grid>();
                if (_grid == null)
                {
                    GameObject gridObj = new GameObject("Grid");
                    gridObj.transform.SetParent(transform);
                    gridObj.transform.localPosition = Vector3.zero;
                    _grid = gridObj.AddComponent<Grid>();
                    _grid.cellSize = new Vector3(1f, 1f, 0f);
                }
            }
            
            if (_tilemap == null)
            {
                _tilemap = GetComponentInChildren<Tilemap>();
                if (_tilemap == null && _grid != null)
                {
                    GameObject tilemapObj = new GameObject("Tilemap_Ship");
                    tilemapObj.transform.SetParent(_grid.transform);
                    tilemapObj.transform.localPosition = Vector3.zero;
                    _tilemap = tilemapObj.AddComponent<Tilemap>();
                }
            }
            if (_tilemap != null)
            {
                TilemapRenderer renderer = _tilemap.GetComponent<TilemapRenderer>();
                if (renderer == null)
                    renderer = _tilemap.gameObject.AddComponent<TilemapRenderer>();
                ConfigureTilemapRenderer(renderer, 0);
            }
            
            if (_roomTilemap == null)
            {
                Transform[] children = _grid.GetComponentsInChildren<Transform>();
                foreach (Transform child in children)
                {
                    if (child.name == "Tilemap_Rooms")
                    {
                        _roomTilemap = child.GetComponent<Tilemap>();
                        break;
                    }
                }
                
                if (_roomTilemap == null && _grid != null)
                {
                    GameObject roomTilemapObj = new GameObject("Tilemap_Rooms");
                    roomTilemapObj.transform.SetParent(_grid.transform);
                    roomTilemapObj.transform.localPosition = new Vector3(0f, 0f, -0.02f);
                    _roomTilemap = roomTilemapObj.AddComponent<Tilemap>();
                }
            }
            if (_roomTilemap != null)
            {
                TilemapRenderer roomRenderer = _roomTilemap.GetComponent<TilemapRenderer>();
                if (roomRenderer == null)
                    roomRenderer = _roomTilemap.gameObject.AddComponent<TilemapRenderer>();
                ConfigureTilemapRenderer(roomRenderer, 20);
            }
        }

        private void ConfigureTilemapRenderer(TilemapRenderer renderer, int sortingOrder)
        {
            if (renderer == null)
                return;

            renderer.sortingOrder = sortingOrder;
            if (string.IsNullOrEmpty(renderer.sortingLayerName))
                renderer.sortingLayerName = "Default";
            renderer.mode = TilemapRenderer.Mode.Chunk;

            if (renderer.sharedMaterial == null || renderer.sharedMaterial.name == "Default-Material")
            {
                Material spriteMaterial = Resources.Load<Material>("Sprites-Default");
                if (spriteMaterial == null)
                    spriteMaterial = new Material(Shader.Find("Sprites/Default"));
                renderer.sharedMaterial = spriteMaterial;
            }
        }

        private void LoadRoomDefinitions()
        {
            // If already populated in inspector, skip
            if (_roomDefinitions != null && _roomDefinitions.Count > 0)
            {
                Debug.Log($"Using {_roomDefinitions.Count} room definitions from Inspector");
                return;
            }
            
            // Try to load from Resources folder
            var definitions = Resources.LoadAll<CruiseLineInc.Room.Data.RoomDefinition>("Data/Rooms");
            if (definitions != null && definitions.Length > 0)
            {
                _roomDefinitions = new List<CruiseLineInc.Room.Data.RoomDefinition>(definitions);
                Debug.Log($"Loaded {_roomDefinitions.Count} room definitions from Resources/Data/Rooms");
            }
            else
            {
                Debug.LogWarning("No room definitions found. Please assign them in ShipView Inspector or place in Resources/Data/Rooms folder");
            }
        }
        
        #endregion
        
        #region Rendering
        
        /// <summary>
        /// Renders the entire ship data onto the tilemap
        /// </summary>
        public void Render(ShipData shipData)
        {
            if (shipData == null)
            {
                Debug.LogError("ShipView: Cannot render null ShipData!");
                return;
            }
            
            if (_tilemap == null)
            {
                Debug.LogError("ShipView: Tilemap is null!");
                return;
            }
            
            if (_tileVisualData == null)
            {
                Debug.LogWarning("ShipView: TileVisualData is null - tiles will not render!");
                return;
            }
            
            _currentShipData = shipData;
            
            // Debug: Check tilemap renderer
            TilemapRenderer renderer = _tilemap.GetComponent<TilemapRenderer>();
            if (renderer != null)
            {
                Debug.Log($"TilemapRenderer - SortingLayer: {renderer.sortingLayerName}, Order: {renderer.sortingOrder}");
            }
            
            // Clear existing tiles
            _tilemap.ClearAllTiles();
            
            // Render all tiles
            int tilesRendered = 0;
            int nullTileBases = 0;
            foreach (Deck deck in shipData.Decks)
            {
                foreach (ShipTile ShipTile in deck.Tiles)
                {
                    TileBase tileBase = _tileVisualData.GetTileBaseForType(ShipTile.TileType);
                    if (tileBase == null)
                    {
                        nullTileBases++;
                    }
                    else
                    {
                        RenderTile(ShipTile);
                        tilesRendered++;
                    }
                }
            }
            
            Debug.Log($"ShipView: Rendered {tilesRendered} tiles ({nullTileBases} null TileBases) for ship '{shipData.ShipName}'");
            
            // Debug: Check if any tiles actually exist on the tilemap
            if (tilesRendered > 0)
            {
                Debug.Log($"Tilemap bounds: {_tilemap.cellBounds}");
                Debug.Log($"Tilemap position: {_tilemap.transform.position}");
                Debug.Log($"Grid position: {_grid.transform.position}");
                
                // Sample some actual ShipTile data
                ShipTile sampleTile = shipData.Decks[0].Tiles[0, 0];
                Debug.Log($"Sample ShipTile data: XPosition={sampleTile.XPosition}, DeckLevel={sampleTile.ActualDeckLevel}, Type={sampleTile.TileType}");
                
                // Verify a specific ShipTile is set
                TileBase testTile = _tilemap.GetTile(new Vector3Int(0, 0, 0));
                Debug.Log($"Test GetTile(0,0,0): {(testTile != null ? testTile.name : "NULL")}");
                
                // Check what tiles are actually in tilemap
                int tileCount = 0;
                BoundsInt bounds = _tilemap.cellBounds;
                for (int x = bounds.xMin; x < bounds.xMax; x++)
                {
                    for (int y = bounds.yMin; y < bounds.yMax; y++)
                    {
                        if (_tilemap.HasTile(new Vector3Int(x, y, 0)))
                            tileCount++;
                    }
                }
                Debug.Log($"Tilemap HasTile count: {tileCount}");
                
                // Check renderer
                TilemapRenderer testRenderer = _tilemap.GetComponent<TilemapRenderer>();
                Debug.Log($"TilemapRenderer enabled: {testRenderer.enabled}, Mode: {testRenderer.mode}, Material: {testRenderer.sharedMaterial.name}");
            }
            
            // Render rooms on top of tiles
            RenderRooms(shipData);
        }
        
        /// <summary>
        /// Renders a single ShipTile
        /// </summary>
        private void RenderTile(ShipTile ShipTile)
        {
            Vector3Int position = new Vector3Int(ShipTile.XPosition, ShipTile.ActualDeckLevel, 0);
            TileBase tileBase = _tileVisualData.GetTileBaseForType(ShipTile.TileType);
            
            if (tileBase == null)
            {
                Debug.LogWarning($"No TileBase assigned for {ShipTile.TileType} at ({ShipTile.XPosition}, {ShipTile.ActualDeckLevel})");
                return;
            }
            
            _tilemap.SetTile(position, tileBase);
        }
        
        /// <summary>
        /// Renders all rooms from ship data
        /// </summary>
        private void RenderRooms(ShipData shipData)
        {
            if (_roomTilemap == null)
            {
                Debug.LogWarning("ShipView: Room tilemap is null - rooms will not render!");
                return;
            }

            // Clear existing room tiles
            _roomTilemap.ClearAllTiles();
            
            if (shipData.Rooms == null || shipData.Rooms.Count == 0)
                return;
            
            int roomsRendered = 0;
            foreach (var room in shipData.Rooms)
            {
                if (RenderRoom(room))
                    roomsRendered++;
            }
            
            Debug.Log($"ShipView: Rendered {roomsRendered} rooms");
        }
        
        /// <summary>
        /// Renders a single room
        /// </summary>
        private bool RenderRoom(CruiseLineInc.Room.Room room)
        {
            // Find the room definition
            CruiseLineInc.Room.Data.RoomDefinition definition = null;
            
            if (_roomDefinitions != null)
            {
                foreach (var def in _roomDefinitions)
                {
                    if (def.RoomId == room.RoomDefinitionId)
                    {
                        definition = def;
                        break;
                    }
                }
            }

            if (definition == null)
            {
                Debug.LogWarning($"No room definition found for room {room.RoomId} (looking for RoomDefinitionId: {room.RoomDefinitionId})");
                return false;
            }
            
            Debug.Log($"RenderRoom -> {room.RoomDefinitionId}, sprite: {definition.RoomSprite?.name}");
            if (definition.RoomSprite == null)
            {
                Debug.LogWarning($"Room definition '{definition.DisplayName}' has no sprite assigned!");
                return false;
            }
            
            // Create a tile from the sprite
            UnityEngine.Tilemaps.Tile roomTile = ScriptableObject.CreateInstance<UnityEngine.Tilemaps.Tile>();
            roomTile.sprite = definition.RoomSprite;
            roomTile.color = Color.white;
            roomTile.color = Color.white;
            
            // Render the room across its width and height
            for (int h = 0; h < room.Height; h++)
            {
                for (int w = 0; w < room.Width; w++)
                {
                    Vector3Int position = new Vector3Int(room.XPosition + w, room.DeckLevel + h, 0);
                    _roomTilemap.SetTile(position, roomTile);
                }
            }
            
            return true;
        }
        
        /// <summary>
        /// Refreshes specific tiles in a region
        /// </summary>
        public void RefreshTiles(int xPosition, int zPosition, int width, int depth, int deckLevel)
        {
            if (_currentShipData == null)
                return;
            
            Deck deck = _currentShipData.GetDeck(deckLevel);
            if (deck == null)
                return;
            
            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < depth; z++)
                {
                    ShipTile shipTile = deck.GetTile(xPosition + x, zPosition + z);
                    if (shipTile != null)
                    {
                        RenderTile(shipTile);
                    }
                }
            }

            RenderRooms(_currentShipData);
        }

        public void RefreshTiles(int xPosition, int width, int deckLevel) => RefreshTiles(xPosition, 0, width, 1, deckLevel);
        
        /// <summary>
        /// Refreshes all tiles
        /// </summary>
        public void RefreshAll()
        {
            if (_currentShipData != null)
            {
                Render(_currentShipData);
            }
        }
        
        #endregion
        
        #region Public API
        
        public void SetTileVisualData(TileVisualData visualData)
        {
            _tileVisualData = visualData;
            RefreshAll();
        }
        
        #endregion
        
        #region Gizmos
        
        private void OnDrawGizmos()
        {
            if (_currentShipData == null)
                return;
            
            // Draw each ShipTile as a wireframe cube
            foreach (Deck deck in _currentShipData.Decks)
            {
                foreach (ShipTile ShipTile in deck.Tiles)
                {
                    Vector3 worldPos = new Vector3(ShipTile.XPosition + 0.5f, ShipTile.ActualDeckLevel + 0.5f, 0f);
                    
                    // Color based on ShipTile state
                    if (ShipTile.IsOccupied)
                    {
                        Gizmos.color = GetTileTypeColor(ShipTile.TileType);
                    }
                    else if (ShipTile.IsBuildable)
                    {
                        Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 0.3f); // Gray buildable
                    }
                    else
                    {
                        Gizmos.color = new Color(0.3f, 0.3f, 0.3f, 0.2f); // Dark non-buildable
                    }
                    
                    Gizmos.DrawWireCube(worldPos, new Vector3(0.9f, 0.9f, 0.1f));
                    
                    // Draw filled cube for occupied tiles
                    if (ShipTile.IsOccupied)
                    {
                        Color fillColor = Gizmos.color;
                        fillColor.a = 0.3f;
                        Gizmos.color = fillColor;
                        Gizmos.DrawCube(worldPos, new Vector3(0.85f, 0.85f, 0.1f));
                    }
                }
            }
            
            // Draw ship bounds
            if (_currentShipData.Decks.Length > 0)
            {
                Gizmos.color = Color.cyan;
                float width = _currentShipData.Decks[0].Width;
                float height = _currentShipData.Decks.Length;
                Vector3 center = new Vector3(width * 0.5f, height * 0.5f, 0f);
                Gizmos.DrawWireCube(center, new Vector3(width, height, 0.2f));
            }
        }
        
        private Color GetTileTypeColor(TileType tileType)
        {
            return tileType switch
            {
                TileType.Indoor => new Color(1f, 0.9f, 0.7f, 0.8f),      // Warm beige
                TileType.Outdoor => new Color(0.7f, 0.9f, 1f, 0.8f),     // Sky blue
                TileType.Entrance => new Color(0.7f, 1f, 0.7f, 0.8f),    // Green
                TileType.Corridor => new Color(0.9f, 0.9f, 0.9f, 0.8f),  // Light gray
                TileType.Utility => new Color(1f, 0.5f, 0.3f, 0.8f),     // Orange
                TileType.Restricted => new Color(1f, 0.3f, 0.3f, 0.8f),  // Red
                TileType.Special => new Color(1f, 0.7f, 1f, 0.8f),       // Pink
                _ => Color.white
            };
        }
        
        #endregion
    }
}





