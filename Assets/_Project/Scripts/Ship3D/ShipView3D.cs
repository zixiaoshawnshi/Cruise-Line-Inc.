using System.Collections.Generic;
using CruiseLineInc.Ship;
using CruiseLineInc.Ship.Data;
using UnityEngine;
using UnityEngine.Serialization;

namespace CruiseLineInc.Ship3D
{
    /// <summary>
    /// WIP 3D ship renderer that mirrors ShipData decks/tiles using lightweight tile instances.
    /// Current goals:
    /// - Provide a view layer ready for future interaction/highlight tooling.
    /// - Keep rendering decoupled from ShipManager (data remains authoritative in ShipData).
    /// </summary>
    public class ShipView3D : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ShipManager _shipManager;
        [SerializeField] private GameObject _tilePrefab;

        [Header("Layout")]
        [SerializeField] private Vector2 _tileSize = Vector2.one;
        [FormerlySerializedAs("_deckHeight")]
        [SerializeField] private float _defaultDeckHeight = 3f;
        [SerializeField] private bool _autoRenderOnStart = true;
        [SerializeField] private bool _drawTileGizmos = true;
        [SerializeField] private Color _tileGizmoColor = new Color(0f, 1f, 0f, 0.65f);
        [SerializeField] private float _gizmoHeight = 0.05f;
        [SerializeField] private int _tileLayer = 0;

        [Header("Zone Colors")]
        [FormerlySerializedAs("_zoneColorPublic")]
        [SerializeField] private Color _zoneColorCorridor = new Color(0.68f, 0.82f, 1f);
        [FormerlySerializedAs("_zoneColorFamily")]
        [SerializeField] private Color _zoneColorStateroom = new Color(0.85f, 0.95f, 0.7f);
        [FormerlySerializedAs("_zoneColorPremium")]
        [SerializeField] private Color _zoneColorDining = new Color(1f, 0.85f, 0.6f);
        [FormerlySerializedAs("_zoneColorAdult")]
        [SerializeField] private Color _zoneColorLeisure = new Color(0.98f, 0.75f, 0.9f);
        [FormerlySerializedAs("_zoneColorStaff")]
        [SerializeField] private Color _zoneColorService = new Color(0.75f, 0.9f, 0.7f);
        [FormerlySerializedAs("_zoneColorRestricted")]
        [SerializeField] private Color _zoneColorEngine = new Color(1f, 0.6f, 0.4f);
        [SerializeField] private Color _zoneColorStorage = new Color(0.8f, 0.75f, 0.6f);
        [SerializeField] private Color _zoneColorMedical = new Color(0.7f, 0.95f, 0.95f);
        [SerializeField] private Color _zoneColorUtility = new Color(0.75f, 0.75f, 0.85f);
        [FormerlySerializedAs("_zoneColorLuxury")]
        [SerializeField] private Color _zoneColorCrew = new Color(0.7f, 0.8f, 0.95f);
        [SerializeField] private Color _zoneColorBridge = new Color(0.95f, 0.95f, 0.75f);
        [SerializeField] private Color _zoneColorElevator = new Color(0.8f, 0.8f, 0.8f);
        [SerializeField] private Color _zoneColorExit = new Color(0.9f, 0.65f, 0.65f);
        [SerializeField] private Color _zoneColorDefault = new Color(0.78f, 0.78f, 0.78f);

        [Header("Overlay Colors")]
        [SerializeField] private Color _highlightOverlayColor = new Color(0.2f, 0.6f, 1f, 0.4f);
        [SerializeField] private Color _selectionOverlayColor = new Color(1f, 0.9f, 0.25f, 0.45f);
        [SerializeField] private Color _blockedOverlayColor = new Color(1f, 0.25f, 0.25f, 0.45f);

        private readonly Dictionary<int, DeckVisual> _deckVisuals = new();
        private readonly Dictionary<int, float> _deckOffsets = new();
        private ShipData _currentShipData;

        private void Awake()
        {
            if (_shipManager == null)
            {
                _shipManager = Object.FindFirstObjectByType<ShipManager>();
            }
        }

