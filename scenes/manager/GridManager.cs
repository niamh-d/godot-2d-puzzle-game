using System.Collections.Generic;
using System.Linq;
using Game.Autoload;
using Game.Component;
using Godot;

namespace Game.Manager;

public partial class GridManager : Node
{
	private HashSet<Vector2I> validBuildableTiles = new();

	[Export]
	private TileMapLayer highlightTilemapLayer;
	[Export]
	private TileMapLayer baseTerrainTilemapLayer;

	public override void _Ready()
	{
		GameEvents.Instance.BuildingPlaced += OnBuildingPlaced;
	}

	public bool IsTilePosValid(Vector2I tilePos)
	{
		var customData = baseTerrainTilemapLayer.GetCellTileData(tilePos);
		if (customData == null) return false;
		return (bool)customData.GetCustomData("buildable");
	}

	public bool IsTilePosBuildable(Vector2I tilePos)
	{
		return validBuildableTiles.Contains(tilePos);
	}

	public void HighlightBuildableTiles()
	{
		foreach (var tilePosition in validBuildableTiles)
		{
			highlightTilemapLayer.SetCell(tilePosition, 0, Vector2I.Zero);
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

	private void UpdateValidBuildableTiles(BuildingComponent buildingComponent)
	{
		var rootCell = buildingComponent.GetGridCellPos();
		for (var x = rootCell.X - buildingComponent.BuildableRadius; x <= rootCell.X + buildingComponent.BuildableRadius; x++)
		{
			for (var y = rootCell.Y - buildingComponent.BuildableRadius; y <= rootCell.Y + buildingComponent.BuildableRadius; y++)
			{
				var tilePos = new Vector2I(x, y);
				if (!IsTilePosValid(tilePos)) continue;
				validBuildableTiles.Add(tilePos);
			}
		}
		validBuildableTiles.Remove(buildingComponent.GetGridCellPos());
	}

	private void OnBuildingPlaced(BuildingComponent buildingComponent)
	{
		UpdateValidBuildableTiles(buildingComponent);
	}
}
