namespace Game.Manager;

using System.Collections.Generic;
using Godot;

public partial class GridManager : Node
{
	private HashSet<Vector2I> occupiedCells = new();

	[Export]
	private TileMapLayer highlightTilemapLayer;
	[Export]
	private TileMapLayer baseTerrainTilemapLayer;

	public bool IsTilePosValid(Vector2I tilePos)
	{
		var customData = baseTerrainTilemapLayer.GetCellTileData(tilePos);
		if (customData == null) return false;
		if (!(bool)customData.GetCustomData("buildable")) return false;
		return !occupiedCells.Contains(tilePos);
	}

	public void MarkTileAsOccupied(Vector2I tilePos)
	{
		occupiedCells.Add(tilePos);
	}

	public void HighlightValidTilesInRadius(Vector2I rootCell, int radius)
	{
		ClearHighlightTiles();

		for (var x = rootCell.X - radius; x <= rootCell.X + radius; x++)
		{
			for (var y = rootCell.Y - radius; y <= rootCell.Y + radius; y++)
			{
				var tilePos = new Vector2I(x, y);
				if (!IsTilePosValid(tilePos)) continue;
				highlightTilemapLayer.SetCell(tilePos, 0, Vector2I.Zero);
			}
		}
	}

	public void ClearHighlightTiles()
	{
		highlightTilemapLayer.Clear();
	}

	public Vector2I GetMouseGridCellPos()
	{
		var mousePos = highlightTilemapLayer.GetGlobalMousePosition();
		var gridPos = mousePos / 64;
		gridPos = gridPos.Floor();
		return new Vector2I((int)gridPos.X, (int)gridPos.Y);
	}
}
