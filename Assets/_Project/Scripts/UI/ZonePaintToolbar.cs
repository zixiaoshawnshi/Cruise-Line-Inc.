using System.Collections.Generic;
using CruiseLineInc.Ship3D;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CruiseLineInc.UI
{
    /// <summary>
    /// Minimal toolbar for toggling zone paint mode and switching between hard-coded zone profiles.
    /// This will evolve into a richer build UI later.
    /// </summary>
    public class ZonePaintToolbar : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ZonePaintTool _paintTool;
        [SerializeField] private Toggle _paintModeToggle;
        [SerializeField] private TMP_Dropdown _profileDropdown;
        [SerializeField] private Button _confirmButton;
        [SerializeField] private Button _cancelButton;

        private void Awake()
        {
            if (_paintTool == null)
            {
                _paintTool = FindFirstObjectByType<ZonePaintTool>();
            }

            EnsureRuntimeButtons();

            if (_paintModeToggle != null)
            {
                _paintModeToggle.onValueChanged.AddListener(OnPaintModeToggled);
            }

            if (_profileDropdown != null)
            {
                _profileDropdown.onValueChanged.AddListener(OnProfileChanged);
            }

            if (_confirmButton != null)
            {
                _confirmButton.onClick.AddListener(OnConfirmPressed);
            }

            if (_cancelButton != null)
            {
                _cancelButton.onClick.AddListener(OnCancelPressed);
            }
        }

        private void Start()
        {
            if (_paintTool == null)
            {
                Debug.LogWarning("ZonePaintToolbar: No ZonePaintTool found.");
                return;
            }

            if (_profileDropdown != null)
            {
                PopulateProfiles();
            }

            if (_paintModeToggle != null)
            {
                _paintModeToggle.isOn = _paintTool.PaintModeEnabled;
            }

            RefreshButtonState();
        }

        private void OnDestroy()
        {
            if (_paintModeToggle != null)
            {
                _paintModeToggle.onValueChanged.RemoveListener(OnPaintModeToggled);
            }

            if (_profileDropdown != null)
            {
                _profileDropdown.onValueChanged.RemoveListener(OnProfileChanged);
            }

            if (_confirmButton != null)
            {
                _confirmButton.onClick.RemoveListener(OnConfirmPressed);
            }

            if (_cancelButton != null)
            {
                _cancelButton.onClick.RemoveListener(OnCancelPressed);
            }
        }

        private void Update()
        {
            RefreshButtonState();
        }

        private void PopulateProfiles()
        {
            if (_profileDropdown == null || _paintTool == null)
                return;

            _profileDropdown.ClearOptions();
            string[] names = _paintTool.GetProfileNames();
            if (names != null && names.Length > 0)
            {
                List<string> options = new List<string>(names);
                _profileDropdown.AddOptions(options);
                _profileDropdown.value = Mathf.Clamp(_paintTool.ActiveProfileIndex, 0, options.Count - 1);
            }
        }

        private void OnPaintModeToggled(bool isOn)
        {
            if (_paintTool != null)
            {
                _paintTool.SetPaintModeEnabled(isOn);
            }
        }

        private void OnProfileChanged(int index)
        {
            if (_paintTool != null)
            {
                _paintTool.SetActiveProfile(index);
            }
        }

        private void OnConfirmPressed()
        {
            _paintTool?.ConfirmPendingPaint();
        }

        private void OnCancelPressed()
        {
            _paintTool?.CancelPendingPaint();
        }

        private void RefreshButtonState()
        {
            bool hasPending = _paintTool != null && _paintTool.HasPendingPaint;
            if (_confirmButton != null)
            {
                _confirmButton.interactable = hasPending;
            }

            if (_cancelButton != null)
            {
                _cancelButton.interactable = hasPending;
            }
        }

        private void EnsureRuntimeButtons()
        {
            if (_confirmButton == null)
            {
                _confirmButton = CreateToolbarButton("ConfirmButton", "Confirm");
            }

            if (_cancelButton == null)
            {
                _cancelButton = CreateToolbarButton("CancelButton", "Cancel");
            }
        }

        private Button CreateToolbarButton(string name, string label)
        {
            GameObject buttonObj = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            buttonObj.layer = gameObject.layer;
            RectTransform rect = buttonObj.GetComponent<RectTransform>();
            rect.SetParent(transform, false);
            rect.sizeDelta = new Vector2(160f, 60f);

            Image image = buttonObj.GetComponent<Image>();
            image.color = new Color(0.85f, 0.85f, 0.85f, 1f);

            Button button = buttonObj.GetComponent<Button>();
            button.targetGraphic = image;

            GameObject textObj = new GameObject("Label", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
            textObj.layer = gameObject.layer;
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.SetParent(rect, false);
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            TextMeshProUGUI text = textObj.GetComponent<TextMeshProUGUI>();
            text.text = label;
            text.alignment = TextAlignmentOptions.Center;
            text.fontSize = 26f;

            LayoutElement layout = buttonObj.AddComponent<LayoutElement>();
            layout.minWidth = 150f;
            layout.minHeight = 60f;

            return button;
        }
    }
}
