using Game.Resources.Building;
using Godot;

namespace Game.UI;

public partial class GameUI : CanvasLayer
{
	[Signal]
	public delegate void BuildingResourceSelectedEventHandler(BuildingResource buildingResource);

	private VBoxContainer buildingSectionContainer;

	[Export]
	private BuildingResource[] buildingResources;
	[Export]
	private PackedScene buildingSectionScene;

	public override void _Ready()
	{
		buildingSectionContainer = GetNode<VBoxContainer>("%BuildingSectionContainer");
		CreateBuildingSections();
	}

	private void CreateBuildingSections()
	{
		foreach (var buildingResource in buildingResources)
		{
			var buildingSection = buildingSectionScene.Instantiate<BuildingSection>();
			buildingSectionContainer.AddChild(buildingSection);
			buildingSection.SetBuildingResource(buildingResource);

			buildingSection.SelectButtonPressed += () =>
			{
				EmitSignal(SignalName.BuildingResourceSelected, buildingResource);
			};
		}
	}
}
