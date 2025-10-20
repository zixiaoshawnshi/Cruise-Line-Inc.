using System;
using System.Collections.Generic;
using CruiseLineInc.Ship;
using CruiseLineInc.Ship.Data;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace CruiseLineInc.Ship3D
{
    /// <summary>
    /// Simple zone painting tool that lets designers drag out rectangular footprints on a deck.
    /// Later we will add confirmation UI, trimming, and advanced brushes.
    /// </summary>
    public class ZonePaintTool : MonoBehaviour
    {
        [Serializable]
        private class ZonePaintProfile
        {
            [SerializeField] private string _displayName = "Corridor";
            [SerializeField] private ZoneFunctionType _functionType = ZoneFunctionType.Corridor;
            [SerializeField] private bool _assignConnector;
            [SerializeField] private ConnectorType _connectorType = ConnectorType.Corridor;
            [SerializeField] private bool _isDeletable = true;
            [SerializeField] private bool _isOperational = true;
            [SerializeField] private string _blueprintId;

            public ZonePaintProfile()
            {
            }

            public ZonePaintProfile(string displayName, ZoneFunctionType functionType)
            {
                _displayName = displayName;
                _functionType = functionType;
            }

            public string DisplayName => string.IsNullOrEmpty(_displayName) ? _functionType.ToString() : _displayName;
            public ZoneFunctionType FunctionType => _functionType;
            public bool AssignConnector => _assignConnector;
            public ConnectorType ConnectorType => _connectorType;
            public bool IsDeletable => _isDeletable;
            public bool IsOperational => _isOperational;
            public string BlueprintId => _blueprintId;
        }

        [Header("References")]
        [SerializeField] private Camera _camera;
        [SerializeField] private ShipView3D _shipView;
        [SerializeField] private ShipManager _shipManager;

        [Header("Behaviour")]
        [SerializeField] private LayerMask _tileLayerMask = ~0;
        [SerializeField] private bool _paintModeEnabled = true;
        [SerializeField] private ZonePaintProfile[] _profiles;
        [SerializeField] private int _activeProfileIndex;

        private bool _isDragging;
        private int _activeDeckLevel;
        private Vector2Int _startTile;
        private bool _isExtending;
        private ZoneId _targetZoneId = ZoneId.Invalid;
        private ZoneData _targetZone;

        private readonly List<Vector3Int> _previewTiles = new List<Vector3Int>();
        private ShipData CurrentShipData => _shipManager != null ? _shipManager.GetCurrentShipData() : null;

        private void Awake()
        {
            if (_camera == null)
            {
                _camera = Camera.main;
            }

            if (_shipView == null)
            {
                _shipView = UnityEngine.Object.FindFirstObjectByType<ShipView3D>();
            }

            if (_shipManager == null)
            {
                _shipManager = ShipManager.Instance ?? UnityEngine.Object.FindFirstObjectByType<ShipManager>();
            }

            if (_profiles == null || _profiles.Length == 0)
            {
                _profiles = new[]
                {
                    new ZonePaintProfile("Corridor", ZoneFunctionType.Corridor),
                    new ZonePaintProfile("Storage", ZoneFunctionType.Storage),
                    new ZonePaintProfile("Dining", ZoneFunctionType.Dining)
                };
            }

            _activeProfileIndex = Mathf.Clamp(_activeProfileIndex, 0, _profiles.Length - 1);
        }

        private void OnDisable()
        {
            CancelPaint();
        }

        public void SetPaintModeEnabled(bool enabled)
        {
            _paintModeEnabled = enabled;
            if (!enabled)
            {
                CancelPaint();
            }
        }

        public void SetActiveProfile(int index)
        {
            if (_profiles == null || _profiles.Length == 0)
                return;

            _activeProfileIndex = Mathf.Clamp(index, 0, _profiles.Length - 1);
        }

        public bool PaintModeEnabled => _paintModeEnabled;
        public int ActiveProfileIndex => Mathf.Clamp(_activeProfileIndex, 0, (_profiles?.Length ?? 1) - 1);

        public string[] GetProfileNames()
        {
            if (_profiles == null || _profiles.Length == 0)
                return Array.Empty<string>();

            string[] names = new string[_profiles.Length];
            for (int i = 0; i < _profiles.Length; i++)
            {
                names[i] = _profiles[i]?.DisplayName ?? $"Profile {i}";
            }

            return names;
        }

        private void Update()
        {
            if (!_paintModeEnabled || _profiles == null || _profiles.Length == 0)
                return;

            if (_camera == null || _shipView == null)
                return;

            Mouse mouse = Mouse.current;
            if (mouse == null)
                return;

            if (!_isDragging)
            {
                if (mouse.leftButton.wasPressedThisFrame)
                {
                    if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                        return;

                    if (TryGetTileUnderCursor(out int deck, out int x, out int z))
                    {
                        StartPaint(deck, x, z);
                    }
                }
            }
            else
            {
                bool hasTile = TryGetTileUnderCursor(out int deck, out int x, out int z);
                if (mouse.leftButton.isPressed && hasTile && deck == _activeDeckLevel)
                {
                    UpdatePreview(x, z);
                }

                if (mouse.leftButton.wasReleasedThisFrame)
                {
                    CompletePaint();
                }
                else if (mouse.rightButton.wasPressedThisFrame || (Keyboard.current != null && Keyboard.current.escapeKey?.wasPressedThisFrame == true))
                {
                    CancelPaint();
                }
            }
        }

        private void StartPaint(int deckLevel, int x, int z)
        {
            _isDragging = true;
            _activeDeckLevel = deckLevel;
            _startTile = new Vector2Int(x, z);

            _isExtending = false;
            _targetZoneId = ZoneId.Invalid;
            _targetZone = null;

            ShipData shipData = CurrentShipData;
            if (shipData != null && shipData.TryGetZoneAtPosition(deckLevel, x, z, out ZoneData zone))
            {
                _isExtending = true;
                _targetZoneId = zone.Id;
                _targetZone = zone;
            }

            UpdatePreview(x, z);
        }

        private void UpdatePreview(int currentX, int currentZ)
        {
            ClearPreview();

            int minX = Mathf.Min(_startTile.x, currentX);
            int maxX = Mathf.Max(_startTile.x, currentX);
            int minZ = Mathf.Min(_startTile.y, currentZ);
            int maxZ = Mathf.Max(_startTile.y, currentZ);
            HashSet<TileCoord> existingTiles = _isExtending && _targetZone != null ? _targetZone.Tiles : null;

            for (int x = minX; x <= maxX; x++)
            {
                for (int z = minZ; z <= maxZ; z++)
                {
                    TileCoord coord = new TileCoord(_activeDeckLevel, x, z);
                    bool alreadyInZone = existingTiles != null && existingTiles.Contains(coord);

                    if (!_isExtending || !alreadyInZone)
                    {
                        Vector3Int tile = new Vector3Int(x, _activeDeckLevel, z);
                        _previewTiles.Add(tile);
                        _shipView.SetTileHighlighted(_activeDeckLevel, x, z, true);
                    }
                }
            }
        }

        private void CompletePaint()
        {
            if (!_isDragging)
                return;

            ShipData shipData = CurrentShipData;
            if (shipData == null)
            {
                Debug.LogWarning("[ZonePaintTool] No ShipData available to paint zones.");
                CancelPaint();
                return;
            }

            if (_previewTiles.Count == 0)
            {
                CancelPaint();
                return;
            }

            List<Vector2Int> footprint = new List<Vector2Int>(_previewTiles.Count);
            foreach (Vector3Int tile in _previewTiles)
            {
                footprint.Add(new Vector2Int(tile.x, tile.z));
            }

            if (_isExtending && _targetZoneId.IsValid)
            {
                if (!shipData.TryExtendZone(_targetZoneId, footprint, out _))
                {
                    Debug.LogWarning("[ZonePaintTool] Zone extension failed validation.");
                }
            }
            else
            {
                ZonePaintProfile profile = _profiles[Mathf.Clamp(_activeProfileIndex, 0, _profiles.Length - 1)];
                ConnectorType? connector = profile.AssignConnector ? profile.ConnectorType : (ConnectorType?)null;

                if (!shipData.TryPaintZone(
                        profile.FunctionType,
                        _activeDeckLevel,
                        footprint,
                        out ZoneData _,
                        profile.IsOperational,
                        isDefaultPlacement: false,
                        profile.IsDeletable,
                        connector,
                        profile.BlueprintId))
                {
                    Debug.LogWarning("[ZonePaintTool] Zone paint failed validation.");
                }
            }

            ClearPreview();
            ResetPaintState();
        }

        private void CancelPaint()
        {
            ClearPreview();
            ResetPaintState();
        }

        private void ClearPreview()
        {
            if (_shipView != null)
            {
                foreach (Vector3Int tile in _previewTiles)
                {
                    _shipView.SetTileHighlighted(tile.y, tile.x, tile.z, false);
                }
            }

            _previewTiles.Clear();
        }

        private void ResetPaintState()
        {
            _isDragging = false;
            _isExtending = false;
            _targetZoneId = ZoneId.Invalid;
            _targetZone = null;
        }

        private bool TryGetTileUnderCursor(out int deckLevel, out int x, out int z)
        {
            deckLevel = 0;
            x = 0;
            z = 0;

            if (_camera == null)
                return false;

            Mouse mouse = Mouse.current;
            if (mouse == null)
                return false;

            Vector2 position = mouse.position.ReadValue();
            Ray ray = _camera.ScreenPointToRay(position);

            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _tileLayerMask, QueryTriggerInteraction.Ignore))
                return false;

            TileVisualHandle handle = hit.collider.GetComponent<TileVisualHandle>();
            if (handle == null)
            {
                handle = hit.collider.GetComponentInParent<TileVisualHandle>();
            }

            if (handle == null)
                return false;

            deckLevel = handle.DeckLevel;
            x = handle.X;
            z = handle.Z;
            return true;
        }
    }
}
