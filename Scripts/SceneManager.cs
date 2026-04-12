using Godot;
using System;

public partial class SceneManager : Node
{
	public string NextSpawnName { get; private set; } = "";
	public string PreviousScenePath { get; private set; } = "";
	public string CurrentScenePath { get; private set; } = "";

	public override void _Ready()
	{
		var currentScene = GetTree().CurrentScene;
		if (currentScene != null)
		{
			CurrentScenePath = currentScene.SceneFilePath;
		}
	}

	public void ChangeScene(string scenePath, string spawnName = "")
	{
		var currentScene = GetTree().CurrentScene;
		if (currentScene != null)
		{
			PreviousScenePath = currentScene.SceneFilePath;
		}

		CurrentScenePath = scenePath;
		NextSpawnName = spawnName;
		GetTree().ChangeSceneToFile(scenePath);
	}

	public void ClearSpawnName()
	{
		NextSpawnName = "";
	}

	public void PlacePlayerAtSpawn(Node rootNode, Node2D player)
	{
		if (rootNode == null || player == null)
		{
			return;
		}

		if (string.IsNullOrWhiteSpace(NextSpawnName))
		{
			return;
		}

		var spawnPoint = FindSpawnPoint(rootNode, NextSpawnName);
		if (spawnPoint == null)
		{
			GD.PrintErr($"Spawn point '{NextSpawnName}' was not found in scene '{rootNode.Name}'.");
			return;
		}

		player.GlobalPosition = spawnPoint.GlobalPosition;
		ClearSpawnName();
	}

	public Marker2D FindSpawnPoint(Node rootNode, string spawnName)
	{
		if (rootNode == null || string.IsNullOrWhiteSpace(spawnName))
		{
			return null;
		}

		return FindSpawnPointRecursive(rootNode, spawnName);
	}

	private Marker2D FindSpawnPointRecursive(Node currentNode, string spawnName)
	{
		if (currentNode is Marker2D marker && marker.Name == spawnName)
		{
			return marker;
		}

		foreach (Node child in currentNode.GetChildren())
		{
			var result = FindSpawnPointRecursive(child, spawnName);
			if (result != null)
			{
				return result;
			}
		}

		return null;
	}
}
