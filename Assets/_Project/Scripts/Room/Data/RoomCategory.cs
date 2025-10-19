using System.Collections.Generic;
using UnityEngine;

namespace CruiseLineInc.Room.Data
{
    /// <summary>
    /// Defines a category of rooms (e.g., Staterooms, Dining, Entertainment)
    /// </summary>
    [CreateAssetMenu(fileName = "RoomCategory", menuName = "Cruise Line Inc/Room/Room Category", order = 0)]
    public class RoomCategory : ScriptableObject
    {
        [Header("Category Info")]
        [SerializeField] private string _categoryId = "staterooms";
        [SerializeField] private string _displayName = "Staterooms";
        [SerializeField] private Sprite _categoryIcon;
        [SerializeField] private Color _categoryColor = Color.white;
        
        [TextArea(2, 3)]
        [SerializeField] private string _description = "Passenger accommodations and cabins";
        
        [Header("Rooms in Category")]
        [Tooltip("Drag RoomDefinition assets here to add them to this category")]
        [SerializeField] private List<RoomDefinition> _rooms = new List<RoomDefinition>();
        
        [Header("UI Settings")]
        [SerializeField] private int _displayOrder = 0; // Lower = shown first
        
        #region Properties
        
        public string CategoryId => _categoryId;
        public string DisplayName => _displayName;
        public Sprite CategoryIcon => _categoryIcon;
        public Color CategoryColor => _categoryColor;
        public string Description => _description;
        public List<RoomDefinition> Rooms => _rooms;
        public int DisplayOrder => _displayOrder;
        
        #endregion
        
        #region Validation
        
        // OnValidate disabled to allow adding items in Inspector
        // If you need to clean up null entries, do it manually via context menu
        /*
        private void OnValidate()
        {
            if (_rooms != null)
            {
                _rooms.RemoveAll(r => r == null);
            }
        }
        */
        
        [ContextMenu("Clean Up Null Entries")]
        private void CleanUpNullEntries()
        {
            if (_rooms != null)
            {
                int before = _rooms.Count;
                _rooms.RemoveAll(r => r == null);
                int removed = before - _rooms.Count;
                Debug.Log($"Removed {removed} null entries from {_displayName} category");
            }
        }
        
        #endregion
    }
}
