using System;
using System.Collections.Generic;
using CruiseLineInc.Ship.Data;

namespace CruiseLineInc.Ship
{
    /// <summary>
    /// Central change pump that ensures ship mutations are delivered on the main thread.
    /// ShipData enqueues events; gameplay systems call ProcessPending (typically once per frame).
    /// </summary>
    public sealed class ShipUpdateDispatcher
    {
        private static ShipUpdateDispatcher _instance;
        public static ShipUpdateDispatcher Instance => _instance ??= new ShipUpdateDispatcher();
        public static bool HasInstance => _instance != null;

        private readonly Queue<ShipChangeEventArgs> _pending = new Queue<ShipChangeEventArgs>();
        private bool _isProcessing;

        private ShipUpdateDispatcher()
        {
        }

        public event Action<ShipChangeEventArgs> ShipChanged;

        public void Enqueue(ShipChangeEventArgs change)
        {
            if (change == null || !change.HasChanges)
                return;

            lock (_pending)
            {
                _pending.Enqueue(change);
            }
        }

        public void ProcessPending()
        {
            if (_isProcessing)
                return;

            _isProcessing = true;
            try
            {
                while (true)
                {
                    ShipChangeEventArgs change;
                    lock (_pending)
                    {
                        if (_pending.Count == 0)
                            break;

                        change = _pending.Dequeue();
                    }

                    ShipChanged?.Invoke(change);
                }
            }
            finally
            {
                _isProcessing = false;
            }
        }

        public void Clear()
        {
            lock (_pending)
            {
                _pending.Clear();
            }
        }
    }
}
