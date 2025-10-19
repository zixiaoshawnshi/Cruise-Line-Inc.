using UnityEngine;
using UnityEditor;
using CruiseLineInc.Ship.Data;

namespace CruiseLineInc.Editor
{
    /// <summary>
    /// Editor utility to quickly create Mistral ship configuration
    /// </summary>
    public class MistralSetupWizard : EditorWindow
    {
        private string _assetPath = "Assets/_Project/Data/";
        
        [MenuItem("Cruise Line Inc/Setup/Create Mistral Configuration")]
        public static void ShowWindow()
        {
            MistralSetupWizard window = GetWindow<MistralSetupWizard>("Mistral Setup");
            window.minSize = new Vector2(400, 300);
            window.Show();
        }
        
        private void OnGUI()
        {
            GUILayout.Label("Mistral Ship Setup Wizard", EditorStyles.boldLabel);
            GUILayout.Space(10);
            
            EditorGUILayout.HelpBox(
                "This wizard will create the Mistral ship configuration with all 6 decks (2D cross-section):\n\n" +
                "• Bridge Deck (6 tiles wide)\n" +
                "• Main Deck A (14 tiles wide)\n" +
                "• Main Deck B (14 tiles wide)\n" +
                "• Promenade Deck (14 tiles wide)\n" +
                "• Crew Deck 2 (10 tiles wide)\n" +
                "• Crew Deck 1 (6 tiles wide)",
                MessageType.Info
            );
            
            GUILayout.Space(10);
            
            _assetPath = EditorGUILayout.TextField("Asset Path:", _assetPath);
            
            GUILayout.Space(20);
            
            if (GUILayout.Button("Create Mistral Configuration", GUILayout.Height(40)))
            {
                CreateMistralConfiguration();
            }
        }
        
        private void CreateMistralConfiguration()
        {
            // Ensure directories exist
            string decksPath = _assetPath + "Decks/";
            string shipsPath = _assetPath + "Ships/";
            string tilesPath = _assetPath + "Tiles/";
            
            if (!AssetDatabase.IsValidFolder(decksPath.TrimEnd('/')))
            {
                System.IO.Directory.CreateDirectory(decksPath);
            }
            if (!AssetDatabase.IsValidFolder(shipsPath.TrimEnd('/')))
            {
                System.IO.Directory.CreateDirectory(shipsPath);
            }
            if (!AssetDatabase.IsValidFolder(tilesPath.TrimEnd('/')))
            {
                System.IO.Directory.CreateDirectory(tilesPath);
            }
            
            // Create TileVisualData (shared across all ships)
            TileVisualData tileVisuals = CreateTileVisualData();
            AssetDatabase.CreateAsset(tileVisuals, tilesPath + "DefaultTileVisualData.asset");
            
            // Create Deck Type Data assets
            DeckTypeData bridgeDeck = CreateDeckTypeData(
                "BridgeDeck",
                DeckType.Bridge,
                5,
                6,  // 6 tiles wide
                false, false, false, false,
                ZoneTag.Restricted,
                "Command deck with bridge and captain's quarters"
            );
            AssetDatabase.CreateAsset(bridgeDeck, decksPath + "BridgeDeck.asset");
            
            DeckTypeData mainDeckA = CreateDeckTypeData(
                "MainDeck_A",
                DeckType.Main,
                4,
                14,  // 14 tiles wide
                true, false, false, false,
                ZoneTag.Public,
                "Passenger cabins and staterooms"
            );
            AssetDatabase.CreateAsset(mainDeckA, decksPath + "MainDeck_A.asset");
            
            DeckTypeData mainDeckB = CreateDeckTypeData(
                "MainDeck_B",
                DeckType.Main,
                3,
                14,  // 14 tiles wide
                true, false, false, false,
                ZoneTag.Public,
                "Additional passenger accommodations"
            );
            AssetDatabase.CreateAsset(mainDeckB, decksPath + "MainDeck_B.asset");
            
            DeckTypeData promenadeDeck = CreateDeckTypeData(
                "PromenadeDeck",
                DeckType.Promenade,
                2,
                14,  // 14 tiles wide
                true, false, false, true,
                ZoneTag.Public,
                "Public areas, entrances, dining and entertainment"
            );
            AssetDatabase.CreateAsset(promenadeDeck, decksPath + "PromenadeDeck.asset");
            
            DeckTypeData crewDeck2 = CreateDeckTypeData(
                "CrewDeck_2",
                DeckType.Crew,
                1,
                10,  // 10 tiles wide
                false, true, false, false,
                ZoneTag.Staff,
                "Crew quarters and facilities"
            );
            AssetDatabase.CreateAsset(crewDeck2, decksPath + "CrewDeck_2.asset");
            
            DeckTypeData crewDeck1 = CreateDeckTypeData(
                "CrewDeck_1",
                DeckType.Crew,
                0,
                6,  // 6 tiles wide
                false, false, true, false,
                ZoneTag.Staff,
                "Engine room and core systems"
            );
            AssetDatabase.CreateAsset(crewDeck1, decksPath + "CrewDeck_1.asset");
            
            // Create Ship Class Data
            ShipClassData mistral = CreateInstance<ShipClassData>();
            mistral.name = "Mistral_CoastalClass";
            
            // Use reflection to set private fields (since we don't have public setters)
            var shipClassType = typeof(ShipClassData);
            
            SetPrivateField(mistral, "_shipName", "Mistral");
            SetPrivateField(mistral, "_className", "Coastal Cruiser");
            SetPrivateField(mistral, "_shipId", "mistral_coastal");
            SetPrivateField(mistral, "_maxPassengers", 40);
            SetPrivateField(mistral, "_maxCrew", 10);
            SetPrivateField(mistral, "_totalTiles", 64);
            SetPrivateField(mistral, "_waterCapacity", 150f);
            SetPrivateField(mistral, "_foodCapacity", 200f);
            SetPrivateField(mistral, "_wasteCapacity", 180f);
            SetPrivateField(mistral, "_fuelCapacity", 1000f);
            SetPrivateField(mistral, "_startingMoney", 5000f);
            SetPrivateField(mistral, "_startingWater", 150f);
            SetPrivateField(mistral, "_startingFood", 200f);
            SetPrivateField(mistral, "_startingFuel", 800f);
            SetPrivateField(mistral, "_defaultVoyageDays", 2);
            SetPrivateField(mistral, "_cruiseSpeed", 20f);
            SetPrivateField(mistral, "_dailyUpkeep", 1200f);
            SetPrivateField(mistral, "_standardTicketPrice", 120f);
            SetPrivateField(mistral, "_premiumTicketPrice", 300f);
            SetPrivateField(mistral, "_description", 
                "The Mistral is a coastal cruise vessel, perfect for short voyages. " +
                "Your uncle's pride and joy, now passed down to you. " +
                "A modest ship with 40 passenger capacity, 6 decks, and basic amenities.");
            
            // Create deck configs
            var deckConfigs = new System.Collections.Generic.List<ShipClassData.DeckConfig>
            {
                CreateDeckConfig(5, bridgeDeck),
                CreateDeckConfig(4, mainDeckA),
                CreateDeckConfig(3, mainDeckB),
                CreateDeckConfig(2, promenadeDeck),
                CreateDeckConfig(1, crewDeck2),
                CreateDeckConfig(0, crewDeck1)
            };
            
            SetPrivateField(mistral, "_deckConfigs", deckConfigs);
            
            AssetDatabase.CreateAsset(mistral, shipsPath + "Mistral_CoastalClass.asset");
            
            // Save and refresh
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            Debug.Log("✅ Mistral configuration created successfully!");
            Debug.Log($"Deck assets: {decksPath}");
            Debug.Log($"Ship asset: {shipsPath}Mistral_CoastalClass.asset");
            Debug.Log($"Tile visuals: {tilesPath}DefaultTileVisualData.asset");
            
            EditorUtility.DisplayDialog("Setup Complete", 
                "Mistral configuration created!\n\n" +
                "⚠️ NEXT STEPS:\n" +
                "1. Create Unity Tiles (Palette window)\n" +
                "2. Assign tiles to TileVisualData asset\n" +
                "3. Use Window > 2D > Tile Palette to create tiles\n\n" +
                "The TileVisualData asset is shared between all ships.", 
                "OK");
            
            // Select the created asset
            Selection.activeObject = tileVisuals;
            EditorGUIUtility.PingObject(tileVisuals);
        }
        
