# MVP Fundamental Systems Overview

## 🎯 **Goal**

Deliver a functional **coastal cruise MVP** simulating a short voyage (2–3 in-game days) with a few hundred agents and a complete gameplay loop:

> Plan → Build → Sail → Debrief
> 

---

## 🧱 **A. World & Structural Systems**

### 1. **Tile System**

- Each tile stores:
    - `deck_level`
    - `deck_type` (Promenade / Main / Libo / Sun)
    - `is_buildable`, `is_entrance`
    - `zone_tag` (e.g., family/adult/service)
- Tiles compose **Decks**, which form **Ships**.

### 2. **Deck & Ship Hierarchy**

- **Deck** = grid of tiles + nav nodes.
- **Ship** = collection of decks + ship-level stats (capacity, tank sizes, etc.).
- **Ship class** determines:
    - Deck count & layout template.
    - Tile dimensions & max buildable zones.
    - Default starting resources.

### 3. **Special Tiles**

- **Entrances**: boarding points for embark/disembark.
- **Vertical connectors**: elevators, stairs (navigation graph nodes).

---

## ⚙️ **B. Core Simulation Systems**

### 4. **Resource Management**

- Tracks **Money**, **Water**, **Food**, **Waste**.
- Each tick:
    - `Δ = Σ(production − consumption)` from active modules and agents.
- Simple thresholds trigger alerts and events.
- Abstract network model (numeric pools only for MVP).

### 5. **Building / Room System**

- **RoomType ScriptableObject** defines:
    - Allowed decks, size (tiles), cost, upkeep.
    - Effects on agents (comfort, food, entertainment).
    - Resource use and production.
- **Construction System**:
    - Timed builds with local disruption (−comfort/−hygiene radius).
    - Refund on cancel; max 5 queued jobs.
    - Room instance activates after timer completes.

### 6. **Tile-to-Building Rules**

- Deck types restrict module placement:
    - *Sun Deck*: pools, observatory.
    - *Main Deck*: cabins, buffet.
    - *Crew Deck*: crew, utilities.
    - *Promenade Deck*: entrances, lounges, public areas.

---

## 🧍 **C. Agent & Behavior Systems**

### 7. **Agent Core**

- `Agent` base → `Passenger`, `Crew` subclasses.
- Stats: `energy`, `satisfaction`, `position`, `state`.
- Simple FSM: Idle / Moving / UsingRoom / Resting.
- Uses deck graph pathfinding (queues for elevators).

### 8. **Passenger System**

- Attributes: `passenger_type`, `budget`, `comfort_pref`, `avoid_tags`.
- Tick effects from room usage, crowding, and resource shortages.
- Example types: Family / Couple / Retiree / College.

### 9. **Crew System**

- Roles: Cleaner / Caterer / Engineer / Medic / Entertainer.
- Attributes: `energy`, `morale`, `skill`.
- Perform background tasks affecting room efficiency.

### 10. **Access & Zone System (light version)**

- Rooms and edges carry `zone_tags` (family/adult/staff).
- Cohort × zone policy matrix: **FORBID**, **AVOID**, **PREFER**, **NEUTRAL**.
- Used by pathfinding + satisfaction modifiers.

---

## 📈 **D. Simulation Loop & Managers**

### 11. **Simulation Loop**

Each tick:

1. Update agents (movement, state, stats).
2. Update rooms (production/consumption).
3. Update global resources (pool totals).
4. Check triggers (events, warnings).
5. Update UI metrics.

### 12. **LOD Simulation**

- LOD0: visible passengers (≤150).
- LOD2: cohort aggregates drive occupancy & resource use.
- Switch dynamically with camera zoom.

### 13. **Event & Trigger System**

- Lightweight dispatcher for:
    - Resource shortages
    - Mechanical jams
    - Crew fatigue
    - Passenger complaints
- Simple choice UI (A/B options with numeric modifiers).

---

## ⏱ **E. Phase & Voyage Systems**

### 14. **Phase Controller**

Controls the main gameplay loop:

```

```