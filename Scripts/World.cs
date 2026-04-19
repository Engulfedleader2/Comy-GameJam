using Godot;
using System;

public partial class World : Node2D
{
	private CanvasLayer? _questDisplayUi;
	private Label? _questTitleLabel;
	private Label? _questObjectiveLabel;
	public override void _Ready()
	{
		PlacePlayerAtSpawn();
		SpawnNpcsForCurrentLoop();
		TriggerOpeningDialogueIfNeeded();
		SetupQuestDisplayUi();
	}

	private void SetupQuestDisplayUi()
	{
		_questDisplayUi = GetNodeOrNull<CanvasLayer>("QuestDisplayUI");
		if (_questDisplayUi == null)
		{
			GD.PrintErr("QuestDisplayUI node was not found in World.");
			return;
		}

		_questTitleLabel = _questDisplayUi.GetNodeOrNull<Label>("PanelContainer/MarginContainer/VBoxContainer/QuestTitleLabel");
		_questObjectiveLabel = _questDisplayUi.GetNodeOrNull<Label>("PanelContainer/MarginContainer/VBoxContainer/QuestObjectiveLabel");

		if (_questTitleLabel != null)
		{
			_questTitleLabel.Text = "Current Objective";
		}

		if (_questObjectiveLabel == null)
		{
			GD.PrintErr("QuestObjectiveLabel was not found under QuestDisplayUI.");
			return;
		}

		var questManager = GetNodeOrNull<QuestManager>("/root/QuestManager");
		if (questManager == null)
		{
			GD.PrintErr("QuestManager autoload not found when setting up QuestDisplayUI.");
			return;
		}

		questManager.ObjectiveUpdated += OnQuestObjectiveUpdated;
		_questObjectiveLabel.Text = questManager.GetCurrentObjective();
	}

	private void OnQuestObjectiveUpdated(string objectiveText)
	{
		if (_questObjectiveLabel == null)
		{
			return;
		}

		_questObjectiveLabel.Text = objectiveText;
	}

	private void PlacePlayerAtSpawn()
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

	private void SpawnNpcsForCurrentLoop()
	{
		var questManager = GetNodeOrNull<QuestManager>("/root/QuestManager");
		if (questManager == null)
		{
			GD.PrintErr("QuestManager autoload not found.");
			return;
		}

		int currentLoop = questManager.CurrentLoop;
		GD.Print($"Spawning NPCs for loop {currentLoop} at step '{questManager.GetCurrentStep()?.Id ?? "none"}'");

		SetNpcVisible("RoseNPC", true);
		SetNpcVisible("ShihabNPC", true);
		SetNpcVisible("NikoNPC", true);
		SetNpcVisible("EsmereldaNPC", true);

		MoveNpcToLoopSpawn("RoseNPC", $"NPCSpawnPoints/Rose_Loop{currentLoop}");
		MoveNpcToLoopSpawn("ShihabNPC", $"NPCSpawnPoints/Shihab_Loop{currentLoop}");
		MoveNpcToLoopSpawn("NikoNPC", $"NPCSpawnPoints/Nikos_Loop{currentLoop}");
		MoveNpcToLoopSpawn("EsmereldaNPC", $"NPCSpawnPoints/Esmerelda_Loop{currentLoop}");
	}

	private void TriggerOpeningDialogueIfNeeded()
	{
		var questManager = GetNodeOrNull<QuestManager>("/root/QuestManager");
		if (questManager == null)
		{
			GD.PrintErr("QuestManager autoload not found.");
			return;
		}

		if (questManager.CurrentLoop != 1 || !questManager.IsCurrentStep("open_shrine"))
		{
			return;
		}

		var dialogueManager = GetNodeOrNull<Node>("/root/DialogueManager");
		if (dialogueManager == null)
		{
			GD.PrintErr("DialogueManager autoload not found.");
			return;
		}

		GD.Print("Triggering official opening dialogue: Game Opening -1st loop");
		dialogueManager.CallDeferred(
			"start_dialogue",
			"Shihab",
			new Godot.Collections.Array(),
			new Godot.Collections.Array<bool>(),
			"Game Opening -1st loop"
		);
	}

	private void SetNpcVisible(string npcNodeName, bool isVisible)
	{
		var npc = GetNodeOrNull<Node2D>($"NPCs/{npcNodeName}");
		if (npc == null)
		{
			GD.PrintErr($"NPC '{npcNodeName}' not found under NPCs.");
			return;
		}

		npc.Visible = isVisible;
		npc.ProcessMode = isVisible ? ProcessModeEnum.Inherit : ProcessModeEnum.Disabled;
	}

	private void MoveNpcToLoopSpawn(string npcNodeName, string spawnNodePath)
	{
		var npc = GetNodeOrNull<Node2D>($"NPCs/{npcNodeName}");
		if (npc == null)
		{
			GD.PrintErr($"NPC '{npcNodeName}' not found under NPCs.");
			return;
		}

		var spawnPoint = GetNodeOrNull<Marker2D>(spawnNodePath);
		if (spawnPoint == null)
		{
			GD.PrintErr($"NPC spawn point '{spawnNodePath}' not found.");
			return;
		}

		npc.GlobalPosition = spawnPoint.GlobalPosition;
		GD.Print($"Moved {npcNodeName} to {spawnNodePath}");
	}
}
