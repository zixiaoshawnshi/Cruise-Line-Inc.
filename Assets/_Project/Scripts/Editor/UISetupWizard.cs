using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

namespace CruiseLineInc.Editor
{
    /// <summary>
    /// Creates a basic UI layout with top banner resource panel
    /// </summary>
    public class UISetupWizard : EditorWindow
    {
        [MenuItem("Cruise Line Inc/Setup/Create Resource UI")]
        public static void CreateResourceUI()
        {
            // Find or create Canvas
            Canvas canvas = Object.FindFirstObjectByType<Canvas>();
            if (canvas == null)
            {
                GameObject canvasObj = new GameObject("Canvas");
                canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasObj.AddComponent<CanvasScaler>();
                canvasObj.AddComponent<GraphicRaycaster>();
                
                Debug.Log("✅ Created Canvas");
            }
            
            // Create Top Banner
            GameObject topBanner = new GameObject("TopBanner");
            topBanner.transform.SetParent(canvas.transform, false);
            
            RectTransform bannerRect = topBanner.AddComponent<RectTransform>();
            bannerRect.anchorMin = new Vector2(0, 1);
            bannerRect.anchorMax = new Vector2(1, 1);
            bannerRect.pivot = new Vector2(0.5f, 1);
            bannerRect.sizeDelta = new Vector2(0, 60);
            bannerRect.anchoredPosition = Vector2.zero;
            
            Image bannerBg = topBanner.AddComponent<Image>();
            bannerBg.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);
            
            // Create Resource Panel container
            GameObject resourcePanel = new GameObject("ResourcePanel");
            resourcePanel.transform.SetParent(topBanner.transform, false);
            
            RectTransform panelRect = resourcePanel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0, 0);
            panelRect.anchorMax = new Vector2(1, 1);
            panelRect.offsetMin = new Vector2(20, 0);
            panelRect.offsetMax = new Vector2(-20, 0);
            
            HorizontalLayoutGroup layout = resourcePanel.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 30;
            layout.childAlignment = TextAnchor.MiddleLeft;
            layout.childControlWidth = false;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;
            
            // Add ResourcePanel script
            var resourcePanelScript = resourcePanel.AddComponent<UI.ResourcePanel>();
            
            // Create resource displays
            var moneyText = CreateResourceDisplay(resourcePanel.transform, "💰 Money", "MoneyText");
            var waterText = CreateResourceDisplay(resourcePanel.transform, "💧 Water", "WaterText");
            var foodText = CreateResourceDisplay(resourcePanel.transform, "🍽️ Food", "FoodText");
            var wasteText = CreateResourceDisplay(resourcePanel.transform, "🗑️ Waste", "WasteText");
            
            // Assign references using SerializedObject
            SerializedObject serializedPanel = new SerializedObject(resourcePanelScript);
            serializedPanel.FindProperty("_moneyText").objectReferenceValue = moneyText;
            serializedPanel.FindProperty("_waterText").objectReferenceValue = waterText;
            serializedPanel.FindProperty("_foodText").objectReferenceValue = foodText;
            serializedPanel.FindProperty("_wasteText").objectReferenceValue = wasteText;
            serializedPanel.ApplyModifiedProperties();
            
            // Select the resource panel
            Selection.activeGameObject = resourcePanel;
            
            Debug.Log("✅ Created Resource UI in Top Banner");
            
            EditorUtility.DisplayDialog("Resource UI Created!", 
                "Top banner with resource display created!\n\n" +
                "Components:\n" +
                "- Canvas (ScreenSpaceOverlay)\n" +
                "- TopBanner (dark background)\n" +
                "- ResourcePanel (Money, Water, Food, Waste)\n\n" +
                "The panel will auto-update when resources change.\n" +
                "Make sure ResourceManager exists in the scene!", 
                "OK");
        }
        
        private static TextMeshProUGUI CreateResourceDisplay(Transform parent, string label, string name)
        {
            GameObject container = new GameObject(name);
            container.transform.SetParent(parent, false);
            
            RectTransform rect = container.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(150, 40);
            
            // Icon/Label
            GameObject labelObj = new GameObject("Label");
            labelObj.transform.SetParent(container.transform, false);
            
            RectTransform labelRect = labelObj.AddComponent<RectTransform>();
            labelRect.anchorMin = new Vector2(0, 0);
            labelRect.anchorMax = new Vector2(1, 0.5f);
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;
            
            TextMeshProUGUI labelText = labelObj.AddComponent<TextMeshProUGUI>();
            labelText.text = label;
            labelText.fontSize = 12;
            labelText.color = new Color(0.7f, 0.7f, 0.7f);
            labelText.alignment = TextAlignmentOptions.BottomLeft;
            
            // Value
            GameObject valueObj = new GameObject("Value");
            valueObj.transform.SetParent(container.transform, false);
            
            RectTransform valueRect = valueObj.AddComponent<RectTransform>();
            valueRect.anchorMin = new Vector2(0, 0.5f);
            valueRect.anchorMax = new Vector2(1, 1);
            valueRect.offsetMin = Vector2.zero;
            valueRect.offsetMax = Vector2.zero;
            
            TextMeshProUGUI valueText = valueObj.AddComponent<TextMeshProUGUI>();
            valueText.text = "0";
            valueText.fontSize = 18;
            valueText.fontStyle = FontStyles.Bold;
            valueText.color = Color.white;
            valueText.alignment = TextAlignmentOptions.TopLeft;
            
            return valueText;
        }
    }
}
