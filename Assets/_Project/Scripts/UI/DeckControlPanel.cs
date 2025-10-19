using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CruiseLineInc.UI
{
    /// <summary>
    /// Simple controller for deck navigation buttons.
    /// </summary>
    public class DeckControlPanel : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Ship3D.ShipCameraController _cameraController;
        [SerializeField] private Ship3D.ShipView3D _shipView;
        [SerializeField] private Button _previousDeckButton;
        [SerializeField] private Button _nextDeckButton;
        [SerializeField] private TextMeshProUGUI _deckLabel;

        private readonly List<int> _deckLevels = new();
        private Coroutine _refreshRoutine;
        private bool _loggedMissingRefs;
        private bool _loggedMissingDecks;

        private void Awake()
        {
            if (_cameraController == null)
            {
                _cameraController = Object.FindFirstObjectByType<Ship3D.ShipCameraController>();
            }

            if (_shipView == null)
            {
                _shipView = Object.FindFirstObjectByType<Ship3D.ShipView3D>();
            }

            if (_previousDeckButton != null)
            {
                _previousDeckButton.onClick.AddListener(OnPreviousDeckClicked);
            }

            if (_nextDeckButton != null)
            {
                _nextDeckButton.onClick.AddListener(OnNextDeckClicked);
            }
        }

        private void OnEnable()
        {
            Refresh();
        }

        private void OnDestroy()
        {
            if (_previousDeckButton != null)
            {
                _previousDeckButton.onClick.RemoveListener(OnPreviousDeckClicked);
            }

            if (_nextDeckButton != null)
            {
                _nextDeckButton.onClick.RemoveListener(OnNextDeckClicked);
            }
        }

        public void Refresh()
        {
            if (!TryRefreshImmediate(logWarnings: true))
            {
                ScheduleRefreshRetry();
            }
        }

        private bool TryRefreshImmediate(bool logWarnings)
        {
            if (_cameraController == null)
            {
                _cameraController = Object.FindFirstObjectByType<Ship3D.ShipCameraController>();
            }

            if (_shipView == null)
            {
                _shipView = Object.FindFirstObjectByType<Ship3D.ShipView3D>();
            }

            if (_cameraController == null || _shipView == null)
            {
                SetLabel("-");
                SetButtonState(false, false);

                if (logWarnings && !_loggedMissingRefs)
                {
                    Debug.LogWarning("DeckControlPanel: Waiting for ShipCameraController or ShipView3D references.");
                    _loggedMissingRefs = true;
                }

                return false;
            }

            _loggedMissingRefs = false;

            if (!_shipView.TryGetDeckLevels(_deckLevels) || _deckLevels.Count == 0)
            {
                SetLabel("-");
                SetButtonState(false, false);

                if (logWarnings && !_loggedMissingDecks)
                {
                    Debug.Log("DeckControlPanel: Deck data not yet available, will retry shortly.");
                    _loggedMissingDecks = true;
                }

                return false;
            }

            _loggedMissingDecks = false;

            bool hasPrev = false;
            bool hasNext = false;
            string labelText = "-";

            int activeLevel;
            if (!_cameraController.TryGetActiveDeckLevel(out activeLevel))
            {
                if (_deckLevels.Count > 0)
                {
                    activeLevel = _deckLevels[0];
                    _cameraController.FocusDeck(activeLevel);
                }
                else
                {
                    SetLabel("-");
                    SetButtonState(false, false);
                    return true;
                }
            }

            labelText = $"Deck {activeLevel}";
            int idx = _deckLevels.IndexOf(activeLevel);
            if (idx >= 0)
            {
                hasPrev = idx > 0;
                hasNext = idx < _deckLevels.Count - 1;
            }

            SetLabel(labelText);
            SetButtonState(hasPrev, hasNext);
            return true;
        }

        private void ScheduleRefreshRetry()
        {
            if (!isActiveAndEnabled || gameObject == null)
                return;

            if (_refreshRoutine != null)
                return;

            _refreshRoutine = StartCoroutine(WaitForDeckData());
        }

        private System.Collections.IEnumerator WaitForDeckData()
        {
            while (isActiveAndEnabled)
            {
                if (TryRefreshImmediate(logWarnings: false))
                {
                    _refreshRoutine = null;
                    yield break;
                }
                yield return null;
            }

            _refreshRoutine = null;
        }

        private void OnPreviousDeckClicked()
        {
            if (_cameraController == null) return;

            _cameraController.FocusPreviousDeck();
            Refresh();
        }

        private void OnNextDeckClicked()
        {
            if (_cameraController == null) return;

            _cameraController.FocusNextDeck();
            Refresh();
        }

        private void SetLabel(string text)
        {
            if (_deckLabel != null)
            {
                _deckLabel.text = text;
            }
        }

        private void SetButtonState(bool prevEnabled, bool nextEnabled)
        {
            if (_previousDeckButton != null)
            {
                _previousDeckButton.interactable = prevEnabled;
            }

            if (_nextDeckButton != null)
            {
                _nextDeckButton.interactable = nextEnabled;
            }
        }

        private void OnDisable()
        {
            if (_refreshRoutine != null)
            {
                StopCoroutine(_refreshRoutine);
                _refreshRoutine = null;
            }
        }
    }
}
