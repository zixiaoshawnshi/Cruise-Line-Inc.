# Grid & View Integration Notes

## Current Direction
- Keep `ShipData` as the single source of truth for decks, tiles, rooms, and occupancy.
- Use custom renderers (`ShipView`, `ShipView3D`) as thin views that subscribe to data events or are refreshed by systems that mutate `ShipData`.
- Borrow ideas from SoulGames Easy Grid Builder (ghost previews, heatmaps, per-cell visuals) but avoid embedding the third-party grid runtime directly.

## ShipView3D Roadmap (High Level)
1. **Deck Grids**  
   - Build a lightweight `DeckVisual` per deck mirroring width/depth.  
   - Maintain tile nodes (position, renderer reference, flags) for fast lookup.
2. **Tile Rendering**  
   - Switch from single quad meshes to per-tile prefabs (already in progress).  
   - Support zone colours, buildable overlays, and selection highlights via material instances or decals.
3. **Interaction Layer**  
   - Introduce a grid interaction controller that handles raycasts, hover info, and placement previews.  
   - Emit events so build tools can request placement/validation without referencing rendering internals.
4. **Visual Overlays**  
   - Implement ghost objects, area highlights, and navigation heatmaps using pooled quads or render textures.
5. **Performance**  
   - Batch or instance tile meshes where possible.  
   - Allow hiding/inactivating decks above/below the active focus deck to keep scene light.

## Open Items
- Define how and when ShipData mutations trigger renderer refresh (events vs. manual calls).  
- Decide on furniture data storage; current plan is to introduce a new data layer before visuals depend on it.  
- Establish undo/redo story once grid interaction tooling is in place.

Keep this document lightweight—update it as systems evolve so the team has a quick reference to current grid/view architecture decisions.
