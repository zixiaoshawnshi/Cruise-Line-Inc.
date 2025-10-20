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

        private void Awake()
        {
            if (_paintTool == null)
            {
                _paintTool = FindFirstObjectByType<ZonePaintTool>();
            }

            if (_paintModeToggle != null)
            {
                _paintModeToggle.onValueChanged.AddListener(OnPaintModeToggled);
            }

            if (_profileDropdown != null)
            {
                _profileDropdown.onValueChanged.AddListener(OnProfileChanged);
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
    }
}
