using Game.Manager;
using Godot;

namespace Game;

public partial class Main : Node

{
	private GridManager gridManager;
	private Sprite2D cursor;
	private PackedScene buildingScene;
	private Button placeBuildingButton;
	private Node2D ySortRoot;
	private Vector2I? hoveredGridCell;

	public override void _Ready()
	{
		buildingScene = GD.Load<PackedScene>("res://scenes/building/Building.tscn");
		gridManager = GetNode<GridManager>("GridManager");
		cursor = GetNode<Sprite2D>("Cursor");
		placeBuildingButton = GetNode<Button>("PlaceBuildingButton");
		ySortRoot = GetNode<Node2D>("YSortRoot");

		cursor.Visible = false;

		placeBuildingButton.Pressed += OnButtonPressed;
	}

	public override void _UnhandledInput(InputEvent evt)
	{
		if (hoveredGridCell.HasValue && evt.IsActionPressed("left_click") && gridManager.IsTilePosBuildable(hoveredGridCell.Value))
		{
			PlaceBuildingAtHoveredCellPos();
			cursor.Visible = false;
		}
	}

	public override void _Process(double delta)
	{
		var gridPos = gridManager.GetMouseGridCellPos();
		cursor.GlobalPosition = gridPos * 64;
		if (cursor.Visible && (!hoveredGridCell.HasValue || hoveredGridCell.Value != gridPos))
		{
			hoveredGridCell = gridPos;
			gridManager.HighlightExpandedBuildableTiles(hoveredGridCell.Value, 3);
		}
	}

	private void PlaceBuildingAtHoveredCellPos()
	{
		if (!hoveredGridCell.HasValue)
		{
			return;
		}

		var building = buildingScene.Instantiate<Node2D>();
		ySortRoot.AddChild(building);

		building.GlobalPosition = hoveredGridCell.Value * 64;

		hoveredGridCell = null;
		gridManager.ClearHighlightTiles();
	}

	private void OnButtonPressed()
	{
		cursor.Visible = true;
		gridManager.HighlightBuildableTiles();
	}
}
