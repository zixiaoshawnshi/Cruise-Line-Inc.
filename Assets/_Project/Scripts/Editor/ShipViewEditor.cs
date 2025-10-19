using UnityEditor;
using UnityEngine;
using CruiseLineInc.Ship;
using System.Linq;

namespace CruiseLineInc.Editor
{
    [CustomEditor(typeof(ShipView))]
    public class ShipViewEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            
            ShipView shipView = (ShipView)target;
            
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Room Definition Management", EditorStyles.boldLabel);
            
            if (GUILayout.Button("🔄 Auto-Load Room Definitions", GUILayout.Height(30)))
            {
                LoadRoomDefinitions(shipView);
            }
            
            EditorGUILayout.HelpBox(
                "Click 'Auto-Load Room Definitions' to find all RoomDefinition assets in your project.\n\n" +
                "Room definitions must be in a 'Resources/Data/Rooms' folder OR manually assigned above.",
                MessageType.Info
            );
        }
        
        private void LoadRoomDefinitions(ShipView shipView)
        {
            // Find all RoomDefinition assets in the project
            string[] guids = AssetDatabase.FindAssets("t:RoomDefinition");
            
            if (guids.Length == 0)
            {
                EditorUtility.DisplayDialog(
                    "No Room Definitions Found",
                    "No RoomDefinition assets found in the project.\n\n" +
                    "Create some using: Assets > Create > Cruise Line Inc > Room > Room Definition",
                    "OK"
                );
                return;
            }
            
            var roomDefinitions = new System.Collections.Generic.List<Room.Data.RoomDefinition>();
            
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var roomDef = AssetDatabase.LoadAssetAtPath<Room.Data.RoomDefinition>(path);
                if (roomDef != null)
                {
                    roomDefinitions.Add(roomDef);
                }
            }
            
            // Use reflection to set the private field
            var field = typeof(ShipView).GetField("_roomDefinitions", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (field != null)
            {
                field.SetValue(shipView, roomDefinitions);
                EditorUtility.SetDirty(shipView);
                
                Debug.Log($"✅ Loaded {roomDefinitions.Count} room definitions into ShipView:");
                foreach (var def in roomDefinitions)
                {
                    Debug.Log($"  - {def.DisplayName} (ID: {def.RoomId})");
                }
                
                EditorUtility.DisplayDialog(
                    "Success!",
                    $"Successfully loaded {roomDefinitions.Count} room definition(s):\n\n" +
                    string.Join("\n", roomDefinitions.Select(d => $"• {d.DisplayName}")),
                    "OK"
                );
            }
        }
    }
}
