using Game.Autoload;
using Godot;

namespace Game.Component;

public partial class BuildingComponent : Node2D
{
	[Export]
	public int BuildableRadius { get; private set; }

	public override void _Ready()
	{
		AddToGroup(nameof(BuildingComponent));
		GameEvents.EmitBuildingPlaced(this);
	}

	public Vector2I GetGridCellPos()
	{
		var gridPos = GlobalPosition / 64;
		gridPos = gridPos.Floor();
		return new Vector2I((int)gridPos.X, (int)gridPos.Y);
	}
}