        private Coroutine _autoRenderCoroutine;

        private void Start()
        {
            if (!_autoRenderOnStart)
                return;

            ShipData shipData = _shipManager != null ? _shipManager.GetCurrentShipData() : null;
            if (shipData != null)
            {
                Render(shipData);
            }
            else if (_shipManager != null)
            {
                _autoRenderCoroutine = StartCoroutine(WaitForShipDataAndRender());
            }
        }

        private System.Collections.IEnumerator WaitForShipDataAndRender()
        {
            while (_shipManager != null && _shipManager.GetCurrentShipData() == null)
            {
                yield return null;
            }

            _autoRenderCoroutine = null;

            ShipData shipData = _shipManager != null ? _shipManager.GetCurrentShipData() : null;
            if (shipData != null)
            {
                Render(shipData);
            }
        }

        /// <summary>
        /// Entry point called when new ShipData is available.
        /// </summary>
        public void Render(ShipData shipData)
        {
            if (shipData == null)
            {
                Debug.LogWarning("ShipView3D.Render called with null ShipData.");
                return;
            }

            Clear();
            _currentShipData = shipData;
            ComputeDeckOffsets(shipData.Decks);

            foreach (Deck deck in shipData.Decks)
            {
                if (deck == null) continue;
                Debug.Log($"Rendering Deck {deck.DeckLevel}; width={deck.Width}, depth={deck.Depth}");
                CreateDeckVisual(deck);
            }

            ApplyOffsetsToDeckVisuals();
        }

        /// <summary>
        /// Refreshes an entire deck view.
        /// </summary>
        public void RefreshDeck(int deckLevel)
        {
            if (_currentShipData == null)
                return;

            ComputeDeckOffsets(_currentShipData.Decks);
            ApplyOffsetsToDeckVisuals();
            Deck deck = _currentShipData.GetDeck(deckLevel);
            if (deck == null)
                return;

            if (_deckVisuals.TryGetValue(deckLevel, out DeckVisual deckVisual))
            {
                deckVisual.Refresh(deck);
                UpdateDeckVisuals(deckLevel, deckVisual, deck);
            }
            else
            {
                CreateDeckVisual(deck);
            }
        }

        /// <summary>
        /// Refreshes specific tiles by coordinates (x, y=deckLevel, z).
        /// </summary>
        public void RefreshTiles(IEnumerable<Vector3Int> tileCoords)
        {
            if (tileCoords == null || _currentShipData == null)
                return;

            bool offsetsComputed = false;
            foreach (Vector3Int coord in tileCoords)
            {
                if (_deckVisuals.TryGetValue(coord.y, out DeckVisual deckVisual))
                {
                    if (!offsetsComputed)
                    {
                        ComputeDeckOffsets(_currentShipData.Decks);
                        ApplyOffsetsToDeckVisuals();
                        offsetsComputed = true;
                    }
                    deckVisual.RefreshTile(coord.x, coord.z);
                    ApplyTileVisuals(coord.y, coord.x, coord.z);
                }
            }
        }

        public void SetTileSelected(int deckLevel, int x, int z, bool selected)
        {
            if (_currentShipData == null)
                return;

            Deck deck = _currentShipData.GetDeck(deckLevel);
            if (deck == null || !deck.IsValidPosition(x, z) || !deck.IsActiveTile(x, z))
                return;

            ShipTile tile = deck.GetTile(x, z);
            if (tile == null)
                return;

            tile.IsSelected = selected;
            ApplyTileVisuals(deckLevel, x, z);
        }

        public void SetTileHighlighted(int deckLevel, int x, int z, bool highlighted)
        {
            if (_currentShipData == null)
                return;

            Deck deck = _currentShipData.GetDeck(deckLevel);
            if (deck == null || !deck.IsValidPosition(x, z) || !deck.IsActiveTile(x, z))
                return;

            ShipTile tile = deck.GetTile(x, z);
            if (tile == null)
                return;

            tile.IsHighlighted = highlighted;
            ApplyTileVisuals(deckLevel, x, z);
        }

