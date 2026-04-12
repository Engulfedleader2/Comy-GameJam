using Godot;

public partial class World : Node2D
{
	public override void _Ready()
	{
		var player = GetNode<CharacterBody2D>("Player");
		var spawnPoint = GetNode<Marker2D>("SpawnPoint");

		player.GlobalPosition = spawnPoint.GlobalPosition;
	}
}
