# 🚢 Cruise Line Inc - Quick Start Guide

## Phase 1 Complete: Tile + Deck + Ship System ✅

You now have the foundational ship architecture system ready to use!

## What's Been Created

### 📁 Folder Structure
```
Assets/_Project/
├── Scripts/
│   ├── Core/          (ready for GameManager, PhaseController)
│   ├── Ship/          ✅ Complete!
│   ├── Construction/  (ready)
│   ├── Agents/        (ready)
│   ├── Navigation/    (ready)
│   ├── Rooms/         (ready)
│   ├── Resources/     (ready)
│   ├── Events/        (ready)
│   ├── Tutorial/      (ready)
│   ├── UI/            (ready)
│   └── Utilities/     ✅ Complete!
├── Data/              (ready for ScriptableObjects)
└── Editor/            ✅ Wizard created!
```

### ✅ Completed Systems

1. **Tile System** (`Tile.cs`)
   - Grid-based positioning
   - Occupancy tracking
   - Zone access control
   - Navigation flags

2. **Deck System** (`Deck.cs`)
   - Tile grid management
   - Room placement validation
   - Special tile registration (entrances, elevators)
   - World/grid coordinate conversion
   - Visual debugging (Gizmos)

3. **Ship System** (`Ship.cs`)
   - Multi-deck management
   - Capacity tracking
   - Resource limits
   - Agent counting

4. **ShipManager** (`ShipManager.cs`)
   - Singleton pattern
   - Ship loading from data
   - Global ship access
   - Debug utilities

5. **Data Classes**
   - `DeckTypeData.cs` - Deck templates
   - `ShipClassData.cs` - Complete ship configs

6. **Utilities**
   - `Enums.cs` - All game enums
   - `Constants.cs` - Game constants
   - `Extensions.cs` - Helper methods

7. **Editor Tools**
   - `MistralSetupWizard.cs` - One-click Mistral creation

---

## 🎮 How to Use

### Step 1: Create Mistral Configuration

**Option A: Use the Wizard (Recommended)**
```
1. In Unity menu: Cruise Line Inc → Setup → Create Mistral Configuration
2. Click "Create Mistral Configuration"
3. Done! All assets created automatically.
```

**Option B: Manual Creation**
```
1. Create Deck Type Data assets (6 total)
2. Create Ship Class Data asset
3. Configure manually
```

### Step 2: Set Up the Scene

1. **Create Ship Manager:**
   ```
   Create → Empty GameObject
   Name: "ShipManager"
   Add Component → ShipManager
   ```

2. **Assign Configuration:**
   ```
   In Inspector:
   - Ship Class Data: Drag "Mistral_CoastalClass"
   - Auto Load On Start: ✓
   ```

3. **Optional - Create Ship Container:**
   ```
   Create → Empty GameObject
   Name: "ShipContainer"
   Drag to ShipManager's "Ship Container" field
   ```

### Step 3: Test It!

Press **Play** and check the Console:

```
Loading ship: Mistral (Coastal Cruiser)
Created Deck 0: CrewDeck_1 (6x1)
Created Deck 1: CrewDeck_2 (10x1)
Created Deck 2: PromenadeDeck (14x1)
Created Deck 3: MainDeck_B (14x1)
Created Deck 4: MainDeck_A (14x1)
Created Deck 5: BridgeDeck (6x1)
=== SHIP INFO ===
Name: Mistral
Class: Coastal Cruiser
Decks: 6
...
```

---

## 🔍 Testing & Debugging

### Visual Debugging

**Scene View:**
- Select any Deck GameObject
- You'll see:
  - Gray grid lines (tile boundaries)
  - Red cubes (occupied tiles)

**Gizmos are always on** - move the Scene camera to see the ship structure!

### Console Commands

**ShipManager Context Menu** (Right-click component):
- "Reload Ship" - Recreate ship from data
- "Print Ship Info" - Log current stats

**Ship Component:**
- "Print Ship Info" - Detailed deck breakdown

### Code Access Examples

```csharp
using CruiseLineInc.Ship;

// Get the ship
Ship mistral = ShipManager.Instance.GetCurrentShip();
Debug.Log($"Ship: {mistral.ShipName}");

// Access a deck
Deck mainDeck = mistral.GetDeck(4);
Debug.Log($"Main Deck: {mainDeck.GridSize}");

// Access a tile
Tile tile = mainDeck.GetTile(5, 0);
Debug.Log($"Tile: {tile}");

// Check if we can build
Vector2Int roomPos = new Vector2Int(0, 0);
Vector2Int roomSize = new Vector2Int(2, 1);

if (mainDeck.CanPlaceRoom(roomPos, roomSize))
{
    mainDeck.OccupyTiles(roomPos, roomSize, "my_room_01");
    Debug.Log("Room placed!");
}

// Get ship stats
Debug.Log($"Passengers: {mistral.CurrentPassengerCount}/{mistral.MaxPassengers}");
Debug.Log($"Available Tiles: {mistral.AvailableTiles}");
```

---

## 📊 Mistral Specifications

| Property | Value |
|----------|-------|
| **Class** | Coastal Cruiser |
| **Decks** | 6 (Bridge + 2 Main + Promenade + 2 Crew) |
| **Passengers** | 40 max (30 Standard, 2 Premium) |
| **Crew** | 10 max |
| **Total Tiles** | 64 |
| **Water Capacity** | 150 units |
| **Food Capacity** | 200 units |
| **Waste Capacity** | 180 units |
| **Voyage Length** | 2 in-game days |

### Deck Breakdown

| Level | Type | Size | Purpose |
|-------|------|------|---------|
| 5 | Bridge | 6x1 | Command deck |
| 4 | Main A | 14x1 | Passenger cabins |
| 3 | Main B | 14x1 | Passenger cabins |
| 2 | Promenade | 14x1 | Public areas, dining |
| 1 | Crew 2 | 10x1 | Crew quarters |
| 0 | Crew 1 | 6x1 | Engine, storage |

---

## 🎯 What's Next?

The foundation is complete! Ready for:

### Phase 2: Construction System (Week 3-4)
- Room placement UI
- Construction queue
- Timed builds
- Disruption system

### Phase 3: Room Types (Week 4-5)
- Cabin variants (Standard, Deluxe)
- Restaurant, Lounge
- Crew facilities
- Engine room, storage

### Phase 4: Agents (Week 6-8)
- Passenger spawning
- Crew assignment
- Basic FSM
- Need system

---

## 🐛 Troubleshooting

**"No ship loaded!"**
- Check ShipManager has ShipClassData assigned
- Ensure "Auto Load On Start" is checked
- Try right-click → Reload Ship

**"Deck config at level X has no DeckTypeData!"**
- Run the Mistral Setup Wizard
- Check DeckTypeData assets exist in Data/Decks/

**No Gizmos visible:**
- Click the Gizmos button in Scene view (top right)
- Ensure a Deck is selected
- Try zooming in on the ship

**Can't find menu "Cruise Line Inc":**
- Ensure Editor scripts compiled
- Check for compilation errors
- Try Assets → Reimport All

---

## 📚 Documentation

See `Assets/_Project/Scripts/Ship/README.md` for full technical documentation.

---

## ✨ Summary

You now have:
- ✅ Complete tile/deck/ship hierarchy
- ✅ ScriptableObject-based configuration
- ✅ Mistral ship ready to use
- ✅ Visual debugging tools
- ✅ Clean architecture for expansion

**Next step:** Build the Construction System to place rooms on these decks!

---

*Created: Phase 1 - Foundation Complete*
*Ready for: Phase 2 - Construction & Rooms*