        public void Clear()
        {
            foreach (DeckVisual visual in _deckVisuals.Values)
            {
                visual.Dispose();
            }
            _deckVisuals.Clear();
            _deckOffsets.Clear();
        }

        private void CreateDeckVisual(Deck deck)
        {
            if (_tilePrefab == null)
            {
                Debug.LogWarning("ShipView3D tile prefab not assigned. Skipping deck render.");
                return;
            }

            GameObject deckRoot = new GameObject($"Deck_{deck.DeckLevel:D2}");
            deckRoot.transform.SetParent(transform, false);
            float offsetY = GetDeckOffset(deck.DeckLevel);
            deckRoot.transform.localPosition = new Vector3(0f, offsetY, 0f);

            float deckHeight = deck.DeckHeight > 0f ? deck.DeckHeight : _defaultDeckHeight;
            DeckVisual deckVisual = new DeckVisual(this, deck.DeckLevel, deckRoot.transform, deck.Width, deck.Depth, deckHeight, _tilePrefab, _tileSize, _zoneColorDefault, _tileLayer, deck.ActiveBounds);
            deckVisual.UpdateRootOffset(offsetY);
            deckVisual.PopulateTiles();
            _deckVisuals[deck.DeckLevel] = deckVisual;
            UpdateDeckVisuals(deck.DeckLevel, deckVisual, deck);
        }

        private void OnEnable()
        {
            SubscribeToEvents();
        }

        private void OnDisable()
        {
            if (_autoRenderCoroutine != null)
            {
                StopCoroutine(_autoRenderCoroutine);
                _autoRenderCoroutine = null;
            }

            UnsubscribeFromEvents();
        }

        private void SubscribeToEvents()
        {
            // TODO: hook into ShipData change notifications when available.
        }

        private void UnsubscribeFromEvents()
        {
            // TODO: detach listeners.
        }

        private void ComputeDeckOffsets(IEnumerable<Deck> decks)
        {
            _deckOffsets.Clear();
            if (decks == null)
                return;

            List<Deck> orderedDecks = new List<Deck>(decks);
            orderedDecks.Sort((a, b) => a.DeckLevel.CompareTo(b.DeckLevel));

            float cumulativeHeight = 0f;
            foreach (Deck deck in orderedDecks)
            {
                if (deck == null)
                    continue;

                _deckOffsets[deck.DeckLevel] = cumulativeHeight;
                float height = deck.DeckHeight > 0f ? deck.DeckHeight : _defaultDeckHeight;
                cumulativeHeight += height;
            }
        }

        private float GetDeckOffset(int deckLevel)
        {
            if (_deckOffsets.TryGetValue(deckLevel, out float offset))
                return offset;

            return deckLevel * _defaultDeckHeight;
        }

        private void ApplyOffsetsToDeckVisuals()
        {
            foreach (KeyValuePair<int, DeckVisual> kvp in _deckVisuals)
            {
                if (_deckOffsets.TryGetValue(kvp.Key, out float offset))
                {
                    kvp.Value.UpdateRootOffset(offset);
                }
            }
        }

        public bool TryGetDeckWorldHeight(int deckLevel, out float worldY)
        {
            worldY = 0f;

            if (_currentShipData == null || _currentShipData.GetDeck(deckLevel) == null)
                return false;

            if (_deckVisuals.TryGetValue(deckLevel, out DeckVisual deckVisual))
            {
                worldY = deckVisual.GetWorldHeight();
                return true;
            }

            if (!_deckOffsets.ContainsKey(deckLevel))
            {
                ComputeDeckOffsets(_currentShipData.Decks);
            }

            if (!_deckOffsets.TryGetValue(deckLevel, out float offset))
                return false;

            worldY = transform.TransformPoint(new Vector3(0f, offset, 0f)).y;
            return true;
        }

