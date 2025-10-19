# Cruise Line Inc. Game Design Doc

<aside>
💡

### 🎮 **Cruise Line Inc. – Game Summary**

> Tagline:
> 
> 
> *Luxury, logistics, and a little bit of madness — welcome aboard Cruise Line Inc.*
> 

**Genre:** Simulation / Tycoon / Management

**Engine:** Unity (URP)

**Core Fantasy:** Build and operate living cruise ships — floating micro-cities that balance comfort, efficiency, and chaos.

**Core Loop:**

🛠 **Build → 🌊 Voyage → 📊 Debrief**

Design decks, manage thousands of passengers and crew, survive dynamic voyages, and expand your cruise empire.

**Pillars:**

- 🎨 *Design Freedom* — Deck-by-deck creativity with real trade-offs.
- ⚙️ *Simulation Depth* — 10 k agents, live utilities, emergent behavior.
- 💬 *Humor & Humanity* — Light satire and personality-driven events.
- 🌍 *Replayability* — Different routes, ships, and conditions every run.

**Progression:**

Profit, reputation, and ship class unlocks drive long-term growth.

Each voyage tests how your design handles stress, storms, and human chaos.

**Current Focus:**

Finalize passenger archetypes, build AI / LOD prototype, and implement deck-graph + construction disruption systems.

