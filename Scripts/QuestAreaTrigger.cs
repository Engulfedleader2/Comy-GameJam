using Godot;
using System;

public partial class QuestAreaTrigger : Area2D
{
	[Export] public string AreaId { get; set; } = string.Empty;
	[Export] public bool TriggerOnce { get; set; } = true;
	[Export] public bool RequirePlayerGroup { get; set; } = false;
	[Export] public string TutorialConversationKey { get; set; } = string.Empty;
	[Export] public string CalmBlossomType { get; set; } = string.Empty;
	[Export] public string TameBlossomType { get; set; } = string.Empty;
	[Export] public string BossId { get; set; } = string.Empty;
	[Export] public string ObjectInteractionId { get; set; } = string.Empty;
	[Export] public string EncounterConversationKey { get; set; } = string.Empty;
	[Export] public bool AutoResolveQuestProgress { get; set; } = true;
	[Export] public string RequiredStepId { get; set; } = string.Empty;

	private bool _hasTriggered = false;
	[Signal] public delegate void EncounterTriggeredEventHandler(string calmBlossomType, string tameBlossomType, string bossId, string encounterConversationKey);

	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
	}

	private void OnBodyEntered(Node body)
	{
		if (_hasTriggered && TriggerOnce)
		{
			return;
		}

		if (!IsValidPlayerBody(body))
		{
			return;
		}

		if (!string.IsNullOrWhiteSpace(RequiredStepId))
		{
			QuestManager? questManager = QuestManager.Instance;
			if (questManager == null)
			{
				GD.PrintErr("QuestManager was not available for QuestAreaTrigger step validation.");
				return;
			}

			if (!questManager.IsCurrentStep(RequiredStepId))
			{
				GD.Print($"QuestAreaTrigger ignored because current step is not '{RequiredStepId}'.");
				return;
			}
		}

		if (!string.IsNullOrWhiteSpace(AreaId))
		{
			QuestManager.Instance?.ReportAreaEntered(AreaId);
			GD.Print($"Quest area entered: {AreaId}");
		}

		if (!string.IsNullOrWhiteSpace(ObjectInteractionId) && AutoResolveQuestProgress)
		{
			QuestManager.Instance?.ReportObjectInteracted(ObjectInteractionId);
			GD.Print($"Quest object interaction reported: {ObjectInteractionId}");
		}

		bool hasEncounterData =
			!string.IsNullOrWhiteSpace(CalmBlossomType) ||
			!string.IsNullOrWhiteSpace(TameBlossomType) ||
			!string.IsNullOrWhiteSpace(BossId) ||
			!string.IsNullOrWhiteSpace(ObjectInteractionId) ||
			!string.IsNullOrWhiteSpace(EncounterConversationKey);

		if (hasEncounterData)
		{
			EmitSignal(
				SignalName.EncounterTriggered,
				CalmBlossomType,
				TameBlossomType,
				BossId,
				EncounterConversationKey
			);
			GD.Print(
				$"Encounter triggered. Calm='{CalmBlossomType}' Tame='{TameBlossomType}' Boss='{BossId}' Conversation='{EncounterConversationKey}'"
			);
		}

		if (AutoResolveQuestProgress)
		{
			if (!string.IsNullOrWhiteSpace(CalmBlossomType))
			{
				QuestManager.Instance?.ReportBlossomCalmed(CalmBlossomType);
				GD.Print($"Fake blossom calm reported: {CalmBlossomType}");
			}

			if (!string.IsNullOrWhiteSpace(TameBlossomType))
			{
				QuestManager.Instance?.ReportBlossomTamed(TameBlossomType);
				GD.Print($"Fake blossom tame reported: {TameBlossomType}");
			}

			if (!string.IsNullOrWhiteSpace(BossId))
			{
				QuestManager.Instance?.ReportBossDefeated(BossId);
				GD.Print($"Fake boss defeat reported: {BossId}");
			}
		}

		if (!string.IsNullOrWhiteSpace(TutorialConversationKey) && HasNode("/root/DialogueManager"))
		{
			Node dialogueManager = GetNode("/root/DialogueManager");
			dialogueManager.CallDeferred(
				"start_dialogue",
				"Keya",
				new Godot.Collections.Array(),
				new Godot.Collections.Array<bool>(),
				TutorialConversationKey
			);
		}

		_hasTriggered = true;
	}

	private bool IsValidPlayerBody(Node body)
	{
		if (body == null)
		{
			return false;
		}

		if (RequirePlayerGroup)
		{
			return body.IsInGroup("player");
		}

		return body is CharacterBody2D || body.Name == "Player";
	}
}