        public bool TryGetTileCoordinates(int deckLevel, Vector3 worldPoint, out int x, out int z)
        {
            x = -1;
            z = -1;

            if (_currentShipData == null)
                return false;

            Deck deck = _currentShipData.GetDeck(deckLevel);
            if (deck == null)
                return false;

            Vector3 local = transform.InverseTransformPoint(worldPoint);

            if (_deckVisuals.TryGetValue(deckLevel, out DeckVisual deckVisual) && deckVisual.TryGetTileAtWorldPoint(worldPoint, out x, out z))
            {
                return true;
            }

            if (!_deckOffsets.ContainsKey(deckLevel))
            {
                ComputeDeckOffsets(_currentShipData.Decks);
            }

            if (!_deckOffsets.TryGetValue(deckLevel, out float offset))
                return false;

            local.y -= offset;

            float normalizedX = (local.x / _tileSize.x) + (deck.Width - 1) * 0.5f;
            float normalizedZ = (local.z / _tileSize.y) + (deck.Depth - 1) * 0.5f;

            int tileX = Mathf.FloorToInt(normalizedX);
            int tileZ = Mathf.FloorToInt(normalizedZ);

            if (tileX < 0 || tileX >= deck.Width || tileZ < 0 || tileZ >= deck.Depth)
                return false;

            x = tileX;
            z = tileZ;
            return true;
        }

        private void UpdateDeckVisuals(int deckLevel, DeckVisual deckVisual, Deck deck)
        {
            if (_currentShipData == null || deckVisual == null || deck == null)
                return;

            for (int x = 0; x < deck.Width; x++)
            {
                for (int z = 0; z < deck.Depth; z++)
                {
                    ApplyTileVisuals(deckLevel, x, z);
                }
            }
        }

        private void ApplyTileVisuals(int deckLevel, int x, int z)
        {
            if (_currentShipData == null)
                return;

            Deck deck = _currentShipData.GetDeck(deckLevel);
            if (deck == null || !deck.IsValidPosition(x, z) || !deck.IsActiveTile(x, z))
                return;

            ShipTile tile = deck.GetTile(x, z);
            if (tile == null)
                return;

            if (!_deckVisuals.TryGetValue(deckLevel, out DeckVisual deckVisual))
                return;

            ZoneData zone = null;
            ZoneFunctionType? functionType = null;
            if (_currentShipData.TryGetZoneAtPosition(deckLevel, x, z, out ZoneData foundZone))
            {
                zone = foundZone;
                functionType = zone.FunctionType;
            }

            Color zoneColor = GetZoneColor(functionType);
            bool isBuildable = tile.CanBuild();
            bool isSelected = tile.IsSelected;
            bool highlighted = tile.IsHighlighted;

            deckVisual.UpdateTileVisual(
                x,
                z,
                zoneColor,
                isBuildable,
                isSelected,
                highlighted,
                _selectionOverlayColor,
                _highlightOverlayColor,
                _blockedOverlayColor);
        }

        private Color GetZoneColor(ZoneFunctionType? function)
        {
            if (!function.HasValue)
                return _zoneColorDefault;

            return function.Value switch
            {
                ZoneFunctionType.Corridor => _zoneColorCorridor,
                ZoneFunctionType.Stateroom => _zoneColorStateroom,
                ZoneFunctionType.Dining => _zoneColorDining,
                ZoneFunctionType.Leisure => _zoneColorLeisure,
                ZoneFunctionType.Service => _zoneColorService,
                ZoneFunctionType.Engine => _zoneColorEngine,
                ZoneFunctionType.Storage => _zoneColorStorage,
                ZoneFunctionType.Medical => _zoneColorMedical,
                ZoneFunctionType.Utility => _zoneColorUtility,
                ZoneFunctionType.Crew => _zoneColorCrew,
                ZoneFunctionType.Bridge => _zoneColorBridge,
                ZoneFunctionType.Elevator => _zoneColorElevator,
                ZoneFunctionType.Exit => _zoneColorExit,
                _ => _zoneColorDefault
            };
        }

        public bool TryGetDeckCenterWorld(int deckLevel, out Vector3 center)
        {
            if (_deckVisuals.TryGetValue(deckLevel, out DeckVisual deckVisual))
            {
                center = deckVisual.GetCenterWorld();
                return true;
            }

            if (_currentShipData != null)
            {
                if (!_deckOffsets.ContainsKey(deckLevel))
                    ComputeDeckOffsets(_currentShipData.Decks);

                float offset = GetDeckOffset(deckLevel);
                center = transform.TransformPoint(new Vector3(0f, offset, 0f));
                return true;
            }

            center = Vector3.zero;
            return false;
        }

