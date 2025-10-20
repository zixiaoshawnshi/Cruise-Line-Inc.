using System.Collections.Generic;
using CruiseLineInc.Ship;
using CruiseLineInc.Ship.Data;
using CruiseLineInc.Ship3D;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace CruiseLineInc.UI
{
    /// <summary>
    /// Handles zone selection, displays basic info, and supports deletion.
    /// </summary>
    public class ZoneSelectionUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Camera _camera;
        [SerializeField] private ShipManager _shipManager;
        [SerializeField] private ShipView3D _shipView;
        [SerializeField] private TextMeshProUGUI _infoLabel;
        [SerializeField] private Button _deleteButton;

        private readonly List<Vector3Int> _highlightedTiles = new List<Vector3Int>();
        private ZoneId _selectedZoneId = ZoneId.Invalid;

        private ShipData CurrentShipData => _shipManager != null ? _shipManager.GetCurrentShipData() : null;

        private void Awake()
        {
            if (_camera == null)
            {
                _camera = Camera.main;
            }

            if (_shipManager == null)
            {
                _shipManager = ShipManager.Instance ?? FindFirstObjectByType<ShipManager>();
            }

            if (_shipView == null)
            {
                _shipView = FindFirstObjectByType<ShipView3D>();
            }

            if (_deleteButton != null)
            {
                _deleteButton.onClick.AddListener(OnDeletePressed);
            }
        }

        private void OnEnable()
        {
            if (ShipUpdateDispatcher.HasInstance)
            {
                ShipUpdateDispatcher.Instance.ShipChanged += OnShipChanged;
            }
        }

        private void OnDisable()
        {
            if (ShipUpdateDispatcher.HasInstance)
            {
                ShipUpdateDispatcher.Instance.ShipChanged -= OnShipChanged;
            }

            ClearSelection();
        }

        private void OnDestroy()
        {
            if (_deleteButton != null)
            {
                _deleteButton.onClick.RemoveListener(OnDeletePressed);
            }
        }

        private void Update()
        {
            Mouse mouse = Mouse.current;
            if (mouse == null || _camera == null)
                return;

            if (mouse.leftButton.wasPressedThisFrame)
            {
                if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                    return;

                if (TrySelectZoneUnderCursor())
                    return;

                ClearSelection();
            }
        }

        private bool TrySelectZoneUnderCursor()
        {
            if (_camera == null)
                return false;

            ShipData shipData = CurrentShipData;
            if (shipData == null)
                return false;

            Mouse mouse = Mouse.current;
            if (mouse == null)
                return false;

            Ray ray = _camera.ScreenPointToRay(mouse.position.ReadValue());
            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, ~0, QueryTriggerInteraction.Ignore))
                return false;

            TileVisualHandle handle = hit.collider.GetComponent<TileVisualHandle>() ?? hit.collider.GetComponentInParent<TileVisualHandle>();
            if (handle == null)
                return false;

            if (!shipData.TryGetZoneAtPosition(handle.DeckLevel, handle.X, handle.Z, out ZoneData zone))
                return false;

            HighlightZone(zone);
            UpdateInfo(zone);
            _deleteButton.interactable = zone.IsDeletable;
            _selectedZoneId = zone.Id;
            return true;
        }

        private void HighlightZone(ZoneData zone)
        {
            ClearHighlight();

            if (_shipView == null || zone == null)
                return;

            foreach (TileCoord coord in zone.Tiles)
            {
                Vector3Int tile = new Vector3Int(coord.X, coord.Deck, coord.Z);
                _highlightedTiles.Add(tile);
                _shipView.SetTileSelected(coord.Deck, coord.X, coord.Z, true);
            }
        }

        private void ClearHighlight()
        {
            if (_shipView != null)
            {
                foreach (Vector3Int tile in _highlightedTiles)
                {
                    _shipView.SetTileSelected(tile.y, tile.x, tile.z, false);
                }
            }

            _highlightedTiles.Clear();
        }

        private void UpdateInfo(ZoneData zone)
        {
            if (_infoLabel == null)
                return;

            if (zone == null)
            {
                _infoLabel.text = "-";
                return;
            }

            _infoLabel.text = $"{zone.FunctionType} (Deck {zone.Deck})\nTiles: {zone.Tiles.Count}";
        }

        private void OnDeletePressed()
        {
            if (!_selectedZoneId.IsValid)
                return;

            ShipData shipData = CurrentShipData;
            if (shipData != null && shipData.RemoveZone(_selectedZoneId))
            {
                ClearSelection();
            }
        }

        private void OnShipChanged(ShipChangeEventArgs args)
        {
            if (!_selectedZoneId.IsValid || args == null)
                return;

            foreach (ZoneId removed in args.RemovedZones)
            {
                if (removed == _selectedZoneId)
                {
                    ClearSelection();
                    break;
                }
            }
        }

        private void ClearSelection()
        {
            ClearHighlight();
            _selectedZoneId = ZoneId.Invalid;
            UpdateInfo(null);
        }
    }
}
