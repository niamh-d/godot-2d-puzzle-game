namespace Game.Manager;

using System.Collections.Generic;
using Godot;

public partial class GridManager : Node
{
	private HashSet<Vector2> occupiedCells = new();

	[Export]
	private TileMapLayer highlightTilemapLayer;
	[Export]
	private TileMapLayer baseTerrainTilemapLayer;

	public override void _Ready()
	{
	}

	public bool IsTilePosValid(Vector2 tilePos)
	{
		return !occupiedCells.Contains(tilePos);
	}

	public void MarkTileAsOccupied(Vector2 tilePos)
	{
		occupiedCells.Add(tilePos);
	}

	public void HighlightValidTilesInRadius(Vector2 rootCell, int radius)
	{
		ClearHighlightTiles();

		for (var x = rootCell.X - radius; x <= rootCell.X + radius; x++)
		{
			for (var y = rootCell.Y - radius; y <= rootCell.Y + radius; y++)
			{
				if (!IsTilePosValid(new Vector2(x, y))) continue;
				highlightTilemapLayer.SetCell(new Vector2I((int)x, (int)y), 0, Vector2I.Zero);
			}
		}
	}

	public void ClearHighlightTiles()
	{
		highlightTilemapLayer.Clear();
	}

	public Vector2 GetMouseGridCellPos()
	{
		var mousePos = highlightTilemapLayer.GetGlobalMousePosition();
		var gridPos = mousePos / 64;
		gridPos = gridPos.Floor();
		return gridPos;
	}
}