        public bool TryGetDeckBounds(int deckLevel, out Bounds bounds)
        {
            bounds = default;

            if (_deckVisuals.TryGetValue(deckLevel, out DeckVisual deckVisual))
            {
                bounds = deckVisual.GetBoundsWorld();
                return true;
            }

            if (_currentShipData != null)
            {
                Deck deck = _currentShipData.GetDeck(deckLevel);
                if (deck != null)
                {
                    if (!_deckOffsets.ContainsKey(deckLevel))
                        ComputeDeckOffsets(_currentShipData.Decks);

                    float offset = GetDeckOffset(deckLevel);
                    float height = deck.DeckHeight > 0f ? deck.DeckHeight : _defaultDeckHeight;
                    Vector3 center = transform.TransformPoint(new Vector3(0f, offset + height * 0.5f, 0f));
                    Vector3 size = new Vector3(deck.Width * _tileSize.x, height, deck.Depth * _tileSize.y);
                    bounds = new Bounds(center, size);
                    return true;
                }
            }

            return false;
        }

        public void SetDeckVisibility(int focusDeckLevel, bool showBelow = true, bool hideAbove = true)
        {
            foreach (KeyValuePair<int, DeckVisual> kvp in _deckVisuals)
            {
                int level = kvp.Key;
                bool visible = true;

                if (hideAbove && level > focusDeckLevel)
                    visible = false;

                if (!showBelow && level < focusDeckLevel)
                    visible = false;

                kvp.Value.SetVisible(visible);
            }
        }

        public bool TryGetDeckLevels(List<int> levels)
        {
            if (levels == null)
                return false;

            levels.Clear();

            if (_currentShipData?.Decks == null)
                return false;

            foreach (Deck deck in _currentShipData.Decks)
            {
                if (deck == null)
                    continue;

                if (!levels.Contains(deck.DeckLevel))
                    levels.Add(deck.DeckLevel);
            }

            levels.Sort();
            return levels.Count > 0;
        }

        private readonly struct TileVisual
        {
            public readonly Transform Transform;

            public TileVisual(Transform transform)
            {
                Transform = transform;
            }
        }

        private sealed class DeckVisual
        {
            private readonly struct TileInstance
            {
                public readonly Transform Transform;
                public readonly MeshRenderer Renderer;
                public readonly MaterialPropertyBlock PropertyBlock;

                public TileInstance(Transform transform, MeshRenderer renderer)
                {
                    Transform = transform;
                    Renderer = renderer;
                    PropertyBlock = renderer != null ? new MaterialPropertyBlock() : null;
                }
            }

            private readonly ShipView3D _view;
            private readonly Transform _root;
            private readonly int _deckLevel;
            private readonly int _width;
            private readonly int _depth;
            private readonly float _deckHeight;
            private readonly GameObject _tilePrefab;
            private readonly Vector2 _tileSize;
            private readonly Color _defaultZoneColor;
            private readonly TileInstance[,] _tiles;
            private readonly int _tileLayer;
            private readonly RectInt _activeBounds;

            private static readonly int ZoneColorId = Shader.PropertyToID("_ZoneColor");
            private static readonly int OverlayColorId = Shader.PropertyToID("_OverlayColor");
            private static readonly int OverlayIntensityId = Shader.PropertyToID("_OverlayIntensity");

            public DeckVisual(ShipView3D view, int deckLevel, Transform root, int width, int depth, float deckHeight, GameObject tilePrefab, Vector2 tileSize, Color defaultZoneColor, int tileLayer, RectInt activeBounds)
            {
                _view = view;
                _deckLevel = deckLevel;
                _root = root;
                _width = Mathf.Max(1, width);
                _depth = Mathf.Max(1, depth);
                _deckHeight = deckHeight > 0f ? deckHeight : 3f;
                _tilePrefab = tilePrefab;
                _tileSize = tileSize;
                _defaultZoneColor = defaultZoneColor;
                _tileLayer = tileLayer;
                _tiles = new TileInstance[_width, _depth];
                _activeBounds = activeBounds;
            }

