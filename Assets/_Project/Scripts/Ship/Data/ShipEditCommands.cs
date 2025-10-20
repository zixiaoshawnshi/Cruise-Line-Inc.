using System;

namespace CruiseLineInc.Ship.Data
{
    /// <summary>
    /// Command interface for ship edits so we can build undo/redo and memento logs.
    /// </summary>
    public interface IShipEditCommand
    {
        string Description { get; }
        void Apply(ShipData shipData);
        void Revert(ShipData shipData);
    }

    /// <summary>
    /// Lightweight record capturing before/after ids for simple edits.
    /// </summary>
    [Serializable]
    public struct ShipEditMemento
    {
        public int Sequence;
        public DateTime Timestamp;
        public string CommandDescription;

        public ShipEditMemento(int sequence, DateTime timestamp, string commandDescription)
        {
            Sequence = sequence;
            Timestamp = timestamp;
            CommandDescription = commandDescription;
        }
    }
}
