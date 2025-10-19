using System;
using System.Collections.Generic;
using UnityEngine;

namespace CruiseLineInc.Systems
{
    /// <summary>
    /// Singleton that tracks ship resources: Money, Water, Food, Waste
    /// Simple numeric pools with capacity limits and change events
    /// </summary>
    public class ResourceManager : MonoBehaviour
    {
        #region Singleton
        
        private static ResourceManager _instance;
        public static ResourceManager Instance => _instance;
        
        #endregion
        
        #region Resources
        
        private Dictionary<ResourceType, float> _currentResources = new Dictionary<ResourceType, float>();
        private Dictionary<ResourceType, float> _capacities = new Dictionary<ResourceType, float>();
        
        #endregion
        
        #region Events
        
        public event Action<ResourceType, float, float> OnResourceChanged; // (type, oldValue, newValue)
        public event Action<ResourceType> OnResourceDepleted;
        public event Action<ResourceType> OnResourceFull;
        
        #endregion
        
        #region Unity Lifecycle
        
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            
            InitializeResources();
        }
        
        #endregion
        
        #region Initialization
        
        private void InitializeResources()
        {
            // Initialize all resource types with zero
            foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
            {
                _currentResources[type] = 0f;
                _capacities[type] = float.MaxValue; // Default unlimited capacity
            }
            
            Debug.Log("ResourceManager initialized");
        }
        
        /// <summary>
        /// Set starting resources from ship data
        /// </summary>
        public void SetStartingResources(float money, float water, float food, float waste = 0f)
        {
            SetResource(ResourceType.Money, money);
            SetResource(ResourceType.Water, water);
            SetResource(ResourceType.Food, food);
            SetResource(ResourceType.Waste, waste);
            
            Debug.Log($"Starting resources - Money: {money}, Water: {water}, Food: {food}, Waste: {waste}");
        }
        
        /// <summary>
        /// Set resource capacities from ship data
        /// </summary>
        public void SetCapacities(float waterCap, float foodCap, float wasteCap, float fuelCap)
        {
            _capacities[ResourceType.Water] = waterCap;
            _capacities[ResourceType.Food] = foodCap;
            _capacities[ResourceType.Waste] = wasteCap;
            _capacities[ResourceType.Fuel] = fuelCap;
            _capacities[ResourceType.Money] = float.MaxValue; // Money has no cap
            _capacities[ResourceType.Power] = float.MaxValue; // Power is generated, not stored
            
            Debug.Log($"Capacities - Water: {waterCap}, Food: {foodCap}, Waste: {wasteCap}, Fuel: {fuelCap}");
        }
        
        #endregion
        
        #region Public API
        
        /// <summary>
        /// Get current amount of a resource
        /// </summary>
        public float GetCurrent(ResourceType type)
        {
            return _currentResources.TryGetValue(type, out float value) ? value : 0f;
        }
        
        /// <summary>
        /// Get capacity limit for a resource
        /// </summary>
        public float GetCapacity(ResourceType type)
        {
            return _capacities.TryGetValue(type, out float cap) ? cap : float.MaxValue;
        }
        
        /// <summary>
        /// Get fill percentage (0-1)
        /// </summary>
        public float GetFillPercentage(ResourceType type)
        {
            float current = GetCurrent(type);
            float capacity = GetCapacity(type);
            
            if (float.IsInfinity(capacity) || capacity <= 0)
                return 0f;
            
            return Mathf.Clamp01(current / capacity);
        }
        
        /// <summary>
        /// Check if we can afford to spend this amount
        /// </summary>
        public bool CanAfford(ResourceType type, float amount)
        {
            return GetCurrent(type) >= amount;
        }
        
        /// <summary>
        /// Add resource (production)
        /// </summary>
        public bool AddResource(ResourceType type, float amount)
        {
            if (amount <= 0) return false;
            
            float oldValue = GetCurrent(type);
            float capacity = GetCapacity(type);
            float newValue = Mathf.Min(oldValue + amount, capacity);
            
            _currentResources[type] = newValue;
            OnResourceChanged?.Invoke(type, oldValue, newValue);
            
            if (newValue >= capacity)
            {
                OnResourceFull?.Invoke(type);
            }
            
            return true;
        }
        
        /// <summary>
        /// Spend/consume resource
        /// </summary>
        public bool SpendResource(ResourceType type, float amount)
        {
            if (amount <= 0) return false;
            if (!CanAfford(type, amount)) return false;
            
            float oldValue = GetCurrent(type);
            float newValue = oldValue - amount;
            
            _currentResources[type] = newValue;
            OnResourceChanged?.Invoke(type, oldValue, newValue);
            
            if (newValue <= 0)
            {
                OnResourceDepleted?.Invoke(type);
            }
            
            return true;
        }
        
        /// <summary>
        /// Set resource to exact value (for initialization or loading saves)
        /// </summary>
        public void SetResource(ResourceType type, float value)
        {
            float oldValue = GetCurrent(type);
            _currentResources[type] = Mathf.Max(0, value);
            OnResourceChanged?.Invoke(type, oldValue, value);
        }
        
        #endregion
        
        #region Debug
        
        public void LogAllResources()
        {
            Debug.Log("=== Current Resources ===");
            foreach (var kvp in _currentResources)
            {
                float capacity = GetCapacity(kvp.Key);
                string capStr = float.IsInfinity(capacity) ? "∞" : capacity.ToString("F0");
                Debug.Log($"{kvp.Key}: {kvp.Value:F1} / {capStr}");
            }
        }
        
        #endregion
    }
}
