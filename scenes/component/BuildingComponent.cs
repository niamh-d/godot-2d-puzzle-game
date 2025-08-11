using System.Collections.Generic;
using Game.Autoload;
using Game.Resources.Building;
using Godot;

namespace Game.Component;

public partial class BuildingComponent : Node2D
{
	[Export(PropertyHint.File, "*.tres")]
	private string BuildingResourcePath;

	public BuildingResource BuildingResource { get; private set; }

	private HashSet<Vector2I> occupiedTiles = new();


	public override void _Ready()
	{
		if (BuildingResourcePath != null)
		{
			BuildingResource = GD.Load<BuildingResource>(BuildingResourcePath);
		}

		AddToGroup(nameof(BuildingComponent));
		Callable.From(Initialize).CallDeferred();
	}

	public Vector2I GetGridCellPos()
	{
		var gridPos = GlobalPosition / 64;
		gridPos = gridPos.Floor();
		return new Vector2I((int)gridPos.X, (int)gridPos.Y);
	}

	public HashSet<Vector2I> GetOccupiedCellPositions()
	{
		return [.. occupiedTiles];
	}

	public bool IsTileInBuildingArea(Vector2I tilePos)
	{
		return occupiedTiles.Contains(tilePos);
	}

	public void Destroy()
	{
		GameEvents.EmitBuildingDestroyed(this);
		Owner.QueueFree();
	}

	private void CalcOccupiedCellPositions()
	{
		var gridPos = GetGridCellPos();
		for (int x = gridPos.X; x < gridPos.X + BuildingResource.Dimensions.X; x++)
		{
			for (int y = gridPos.Y; y < gridPos.Y + BuildingResource.Dimensions.Y; y++)
			{
				occupiedTiles.Add(new Vector2I(x, y));
			}
		}
	}

	private void Initialize()
	{
		CalcOccupiedCellPositions();
		GameEvents.EmitBuildingPlaced(this);
	}
}
