using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CruiseLineInc.Systems;

namespace CruiseLineInc.UI
{
    /// <summary>
    /// Top banner UI displaying current resources: Money, Water, Food, Waste
    /// Updates in real-time when resources change
    /// </summary>
    public class ResourcePanel : MonoBehaviour
    {
        [Header("Resource Display Elements")]
        [SerializeField] private TextMeshProUGUI _moneyText;
        [SerializeField] private TextMeshProUGUI _waterText;
        [SerializeField] private TextMeshProUGUI _foodText;
        [SerializeField] private TextMeshProUGUI _wasteText;
        
        [Header("Color Settings")]
        [SerializeField] private Color _normalColor = Color.white;
        [SerializeField] private Color _lowColor = Color.yellow;
        [SerializeField] private Color _criticalColor = Color.red;
        [SerializeField] private Color _fullColor = Color.cyan;
        
        [Header("Thresholds")]
        [SerializeField] private float _lowThreshold = 0.25f;  // 25% = yellow
        [SerializeField] private float _criticalThreshold = 0.1f;  // 10% = red
        [SerializeField] private float _fullThreshold = 0.95f;  // 95% = cyan
        
        private ResourceManager _resourceManager;
        
        #region Unity Lifecycle
        
        private void Start()
        {
            _resourceManager = ResourceManager.Instance;
            
            if (_resourceManager == null)
            {
                Debug.LogError("ResourcePanel: ResourceManager not found!");
                return;
            }
            
            // Subscribe to resource change events
            _resourceManager.OnResourceChanged += OnResourceChanged;
            
            // Initial update
            UpdateAllDisplays();
        }
        
        private void OnDestroy()
        {
            if (_resourceManager != null)
            {
                _resourceManager.OnResourceChanged -= OnResourceChanged;
            }
        }
        
        #endregion
        
        #region Event Handlers
        
        private void OnResourceChanged(ResourceType type, float oldValue, float newValue)
        {
            UpdateDisplay(type);
        }
        
        #endregion
        
        #region Update Display
        
        private void UpdateAllDisplays()
        {
            UpdateDisplay(ResourceType.Money);
            UpdateDisplay(ResourceType.Water);
            UpdateDisplay(ResourceType.Food);
            UpdateDisplay(ResourceType.Waste);
        }
        
        private void UpdateDisplay(ResourceType type)
        {
            if (_resourceManager == null) return;
            
            float current = _resourceManager.GetCurrent(type);
            float capacity = _resourceManager.GetCapacity(type);
            float fillPercent = _resourceManager.GetFillPercentage(type);
            
            TextMeshProUGUI targetText = GetTextForType(type);
            if (targetText == null) return;
            
            // Format text
            string text = FormatResourceText(type, current, capacity);
            targetText.text = text;
            
            // Set color based on fill percentage
            targetText.color = GetColorForFillLevel(type, fillPercent);
        }
        
        private string FormatResourceText(ResourceType type, float current, float capacity)
        {
            // Money has no capacity limit
            if (type == ResourceType.Money)
            {
                return $"${current:F0}";
            }
            
            // Other resources show current/max
            if (float.IsInfinity(capacity))
            {
                return $"{current:F0}";
            }
            
            return $"{current:F0}/{capacity:F0}";
        }
        
        private Color GetColorForFillLevel(ResourceType type, float fillPercent)
        {
            // Waste is inverted - full is bad
            if (type == ResourceType.Waste)
            {
                if (fillPercent >= _fullThreshold) return _criticalColor;
                if (fillPercent >= _lowThreshold) return _lowColor;
                return _normalColor;
            }
            
            // Normal resources - low is bad
            if (fillPercent <= _criticalThreshold) return _criticalColor;
            if (fillPercent <= _lowThreshold) return _lowColor;
            if (fillPercent >= _fullThreshold) return _fullColor;
            
            return _normalColor;
        }
        
        private TextMeshProUGUI GetTextForType(ResourceType type)
        {
            return type switch
            {
                ResourceType.Money => _moneyText,
                ResourceType.Water => _waterText,
                ResourceType.Food => _foodText,
                ResourceType.Waste => _wasteText,
                _ => null
            };
        }
        
        #endregion
        
        #region Public API
        
        /// <summary>
        /// Force refresh all resource displays
        /// </summary>
        public void RefreshAll()
        {
            UpdateAllDisplays();
        }
        
        #endregion
    }
}
