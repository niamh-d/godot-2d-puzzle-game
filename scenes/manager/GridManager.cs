using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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

	public void HighlightExpandedBuildableTiles(Vector2I rootCell, int radius)
	{
		ClearHighlightTiles();
		HighlightBuildableTiles();

		var validTiles = GetValidTilesInRadius(rootCell, radius).ToHashSet();
		var expandedTiles = validTiles.Except(validBuildableTiles).Except(GetOccupiedTiles());
		var atlasCoords = new Vector2I(1, 0);
		foreach (var tilePosition in expandedTiles)
		{
			highlightTilemapLayer.SetCell(tilePosition, 0, atlasCoords);
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
		var validTiles = GetValidTilesInRadius(rootCell, buildingComponent.BuildableRadius);
		validBuildableTiles.UnionWith(validTiles);

		validBuildableTiles.ExceptWith(GetOccupiedTiles());
	}

	private List<Vector2I> GetValidTilesInRadius(Vector2I rootCell, int radius)
	{
		var result = new List<Vector2I>();
		for (var x = rootCell.X - radius; x <= rootCell.X + radius; x++)
		{
			for (var y = rootCell.Y - radius; y <= rootCell.Y + radius; y++)
			{
				var tilePos = new Vector2I(x, y);
				if (!IsTilePosValid(tilePos)) continue;
				result.Add(tilePos);
			}
		}
		return result;
	}

	private IEnumerable<Vector2I> GetOccupiedTiles()
	{
		var buildingComponents = GetTree().GetNodesInGroup(nameof(BuildingComponent)).Cast<BuildingComponent>();
		var occupiedTiles = buildingComponents.Select(x => x.GetGridCellPos());
		return occupiedTiles;
	}

	private void OnBuildingPlaced(BuildingComponent buildingComponent)
	{
		UpdateValidBuildableTiles(buildingComponent);
	}
}
