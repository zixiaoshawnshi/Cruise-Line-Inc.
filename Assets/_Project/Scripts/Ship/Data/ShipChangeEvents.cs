using System;
using System.Collections.Generic;
using System.Linq;

namespace CruiseLineInc.Ship.Data
{
    public enum ShipChangeType
    {
        Created = 0,
        Updated,
        Removed
    }

    /// <summary>
    /// Snapshot of ship mutations that occurred during an edit scope.
    /// Consumers should treat collections as immutable.
    /// </summary>
    public sealed class ShipChangeEventArgs : EventArgs
    {
        public ShipChangeEventArgs(
            string reason,
            IEnumerable<int> dirtyDecks,
            IEnumerable<ZoneId> createdZones,
            IEnumerable<ZoneId> updatedZones,
            IEnumerable<ZoneId> removedZones,
            IEnumerable<RoomId> createdRooms,
            IEnumerable<RoomId> updatedRooms,
            IEnumerable<RoomId> removedRooms,
            bool isBatch)
        {
            Reason = reason;
            DirtyDecks = ToArray(dirtyDecks);
            CreatedZones = ToArray(createdZones);
            UpdatedZones = ToArray(updatedZones);
            RemovedZones = ToArray(removedZones);
            CreatedRooms = ToArray(createdRooms);
            UpdatedRooms = ToArray(updatedRooms);
            RemovedRooms = ToArray(removedRooms);
            IsBatch = isBatch;
            TimestampUtc = DateTime.UtcNow;
        }

        public string Reason { get; }
        public IReadOnlyList<int> DirtyDecks { get; }
        public IReadOnlyList<ZoneId> CreatedZones { get; }
        public IReadOnlyList<ZoneId> UpdatedZones { get; }
        public IReadOnlyList<ZoneId> RemovedZones { get; }
        public IReadOnlyList<RoomId> CreatedRooms { get; }
        public IReadOnlyList<RoomId> UpdatedRooms { get; }
        public IReadOnlyList<RoomId> RemovedRooms { get; }
        public bool IsBatch { get; }
        public DateTime TimestampUtc { get; }

        public bool HasChanges =>
            DirtyDecks.Count > 0 ||
            CreatedZones.Count > 0 ||
            UpdatedZones.Count > 0 ||
            RemovedZones.Count > 0 ||
            CreatedRooms.Count > 0 ||
            UpdatedRooms.Count > 0 ||
            RemovedRooms.Count > 0;

        private static IReadOnlyList<T> ToArray<T>(IEnumerable<T> source)
        {
            if (source == null)
                return Array.Empty<T>();

            T[] items = source as T[] ?? source.ToArray();
            return items.Length == 0 ? Array.Empty<T>() : items;
        }
    }
}
