namespace Game;

using Godot;
using System.Collections.Generic;

public partial class Main : Node

{
	private Sprite2D cursor;
	private PackedScene buildingScene;
	private Button placeBuildingButton;
	private TileMapLayer highlightTilemapLayer;
	private Vector2? hoveredGridCell;
	private HashSet<Vector2> occupiedCells = new();

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
		if (hoveredGridCell.HasValue && evt.IsActionPressed("left_click") && !occupiedCells.Contains(hoveredGridCell.Value))
		{
			PlaceBuildingAtHoveredCellPos();
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
		var mousePos = highlightTilemapLayer.GetGlobalMousePosition();
		var gridPos = mousePos / 64;
		gridPos = gridPos.Floor();
		return gridPos;
	}

	private void PlaceBuildingAtHoveredCellPos()
	{
		if (!hoveredGridCell.HasValue)
		{
			return;
		}

		var building = buildingScene.Instantiate<Node2D>();
		AddChild(building);

		building.GlobalPosition = hoveredGridCell.Value * 64;
		occupiedCells.Add(hoveredGridCell.Value);

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
