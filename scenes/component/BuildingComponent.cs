using Game.Autoload;
using Game.Resources.Building;
using Godot;

namespace Game.Component;

public partial class BuildingComponent : Node2D
{
	[Export(PropertyHint.File, "*.tres")]
	public string BuildingResourcePath;

	public BuildingResource BuildingResource { get; private set; }


	public override void _Ready()
	{
		if (BuildingResourcePath != null)
		{
			BuildingResource = GD.Load<BuildingResource>(BuildingResourcePath);
		}

		AddToGroup(nameof(BuildingComponent));
		Callable.From(() => GameEvents.EmitBuildingPlaced(this)).CallDeferred();
	}

	public Vector2I GetGridCellPos()
	{
		var gridPos = GlobalPosition / 64;
		gridPos = gridPos.Floor();
		return new Vector2I((int)gridPos.X, (int)gridPos.Y);
	}
}