[MVP/Tutorial Ship (Coastal Class - ‘Mistral)](https://www.notion.so/MVP-Tutorial-Ship-Coastal-Class-Mistral-28ccdd56dcd880ed8adaf8f57aa41ca6?pvs=21)

[**MVP Fundamental Systems Overview**](https://www.notion.so/MVP-Fundamental-Systems-Overview-28ccdd56dcd8803a89d6e8a5a92eecc6?pvs=21)

**Long-Term Vision:**

- 🧩 Modding support
- 🎭 Expanded event system
- ❄️ New cruise types (research, salvage, arctic)
- 🕹 Themed cruise templates (gaming, retro, medieval, family)
</aside>

# **Game Overview**

### **Title:**

**Cruise Line Inc.** *(working title)*

### **Genre:**

Simulation / Tycoon / Management

### Elevator Pitch:

Build your dream cruise ships in this hybrid of Sim Tower + Theme Park.

### **Core Concept:**

Design, build, and operate your own cruise ships — massive floating cities that expresses your creativity.

The player must balance **comfort, efficiency, and profit** across a living ship ecosystem of passengers, crew, and systems. Every voyage tells a new story shaped by design choices, environmental challenges, and human behavior.

### **Vision Statement:**

To capture the charm and complexity of cruise life — from bustling buffets to engine room repairs — in a management game that’s **easy to play, but difficult to master**.

The world should feel *alive*, *reactive*, and *slightly absurd* — a cheerful satire of modern mega-cruises and their floating micro-economies.

### **Target Audience:**

- Fans of **SimTower**, **Theme Hospital**, **Project Highrise**, and **Two Point Hospital**.
- Players who enjoy optimization, emergent simulation, and base-building.
- Casual players drawn by the cozy, comedic tone but who can grow into deep systems mastery.

### **Player Fantasy:**

You are the architect, captain, and CEO of a mobile empire at sea — designing deck layouts, managing guests and crew, and charting global voyages while navigating unpredictable challenges.

### **Game Pillars:**

1. **Design Freedom:** Every ship layout feels unique and personal.
2. **Simulation Depth:** Agents and systems interact in believable ways.
3. **Reactive World:** Choices ripple through the entire voyage — from comfort to chaos.
4. **Humor & Humanity:** Guests complain, staff gossip, disasters escalate — but it’s always fun to watch.
5. **Replayability:** No voyage is ever quite the same; new routes, passengers, and crises emerge dynamically.

### **Tone & Aesthetic:**

Bright, stylized, and approachable — equal parts luxury brochure and absurd floating city.

A mix of cozy and chaotic: elegant decks, crowded elevators, and the quiet hum of an unstoppable engine below.

### **Platform:**

PC (Steam), later possible ports to console and mobile tablets.

Built with **Unity (URP)** using a **2.5D cross-section** visualization.

# **Core Gameplay Loop**

### **Loop Summary**

A continuous rhythm of **Design → Operate → Observe → Adjust**.

Players construct and manage a functioning cruise ship in real time, balancing passenger happiness, staff efficiency, and profitability across each voyage.

---

### **Primary Loop (Minute-to-Minute)**

1. **Observe:** Monitor passenger comfort, staff energy, and resource meters through dashboards and overlays.
2. **Decide:** Identify weak points (crowding, noise, shortages).
3. **Act:** Adjust operations or start construction/repairs — remembering that work takes time and causes disruption.
4. **Evaluate:** Watch agent behavior change and stats stabilize; small victories feed back into satisfaction and profit.

This loop keeps the player engaged every few minutes — constantly nudging, optimizing, and firefighting.

---

### **Secondary Loop (Voyage-to-Voyage)**

1. **Pre-Voyage Planning (Port):** Design deck layouts, hire crew, stock supplies, choose route.
2. **Voyage Operations:** Run the ship through days at sea — handle passenger flow, crew morale, events, and emergencies.
3. **Post-Voyage Debrief:** Review performance (profit, comfort, reputation).
4. **Progression:** Unlock new ship classes, technologies, and destinations.
    
    Each voyage acts as a discrete *mission* that tests how well the player’s design endures real conditions.
    

---

### **Long-Term Loop (Meta Progression)**

- Grow from a small coastal ferry to a global luxury fleet.
- Invest profits into research (fuel efficiency, noise reduction, luxury amenities).
- Maintain company reputation to access elite passengers and high-value routes.
- Compete on leaderboards or events for “World’s Best Cruise Line.”

---

### **Core Tension**

- **Build Freedom vs. Voyage Risk:** The player can rebuild mid-cruise, but construction time and passenger disruption make every fix costly.
- **Comfort vs. Efficiency:** Luxuries raise happiness but strain systems and margins.
- **Automation vs. Micromanagement:** High-skill staff and technology reduce problems, but remove fine-tuned control.

---

### **Success Conditions**

- Complete voyage objectives (arrival, satisfaction, profit).
- Maintain reputation and expand operations.
- Long-term mastery = ships that run smoothly with minimal mid-voyage interference.

# **Passengers (Agents)**

### **Overview**

Passengers are lightweight autonomous agents who bring the ship to life.

They each have **visible needs** that drive behavior and **hidden stats** that influence long-term satisfaction, health, and spending.

The player never controls them directly — instead, they shape outcomes through design, staffing, and policy.

---

### **Passenger Archetypes**

Each voyage spawns a mix of archetypes, balancing demographics and expectations:

| Archetype | Traits | Priorities | Behavior Example |
| --- | --- | --- | --- |
| **Families** | Higher crowd tolerance, fast need decay. | Food, fun, safety. | Children get bored quickly, parents cluster near pools/restaurants. |
| **Retirees** | Slow movement, prefer quiet. | Comfort, hygiene, privacy. | Avoid stairs, dislike crowding or loud music. |
| **Couples** | Mid-income, balanced preferences. | Privacy, novelty. | Visit bars, theaters, and cabins frequently. |
| **Influencers** | High expectations, high spending. | Novelty, luxury, social presence. | Visit “photo op” spots, amplify satisfaction/reputation if happy. |
| **Scientists / Expedition Guests** | Event-specific archetype. | Health, reliability. | Travel for special routes (Arctic, research voyages). |

Each archetype modifies base need weights and patience thresholds.

---

### **Visible Needs (Primary Meters)**

Displayed to the player through heatmaps and dashboards.

| Need | Description | Fulfilled By |
| --- | --- | --- |
| **Comfort** | Cabin quality, temperature, noise. | HVAC, decor, soundproofing, room design. |
| **Hunger / Thirst** | Access to food and drink. | Buffets, bars, room service. |
| **Entertainment** | Fun and novelty. | Shows, pools, casinos, events. |
| **Social** | Interaction and shared spaces. | Lounges, events, crowd zones. |
| **Privacy** | Space and quiet. | Cabin design, layout isolation, traffic flow. |

Needs decay over time and are restored through facility interaction.

Agents dynamically choose destinations based on lowest current need.

---

### **Hidden Stats (Internal Modifiers)**

Invisible to the player but reflected indirectly in satisfaction, health, and event outcomes.

| Stat | Description | Impact |
| --- | --- | --- |
| **Hygiene (0–100)** | Personal cleanliness; decays over time or in dirty areas. | Affects comfort, infection risk. |
| **Health (0–100)** | Physical well-being. | Low health triggers clinic visits or illness events. |
| **Seasickness (Transient)** | Reaction to motion and noise. | Reduces activity and hunger temporarily. |
| **Stress (Transient)** | Builds from queues, crowding, or crises. | Reduces patience, increases complaint rate. |
| **Money (Wallet)** | Budget available for onboard spending. | Determines luxury purchases and shop usage. |
| **Loyalty (Hidden Reputation)** | Builds over repeat voyages. | Boosts post-voyage ratings and marketing returns. |

Hidden stats interact subtly — e.g., poor Hygiene + high crowding = illness outbreak risk.

---

### **Decision-Making Behavior**

Each passenger cycles through a light behavior loop:

```
Sense → Evaluate → Move → Interact → Satisfy → Reassess

```

**Utility Scoring Example:**

```
utility = facility_quality * (need_weight / (distance + crowd_penalty))

```

They seek the best facility per need until satisfied or interrupted (events, fatigue, pathfinding).

---

### **Agent Density Targets**

- 200–500 visible agents (high detail).
- 1000–2000 background-simulated for aggregate metrics.
- Simplified crowding math via regional density rather than individual collisions.

---

### **Emergent Behavior Examples**

- Families cluster during meal times, overwhelming buffets.
- Retirees avoid noisy decks; satisfaction drops near theaters.
- Influencers chase novelty — they’ll visit new facilities first.
- Crowding near elevators reduces both Stress and Hygiene scores locally.

---

### **Failure & Feedback Signals**

- Complaints appear as floating dialogue or event logs.
- Overlays visualize problem areas (heat, crowding, noise).
- Summary stats show top three pain points per voyage.

---

### **Design Goals**

- **Accessibility:** Needs are simple to understand visually.
- **Depth:** Hidden stats allow for expert optimization.
- **Emergence:** Small layout tweaks produce big behavioral differences.
- **Charm:** Agents should express personality through animations, chatter, and habits.

---

# **Staff & Crew**

### **Overview**

Staff are the operational agents that keep the ship running smoothly.

They handle cleaning, food service, repairs, entertainment, and passenger care — forming the link between **player decisions** and **passenger experience**.

Each staff member has **three core stats** (Energy, Satisfaction, Skill) and is affected by **placement, scheduling, and facility design**.

Unlike passengers, staff can be managed directly through assignments and policies.

---

### **Core Stats**

| Stat | Description | Gameplay Effects |
| --- | --- | --- |
| **Energy (0–100)** | Represents fatigue; drains while working or walking long distances. Restored in break rooms or quarters. | Low Energy = slower movement, errors, reduced service quality. |
| **Satisfaction (0–100)** | Morale and job happiness; influenced by workload, fairness, pay, and rest. | Low Satisfaction = higher turnover, complaints, decreased efficiency. |
| **Skill (0–5)** | Proficiency at their role; improves with experience or training. | High Skill = faster service, higher quality, fewer mistakes. |

Each tick, staff update their Energy and Satisfaction based on current workload, distance walked, and environment comfort.

---

### **Staff Roles**

| Role | Duties | Key Stats | Placement Strategy |
| --- | --- | --- | --- |
| **Cleaners** | Maintain cleanliness; remove spills and trash. | Energy, Skill. | Position closets near high-traffic zones. |
| **Caterers** | Prepare and serve food/drinks. | Skill, Satisfaction. | Close to kitchens and dining decks. |
| **Entertainers** | Run shows, music, and activities. | Satisfaction, Skill. | Near theaters, pools, lounges. |
| **Engineers / Technicians** | Repair faults, maintain utilities. | Skill, Energy. | Centrally placed maintenance rooms. |
| **Medics** | Treat sick passengers and injured crew. | Skill, Satisfaction. | Near clinics; affected by patient load. |
| **Security / Attendants** | Handle crowds, queues, and disputes. | Energy, Satisfaction. | Positioned at elevators and public decks. |

Later tiers can add specialized subroles (bartenders, stewards, HVAC techs) as the simulation expands.

---

### **Crew Systems**

### **Energy Flow**

- Drains from walking distance, work intensity, and crowding.
- Regained in **break rooms**; higher-quality lounges regenerate faster.
- Staff-only corridors/elevators reduce travel fatigue dramatically.

### **Satisfaction Flow**

- Rises from fair shifts, downtime, clean quarters, and successful service.
- Falls from overwork, poor facilities, unhappy passengers, or long walks.
- When low, staff may underperform, call in sick, or quit post-voyage.

### **Skill Growth**

- Passive XP from working; accelerated through **training rooms**.
- Skill raises throughput and reduces mistakes.
- Senior crew provide small local buffs to nearby juniors.

---

### **Scheduling & Shifts**

- Ships operate on a **3-shift cycle** (Morning / Evening / Night).
- Each staff type can be assigned to a zone and shift.
- **Break Cadence:** every X hours → Y minutes rest.
- **Overtime Policy:** increases short-term performance at long-term morale cost.
- **Rush Mode:** temporarily overclock staff output during peak hours or events.

Proper scheduling is a hidden optimization layer — well-balanced rosters yield stable voyages, while poor ones cause cascading inefficiency.

---

### **Facilities Affecting Crew**

| Facility | Effect |
| --- | --- |
| **Break Room / Lounge** | Restores Energy and boosts Satisfaction. |
| **Crew Quarters** | Long-term rest and morale recovery between voyages. |
| **Training Room** | Skill XP gain; unlocks higher efficiency bonuses. |
| **Maintenance Closet** | Cleaner/engineer storage; shortens pathfinding distance. |
| **Staff-Only Elevator / Corridor** | Cuts travel time and crowd stress. |

Placement of these facilities shapes crew performance and indirectly passenger comfort.

---

### **Crew-Related Events**

- **Overworked Staff:** morale drops → service slowdowns → passenger complaints.
- **Staff Dispute:** two staff roles in conflict; player must reassign or mediate.
- **Hero Moment:** skilled engineer prevents major failure → reputation boost.
- **Sick Leave:** medic shortage increases illness duration.
- **Strike Threat (late game):** repeated low morale may cause partial service halt.

Events make crew feel human and reinforce the operational simulation.

---

### **Design Goals**

- **Simple inputs, deep outcomes:** few stats but strong system interdependencies.
- **Indirect control:** player plans structure and policy, not micromanagement.
- **Emergent trade-offs:** overworking staff fixes short-term crises but builds long-term instability.
- **Visible personality:** animations, chatter, and commentary reflect morale state.

---

# **Construction System**

### **Overview**

The Construction System defines how players shape and expand their cruise ships.

It balances **creative freedom** with **logistical realism** — every addition affects performance, comfort, and efficiency.

Players can build at any time, but construction takes time, consumes resources, and disrupts passenger comfort.

---

### **Core Principles**

1. **Deck-by-Deck Design:**
    - Ships are built vertically like *SimTower* — each deck layer hosts cabins, restaurants, utilities, and facilities.
    - Elevators and stairs connect decks; poor layout causes bottlenecks.
2. **Real-Time Construction with Disruption:**
    - Buildings and demolitions take in-game time to complete.
    - Active construction creates *noise*, *dust*, and *crowding penalties* within a defined radius.
    - Passengers near the site experience reduced **Comfort** and **Hygiene**.
3. **Opportunity Cost, Not Prohibition:**
    - Players *can* remodel mid-voyage but risk reputation loss and crew fatigue.
    - Smart timing and placement reduce the negative impact.
4. **Space, Mass, and Power Constraints:**
    - Each hull class defines deck space, weight limit, and utility throughput.
    - Heavy or power-hungry modules require trade-offs elsewhere.

---

### **Build & Deconstruction Flow**

| Phase | Player Action | Effects |
| --- | --- | --- |
| **Plan** | Select module from catalog; preview footprint and disruption radius. | Displays cost, build time, and utility requirements. |
| **Construct** | Crew assigned; scaffolding appears; countdown begins. | Affected area gains temporary negative modifiers. |
| **Integrate** | System connects to utilities; agents can enter. | Triggers power/water recalculations and route updates. |
| **Deconstruct** | Removes module; debris generated. | Temporary path blockage and morale penalty if active during voyage. |

---

### **Construction Stats**

| Parameter | Description |
| --- | --- |
| **Build Time** | Scales with size and complexity. |
| **Resource Cost** | Materials + labor cost (reduces voyage profit). |
| **Crew Load** | Engineers/Cleaners assigned; increases fatigue. |
| **Disruption Radius** | Area where passenger comfort drops temporarily. |
| **Noise Level** | Adds to ambient noise map; decays after completion. |

---

### **Utility Integration**

Every module connects to:

- **Power Grid**
- **Water Supply**
- **HVAC**
- **Waste System**
- **Path Network (for agent routing)**

If system limits are exceeded, modules underperform (dim lights, cold water, etc.) until upgraded.

---

### **Design Constraints**

- **Zoning:** public, crew, and restricted areas.
- **Adjacency Bonuses:** certain combos improve efficiency (buffet + kitchen; pool + bar).
- **Structural Rules:** hull form restricts certain module placements (no heavy machinery on upper decks).
- **Safety Requirements:** lifeboats and escape routes mandatory; noncompliance hurts rating.

---

### **Player Tools**

- **Blueprint Mode:** paused planning view to queue multiple builds.
- **Simulation Mode:** live adjustments with visualized disruption heatmaps.
- **Deck Overview:** layer navigation for efficient layout editing.
- **Metrics Overlay:** comfort, noise, traffic, and utility balance in real time.

---

### **Construction Events**

- **Delayed Delivery:** materials late; build time extended.
- **Noise Complaint:** nearby passengers unhappy; temporary Satisfaction drop.
- **Crew Injury:** minor setback, reduces morale.
- **Breakthrough Construction:** skilled crew finish early; small bonus.

---

### **Design Goals**

- **Accessibility:** drag-and-place simplicity.
- **Strategic Weight:** each addition has ripple effects.
- **Visual Feedback:** scaffolds, blinking warnings, clear impact radii.
- **Replayability:** no “perfect layout” — every voyage tests a new configuration.

# **Voyage Simulation**

### **Overview**

Each voyage is a living scenario: the ship departs port, travels through changing environments, and faces operational challenges that test passenger comfort and crew efficiency.

The player’s goal is to **arrive safely, profitably, and with a satisfied manifest** — balancing energy use, weather response, and onboard management.

---

### **Simulation Scope**

- **Time Compression:** 1 in-game day ≈ 1–3 minutes of real time.
- **Voyage Duration:** 10–30 minutes per run, representing a full multi-day cruise.
- **Tick System:** All systems (needs, crew fatigue, construction timers, resources) update every tick for consistent pacing.

---

### **Environmental Variables**

| Variable | Description | Gameplay Effect |
| --- | --- | --- |
| **Weather** | Dynamic conditions: calm, rain, storm, heatwave, cold front. | Alters HVAC load, deck closures, seasickness risk. |
| **Sea State** | Determines wave height and roll. | Affects stability, energy use, and passenger stress. |
| **Temperature** | Affects HVAC balance and outdoor facility usability. | Impacts Comfort, Health, and water/power demand. |
| **Visibility** | Impacts event probability and exploration visuals. | Aesthetic + difficulty factor for navigation. |
| **Route Distance** | Length of trip; defines fuel and food consumption rate. | Longer voyages = higher maintenance risk. |

---

### **Route & Destination System**

- **Route Choice:** Players select from world map itineraries before departure (e.g., Caribbean, Arctic, Mediterranean).
- **Destination Traits:**
    - *Caribbean:* low supply demand, normal comfort.
    - *European:* high luxury expectations, frequent ports.
    - *Arctic:* extreme cold, long stretches without resupply.
- **Event Modifiers:** Each region biases toward certain incidents (storms, illnesses, celebrity guests, mechanical stress).

---

### **Operational Systems Simulated**

| System | Player Interaction | Effect |
| --- | --- | --- |
| **Power Grid** | Adjust generator load and priority sectors. | Prevents blackouts; affects Comfort and Hygiene. |
| **Water & Waste** | Allocate freshwater; activate desalination. | Balances Hygiene vs. fuel use. |
| **HVAC** | Auto or manual zoning controls. | Comfort, energy efficiency, air quality. |
| **Fuel & Propulsion** | Cruise speed slider. | Faster travel = higher cost, less event exposure. |
| **Stability / Ballast** | Manual correction or auto-balance. | Reduces Seasickness during rough seas. |

All subsystems tie into visible ship stats: **Comfort**, **Safety**, **Efficiency**, **Profit**, and **Satisfaction**.

---

### **Voyage Phases**

| Phase | Player Focus | Example Actions |
| --- | --- | --- |
| **Departure (Port)** | Final checks and announcements. | Load passengers, start utilities, leave dock. |
| **At Sea (Steady State)** | Core management phase. | Handle needs, adjust systems, respond to events. |
| **Crisis / Weather Phase** | Sudden spike in difficulty. | Reassign crew, toggle systems, manage damage. |
| **Arrival / Debrief** | Review success metrics. | Earnings, satisfaction, reputation summary. |

Each voyage’s pacing follows a calm–stress–recovery rhythm to keep engagement high.

---

### **Events & Incidents**

Procedurally triggered from environment and internal conditions:

- **Storm Warning** – forces navigation choice; higher Seasickness.
- **Food Shortage** – mis-stocked supplies lead to Hunger complaints.
- **Mechanical Failure** – HVAC or generator faults require Engineers.
- **Medical Outbreak** – low Hygiene triggers illness cluster.
- **Passenger Drama** – random narrative mini-event (lost luggage, proposal, fight).
- **Crew Fatigue Cascade** – prolonged rush hours drain multiple roles simultaneously.

Event frequency scales with voyage length and ship complexity.

---

### **Performance Metrics**

At voyage end, player receives:

- **Passenger Satisfaction (%)**
- **Crew Morale**
- **Operational Efficiency** (resource use vs. baseline)
- **Profit Margin**
- **Reputation Change**

These feed back into company progression and unlocks.

---

### **Design Goals**

- **Steady Feedback:** Small adjustments visibly change outcomes.
- **Rhythmic Flow:** Calm routine punctuated by meaningful spikes.
- **Replay Variety:** No two voyages feel identical due to weather, events, and demographics.
- **Scalable Complexity:** Early voyages simple; later ones juggle more systems simultaneously.

# **Economy & Progression**

### **Overview**

The economy in *Cruise Line Inc.* revolves around balancing **income, costs, and reputation** across each voyage.

Every decision — from ship design to staff management — has measurable financial and reputational consequences.

Progression unfolds through **larger ships, new technologies, and higher passenger expectations.**

---

### **Core Economic Concepts**

| Concept | Description |
| --- | --- |
| **Operating Income** | Ticket sales, onboard spending, and special events. |
| **Operating Costs** | Fuel, food, wages, maintenance, and utilities. |
| **Capital Costs** | New construction and ship expansion. |
| **Reputation** | Meta score influencing ticket demand and VIP unlocks. |
| **Voyage Rating** | Combined Satisfaction and Efficiency score, determines rewards. |

Profit isn’t just the goal — it’s the **constraint** that pushes efficient ship design and operation.

---

### **Revenue Streams**

1. **Ticket Sales** – base income, scaled by ship class and reputation.
2. **Onboard Spending** – bars, shops, spas, casinos; depends on passenger wealth and satisfaction.
3. **Excursions** – optional revenue from destination events (if available).
4. **Premium Services** – photography, VIP cabins, “Captain’s Dinner” upsells.
5. **Bonuses** – PR milestones, perfect voyages, or special contracts.

---

### **Expenses**

- **Crew Wages** – scales with number, role, and morale.
- **Fuel & Utilities** – scales with ship size, weather, and speed.
- **Maintenance** – periodic upkeep of modules; ignored systems risk breakdowns.
- **Supplies** – consumables for food, cleaning, and amenities.
- **Construction Costs** – new modules, expansions, and rebuilds mid-voyage.
- **Port Fees** – docking and resupply expenses.

Managing these expenses while maintaining high satisfaction is the central optimization puzzle.

---

### **Profit Calculation**

```
Profit = (Tickets + OnboardSales + Bonuses) - (Wages + Fuel + Maintenance + Supplies + PortFees)

```

Voyage results feed into company cash flow and determine what upgrades can be afforded before the next run.

---

### **Progression Systems**

### **Ship Classes (Core Progression)**

Unlock larger and more complex vessels:

| Tier | Example | Unlock Requirement | Features |
| --- | --- | --- | --- |
| **I – Coastal Liner** | Small ferry-like vessel | Tutorial | Simple systems, short routes. |
| **II – Regional Cruiser** | 5–7 decks | 3 successful voyages | Unlocks staff quarters, theaters, elevators. |
| **III – Ocean Liner** | 10–12 decks | High reputation | Adds advanced utilities, HVAC, and events. |
| **IV – Mega Cruiser** | 15+ decks | Profit milestone | Requires logistics planning, automation upgrades. |

Each new class offers more complexity and new module types.

---

### **Technology Upgrades**

Research tree unlocking performance and comfort improvements:

- **Efficiency:** Fuel, HVAC, water treatment upgrades.
- **Luxury:** Advanced cabins, gourmet kitchens, premium décor.
- **Automation:** Better routing AI, cleaning drones, smart elevators.
- **Safety:** Fire suppression, noise isolation, medical capacity.

Upgrades can be global (affect all ships) or ship-specific.

---

### **Reputation System**

A meta-progression track influencing passenger demand, pricing power, and contract opportunities.

| Reputation Level | Description | Unlocks |
| --- | --- | --- |
| **Bronze Line** | Low-cost cruises; budget passengers. | Discounts on basic supplies. |
| **Silver Line** | Stable operations and decent reviews. | Regional routes. |
| **Gold Line** | High prestige; VIP guests appear. | Luxury contracts and special events. |
| **Platinum Line** | World-renowned brand. | Mega ships, elite clientele, global routes. |

Reputation is gained from high Voyage Ratings, PR events, and repeat customers — and lost from scandals or disasters.

---

### **Player Goals**

- Maintain **steady profit margins** while improving ship efficiency.
- **Unlock and master** each ship tier.
- Reach **prestige milestones** tied to reputation and satisfaction.
- Eventually operate a **fleet of self-sustaining ships** (late-game meta).

---

### **Design Goals**

- **Clarity:** Easy-to-read financial reports after each voyage.
- **Depth:** Indirect feedback between layout, satisfaction, and income.
- **Replayability:** Distinct strategies — luxury-focused, efficiency-focused, or risk-taking operator.
- **Satisfaction = Sustainability:** Profit flows naturally from a well-balanced design, not exploit mechanics.

---

# **Facilities & Modules**

### **Overview**

Facilities (also called *modules*) are the functional rooms, structures, and systems that compose a ship’s decks.

Each facility contributes to at least one of three pillars:

**Passenger Experience → Crew Operations → Core Systems.**

Modules produce or consume resources, influence nearby stats (comfort, hygiene, noise), and can create positive or negative synergies depending on placement.

---

### **Facility Categories**

### 🏨 Passenger Amenities

Provide comfort, food, fun, and social engagement.

| Tier | Example Modules | Primary Effects | Notes |
| --- | --- | --- | --- |
| **Basic** | Cabins (Eco / Standard), Buffet, Lounge | Comfort ↑ / Hunger ↓ | Core needs, low noise tolerance. |
| **Intermediate** | Restaurants, Bars, Pool, Gym | Comfort ↑ / Entertainment ↑ / Social ↑ | Moderate upkeep and crowd load. |
| **Advanced** | Spa, Theater, Casino, Luxury Suites | Comfort ↑ / Entertainment ↑ / Hygiene ↑ | High power & staff demand. |

---

### ⚙️ Crew & Service Areas

Support operations and staff wellbeing.

| Tier | Example Modules | Effects | Strategy |
| --- | --- | --- | --- |
| **Basic** | Crew Closet, Break Room, Staff Elevator | Energy ↑ / Walk Distance ↓ | Keep near work clusters. |
| **Intermediate** | Quarters, Training Room, Staff Cafeteria | Satisfaction ↑ / Skill ↑ | Improves long-term efficiency. |
| **Advanced** | Crew Lounge, Automation Hub | Satisfaction ↑ / Energy regen ↑ | Expensive but stabilizes morale. |

---

### 🔋 Core Systems & Infrastructure

Keep the ship running.

| System | Modules | Gameplay Role |
| --- | --- | --- |
| **Power** | Generator Room, Battery Bank | Feeds all facilities; failure = blackout. |
| **Water & Waste** | Desalination Plant, Recycling Unit | Supports hygiene; affects Comfort. |
| **HVAC** | Climate Hub, Duct Network | Balances temperature and air quality. |
| **Propulsion** | Engine Room, Ballast Control | Influences speed, stability, seasickness. |
| **Maintenance** | Workshop, Parts Storage | Enables repairs and construction speed. |

---

### 🛗 Circulation & Access

Manage passenger and crew movement.

| Type | Modules | Key Notes |
| --- | --- | --- |
| **Elevators** | Guest Elevator, Staff Lift, Freight Lift | Core vertical chokepoints; congestion = Stress ↑. |
| **Stairs / Ramps** | Interior Stairwell, Exterior Stairs | Cheap redundancy; comfort penalty for long climbs. |
| **Corridors** | Standard / Wide / Service | Affect walking distance and crowd heatmaps. |

---

### 🧰 Utility & Safety

Keep everything compliant and prevent disasters.

| Module | Effect |
| --- | --- |
| Fire Suppression System | Reduces damage during fire events. |
| Emergency Exit / Lifeboat Bay | Mandatory; improves Safety Rating. |
| Surveillance Station | Enables Security event detection. |
| Noise Dampeners | Lowers ambient noise around engines or theaters. |

---

### **Facility Properties**

| Property | Description | Typical Range |
| --- | --- | --- |
| **Size** | Tiles occupied on deck grid. | 1×1 – 6×6 |
| **Power Usage** | Draw on generator. | 1 – 20 units |
| **Water Usage** | Consumption or production. | −10 – +5 units |
| **Noise Level** | Adds to local noise map. | 0 – 100 |
| **Comfort Output** | Increases surrounding comfort radius. | +1 – +10 |
| **Maintenance Cost** | Periodic expense. | $–$$$ |
| **Crew Required** | Active staff slots. | 0 – 10 |

These numerical hooks feed directly into the simulation engine.

---

### **Adjacency & Synergy Rules**

- **Positive Pairs:**
    - *Buffet + Bar* → boosts Hunger satisfaction.
    - *Spa + Luxury Cabins* → Comfort multiplier.
    - *Theater + Casino* → Entertainment chain bonus.
- **Negative Pairs:**
    - *Engine Room + Cabins* → Comfort − Noise penalty.
    - *Restaurant + Bathrooms* → Hygiene penalty.
    - *Elevator + Quiet Cabins* → Privacy drop.

Smart deck planning yields compounding advantages.

---

### **Module Tiers & Unlock Flow**

| Tier | Description | Unlock Via |
| --- | --- | --- |
| **Tier 0** | Essential infrastructure. | Tutorial / starter ship. |
| **Tier 1** | Standard amenities. | First profitable voyages. |
| **Tier 2** | Luxury & advanced systems. | Reputation “Gold Line”. |
| **Tier 3** | Mega-ship exclusives. | Platinum Line & tech upgrades. |

---

### **Design Goals**

- **Tactile Construction:** Each module feels purposeful and visible.
- **Meaningful Trade-offs:** Space vs. efficiency vs. satisfaction.
- **Scalable Complexity:** New tiers add depth, not clutter.
- **Visual Variety:** Facilities telegraph function through color, lighting, and sound.

---

# **UI / UX Design**

### **Overview**

The user interface is designed to make a *complex simulation approachable*.

It presents information hierarchically — **from intuitive icons and overlays for casual players** to **deep dashboards for experts**.

The player should always understand *what’s happening*, *why it’s happening*, and *how to fix it.*

---

### **Core UI Philosophy**

1. **Clarity first:** Prioritize visual cleanliness and consistent iconography.
2. **Contextual depth:** Simple metrics up front; drill-down details on click.
3. **Seamless control:** Switching between “management” and “construction” modes should be instantaneous.
4. **Feedback loops:** Every change (building, event, action) shows clear visual and auditory response.
5. **Calm chaos:** The ship is busy, but the interface stays calm — pastel tones, muted sounds, and smooth transitions.

---

### **Primary Game Views**

### **1. Deck View (Main Gameplay)**

- Default isometric / cross-section view of the ship.
- Layer toggles for each deck.
- Click any room or person for contextual info panel.
- Real-time indicators: construction icons, crowd bubbles, noise zones.

### **2. Build Mode**

- Blueprint grid overlay; drag-and-drop module placement.
- Ghost previews show footprint, cost, build time, disruption radius.
- Warnings for overlapping modules or insufficient utilities.
- Time-based construction progress bar visible in-world.

### **3. Voyage Dashboard**

- High-level control panel for ongoing voyage data:
    - **Comfort, Profit, Reputation** meters.
    - **Crew Fatigue**, **Passenger Count**, **Event Log**, **Power/Water** status.
- Split into three tabs: **Operations**, **Passengers**, **Crew**.

### **4. Report Screen**

- Post-voyage breakdown: satisfaction curve, profit summary, resource charts.
- Heatmaps of peak crowding, energy usage, and complaint origins.
- Comparison to last voyage + trends over time.

---

### **Key HUD Elements**

| UI Element | Description |
| --- | --- |
| **Top Bar** | Voyage timer, weather, satisfaction %, profit. |
| **Left Panel** | Quick access: construction, staff, systems, reports. |
| **Right Alerts Panel** | Event notifications: “HVAC Fault,” “Noise Complaint,” etc. |
| **Bottom Info Bar** | Context-sensitive tips, construction feedback, or flavor text. |

---

### **Overlays & Visualization Tools**

Core readability tools that make optimization satisfying.

| Overlay | Purpose |
| --- | --- |
| **Crowding / Flow** | Shows path density and elevator queues. |
| **Noise** | Highlights loud modules and affected zones. |
| **Comfort** | Green-to-red gradient for passenger comfort. |
| **Cleanliness / Hygiene** | Identifies neglected or dirty areas. |
| **Air Quality / Temperature** | Reveals HVAC performance. |
| **Staff Routes** | Displays travel paths and idle time hotspots. |
| **Power & Water Networks** | Visualize resource bottlenecks. |

Players can toggle multiple overlays for expert analysis.

---

### **Context Panels**

Each major object (Room, Passenger, Crew) has an expandable info card:

- **Room Panel:** occupancy, satisfaction, maintenance, power/water draw.
- **Passenger Panel:** current need focus, stress, happiness graph.
- **Crew Panel:** energy, satisfaction, skill, assignment, recent tasks.

Each card supports “pinning” for monitoring multiple entities at once.

---

### **Accessibility & Usability**

- **Colorblind-safe overlays** (red-green alternatives).
- **Scalable font sizes** for readability.
- **Edge scrolling / drag-pan / zoom with mouse wheel or touch.**
- **Tooltips** for every metric and icon.
- **Pause & speed controls** for stress-free play (1× / 3× / pause).

---

### **Tone & Aesthetic**

- **Color Palette:** nautical blues, warm wood interiors, gentle highlights for alerts.
- **Typography:** clear sans-serif for stats, stylized serif for titles.
- **Animations:** soft easing, no flicker; construction and event cues use light and sound subtly.
- **UI Audio:** muted clicks, ambient ship hum, soft alerts.

Goal: make managing chaos feel elegant and relaxing — a floating control center rather than a spreadsheet.

---

### **Design Goals**

- **Information hierarchy:** immediate awareness → deeper context → full detail.
- **Feedback transparency:** every system communicates state changes clearly.
- **Accessibility for all players:** intuitive at glance, deep for power users.
- **Immersion:** UI reinforces theme — feels like the ship’s bridge interface.

# **Art & Audio Direction**

### **Overview**

The game’s art and sound style should balance **clean readability** for management play with **warm character** for immersion.

The world should look inviting and a bit whimsical — a stylized, living miniature of a cruise metropolis where you can see every deck bustle in harmony.

---

### **Art Direction**

### **Visual Style**

- **Stylized realism:** realistic proportions with simplified materials and shapes.
- **Soft lighting & bright palette:** evokes sunlit decks, turquoise seas, and cozy interiors.
- **Readable silhouettes:** every module and agent recognizable at a glance.
- **UI harmony:** game interface feels like part of the ship’s control system — smooth panels, subtle nautical cues.

### **Color Palette**

| Theme | Colors | Emotional Effect |
| --- | --- | --- |
| **Ocean / Exterior** | Teal, navy, turquoise gradients | Calm, expansive |
| **Interiors / Amenities** | Warm beige, wood brown, soft gold | Comfort, luxury |
| **Crew / Utility** | Desaturated blues and greys | Order, professionalism |
| **Alerts / Events** | Amber → red spectrum | Clarity without alarm fatigue |

### **Lighting & Atmosphere**

- **Day–Night Cycle:** slow transitions across voyages for visual rhythm.
- **Weather FX:** soft rain streaks, sun glares, distant lightning flashes through portholes.
- **Interior Bloom:** gentle highlights for luxury areas; dimmed tones for service decks.

### **Character Design**

- **Passengers:** expressive mini-figures, simplified clothing colors denoting archetype (family = bright, retirees = muted, influencers = trendy).
- **Staff:** uniforms by role; small posture and animation differences for fatigue/morale.
- **Animation Language:** idle chatter, queue shuffles, wave gestures — personality through motion, not dialogue.

### **Environmental Details**

- Moving sea horizon, gulls and fog layers.
- Pool water ripples, buffet steam, mechanical vibrations near engines.
- Construction scaffolds and flashing hazard lights during builds.

---

### **Audio Direction**

### **Overall Tone**

Relaxed, immersive, and lightly comedic — conveys both grandeur and underlying chaos.

### **Music**

- **Core Loop:** mellow tropical / lounge jazz that shifts dynamically with game state.
- **Voyage Events:** tempo rises during storms or crises, softens when calm returns.
- **Dock Phase:** ambient port chatter and distant horns.
- **Customization:** player may select playlists (corporate calm, luxury, or quirky fun).

### **Sound Design**

| Category | Examples | Design Intent |
| --- | --- | --- |
| **Ambient Loops** | Waves, engines, muffled conversations | Immersive continuity |
| **UI Feedback** | Subtle clicks, soft pings | Smooth management flow |
| **Construction** | Welding, hammering muffled by hull | Reinforces disruption mechanic |
| **Crowd Life** | Laughter, footsteps, elevator dings | Adds density without clutter |
| **Events** | Storm wind, alarm tones, announcement speakers | Emotional punctuation |

### **Voice & Announcements**

- Optional **AI-style ship voice** for alerts (“Attention: turbulence expected on Deck 7”).
- Captain or Cruise Director snippets for flavor (“Another sunny day aboard!”).
- Short, reusable clips to reduce localization costs.

---

### **Performance Targets**

- **Optimized stylized rendering:** URP + baked lighting.
- **Agent batching:** lightweight rigs with shared materials.
- **Sound culling:** dynamic priority mixing to prevent clutter.

---

### **Design Goals**

- **Cozy scale:** ship feels large but readable as a diorama.
- **Thematic unity:** UI, audio, and visuals reinforce the same tone.
- **Feedback clarity:** sound and light always signal gameplay state.
- **Personality through motion and mood:** the ship breathes, hums, and reacts like a living machine.

---

# **AI & Simulation Layer**

## Goals

- **Scale to 8–10k passengers** on mega ships without spiking CPU/GPU.
- **Look alive up close**, **run cheaply when zoomed out**.
- Preserve **system accuracy** (queues, utilities, satisfaction) even when most agents are aggregated.

---

## Simulation Levels of Detail (LOD)

### Passenger LODs

| LOD | When Used | Tick Rate | What’s Simulated | Rendering |
| --- | --- | --- | --- | --- |
| **LOD0 – Full** | On-camera close; hotspots; paused build previews | 5–10 Hz | Per-group pathing on navgraph, local avoidance, per-need utility choice | Full skinned anims |
| **LOD1 – Light** | Off-camera but important flows (elevators, buffets) | 2–5 Hz | Path on precomputed routes; no avoidance; room entry/exit counters | Simplified anim / impostors |
| **LOD2 – Cohort** | Default for far/zoomed-out view | 1 Hz | **Room-level cohorts** with Poisson arrivals; needs & spending statistically | Crowd sprites / icons |
| **LOD3 – Dormant** | In cabins/asleep/long stays | 0.2–0.5 Hz | Slow decay/rest only | None |

**Zoom policy:**

- At **max zoom-out**, hard-cap to **LOD2/LOD3 only** for passengers (no LOD0/1).
- When you **zoom in** or open a **hotspot panel**, cohorts near the focus **promote** to LOD1/LOD0 with a short warm-up.

### Grouped Agents (Families & Parties)

- Base entity = **Party** with `party_size ∈ [2..6]`, `archetype_mix`, one **shared itinerary** and **shared wallet**; internal offsets add variation.
- Parties stay intact across LODs; only break into a few visible “representatives” at LOD0.

---

## Data Model

```
Party {
  id, party_size, archetype_mix, budget_tier
  needs_weights[Comfort,Hunger,Entertain,Social,Privacy]  // averages
  hidden_stats{hygiene, health, stress, seasickness, loyalty}
  state {room_id, activity, time_left, lod}
}

Room {
  id, type, capacity, quality, service_rates, noise, comfort_field
  counters {occupancy, queue_len, service_throughput}
}

DeckGraph {
  nodes: corridors, stairs landings, elevator lobbies
  edges: length, width_class, travel_cost
  special_edges: elevator_shaft {cabins, capacity, travel, load, unload}
}

```

**SoA layout** for hot buffers (positions[], state[], need0[], …) to stay cache-friendly.

---

## Navigation & Flow

### Deck Graph

- One **graph per deck**, plus **inter-deck edges** (stairs, elevator shafts).
- **Corridor width class** scales edge capacity (affects crowding cost, not collision).

### Elevators as Queues

- Each shaft = **M/M/c** server:
    
    `service_rate = cabins × capacity / (travel_time + load + unload)`
    
- Decks maintain an **integer queue counter**; LOD0 parties you see are a **sample** of that counter.
- **Predicted wait** feeds party utility (may switch to stairs).
- **Crowd fields**: queue length adds to **Noise** & **Stress** around lobbies.

---

## Decision Model

### Utility (party chooses next goal)

```
score(room) =
  (need_weight(room.kind) * room.quality * novelty_factor)
  / (distance_cost + crowd_penalty + wait_penalty)

```

- **Distance cost** from precomputed shortest paths.
- **Crowd penalty** from room occupancy ratio & corridor flow field.
- **Wait penalty** from elevator predicted wait.

### Cohort Flow (LOD2)

- **Arrival** to a room uses **Poisson(λ)** where `λ` derives from utility share;
- **Service** uses room’s **μ (throughput)**;
- Satisfaction & spending update per tick via cohort averages.

---

## Updates & Scheduling

### Multi-rate scheduler (per frame)

1. Update a **slice** (10–20%) of LOD0 parties.
2. Process **room counters** (throughput, queues).
3. Run **LOD1** parties on cheap routes.
4. Advance **LOD2 cohorts** (arrivals/services) once per second (accumulate dt).
5. Apply **fields** diffusion/decay (Noise, Cleanliness) at low Hz.
6. Copy summarized stats → UI.

**Event-driven recompute:** only re-score utilities when local facts change (stock, queue, closures), not every tick.

---

## LOD Promotion/Demotion

**Promote to LOD0 if**: on-camera & in a “hot” zone (top N queues/complaints) or user pins an entity.

**Demote to LOD2 if**: off-camera, idle in room, not in top flows.

**Conservation rules**

- Keep **party_size** invariant; when demoting, add members back to the room cohort; when promoting, **sample** from cohort (and decrement its counter).
- Maintain **mass balance** for room occupancy, queues, and sales totals.

---

## Crowds, Queues, and Avoidance

- **No per-agent collisions** beyond LOD0; use simple velocity obstacles with 8–12 neighbors cap.
- **Corridor capacity** handled statistically via edge cost inflation, not physics.
- **Queues** are counters with **few visible stand-ins** at LOD0 for readability.

---

## Hidden Stats & Fields

- **Fields:** `Noise`, `Cleanliness`, `AirQuality`, `Temperature`, `Contamination` → grid-based, low-frequency blur/decay.
- Parties sample field values in their current room to adjust **Stress**, **Hygiene**, **Seasickness**.
- **Construction zones** inject temporary negative field values in a radius; decay after completion.

---

## Staff Simulation

- Staff counts are far smaller; many can run **LOD0/1** continuously.
- **Pathing** identical graph; **staff-only edges** reduce costs.
- **Energy** drains from meters walked + work intensity; **Satisfaction** from fairness, breaks, environment.
- **Skill** multiplies room service rates (μ) and fault repair MTTR.

---

## Performance Targets (PC baseline)

- **Passengers:** 8–10k total; **LOD0 cap ~900–1,200**, LOD1 ~1–2k; rest in cohorts.
- **CPU budget:** ≤4 ms/frame for sim on mid-tier CPU.
- **Path requests:** ≤200/frame (pooled jobs/Burst).
- **Animation:** GPU instanced crowds; billboard impostors beyond radius.
- **GC:** fixed-size pools; no allocs in hot loops.

**Graphics setting sliders**

- *Crowd Detail:* caps LOD0 count & impostor density.
- *Overlay Detail:* field resolution & update Hz.
- *Animation Complexity:* pose count for distant agents.

---

## Robustness

- **Deterministic seeds** per voyage for reproducible debugging.
- **Graceful degrade:** if sim falls behind, throttle LOD0/1 tick rates first, never cohorts.
- **Autosave** at port/major events; sim state compact (<5 MB).

---

## Pseudocode (high level)

```
loop frame(dt):
  time_accum += dt

  // 1) Promote/demote by camera & hotspots
  UpdateLODTransitions(camera, hotspots)

  // 2) LOD0 slice
  for party in NextSlice(LOD0, pct=0.2):
      if party.needs_dirty: party.RecomputeUtility()
      party.StepPath(dt_slow)
      if party.ReachedRoom(): RoomEnter(party)

  // 3) LOD1 lightweight
  for party in LOD1:
      party.FollowRouteCheap(dt_slow)

  // 4) Rooms & Queues
  for room in Rooms:
      room.ProcessThroughput(dt_slow)
  for shaft in Elevators:
      shaft.ServeQueues(dt_slow)   // M/M/c

  // 5) Cohorts (1 Hz)
  if time_accum >= 1.0:
      for room in Rooms:
          room.CohortArrivalsPoisson()
          room.CohortServices()
      FieldsDiffuseAndDecay()
      time_accum = 0

  // 6) UI snapshots
  PublishStats()

```

---

## Max Zoom Behavior (answering your question)

- **Passengers:** **LOD2/LOD3 only.** No individual pathing; all flows are cohort counters.
- **Rendering:** room-level occupancy bars; optional crowd impostors.
- **Performance:** minimal CPU; ideal for large fleets/mega ships.

---

## Why this works

- You get the **look** of 10k people, the **feel** of living crowds near camera, and a **stable sim** that respects queues, utilities, and satisfaction without per-frame thrash.

---

# **Events & Narrative**

## Purpose

Events transform the voyage from a pure optimization puzzle into a living story.

They:

- Break routine and introduce risk–reward decisions.
- Express the ship’s “personality” through passengers, crew, and random chaos.
- Provide variety, humor, and long-term memorability.

Every voyage should have a mix of **operational**, **social**, and **environmental** events to keep pacing dynamic.

---

## Event Structure

Each event has:

| Field | Description |
| --- | --- |
| **Trigger** | Condition that spawns it (stat threshold, random roll, route modifier). |
| **Context** | Decks, rooms, or roles involved. |
| **Options** | Player decisions (choose A/B/C or passive outcomes). |
| **Consequences** | Temporary modifiers to stats, passengers, reputation, or finances. |
| **Narrative Flavor** | Text + audio snippet reinforcing tone. |

Example:

> “Deck 5 buffet is out of shrimp! The crowd is restless.”
> 
> - **Option A:** Fly in emergency seafood (−$2 000 fuel + +2 Satisfaction)
> - **Option B:** Replace with pasta night (+0 cost −1 Reputation + humor quote)*

---

## Event Categories

### **A. Operational**

- **Mechanical Faults:** generator overload, HVAC failure, elevator jam.
- **Crew Issues:** overwork, staff dispute, sick leave.
- **Resource Shortages:** low food/water/fuel from mis-planning.
- **Utility Surge:** sudden power spike; must reprioritize circuits.

*Gameplay impact:* immediate system trade-offs and micro-optimizations.

---

### **B. Environmental**

- **Storm Front:** rough seas, passengers stressed, seasickness rise.
- **Heatwave / Cold Snap:** HVAC load surge, comfort risk.
- **Fog Bank:** slows travel, increases boredom.
- **Wildlife Sighting:** optional photo event; boosts satisfaction.

*Gameplay impact:* resource strain + emergent mini-objectives.

---

### **C. Passenger Social**

- **Birthday Party:** localized entertainment boost.
- **Complaint Chain:** influencer posts negative review.
- **Lost Child:** temporary crowd alert on deck.
- **Illness Outbreak:** hygiene failure → quarantine mini-event.
- **Proposal / Wedding:** morale spike, minor profit gain.

*Gameplay impact:* short morale waves and local mood fields.

---

### **D. Crew & Staff**

- **Hero Moment:** engineer prevents major fault → reputation +.
- **Strike Threat:** sustained low morale → choice event (raise pay / PR spin / ignore).
- **Training Accident:** downtime in maintenance.
- **Overtime Burnout:** productivity falloff if ignored.

---

### **E. Meta & World Events**

Triggered across multiple voyages; change economic or PR environment.

- **Fuel Price Spike**
- **Celebrity Booking**
- **New Safety Regulation**
- **Viral Video Trend** (influencer passengers trigger memetic bonuses)

*Adds continuity between voyages and keeps meta-loop alive.*

---

## Event Progression System

- **Early Game:** simple 1-step incidents → teach mechanics.
- **Mid Game:** compound events with branching outcomes (e.g., storm + HVAC failure).
- **Late Game:** *chain reactions* and “story arcs” linking voyages (crew burnout → accident → PR fallout).

Probability weights scale with voyage length, ship size, and reputation.

---

## Event Resolution & UI

- Pop-up styled like *ship communications log*.
- Short flavor text + 2–3 decision buttons.
- Timer bar for auto-resolve if ignored.
- Outcome displayed via icons (Comfort ↑ / Profit ↓ / Reputation ↑).
- Logged in “Voyage Events” tab for later review.

---

## Procedural Event Generation

- **Base Pool:** curated hand-written events (≈ 200 unique).
- **Parameterization:** inserts dynamic context (deck, room, passenger type).
- **Conflict Seeding:** simulation stats feed the generator:
    
    ```
    if crowding>0.7 on deck7 and noise>0.8 → chance("NoiseComplaint")
    if hygiene<0.4 → chance("IllnessOutbreak")
    if morale<0.3 → chance("StrikeThreat")
    
    ```
    
- **Frequency Control:** weighted by calm/stress rhythm to avoid fatigue.

---

## Narrative Tone

- **Humorous but grounded:** problems are ridiculous yet relatable.
- **Light satire:** pokes fun at corporate cruise culture and “luxury at any cost.”
- **Human warmth:** small personal stories offset mechanical pressure.

Think *Two Point Hospital* energy with *Project Highrise* clarity.

---

## Long-Term Narrative Hooks

- **Crew Relationships:** persistent morale modifiers across voyages.
- **Passenger Loyalty:** recurring VIPs referencing previous events.
- **Company Reputation Headlines:** small news blurbs summarizing outcomes (“Cruise Line Inc. Apologizes for Buffet Chaos”).
- **Seasonal Events:** holiday cruises, world tours, charity missions.

---

## Design Goals

- **Dynamic pacing:** steady routine punctuated by memorable spikes.
- **Player expression:** consistent tone whether player is a calm manager or chaotic improviser.
- **System integration:** every event connects back to simulation variables.
- **Replay variety:** randomization ensures no voyage feels the same.

---

# **Technical Implementation**

## **Engine & Stack Overview**

| Layer | Tool / Framework | Notes |
| --- | --- | --- |
| **Engine** | **Unity 2022+ (URP)** | Mature toolchain for hybrid 2.5D, strong editor extensibility, Burst/Jobs for simulation. |
| **Renderer** | **URP + Shader Graph** | Stylized lighting, lightweight on mid hardware; supports GPU instancing for crowds. |
| **UI** | **UI Toolkit (UXML / USS)** | Responsive layout for management HUDs; themeable for “nautical glass panel” look. |
| **Audio** | **FMOD Studio** | Dynamic mixing, event-driven cues tied to simulation states. |
| **Scripting Language** | **C#** | All core systems (agents, construction, utilities) jobified; ECS-compatible structure. |
| **Data Storage** | **ScriptableObjects + JSON** | Design-time data tables; runtime serialization for saves. |
| **Save / Load** | **Binary + Delta JSON** | Efficient state diffs; reproducible seeds for debug. |
| **Build Target** | PC (Windows / macOS) → later Console / Tablet | PC first for UI-heavy gameplay. |

---

## **Core Architecture**

### **Modules / Managers**

| System | Responsibility |
| --- | --- |
| **GameManager** | Global state, voyage cycle, time scaling, save control. |
| **ShipManager** | Handles decks, module placement, power/water routing, structural data. |
| **AgentManager** | Updates all passenger/crew groups, runs LOD scheduler. |
| **EventManager** | Procedural event queue; triggers and narrative resolution. |
| **EconomyManager** | Revenue/cost tracking, progression unlocks. |
| **UIManager** | HUD, overlays, and reports. |
| **AudioManager** | FMOD parameters linked to sim metrics. |
| **FieldManager** | Grid maps for noise, temperature, hygiene, etc. |

All managers communicate through **MessageBus / EventChannels** to minimize coupling.

---

## **Simulation Pipeline**

(Integrates directly with Section 10 design.)

1. **Deck Graph Build Phase**
    - Convert player-built decks → navigation graphs + utility meshes.
2. **Tick Scheduler**
    - Multirate update loop (LOD0–3, fields, cohorts).
3. **Event Injection**
    - EventManager samples triggers every 10 s; queues story pop-ups.
4. **System Updates**
    - Power / HVAC / Waste resource equations update, propagate to rooms.
5. **Agent Interaction**
    - Rooms consume/produce stats; update fields + EconomyManager.
6. **UI Sync**
    - Aggregated metrics pushed to dashboards, overlays, FMOD.

Simulation is **deterministic** within a voyage seed → replayable for testing.

---

## **Performance Strategy**

| Technique | Purpose |
| --- | --- |
| **C# Job System + Burst** | Parallelize agent ticks, field diffusion, path cost updates. |
| **SoA Memory Layout** | Cache-efficient arrays for 10 k agents. |
| **LOD Simulation** | Full detail only near camera / events. |
| **GPU Instancing & Impostors** | Render thousands of agents cheaply. |
| **Asynchronous I/O** | Stream deck meshes & audio by distance. |
| **Profiling Targets** | CPU < 4 ms/frame sim; GPU 60 FPS @1080p mid-range. |

---

## **Tooling & Editor Extensions**

- **Deck Editor:** drag-and-drop grid builder for internal designers.
- **Module Database Editor:** ScriptableObject table linking art prefab, cost, stats.
- **Route Visualizer:** debug pathfinding, elevator loads, and flow heatmaps.
- **Field Debugger:** inspect noise/comfort/temperature maps live.
- **Event Authoring Tool:** text+condition editor for designers (export JSON).
- **Balancing Dashboard:** auto-graph metrics across test voyages.

---

## **Data & Serialization**

**ShipSaveData**

```json
{
 "seed": 124532,
 "voyageDay": 4.3,
 "passengerCohorts": [...],
 "crewStates": [...],
 "modules": [...],
 "fields": {...},
 "economy": {...},
 "events": [...]
}

```

- Compression via binary delta; autosave every in-game day.
- Backward-compatible migration pipeline.

---

## **Technical Roadmap (Milestones)**

| Phase | Duration | Deliverables |
| --- | --- | --- |
| **MVP (3 mo)** | Deck builder, 1 voyage loop, ~200 agents, basic UI, economy stub. |  |
| **Alpha (6 mo)** | 2 ship classes, crew system, event framework, field maps. |  |
| **Beta (9 mo)** | 6 k agent LOD sim, full UI/overlays, sound integration. |  |
| **1.0 Launch (12 mo)** | 10 k agents, meta-progression, Steam release. |  |

---

## **External Integration**

- **Analytics hooks:** voyage length, build layouts, player churn (for balancing).
- **Mod support (post-launch):** JSON module defs + Steam Workshop scripts.
- **Localization:** SmartStrings; CSV export for translators.
- **Cloud Saves:** Steam / Itch integration planned.

---

## **Testing & Debugging**

- **Replay Logs:** deterministic re-sim for QA.
- **Stress Tests:** auto-build random ships → monitor sim stability.
- **Fuzz Events:** random event injection to detect softlocks.
- **Telemetry:** frame timings + memory profiler outputs to dashboard.

---

## **Future-Proofing**

- Modular ECS transition possible (Unity Entities 2.0).
- DLCs for Different events, voyages (arctic, research, salvage), and themed-builds (retro, gaming, sustainability, etc.)

---

## **Design Goals**

- **Maintainable:** clear modular managers.
- **Performant:** large simulations stay under fixed CPU budget.
- **Extensible:** easy to add new modules/events without rewiring core.
- **Deterministic:** consistent results for balancing.
- **Stable foundation:** supports years of content growth.

---