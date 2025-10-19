using UnityEngine;

namespace CruiseLineInc
{
    /// <summary>
    /// Helper extension methods for the project
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Safely gets a component or adds it if it doesn't exist
        /// </summary>
        public static T GetOrAddComponent<T>(this GameObject obj) where T : Component
        {
            T component = obj.GetComponent<T>();
            if (component == null)
            {
                component = obj.AddComponent<T>();
            }
            return component;
        }
        
        /// <summary>
        /// Calculates Manhattan distance between two Vector2Int positions
        /// </summary>
        public static int ManhattanDistance(this Vector2Int from, Vector2Int to)
        {
            return Mathf.Abs(from.x - to.x) + Mathf.Abs(from.y - to.y);
        }
        
        /// <summary>
        /// Calculates distance between two tiles in 2D cross-section (X position + deck level)
        /// </summary>
        public static int CrossSectionDistance(int fromX, int fromDeck, int toX, int toDeck)
        {
            return Mathf.Abs(fromX - toX) + Mathf.Abs(fromDeck - toDeck);
        }
        
        /// <summary>
        /// Checks if a value is within a percentage threshold
        /// </summary>
        public static bool IsBelow(this float value, float threshold)
        {
            return value < threshold;
        }
        
        /// <summary>
        /// Clamps a value between 0 and 1
        /// </summary>
        public static float Clamp01(this float value)
        {
            return Mathf.Clamp01(value);
        }
        
        /// <summary>
        /// Remap a value from one range to another
        /// </summary>
        public static float Remap(this float value, float fromMin, float fromMax, float toMin, float toMax)
        {
            return (value - fromMin) / (fromMax - fromMin) * (toMax - toMin) + toMin;
        }
    }
}
