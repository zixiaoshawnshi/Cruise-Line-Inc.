using CruiseLineInc.Ship;
using CruiseLineInc.Ship.Data;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace CruiseLineInc.Ship3D
{
    /// <summary>
    /// Handles pointer hover highlighting for ship tiles.
    /// </summary>
    public class ShipGridInteractionController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ShipCameraController _cameraController;
        [SerializeField] private ShipView3D _shipView;
        [SerializeField] private Camera _camera;

        [Header("Behaviour")]
        [SerializeField] private bool _ignoreUI = true;
        [SerializeField] private bool _respectOccluders = false;
        [SerializeField] private LayerMask _occluderMask = Physics.DefaultRaycastLayers;
        [SerializeField] private LayerMask _tileLayerMask = ~0;

        private bool _hasHighlight;
        private int _highlightDeck = int.MinValue;
        private int _highlightX = -1;
        private int _highlightZ = -1;

        private void Awake()
        {
            if (_camera == null)
            {
                _camera = Camera.main;
            }

            if (_cameraController == null)
            {
                _cameraController = Object.FindFirstObjectByType<ShipCameraController>();
            }

            if (_shipView == null)
            {
                _shipView = Object.FindFirstObjectByType<ShipView3D>();
            }
        }

        private void OnEnable()
        {
            ShipUpdateDispatcher.Instance.ShipChanged += HandleShipChanged;
        }

        private void OnDisable()
        {
            if (ShipUpdateDispatcher.HasInstance)
            {
                ShipUpdateDispatcher.Instance.ShipChanged -= HandleShipChanged;
            }

            ClearHighlight();
        }

        private void Update()
        {
            if (_camera == null || _shipView == null || _cameraController == null)
                return;

            if (_ignoreUI && EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                ClearHighlight();
                return;
            }

            if (Mouse.current == null)
                return;

            Vector2 mousePos = Mouse.current.position.ReadValue();
            Ray ray = _camera.ScreenPointToRay(mousePos);

            float maxDistance = Mathf.Infinity;
            if (_respectOccluders && Physics.Raycast(ray, out RaycastHit occluderHit, Mathf.Infinity, _occluderMask, QueryTriggerInteraction.Ignore))
            {
                maxDistance = occluderHit.distance;
            }

            if (!Physics.Raycast(ray, out RaycastHit tileHit, maxDistance, _tileLayerMask, QueryTriggerInteraction.Ignore))
            {
                ClearHighlight();
                return;
            }

            TileVisualHandle handle = tileHit.collider.GetComponent<TileVisualHandle>();
            if (handle == null)
            {
                handle = tileHit.collider.GetComponentInParent<TileVisualHandle>();
            }

            if (handle == null)
            {
                ClearHighlight();
                return;
            }

            int deckLevel = handle.DeckLevel;
            int tileX = handle.X;
            int tileZ = handle.Z;

            if (_hasHighlight && _highlightDeck == deckLevel && _highlightX == tileX && _highlightZ == tileZ)
                return;

            ClearHighlight();

            handle.SetHighlight(true);
            _highlightDeck = deckLevel;
            _highlightX = tileX;
            _highlightZ = tileZ;
            _hasHighlight = true;
        }

        private void ClearHighlight()
        {
            if (_hasHighlight && _shipView != null)
            {
                _shipView.SetTileHighlighted(_highlightDeck, _highlightX, _highlightZ, false);
            }

            _hasHighlight = false;
            _highlightDeck = int.MinValue;
            _highlightX = -1;
            _highlightZ = -1;
        }

        private void HandleShipChanged(ShipChangeEventArgs args)
        {
            if (!_hasHighlight || args == null)
                return;

            foreach (int deck in args.DirtyDecks)
            {
                if (deck == _highlightDeck)
                {
                    ClearHighlight();
                    return;
                }
            }
        }
    }
}
