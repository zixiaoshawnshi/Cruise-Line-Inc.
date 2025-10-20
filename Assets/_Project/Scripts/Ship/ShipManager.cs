using UnityEngine;
using CruiseLineInc.Ship.Data;

namespace CruiseLineInc.Ship
{
    public class ShipManager : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private ShipClassData _shipClassData;
        [Header("Debug")]
        [SerializeField] private bool _autoLoadOnStart = true;

        private ShipData _currentShipData;

        private static ShipManager _instance;
        public static ShipManager Instance => _instance;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
        }

        private void Start()
        {
            // Ensure ResourceManager exists
            if (Systems.ResourceManager.Instance == null)
            {
                GameObject resourceManagerObj = new GameObject("ResourceManager");
                resourceManagerObj.AddComponent<Systems.ResourceManager>();
                Debug.Log("Created ResourceManager");
            }

            if (_autoLoadOnStart && _shipClassData != null)
            {
                LoadShip(_shipClassData);
            }
        }

        public ShipData LoadShip(ShipClassData shipClassData)
        {
            if (shipClassData == null)
            {
                Debug.LogError("ShipClassData is null!");
                return null;
            }

            Debug.Log($"Loading ship: {shipClassData.ShipName}");

            _currentShipData = shipClassData.CreateShipData();

            // Initialize resources
            if (Systems.ResourceManager.Instance != null)
            {
                Systems.ResourceManager.Instance.SetCapacities(
                    shipClassData.WaterCapacity,
                    shipClassData.FoodCapacity,
                    shipClassData.WasteCapacity,
                    shipClassData.FuelCapacity
                );

                Systems.ResourceManager.Instance.SetStartingResources(
                    shipClassData.StartingMoney,
                    shipClassData.StartingWater,
                    shipClassData.StartingFood,
                    0f // Starting waste
                );
            }

            return _currentShipData;
        }

        public ShipData GetCurrentShipData() => _currentShipData;
        public Deck GetDeck(int deckLevel) => _currentShipData?.GetDeck(deckLevel);
        public ShipTile GetTile(int x, int z, int deckLevel) => _currentShipData?.GetTile(x, z, deckLevel);
        public ShipTile GetTile(int x, int deckLevel) => _currentShipData?.GetTile(x, deckLevel);

        private void LateUpdate()
        {
            ShipUpdateDispatcher.Instance.ProcessPending();
        }
    }
}
