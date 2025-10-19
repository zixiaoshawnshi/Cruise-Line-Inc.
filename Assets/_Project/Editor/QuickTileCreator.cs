using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

namespace CruiseLineInc.Editor
{
    /// <summary>
    /// Quick utility to create colored tile assets for testing
    /// </summary>
    public class QuickTileCreator : EditorWindow
    {
        [MenuItem("Cruise Line Inc/Setup/Create Quick Tiles")]
        public static void CreateQuickTiles()
        {
            string path = "Assets/_Project/Art/Tiles/";
            
            // Ensure folder exists
            if (!AssetDatabase.IsValidFolder("Assets/_Project/Art"))
            {
                AssetDatabase.CreateFolder("Assets/_Project", "Art");
            }
            if (!AssetDatabase.IsValidFolder(path.TrimEnd('/')))
            {
                AssetDatabase.CreateFolder("Assets/_Project/Art", "Tiles");
            }
            
            // Create a white square sprite for tiles to use
            Sprite whiteSquareSprite = CreateWhiteSquareSprite(path);
            
            // Create colored tiles with distinct colors using the white square
            CreateColoredTile("IndoorTile", new Color(0.7f, 0.7f, 0.7f), whiteSquareSprite, path); // Grey
            CreateColoredTile("OutdoorTile", new Color(0.6f, 0.8f, 1f), whiteSquareSprite, path); // Light Blue
            CreateColoredTile("EntranceTile", new Color(1f, 0.9f, 0.3f), whiteSquareSprite, path); // Yellow
            CreateColoredTile("CorridorTile", new Color(0.8f, 0.8f, 0.8f), whiteSquareSprite, path); // Light Grey
            CreateColoredTile("UtilityTile", new Color(0.4f, 0.4f, 0.4f), whiteSquareSprite, path); // Dark Grey
            CreateColoredTile("RestrictedTile", new Color(1f, 0.3f, 0.3f), whiteSquareSprite, path); // Red
            CreateColoredTile("SpecialTile", new Color(0.7f, 0.4f, 1f), whiteSquareSprite, path); // Purple
            CreateColoredTile("EmptyBuildableTile", new Color(0.5f, 1f, 0.5f, 0.5f), whiteSquareSprite, path); // Green (transparent)
            CreateColoredTile("EmptyNonBuildableTile", new Color(0.3f, 0.3f, 0.3f, 0.5f), whiteSquareSprite, path); // Dark Grey (transparent)
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            Debug.Log("✅ Created 9 tile assets in: " + path);
            
            EditorUtility.DisplayDialog("Tiles Created!", 
                "9 colored tile assets created in:\n" + path + "\n\n" +
                "Next steps:\n" +
                "1. Select DefaultTileVisualData asset\n" +
                "2. Assign the tiles in Inspector\n" +
                "3. Indoor Tile → IndoorTile.asset, etc.", 
                "OK");
            
            // Open the folder
            EditorUtility.RevealInFinder(path);
        }
        
        /// <summary>
        /// Creates a white square texture and sprite for tiles to use
        /// </summary>
        private static Sprite CreateWhiteSquareSprite(string path)
        {
            // Check if sprite already exists
            string texturePath = path + "WhiteSquare.png";
            Sprite existingSprite = AssetDatabase.LoadAssetAtPath<Sprite>(texturePath);
            if (existingSprite != null)
            {
                Debug.Log("WhiteSquare sprite already exists, reusing it.");
                return existingSprite;
            }
            
            // Create a 64x64 white texture
            Texture2D texture = new Texture2D(64, 64);
            Color[] pixels = new Color[64 * 64];
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = Color.white;
            }
            texture.SetPixels(pixels);
            texture.Apply();
            
            // Save as PNG
            byte[] bytes = texture.EncodeToPNG();
            System.IO.File.WriteAllBytes(texturePath, bytes);
            AssetDatabase.Refresh();
            
            // Set texture import settings for sprites
            TextureImporter importer = AssetImporter.GetAtPath(texturePath) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.spritePixelsPerUnit = 64; // 1 sprite unit = 1 tile
                importer.filterMode = FilterMode.Point; // Pixel-perfect
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                AssetDatabase.ImportAsset(texturePath, ImportAssetOptions.ForceUpdate);
            }
            
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(texturePath);
            Debug.Log("✅ Created WhiteSquare sprite for tiles");
            return sprite;
        }
        
        /// <summary>
        /// Creates a simple colored tile asset
        /// </summary>
        private static void CreateColoredTile(string name, Color color, Sprite sprite, string path)
        {
            Tile tile = ScriptableObject.CreateInstance<Tile>();
            tile.name = name;
            tile.color = color;
            tile.sprite = sprite;
            
            string assetPath = path + name + ".asset";
            AssetDatabase.CreateAsset(tile, assetPath);
            
            Debug.Log($"Created: {name} ({color})");
        }
    }
}
