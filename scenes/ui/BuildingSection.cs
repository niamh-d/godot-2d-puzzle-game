using Godot;
using Game.Resources.Building;

namespace Game.UI;

public partial class BuildingSection : PanelContainer
{

    [Signal]
    public delegate void SelectButtonPressedEventHandler();

    private Label titleLabel;
    private Label descriptionLabel;
    private Label costLabel;
    private Button selectButton;

    public override void _Ready()
    {
        titleLabel = GetNode<Label>("%TitleLabel");
        descriptionLabel = GetNode<Label>("%DescriptionLabel");
        costLabel = GetNode<Label>("%CostLabel");
        selectButton = GetNode<Button>("%Button");

        selectButton.Pressed += OnSelectButtonPressed;
    }

    private void OnSelectButtonPressed()
    {
        EmitSignal(SignalName.SelectButtonPressed);
    }


    public void SetBuildingResource(BuildingResource buildingResource)
    {
        titleLabel.Text = buildingResource.DisplayName;
        costLabel.Text = $"{buildingResource.ResourceCost}";
        descriptionLabel.Text = buildingResource.Description;
    }
}
