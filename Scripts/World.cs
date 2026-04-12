using Godot;
using System;

public partial class World : Node2D
{
	public override void _Ready()
	{
		var player = GetNodeOrNull<CharacterBody2D>("Player");
		if (player == null)
		{
			GD.PrintErr("Player not found in World.");
			return;
		}

		var sceneManager = GetNodeOrNull<SceneManager>("/root/SceneManager");
		if (sceneManager == null)
		{
			GD.PrintErr("SceneManager autoload not found.");
			return;
		}

		if (!string.IsNullOrWhiteSpace(sceneManager.NextSpawnName))
		{
			sceneManager.PlacePlayerAtSpawn(this, player);
			return;
		}

		var defaultSpawn = GetNodeOrNull<Marker2D>("Spawn_World_Start");
		if (defaultSpawn != null)
		{
			player.GlobalPosition = defaultSpawn.GlobalPosition;
		}
		else
		{
			GD.PrintErr("Default spawn 'Spawn_World_Start' not found in World.");
		}
	}
}
