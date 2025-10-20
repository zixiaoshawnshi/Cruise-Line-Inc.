namespace CruiseLineInc
{
    /// <summary>
    /// Types of decks on a ship
    /// </summary>
    public enum DeckType
    {
        Bridge,      // Command deck with bridge and captain's quarters
        Main,        // Passenger cabins and amenities
        Promenade,   // Public areas, entrances, social spaces
        Crew,        // Crew quarters and service areas
        Sun,         // Outdoor recreational areas (pools, observatory)
        Engine       // Propulsion and core systems
    }

    /// <summary>
    /// Types of individual tiles (allows mixed types per deck)
    /// </summary>
    public enum TileType
    {
        Indoor,      // Regular interior tile (cabins, restaurants, lounges)
        Outdoor,     // Exterior deck tile (sun decks, balconies, pools)
        Entrance,    // Boarding/docking areas
        Corridor,    // Hallways and walkways
        Utility,     // Technical areas (engine room, storage, maintenance)
        Restricted,  // Command areas (bridge, captain's quarters)
        Special      // Event-specific or unique tiles
    }

    /// <summary>
    /// Access control tags for agents
    /// </summary>
    public enum AccessTag
    {
        Public,      // Accessible to all passengers
        Family,      // Family-friendly areas
        Adult,       // Adult-only areas
        Premium,     // Premium passenger access
        Luxury,      // Luxury passenger access
        Staff,       // Crew-only areas
        Restricted   // Command/technical areas
    }

    /// <summary>
    /// Zone access policy for agent navigation
    /// </summary>
    public enum AccessPolicy
    {
        Forbid,      // Cannot enter
        Avoid,       // Can enter but will avoid
        Neutral,     // No preference
        Prefer       // Will actively seek
    }

    /// <summary>
    /// Game phase state machine
    /// </summary>
    public enum GamePhase
    {
        Planning,    // Port - build and prepare
        Voyage,      // At sea - simulation running
        Debrief      // Post-voyage - review and progression
    }

    /// <summary>
    /// Resource types tracked by the ship
    /// </summary>
    public enum ResourceType
    {
        Money,
        Water,
        Food,
        Waste,
        Power,
        Fuel
    }

    /// <summary>
    /// Passenger tier for cabin assignment
    /// </summary>
    public enum PassengerTier
    {
        Standard,
        Premium,
        Luxury,
        Special      // VIPs, events, storyline characters
    }

    /// <summary>
    /// Passenger archetypes
    /// </summary>
    public enum PassengerType
    {
        Couple,
        Family,
        Retiree,
        College,
        Influencer,
        Scientist
    }

    /// <summary>
    /// Crew roles
    /// </summary>
    public enum CrewRole
    {
        Cleaner,
        Caterer,
        Engineer,
        Medic,
        Entertainer,
        Security
    }

    /// <summary>
    /// Agent state machine
    /// </summary>
    public enum AgentState
    {
        Idle,        // Waiting for decision
        Moving,      // Pathfinding to destination
        UsingRoom,   // Interacting with facility
        Resting,     // In cabin/quarters
        Queuing      // Waiting in line
    }
}
