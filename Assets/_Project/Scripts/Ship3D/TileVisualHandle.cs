using UnityEngine;

namespace CruiseLineInc.Ship3D
{
    [DisallowMultipleComponent]
    public class TileVisualHandle : MonoBehaviour
    {
        public int DeckLevel { get; private set; }
        public int X { get; private set; }
        public int Z { get; private set; }

        private ShipView3D _view;

        public void Initialize(ShipView3D view, int deckLevel, int x, int z)
        {
            _view = view;
            DeckLevel = deckLevel;
            X = x;
            Z = z;
        }

        public void SetHighlight(bool highlighted)
        {
            _view?.SetTileHighlighted(DeckLevel, X, Z, highlighted);
        }

        public void SetSelected(bool selected)
        {
            _view?.SetTileSelected(DeckLevel, X, Z, selected);
        }
    }
}