            public void UpdateRootOffset(float offsetY)
            {
                if (_root != null)
                {
                    Vector3 pos = _root.localPosition;
                    pos.y = offsetY;
                    _root.localPosition = pos;
                }
            }

            public void PopulateTiles()
            {
                for (int x = 0; x < _width; x++)
                {
                    for (int z = 0; z < _depth; z++)
                    {
                        Vector3 localPos = new Vector3(
                            (x - (_width - 1) * 0.5f) * _tileSize.x,
                            0f,
                            (z - (_depth - 1) * 0.5f) * _tileSize.y
                        );

                        GameObject tileObj = Object.Instantiate(_tilePrefab, _root);
                        tileObj.name = $"Tile_{x:D2}_{z:D2}";
                        tileObj.transform.localPosition = localPos;
                        tileObj.transform.localRotation = Quaternion.identity;
                        tileObj.transform.localScale = new Vector3(_tileSize.x, 1f, _tileSize.y);
                        tileObj.layer = _tileLayer;

                        BoxCollider collider = tileObj.GetComponent<BoxCollider>();
                        if (collider == null)
                        {
                            collider = tileObj.AddComponent<BoxCollider>();
                        }
                        collider.size = new Vector3(1f, 0.1f, 1f);
                        collider.center = new Vector3(0f, 0f, 0f);

                        TileVisualHandle handle = tileObj.GetComponent<TileVisualHandle>();
                        if (handle == null)
                        {
                            handle = tileObj.AddComponent<TileVisualHandle>();
                        }
                        handle.Initialize(_view, _deckLevel, x, z);

                        MeshRenderer renderer = tileObj.GetComponentInChildren<MeshRenderer>();
                        TileInstance instance = new TileInstance(tileObj.transform, renderer);
                        if (renderer != null && _defaultZoneColor != default)
                        {
                            var defaultBlock = new MaterialPropertyBlock();
                            defaultBlock.SetColor(ZoneColorId, _defaultZoneColor);
                            defaultBlock.SetColor(OverlayColorId, Color.clear);
                            defaultBlock.SetFloat(OverlayIntensityId, 0f);
                            renderer.SetPropertyBlock(defaultBlock);
                        }
                        _tiles[x, z] = instance;

                        bool isActive = IsActiveIndex(x, z);
                        if (!isActive)
                        {
                            if (collider != null)
                                collider.enabled = false;
                            if (handle != null)
                                handle.enabled = false;
                        }
                        tileObj.SetActive(isActive);
                    }
                }
            }

            public void Refresh(Deck deck)
            {
                // TODO: respond to deck size changes (add/remove tiles or rebuild).
            }

            public void RefreshTile(int x, int z)
            {
                // TODO: lookup tile index and update visuals (material, highlight, occupancy marker).
            }

            public void UpdateTileVisual(int x, int z, Color zoneColor, bool buildable, bool isSelected, bool highlighted, Color selectionColor, Color highlightColor, Color blockedColor)
            {
                if (!IsValidIndex(x, z) || !IsActiveIndex(x, z))
                    return;

                TileInstance instance = _tiles[x, z];
                if (instance.Renderer == null || instance.PropertyBlock == null)
                    return;

                instance.PropertyBlock.Clear();
                instance.Renderer.GetPropertyBlock(instance.PropertyBlock);
                instance.PropertyBlock.SetColor(ZoneColorId, zoneColor);
                Color overlayColor = Color.clear;
                float overlayIntensity = 0f;

                if (highlighted)
                {
                    overlayColor = highlightColor;
                    overlayIntensity = 1f;
                }
                else if (isSelected)
                {
                    overlayColor = selectionColor;
                    overlayIntensity = 0.9f;
                }
                else if (!buildable)
                {
                    overlayColor = blockedColor;
                    overlayIntensity = 0.75f;
                }

                instance.PropertyBlock.SetColor(OverlayColorId, overlayColor);
                instance.PropertyBlock.SetFloat(OverlayIntensityId, overlayIntensity);
                instance.Renderer.SetPropertyBlock(instance.PropertyBlock);
            }

