using Godot;
using System;

namespace Game;
public partial class Main : Node2D

{
	private Sprite2D sprite;
	private PackedScene buildingScene;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		buildingScene = GD.Load<PackedScene>("res://scenes/building/Building.tscn");
		sprite = GetNode<Sprite2D>("Cursor");
	}

	public override void _UnhandledInput(InputEvent evt)
	{
		if (evt.IsActionPressed("left_click"))
		{
			PlaceBuildingAtMousePos();
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var gridPos = GetMouseGridCellPos();
		sprite.GlobalPosition = gridPos * 64;
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
	}
}
