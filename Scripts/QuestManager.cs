using Godot;
using System;
using System.Collections.Generic;

public partial class QuestManager : Node
{
	public static QuestManager Instance { get; private set; }

	public enum QuestStatus
	{
		Locked,
		Active,
		Completed
	}

	public class QuestData
	{
		public string Id { get; set; } = string.Empty;
		public string Title { get; set; } = string.Empty;
		public string Objective { get; set; } = string.Empty;
		public QuestStatus Status { get; set; } = QuestStatus.Locked;
	}

	public int CurrentLoop { get; private set; } = 1;
	public string CurrentStage { get; private set; } = "game_opening";

	private readonly Dictionary<string, QuestData> _quests = new();

public override void _Ready()
{
	Instance = this;
	GD.Print("QuestManager loaded and ready.");
	SeedStartingQuests();
}

	private void SeedStartingQuests()
	{
		_quests.Clear();

		_quests["main_loop_1"] = new QuestData
		{
			Id = "main_loop_1",
			Title = "Spring Begins",
			Objective = "Talk to the villagers of Bloom.",
			Status = QuestStatus.Active
		};
	}

	public void SetStage(string stage)
	{
		if (string.IsNullOrWhiteSpace(stage))
		{
			GD.PrintErr("QuestManager.SetStage called with an empty stage.");
			return;
		}

		CurrentStage = stage;
		GD.Print($"Quest stage updated: {CurrentStage}");
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

	public bool UpdateObjective(string questId, string newObjective)
	{
		if (!_quests.TryGetValue(questId, out QuestData? quest))
		{
			GD.PrintErr($"Quest '{questId}' was not found.");
			return false;
		}

		quest.Objective = newObjective ?? string.Empty;
		GD.Print($"Quest objective updated: {quest.Objective}");
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

	public bool UpdateActiveQuestObjective(string newObjective)
	{
		QuestData? activeQuest = GetActiveQuest();
		if (activeQuest == null)
		{
			GD.PrintErr("There is no active quest to update.");
			return false;
		}

		activeQuest.Objective = newObjective ?? string.Empty;
		GD.Print($"Active quest objective updated: {activeQuest.Objective}");
		return true;
	}

	public string GetConversationForNpc(string npcName)
	{
		string safeNpcName = npcName?.Trim() ?? string.Empty;

		if (string.IsNullOrEmpty(safeNpcName))
		{
			return string.Empty;
		}

		if (CurrentLoop == 1 && CurrentStage == "game_opening" && safeNpcName == "Shihab")
		{
			return "Game Opening -1st loop";
		}

		return CurrentLoop switch
		{
			1 => $"{safeNpcName}; Short convo - 1st loop",
			2 => $"{safeNpcName}; Short convo - 2nd loop",
			_ => $"{safeNpcName}; Short convo - 2nd loop"
		};
	}
}