            public void DrawTileGizmos(Color color, float heightMultiplier)
            {
                Vector3 gizmoSize = new Vector3(_tileSize.x, Mathf.Max(0.001f, heightMultiplier), _tileSize.y);

                Gizmos.color = color;
                for (int x = 0; x < _width; x++)
                {
                for (int z = 0; z < _depth; z++)
                {
                    if (!IsActiveIndex(x, z))
                        continue;

                    Transform transform = _tiles[x, z].Transform;
                    if (transform == null)
                        continue;

                        Gizmos.DrawWireCube(transform.position, gizmoSize);
                    }
                }
            }

            public Bounds GetBoundsWorld()
            {
                if (_root == null)
                    return default;

                Vector3 size = new Vector3(_width * _tileSize.x, _deckHeight, _depth * _tileSize.y);
                Vector3 center = _root.position + new Vector3(0f, _deckHeight * 0.5f, 0f);
                return new Bounds(center, size);
            }

            public Vector3 GetCenterWorld()
            {
                return _root != null ? _root.position : Vector3.zero;
            }

            public bool TryGetTileAtWorldPoint(Vector3 worldPoint, out int x, out int z)
            {
                x = -1;
                z = -1;

                if (_root == null)
                    return false;

                Vector3 local = _root.InverseTransformPoint(worldPoint);
                float normalizedX = (local.x / _tileSize.x) + (_width - 1) * 0.5f;
                float normalizedZ = (local.z / _tileSize.y) + (_depth - 1) * 0.5f;

                int tileX = Mathf.FloorToInt(normalizedX + 1e-4f);
                int tileZ = Mathf.FloorToInt(normalizedZ + 1e-4f);

                if (!IsValidIndex(tileX, tileZ))
                    return false;

                x = tileX;
                z = tileZ;
                return true;
            }

            public float GetWorldHeight()
            {
                return _root != null ? _root.position.y : 0f;
            }

            public void SetVisible(bool visible)
            {
                if (_root != null && _root.gameObject.activeSelf != visible)
                {
                    _root.gameObject.SetActive(visible);
                }
            }

            public void Dispose()
            {
                if (_root == null) return;

                if (Application.isPlaying)
                {
                    Object.Destroy(_root.gameObject);
                }
                else
                {
                    Object.DestroyImmediate(_root.gameObject);
                }
            }

            private bool IsValidIndex(int x, int z)
            {
                return x >= 0 && x < _width && z >= 0 && z < _depth;
            }

            private bool IsActiveIndex(int x, int z)
            {
                return _activeBounds.Contains(new Vector2Int(x, z));
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!_drawTileGizmos) return;

            ShipData data = _currentShipData;
            if (data == null && _shipManager != null)
            {
                data = _shipManager.GetCurrentShipData();
            }

            if (data?.Decks == null) return;

            float gizmoHeight = Mathf.Max(0.001f, _gizmoHeight);
            Gizmos.color = _tileGizmoColor;
            ComputeDeckOffsets(data.Decks);

            foreach (Deck deck in data.Decks)
            {
                if (deck == null) continue;

                int width = Mathf.Max(1, deck.Width);
                int depth = Mathf.Max(1, deck.Depth);
                float deckY = GetDeckOffset(deck.DeckLevel);

                for (int x = 0; x < width; x++)
                {
                    for (int z = 0; z < depth; z++)
                    {
                        Vector3 position = transform.TransformPoint(new Vector3(
                            (x - (width - 1) * 0.5f) * _tileSize.x,
                            deckY,
                            (z - (depth - 1) * 0.5f) * _tileSize.y
                        ));

                        Vector3 size = new Vector3(_tileSize.x, gizmoHeight, _tileSize.y);
                        Gizmos.DrawWireCube(position, size);
                    }
                }
            }
        }
#endif
    }
}