        /// <summary>
        /// Creates a TileVisualData asset (tiles will need to be assigned manually)
        /// </summary>
        private TileVisualData CreateTileVisualData()
        {
            TileVisualData data = CreateInstance<TileVisualData>();
            data.name = "DefaultTileVisualData";
            
            // Note: TileBase assets must be created manually via Tile Palette
            // or imported from sprite sheets. They cannot be created via script easily.
            
            Debug.Log("⚠️ TileVisualData created. You must manually assign TileBase assets:");
            Debug.Log("   1. Open Window > 2D > Tile Palette");
            Debug.Log("   2. Create tiles for each type (Indoor, Outdoor, etc.)");
            Debug.Log("   3. Assign them to the TileVisualData asset");
            
            return data;
        }
        
        private DeckTypeData CreateDeckTypeData(
            string name,
            DeckType deckType,
            int defaultLevel,
            int width,
            bool allowsPassenger,
            bool allowsCrew,
            bool allowsUtilities,
            bool allowsRecreation,
            ZoneTag zoneTag,
            string description)
        {
            DeckTypeData data = CreateInstance<DeckTypeData>();
            data.name = name;
            
            SetPrivateField(data, "_deckName", name);
            SetPrivateField(data, "_deckType", deckType);
            SetPrivateField(data, "_defaultLevel", defaultLevel);
            SetPrivateField(data, "_width", width);
            SetPrivateField(data, "_tileSize", 1f);
            SetPrivateField(data, "_allowsPassengerRooms", allowsPassenger);
            SetPrivateField(data, "_allowsCrewRooms", allowsCrew);
            SetPrivateField(data, "_allowsUtilities", allowsUtilities);
            SetPrivateField(data, "_allowsRecreation", allowsRecreation);
            SetPrivateField(data, "_defaultZoneTag", zoneTag);
            SetPrivateField(data, "_description", description);
            
            return data;
        }
        
        private ShipClassData.DeckConfig CreateDeckConfig(int level, DeckTypeData deckTypeData)
        {
            var config = new ShipClassData.DeckConfig();
            SetPrivateField(config, "_deckLevel", level);
            SetPrivateField(config, "_deckTypeData", deckTypeData);
            SetPrivateField(config, "_widthOverride", 0);  // Use DeckTypeData default
            return config;
        }
        
        private void SetPrivateField(object obj, string fieldName, object value)
        {
            var field = obj.GetType().GetField(fieldName, 
                System.Reflection.BindingFlags.NonPublic | 
                System.Reflection.BindingFlags.Instance);
            
            if (field != null)
            {
                field.SetValue(obj, value);
            }
            else
            {
                Debug.LogWarning($"Field '{fieldName}' not found on {obj.GetType().Name}");
            }
        }
    }
}
