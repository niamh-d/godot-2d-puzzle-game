using System;
using Game.Manager;
using Godot;

namespace Game;
public partial class Main : Node

{
	private GridManager gridManager;
	private GoldMine goldMine;

	public override void _Ready()
	{
		gridManager = GetNode<GridManager>("GridManager");
		goldMine = GetNode<GoldMine>("%GoldMine");

		gridManager.GridStateUpdated += OnGridStateUpdated;
	}

	private void OnGridStateUpdated()
	{
		var goldMineTilePos = gridManager.ConvertWorldPositionToTilePosition(goldMine.GlobalPosition);
		if (gridManager.IsTilePosBuildable(goldMineTilePos))
		{
			goldMine.SetActive();
			GD.Print("WIN!!");
		}
	}
}
