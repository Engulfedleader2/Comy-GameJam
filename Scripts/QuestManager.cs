using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class QuestManager : Node
{
	public static QuestManager Instance { get; private set; }
	private const string TheEndScenePath = "res://Scenes/TheEnd.tscn";
	[Signal] public delegate void ObjectiveUpdatedEventHandler(string objectiveText);
	[Signal] public delegate void StepAdvancedEventHandler(string stepId, string objectiveText);

	public enum QuestStatus
	{
		Locked,
		Active,
		Completed
	}

	public enum QuestStepType
	{
		InteractObject,
		TalkToNpc,
		TalkToMultipleNpcs,
		EnterArea,
		CalmBlossoms,
		TameBlossoms,
		DefeatBoss,
		ReturnToNpc,
		ReturnToObject
	}

	public class QuestData
	{
		public string Id { get; set; } = string.Empty;
		public string Title { get; set; } = string.Empty;
		public string Objective { get; set; } = string.Empty;
		public QuestStatus Status { get; set; } = QuestStatus.Locked;
	}

	public class QuestStep
	{
		public string Id { get; set; } = string.Empty;
		public string Objective { get; set; } = string.Empty;
		public QuestStepType Type { get; set; }

		public string TargetId { get; set; } = string.Empty;
		public string AreaId { get; set; } = string.Empty;
		public string BlossomType { get; set; } = string.Empty;
		public int RequiredCount { get; set; } = 1;

		public HashSet<string> RequiredNpcIds { get; set; } = new();
	}

	public int CurrentLoop { get; private set; } = 1;
	public int CurrentStepIndex { get; private set; } = 0;

	private readonly Dictionary<string, QuestData> _quests = new();
	private readonly List<QuestStep> _steps = new();

	private readonly HashSet<string> _talkedNpcIds = new();
	private readonly HashSet<string> _interactedObjectIds = new();
	private readonly HashSet<string> _enteredAreaIds = new();
	private readonly Dictionary<string, int> _calmedBlossoms = new();
	private readonly Dictionary<string, int> _tamedBlossoms = new();
	private readonly HashSet<string> _defeatedBossIds = new();

	public override void _Ready()
	{
		Instance = this;
		GD.Print("QuestManager loaded and ready.");
		SeedStartingQuests();
		StartLoop1();
	}

	private void SeedStartingQuests()
	{
		_quests.Clear();

		_quests["main_loop_1"] = new QuestData
		{
			Id = "main_loop_1",
			Title = "Spring Begins",
			Objective = "Open the shrine.",
			Status = QuestStatus.Active
		};
	}

	private void StartLoop1()
	{
		CurrentLoop = 1;
		CurrentStepIndex = 0;

		_steps.Clear();
		_talkedNpcIds.Clear();
		_interactedObjectIds.Clear();
		_enteredAreaIds.Clear();
		_calmedBlossoms.Clear();
		_tamedBlossoms.Clear();
		_defeatedBossIds.Clear();

		_steps.Add(new QuestStep
		{
			Id = "open_shrine",
			Objective = "Open the shrine.",
			Type = QuestStepType.InteractObject,
			TargetId = "starting_shrine"
		});

		_steps.Add(new QuestStep
		{
			Id = "read_sign",
			Objective = "Read the crossroads sign.",
			Type = QuestStepType.InteractObject,
			TargetId = "crossroads_sign"
		});

		_steps.Add(new QuestStep
		{
			Id = "talk_to_majors",
			Objective = "Speak to Rose, Niko, and Esmerelda.",
			Type = QuestStepType.TalkToMultipleNpcs,
			RequiredNpcIds = new HashSet<string> { "Rose", "Niko", "Esmerelda" }
		});


		_steps.Add(new QuestStep
		{
			Id = "reach_lake",
			Objective = "Go to the lake.", 
			Type = QuestStepType.EnterArea,
			AreaId = "lake_area"
		});

		_steps.Add(new QuestStep
		{
			Id = "calm_water_blossoms",
			Objective = "Calm 1 water blossom.",
			Type = QuestStepType.CalmBlossoms,
			BlossomType = "water",
			RequiredCount = 1
		});

		_steps.Add(new QuestStep
		{
			Id = "tame_rock_blossoms",
			Objective = "Tame 1 rock blossom.",
			Type = QuestStepType.TameBlossoms,
			BlossomType = "rock",
			RequiredCount = 1
		});

		_steps.Add(new QuestStep
		{
			Id = "return_to_shihab",
			Objective = "Return to Shihab.",
			Type = QuestStepType.ReturnToNpc,
			TargetId = "Shihab"
		});

		_steps.Add(new QuestStep
		{
			Id = "learn_boss",
			Objective = "Learn about the boss blossom. Go Talk to Esmerelda.",
			Type = QuestStepType.TalkToNpc,
			TargetId = "Esmerelda"
		});

		_steps.Add(new QuestStep
		{
			Id = "calm_boss",
			Objective = "Calm the boss water blossom.",
			Type = QuestStepType.DefeatBoss,
			TargetId = "water_boss"
		});

		_steps.Add(new QuestStep
		{
			Id = "return_to_shrine",
			Objective = "Return to the shrine.",
			Type = QuestStepType.ReturnToObject,
			TargetId = "starting_shrine"
		});

		SyncActiveQuestObjective();
	}

	public QuestStep? GetCurrentStep()
	{
		if (CurrentStepIndex < 0 || CurrentStepIndex >= _steps.Count)
		{
			return null;
		}

		return _steps[CurrentStepIndex];
	}

	public string GetCurrentObjective()
	{
		return GetCurrentStep()?.Objective ?? string.Empty;
	}

	public bool IsCurrentStep(string stepId)
	{
		return GetCurrentStep()?.Id == stepId;
	}

	public void ReportObjectInteracted(string objectId)
	{
		if (string.IsNullOrWhiteSpace(objectId))
		{
			return;
		}

		_interactedObjectIds.Add(objectId);
		EvaluateCurrentStep();
	}

	public void ReportNpcTalked(string npcId)
	{
		if (string.IsNullOrWhiteSpace(npcId))
		{
			return;
		}

		_talkedNpcIds.Add(npcId);
		EvaluateCurrentStep();
	}

	public void ReportAreaEntered(string areaId)
	{
		if (string.IsNullOrWhiteSpace(areaId))
		{
			return;
		}

		_enteredAreaIds.Add(areaId);
		EvaluateCurrentStep();
	}

	public void ReportBlossomCalmed(string blossomType)
	{
		if (string.IsNullOrWhiteSpace(blossomType))
		{
			return;
		}

		if (!_calmedBlossoms.ContainsKey(blossomType))
		{
			_calmedBlossoms[blossomType] = 0;
		}

		_calmedBlossoms[blossomType] += 1;
		EvaluateCurrentStep();
	}

	public void ReportBlossomTamed(string blossomType)
	{
		if (string.IsNullOrWhiteSpace(blossomType))
		{
			return;
		}

		if (!_tamedBlossoms.ContainsKey(blossomType))
		{
			_tamedBlossoms[blossomType] = 0;
		}

		_tamedBlossoms[blossomType] += 1;
		EvaluateCurrentStep();
	}

	public void ReportBossDefeated(string bossId)
	{
		if (string.IsNullOrWhiteSpace(bossId))
		{
			return;
		}

		_defeatedBossIds.Add(bossId);
		EvaluateCurrentStep();
	}

	private void EvaluateCurrentStep()
	{
		QuestStep? step = GetCurrentStep();
		if (step == null)
		{
			return;
		}

		bool isComplete = step.Type switch
		{
			QuestStepType.InteractObject => _interactedObjectIds.Contains(step.TargetId),
			QuestStepType.TalkToNpc => _talkedNpcIds.Contains(step.TargetId),
			QuestStepType.TalkToMultipleNpcs => step.RequiredNpcIds.All(npcId => _talkedNpcIds.Contains(npcId)),
			QuestStepType.EnterArea => _enteredAreaIds.Contains(step.AreaId),
			QuestStepType.CalmBlossoms => _calmedBlossoms.GetValueOrDefault(step.BlossomType, 0) >= step.RequiredCount,
			QuestStepType.TameBlossoms => _tamedBlossoms.GetValueOrDefault(step.BlossomType, 0) >= step.RequiredCount,
			QuestStepType.DefeatBoss => _defeatedBossIds.Contains(step.TargetId),
			QuestStepType.ReturnToNpc => _talkedNpcIds.Contains(step.TargetId),
			QuestStepType.ReturnToObject => _interactedObjectIds.Contains(step.TargetId),
			_ => false
		};

		if (isComplete)
		{
			AdvanceStep();
		}
		else
		{
			SyncActiveQuestObjective();
		}
	}

	private void AdvanceStep()
	{
		CurrentStepIndex += 1;

		if (CurrentStepIndex >= _steps.Count)
		{
			CompleteQuest("main_loop_1");
			GD.Print("Loop 1 completed.");

			SceneTree? tree = GetTree();
			if (tree == null)
			{
				GD.PrintErr("SceneTree was not available when trying to load The End scene.");
				return;
			}

			Error changeSceneResult = tree.ChangeSceneToFile(TheEndScenePath);
			if (changeSceneResult != Error.Ok)
			{
				GD.PrintErr($"Failed to load The End scene at '{TheEndScenePath}'. Error: {changeSceneResult}");
			}

			return;
		}

		SyncActiveQuestObjective();
		QuestStep? currentStep = GetCurrentStep();
		EmitSignal(SignalName.StepAdvanced, currentStep?.Id ?? string.Empty, currentStep?.Objective ?? string.Empty);
		GD.Print($"Advanced to step: {currentStep?.Id}");
	}

	private void SyncActiveQuestObjective()
	{
		QuestData? activeQuest = GetActiveQuest();
		QuestStep? currentStep = GetCurrentStep();

		if (activeQuest == null || currentStep == null)
		{
			return;
		}

		activeQuest.Objective = currentStep.Objective;
		EmitSignal(SignalName.ObjectiveUpdated, activeQuest.Objective);
		GD.Print($"Current objective: {activeQuest.Objective}");
	}

	public void SetLoop(int loopNumber)
	{
		if (loopNumber < 1)
		{
			GD.PrintErr("QuestManager.SetLoop called with an invalid loop number.");
			return;
		}

		CurrentLoop = loopNumber;
		GD.Print($"Current loop updated: {CurrentLoop}");
	}

	public void AdvanceToNextLoop()
	{
		CurrentLoop += 1;
		GD.Print($"Advanced to loop {CurrentLoop}");
	}

	public void AddQuest(string id, string title, string objective, QuestStatus status = QuestStatus.Locked)
	{
		if (string.IsNullOrWhiteSpace(id))
		{
			GD.PrintErr("QuestManager.AddQuest called with an empty id.");
			return;
		}

		_quests[id] = new QuestData
		{
			Id = id,
			Title = title ?? string.Empty,
			Objective = objective ?? string.Empty,
			Status = status
		};
	}

	public bool HasQuest(string questId)
	{
		return _quests.ContainsKey(questId);
	}

	public QuestData? GetQuest(string questId)
	{
		return _quests.TryGetValue(questId, out QuestData? quest) ? quest : null;
	}

	public bool ActivateQuest(string questId)
	{
		if (!_quests.TryGetValue(questId, out QuestData? quest))
		{
			GD.PrintErr($"Quest '{questId}' was not found.");
			return false;
		}

		quest.Status = QuestStatus.Active;
		GD.Print($"Quest activated: {quest.Title}");
		return true;
	}

	public bool CompleteQuest(string questId)
	{
		if (!_quests.TryGetValue(questId, out QuestData? quest))
		{
			GD.PrintErr($"Quest '{questId}' was not found.");
			return false;
		}

		quest.Status = QuestStatus.Completed;
		GD.Print($"Quest completed: {quest.Title}");
		return true;
	}

	public QuestData? GetActiveQuest()
	{
		foreach (QuestData quest in _quests.Values)
		{
			if (quest.Status == QuestStatus.Active)
			{
				return quest;
			}
		}

		return null;
	}

	public string GetConversationForNpc(string npcName)
	{
		string safeNpcName = npcName?.Trim() ?? string.Empty;

		if (string.IsNullOrEmpty(safeNpcName))
		{
			return string.Empty;
		}

		QuestStep? currentStep = GetCurrentStep();

		if (CurrentLoop == 1 && currentStep != null)
		{
			if (safeNpcName == "Shihab" && currentStep.Id == "open_shrine")
			{
				return "Game Opening -1st loop";
			}

			if (safeNpcName == "Shihab" && currentStep.Id == "return_to_shihab")
			{
				return "Shihab; Return from lake - 1st loop";
			}

			if (safeNpcName == "Esmerelda" && currentStep.Id == "learn_boss")
			{
				return "Esmerelda; Boss blossom intro - 1st loop";
			}
		}

		return CurrentLoop switch
		{
			1 => $"{safeNpcName}; Short convo - 1st loop",
			2 => $"{safeNpcName}; Short convo - 2nd loop",
			_ => $"{safeNpcName}; Short convo - 2nd loop"
		};
	}

	public bool JumpToStep(string stepId)
	{
		if (string.IsNullOrWhiteSpace(stepId))
		{
			GD.PrintErr("QuestManager.JumpToStep called with an empty step id.");
			return false;
		}

		for (int i = 0; i < _steps.Count; i += 1)
		{
			if (_steps[i].Id != stepId)
			{
				continue;
			}

			CurrentStepIndex = i;
			SyncActiveQuestObjective();
			GD.Print($"Jumped to quest step: {stepId}");
			return true;
		}

		GD.PrintErr($"Quest step '{stepId}' was not found.");
		return false;
	}
}
