namespace CruiseLineInc
{
    /// <summary>
    /// Global constants for the game
    /// </summary>
    public static class Constants
    {
        // Simulation
        public const float TickRate = 0.1f; // 10 Hz update rate
        public const float TimeScale_Normal = 1f;
        public const float TimeScale_Fast = 3f;
        
        // MVP Ship Specs (Mistral)
        public const int MistralDeckCount = 6;
        public const int MistralTotalTiles = 64;
        public const int MistralMaxPassengers = 40;
        public const int MistralMaxCrew = 10;
        public const int MistralVoyageDays = 2;
        
        // Resource Starting Values (Mistral)
        public const float StartingMoney = 5000f;
        public const float StartingWater = 150f;
        public const float StartingFood = 200f;
        public const float StartingWaste = 0f;
        public const float WasteCapacity = 180f;
        
        // Resource Rates (per day)
        public const float WaterConsumptionPerPassenger = 1.5f;
        public const float FoodConsumptionPerPassenger = 2f;
        public const float WasteGenerationPerPassenger = 1.5f;
        
        // Construction
        public const int MaxConstructionQueue = 5;
        public const float ConstructionDisruptionRadius = 2f; // tiles
        
        // Agent LOD (MVP: LOD0 only)
        public const int MaxVisibleAgents = 150;
        
        // UI
        public const float TooltipDelay = 0.5f;
        public const float NotificationDuration = 3f;
        
        // Pathfinding
        public const float ElevatorLoadTime = 2f; // seconds
        public const float ElevatorUnloadTime = 1f; // seconds
        public const float ElevatorTravelTimePerDeck = 3f; // seconds
        
        // Satisfaction Thresholds
        public const float SatisfactionGood = 0.7f;
        public const float SatisfactionPoor = 0.4f;
        
        // Resource Alert Thresholds
        public const float ResourceCritical = 0.2f;
        public const float ResourceLow = 0.4f;
    }
}
