using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace CruiseLineInc.Ship3D
{
    /// <summary>
    /// Orbit/pan/zoom controller powered by the new Input System.
    /// Supports deck switching, clamped panning within deck bounds, and deck visibility toggling.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class ShipCameraController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Camera _camera;
        [SerializeField] private ShipView3D _shipView;

        [Header("Movement")]
        [SerializeField] private float _panSpeed = 6f;
        [SerializeField] private float _orbitSpeed = 120f;
        [SerializeField] private float _zoomSpeed = 10f;
        [SerializeField] private float _minDistance = 4f;
        [SerializeField] private float _maxDistance = 80f;
        [SerializeField] private float _minPitch = 15f;
        [SerializeField] private float _maxPitch = 80f;
        [SerializeField] private bool _useCameraBounds = true;

        [Header("Deck Switching")]
        [SerializeField] private bool _showDecksBelow = true;
        [SerializeField] private bool _hideDecksAbove = true;
        [SerializeField] private bool _autoCenterOnDeckSwitch = true;

        [Header("Fallback Keyboard Keys (optional)")]
        [SerializeField] private KeyCode _deckUpKey = KeyCode.PageUp;
        [SerializeField] private KeyCode _deckDownKey = KeyCode.PageDown;

        [Header("Input Actions")]
        [Tooltip("Vector2 action for keyboard/controller movement (x = horizontal, y = vertical).")]
        [SerializeField] private InputActionReference _moveAction;
        [Tooltip("Vector2 action for mouse/pen pan delta (e.g., middle-mouse drag).")]
        [SerializeField] private InputActionReference _panAction;
        [Tooltip("Optional button action used as a modifier for panning (e.g., middle mouse button).")]
        [SerializeField] private InputActionReference _panButtonAction;
        [Tooltip("Vector2 action for orbit delta (mouse look).")]
        [SerializeField] private InputActionReference _orbitAction;
        [Tooltip("Optional button action used as a modifier for orbiting (e.g., right mouse button).")]
        [SerializeField] private InputActionReference _orbitButtonAction;
        [Tooltip("Float action for zoom (e.g., mouse wheel).")]
        [SerializeField] private InputActionReference _zoomAction;
        [Tooltip("Button action for deck-up step.")]
        [SerializeField] private InputActionReference _deckUpAction;
        [Tooltip("Button action for deck-down step.")]
        [SerializeField] private InputActionReference _deckDownAction;

        private readonly List<int> _deckLevels = new();
        private readonly List<int> _deckLevelBuffer = new();

        private int _activeDeckIndex;
        private Bounds _activeBounds;
        private Vector3 _focusPoint;
        private float _distance;
        private float _yaw = 45f;
        private float _pitch = 45f;
        private bool _hasFocus;
        private bool _initialized;
        private ShipView3D _subscribedShipView;

        #region Unity Lifecycle

        private void Awake()
        {
            if (_camera == null)
            {
                _camera = GetComponent<Camera>();
            }

            ShipView3D viewReference = _shipView != null ? _shipView : Object.FindFirstObjectByType<ShipView3D>();
            HookShipView(viewReference);
        }

        private void OnEnable()
        {
            EnableReference(_moveAction);
            EnableReference(_panAction);
            EnableReference(_panButtonAction);
            EnableReference(_orbitAction);
            EnableReference(_orbitButtonAction);
            EnableReference(_zoomAction);
            RegisterDeckAction(_deckUpAction, OnDeckUpPerformed);
            RegisterDeckAction(_deckDownAction, OnDeckDownPerformed);

            if (_shipView == null)
            {
                HookShipView(Object.FindFirstObjectByType<ShipView3D>());
            }
            else
            {
                HookShipView(_shipView);
            }
        }

        private void OnDisable()
        {
            DisableReference(_moveAction);
            DisableReference(_panAction);
            DisableReference(_panButtonAction);
            DisableReference(_orbitAction);
            DisableReference(_orbitButtonAction);
            DisableReference(_zoomAction);
            UnregisterDeckAction(_deckUpAction, OnDeckUpPerformed);
            UnregisterDeckAction(_deckDownAction, OnDeckDownPerformed);

            if (_subscribedShipView != null)
            {
                _subscribedShipView.DefaultDeckFocusRequested -= OnDefaultDeckFocusRequested;
                _subscribedShipView = null;
            }
        }

        private void OnDestroy()
        {
            if (_subscribedShipView != null)
            {
                _subscribedShipView.DefaultDeckFocusRequested -= OnDefaultDeckFocusRequested;
                _subscribedShipView = null;
            }
        }

        private void Update()
        {
            if (_shipView == null)
                return;

            bool hasDecks = EnsureDeckList();

            if (!_initialized)
            {
                if (hasDecks)
                {
                    FocusDeckByIndex(_activeDeckIndex);
                    _initialized = true;
                }
                else
                {
                    return;
                }
            }

            HandleDeckSwitchInput();
            HandleZoomInput();
            HandleOrbitInput();
            HandlePanInput();
        }

        private void LateUpdate()
        {
            if (!_hasFocus || _camera == null)
                return;

            Quaternion rotation = Quaternion.Euler(_pitch, _yaw, 0f);
            Vector3 offset = rotation * (Vector3.back * _distance);

            _camera.transform.position = _focusPoint + offset;
            _camera.transform.rotation = rotation;
            _camera.transform.LookAt(_focusPoint, Vector3.up);
        }

        #endregion

        #region Public API

        public void FocusDeck(int deckLevel)
        {
            if (!EnsureDeckList())
                return;

            int index = _deckLevels.IndexOf(deckLevel);
            if (index < 0)
                return;

            FocusDeckByIndex(index);
        }

        public void FocusNextDeck() => StepDeck(1);
        public void FocusPreviousDeck() => StepDeck(-1);

        public bool TryGetActiveDeckLevel(out int deckLevel)
        {
            if (_deckLevels.Count == 0)
            {
                deckLevel = 0;
                return false;
            }

            deckLevel = _deckLevels[Mathf.Clamp(_activeDeckIndex, 0, _deckLevels.Count - 1)];
            return true;
        }

        public IReadOnlyList<int> GetDeckLevels()
        {
            EnsureDeckList();
            return _deckLevels;
        }

        #endregion

        #region Input Handling

        private void HandleDeckSwitchInput()
        {
            if (_deckLevels.Count == 0)
                return;

            bool deckUp = ReadTriggered(_deckUpAction) || Input.GetKeyDown(_deckUpKey);
            bool deckDown = ReadTriggered(_deckDownAction) || Input.GetKeyDown(_deckDownKey);

            if (deckUp)
            {
                StepDeck(1);
            }
            else if (deckDown)
            {
                StepDeck(-1);
            }
        }

        private void HandleZoomInput()
        {
            float zoomInput = ReadFloat(_zoomAction, Input.mouseScrollDelta.y);
            if (Mathf.Abs(zoomInput) > Mathf.Epsilon)
            {
                _distance = Mathf.Clamp(_distance - zoomInput * _zoomSpeed, _minDistance, _maxDistance);
            }
        }

        private void HandleOrbitInput()
        {
            Vector2 orbitInput = ReadVector2(_orbitAction);
            bool orbitButtonHeld = ReadButton(_orbitButtonAction);

            if (!orbitButtonHeld && _orbitButtonAction != null)
            {
                orbitInput = Vector2.zero;
            }

            // Fallback to legacy mouse delta if action not configured.
            if (orbitInput.sqrMagnitude <= 0f && Mouse.current != null)
            {
                if (_orbitButtonAction == null)
                {
                    if (Mouse.current.rightButton.isPressed)
                        orbitInput = Mouse.current.delta.ReadValue();
                }
                else if (orbitButtonHeld)
                {
                    orbitInput = Mouse.current.delta.ReadValue();
                }
            }

            if (orbitInput.sqrMagnitude > 0f)
            {
                _yaw += orbitInput.x * _orbitSpeed * Time.deltaTime;
                _pitch -= orbitInput.y * _orbitSpeed * Time.deltaTime;
                _pitch = Mathf.Clamp(_pitch, _minPitch, _maxPitch);
            }
        }

        private void HandlePanInput()
        {
            Vector3 move = Vector3.zero;

            // Mouse/pen pan (e.g., middle mouse drag)
            Vector2 panInput = ReadVector2(_panAction);
            bool panButtonHeld = ReadButton(_panButtonAction);
            if (!panButtonHeld && _panButtonAction != null)
            {
                panInput = Vector2.zero;
            }

            if (panInput.sqrMagnitude <= 0f && Mouse.current != null)
            {
                if (_panButtonAction == null)
                {
                    if (Mouse.current.middleButton.isPressed)
                        panInput = Mouse.current.delta.ReadValue();
                }
                else if (panButtonHeld)
                {
                    panInput = Mouse.current.delta.ReadValue();
                }
            }

            if (panInput.sqrMagnitude > 0f)
            {
                Vector3 right = Vector3.ProjectOnPlane(_camera.transform.right, Vector3.up).normalized;
                Vector3 forward = Vector3.ProjectOnPlane(_camera.transform.forward, Vector3.up).normalized;
                if (right.sqrMagnitude < 0.0001f) right = Vector3.right;
                if (forward.sqrMagnitude < 0.0001f) forward = Vector3.forward;

                float scale = _distance * 0.1f + 1f;
                move += (right * -panInput.x + forward * -panInput.y) * _panSpeed * scale * Time.deltaTime;
            }

            // Keyboard/controller pan
            Vector2 moveInput = ReadVector2(_moveAction);
            if (moveInput.sqrMagnitude > Mathf.Epsilon)
            {
                Quaternion yawRotation = Quaternion.Euler(0f, _yaw, 0f);
                Vector3 planarRight = yawRotation * Vector3.right;
                Vector3 planarForward = yawRotation * Vector3.forward;
                move += (planarRight * moveInput.x + planarForward * moveInput.y) * _panSpeed * Time.deltaTime;
            }

            if (move.sqrMagnitude > 0f)
            {
                _focusPoint += move;
                ClampFocusToBounds();
            }
        }

        #endregion

        #region Deck Helpers

        private void FocusDeckByIndex(int index)
        {
            if (!EnsureDeckList() || index < 0 || index >= _deckLevels.Count)
                return;

            int deckLevel = _deckLevels[index];

            if (!_shipView.TryGetDeckBounds(deckLevel, out _activeBounds))
            {
                _hasFocus = false;
                return;
            }

            if (!_shipView.TryGetDeckCenterWorld(deckLevel, out Vector3 center))
            {
                center = _activeBounds.center;
            }

            _focusPoint = _autoCenterOnDeckSwitch
                ? new Vector3(center.x, _activeBounds.center.y, center.z)
                : new Vector3(_focusPoint.x, _activeBounds.center.y, _focusPoint.z);

            float suggestedDistance = Mathf.Max(_activeBounds.extents.x, _activeBounds.extents.z) * 2f;
            if (_distance <= 0f)
            {
                _distance = Mathf.Clamp(suggestedDistance, _minDistance, _maxDistance);
            }
            else
            {
                _distance = Mathf.Clamp(_distance, _minDistance, _maxDistance);
            }

            ClampFocusToBounds();

            _activeDeckIndex = index;
            _hasFocus = true;

            if (_shipView != null)
            {
                _shipView.SetDeckVisibility(deckLevel, _showDecksBelow, _hideDecksAbove);
            }
        }

        private bool EnsureDeckList()
        {
            if (_shipView == null)
            {
                HookShipView(Object.FindFirstObjectByType<ShipView3D>());
            }
            else if (_subscribedShipView == null)
            {
                HookShipView(_shipView);
            }

            if (_shipView == null)
                return false;

            if (!_shipView.TryGetDeckLevels(_deckLevelBuffer))
                return false;

            bool changed = _deckLevels.Count != _deckLevelBuffer.Count;
            if (!changed)
            {
                for (int i = 0; i < _deckLevels.Count; i++)
                {
                    if (_deckLevels[i] != _deckLevelBuffer[i])
                    {
                        changed = true;
                        break;
                    }
                }
            }

            if (changed)
            {
                _deckLevels.Clear();
                _deckLevels.AddRange(_deckLevelBuffer);
            }

            if (_deckLevels.Count == 0)
                return false;

            _activeDeckIndex = Mathf.Clamp(_activeDeckIndex, 0, _deckLevels.Count - 1);

            if (changed && _initialized)
            {
                FocusDeckByIndex(_activeDeckIndex);
            }

            return true;
        }

        private void StepDeck(int delta)
        {
            if (_deckLevels.Count == 0)
                return;

            int newIndex = Mathf.Clamp(_activeDeckIndex + delta, 0, _deckLevels.Count - 1);
            if (newIndex != _activeDeckIndex)
            {
                FocusDeckByIndex(newIndex);
            }
        }

        private void ClampFocusToBounds()
        {
            if (!_useCameraBounds)
                return;

            Vector3 center = _activeBounds.center;
            Vector3 extents = _activeBounds.extents;

            _focusPoint.x = Mathf.Clamp(_focusPoint.x, center.x - extents.x, center.x + extents.x);
            _focusPoint.z = Mathf.Clamp(_focusPoint.z, center.z - extents.z, center.z + extents.z);
            _focusPoint.y = center.y;
        }

        #endregion

        private void HookShipView(ShipView3D view)
        {
            if (_subscribedShipView == view)
            {
                _shipView = view;
                return;
            }

            if (_subscribedShipView != null)
            {
                _subscribedShipView.DefaultDeckFocusRequested -= OnDefaultDeckFocusRequested;
            }

            _subscribedShipView = view;
            _shipView = view;

            if (_subscribedShipView != null)
            {
                _subscribedShipView.DefaultDeckFocusRequested += OnDefaultDeckFocusRequested;
            }
        }

        private void OnDefaultDeckFocusRequested(int deckLevel)
        {
            if (!isActiveAndEnabled)
                return;

            FocusDeck(deckLevel);

            if (_deckLevels.Count > 0)
            {
                _initialized = true;
            }
        }

        #region Input Utility Helpers

        private static void EnableReference(InputActionReference reference)
        {
            reference?.action?.Enable();
        }

        private static void DisableReference(InputActionReference reference)
        {
            reference?.action?.Disable();
        }

        private static Vector2 ReadVector2(InputActionReference reference)
        {
            return reference != null && reference.action != null
                ? reference.action.ReadValue<Vector2>()
                : Vector2.zero;
        }

        private static float ReadFloat(InputActionReference reference, float fallback = 0f)
        {
            return reference != null && reference.action != null
                ? reference.action.ReadValue<float>()
                : fallback;
        }

        private static bool ReadTriggered(InputActionReference reference)
        {
            return reference != null && reference.action != null && reference.action.triggered;
        }

        private static bool ReadButton(InputActionReference reference)
        {
            return reference != null && reference.action != null && reference.action.ReadValue<float>() >= 0.5f;
        }

        private static void RegisterDeckAction(InputActionReference reference, System.Action<InputAction.CallbackContext> callback)
        {
            if (reference != null && reference.action != null && callback != null)
            {
                reference.action.performed += callback;
                reference.action.Enable();
            }
        }

        private static void UnregisterDeckAction(InputActionReference reference, System.Action<InputAction.CallbackContext> callback)
        {
            if (reference != null && reference.action != null && callback != null)
            {
                reference.action.performed -= callback;
            }
        }

        private void OnDeckUpPerformed(InputAction.CallbackContext ctx) => StepDeck(1);
        private void OnDeckDownPerformed(InputAction.CallbackContext ctx) => StepDeck(-1);

        #endregion
    }
}
