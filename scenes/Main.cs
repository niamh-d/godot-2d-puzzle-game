using Godot;
using System;

namespace Game;
public partial class Main : Node2D

{
	private Sprite2D cursor;
	private PackedScene buildingScene;
	private Button placeBuildingButton;
	private TileMapLayer highlightTilemapLayer;
	private Vector2? hoveredGridCell;

	public override void _Ready()
	{
		buildingScene = GD.Load<PackedScene>("res://scenes/building/Building.tscn");
		cursor = GetNode<Sprite2D>("Cursor");
		placeBuildingButton = GetNode<Button>("PlaceBuildingButton");
		highlightTilemapLayer = GetNode<TileMapLayer>("HighlightTileMapLayer");

		cursor.Visible = false;

		placeBuildingButton.Pressed += OnButtonPressed;
	}

	public override void _UnhandledInput(InputEvent evt)
	{
		if (cursor.Visible && evt.IsActionPressed("left_click"))
		{
			PlaceBuildingAtMousePos();
			cursor.Visible = false;
		}
	}

	public override void _Process(double delta)
	{
		var gridPos = GetMouseGridCellPos();
		cursor.GlobalPosition = gridPos * 64;
		if (cursor.Visible && (!hoveredGridCell.HasValue || hoveredGridCell.Value != gridPos))
		{
			hoveredGridCell = gridPos;
			UpdateHighlightTileMapLayer();
		}
	}

	private Vector2 GetMouseGridCellPos()
	{
		var mousePos = GetGlobalMousePosition();
		var gridPos = mousePos / 64;
		gridPos = gridPos.Floor();
		return gridPos;
	}

	private void PlaceBuildingAtMousePos()
	{
		var building = buildingScene.Instantiate<Node2D>();
		AddChild(building);

		var gridPos = GetMouseGridCellPos();
		building.GlobalPosition = gridPos * 64;

		hoveredGridCell = null;
		UpdateHighlightTileMapLayer();
	}

	private void UpdateHighlightTileMapLayer()
	{
		highlightTilemapLayer.Clear();

		if (!hoveredGridCell.HasValue)
		{
			return;
		}

		for (var x = hoveredGridCell.Value.X - 3; x <= hoveredGridCell.Value.X + 3; x++)
		{
			for (var y = hoveredGridCell.Value.Y - 3; y <= hoveredGridCell.Value.Y + 3; y++)
			{
				highlightTilemapLayer.SetCell(new Vector2I((int)x, (int)y), 0, Vector2I.Zero);
			}
		}
	}

	private void OnButtonPressed()
	{
		cursor.Visible = true;
	}
}
