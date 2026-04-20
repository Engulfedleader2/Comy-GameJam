using Godot;
using System;

public partial class BlossomEncounterTrigger : Area2D
{
	[Export] public string RequiredStepId { get; set; } = string.Empty;
	[Export] public string CalmBlossomType { get; set; } = string.Empty;
	[Export] public string TameBlossomType { get; set; } = string.Empty;
	[Export] public string BossId { get; set; } = string.Empty;
	[Export] public string CompletionMessage { get; set; } = string.Empty;
	[Export] public string CompletionPopupScenePath { get; set; } = "res://Scenes/BlossomScenes/BossTamedPopup.tscn";
	[Export] public double CompletionPopupDuration { get; set; } = 2.0;
	[Export] public string NextScenePath { get; set; } = string.Empty;
	[Export] public bool HideUntilRequiredStepActive { get; set; } = false;
	[Export] public bool TriggerOnce { get; set; } = true;
	[Export] public string InteractionAction { get; set; } = "Interact";

	private bool _playerInRange = false;
	private bool _hasTriggered = false;
	private Label? _interactionLabel;
	private Node2D? _encounterRoot;

	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
		BodyExited += OnBodyExited;
		_interactionLabel = GetNodeOrNull<Label>("../InteractionLabel");
		_encounterRoot = GetParentOrNull<Node2D>();
		SetInteractionLabelVisible(false);
		UpdateEncounterVisibility();
	}

	public override void _Process(double delta)
	{
		UpdateEncounterVisibility();
	}

	public override void _Input(InputEvent @event)
	{
		if (!_playerInRange)
		{
			return;
		}

		if (_hasTriggered && TriggerOnce)
		{
			return;
		}

		if (@event.IsActionPressed(InteractionAction))
		{
			GD.Print($"Blossom interaction pressed for step '{RequiredStepId}'.");
			TriggerEncounter();
			GetViewport().SetInputAsHandled();
		}
	}

	private void OnBodyEntered(Node body)
	{
		if (!IsValidPlayerBody(body))
		{
			return;
		}

		if (!IsRequiredStepActive())
		{
			return;
		}

		_playerInRange = true;
		SetInteractionLabelVisible(true);
	}

	private void OnBodyExited(Node body)
	{
		if (!IsValidPlayerBody(body))
		{
			return;
		}

		_playerInRange = false;
		SetInteractionLabelVisible(false);
	}

	private void TriggerEncounter()
	{
		if (!IsRequiredStepActive())
		{
			return;
		}

		QuestManager? questManager = QuestManager.Instance;
		if (questManager == null)
		{
			GD.PrintErr("QuestManager was not available for BlossomEncounterTrigger.");
			return;
		}

		if (!string.IsNullOrWhiteSpace(CalmBlossomType))
		{
			questManager.ReportBlossomCalmed(CalmBlossomType);
			GD.Print($"Blossom encounter calm reported: {CalmBlossomType}");
		}

		if (!string.IsNullOrWhiteSpace(TameBlossomType))
		{
			questManager.ReportBlossomTamed(TameBlossomType);
			GD.Print($"Blossom encounter tame reported: {TameBlossomType}");
		}

		if (!string.IsNullOrWhiteSpace(BossId))
		{
			questManager.ReportBossDefeated(BossId);
			GD.Print($"Blossom encounter boss defeat reported: {BossId}");
		}

		if (!string.IsNullOrWhiteSpace(CompletionMessage))
		{
			ShowCompletionPopup();
		}

		if (!string.IsNullOrWhiteSpace(NextScenePath))
		{
			Error changeSceneResult = GetTree().ChangeSceneToFile(NextScenePath);
			if (changeSceneResult != Error.Ok)
			{
				GD.PrintErr($"Failed to load blossom encounter scene at '{NextScenePath}'. Error: {changeSceneResult}");
				return;
			}
		}

		_hasTriggered = true;
		_playerInRange = false;
		SetInteractionLabelVisible(false);
	}

	private void UpdateEncounterVisibility()
	{
		if (!HideUntilRequiredStepActive || _encounterRoot == null)
		{
			return;
		}

		bool isVisible = IsRequiredStepActive();
		_encounterRoot.Visible = isVisible;
		Monitoring = isVisible;
		Monitorable = isVisible;

		if (!isVisible)
		{
			_playerInRange = false;
			SetInteractionLabelVisible(false);
		}
	}

	private void ShowCompletionPopup()
	{
		if (string.IsNullOrWhiteSpace(CompletionPopupScenePath))
		{
			GD.PrintErr("CompletionPopupScenePath was empty for BlossomEncounterTrigger.");
			return;
		}

		PackedScene? popupScene = GD.Load<PackedScene>(CompletionPopupScenePath);
		if (popupScene == null)
		{
			GD.PrintErr($"Failed to load completion popup scene at '{CompletionPopupScenePath}'.");
			return;
		}

		CanvasLayer? popupInstance = popupScene.Instantiate<CanvasLayer>();
		if (popupInstance == null)
		{
			GD.PrintErr("Failed to instantiate completion popup scene as a CanvasLayer.");
			return;
		}

		popupInstance.Name = "BossCompletionPopup";
		GetTree().Root.AddChild(popupInstance);

		Timer timer = new Timer();
		timer.WaitTime = CompletionPopupDuration;
		timer.OneShot = true;
		popupInstance.AddChild(timer);
		timer.Timeout += () =>
		{
			if (IsInstanceValid(popupInstance))
			{
				popupInstance.QueueFree();
			}
		};
		timer.Start();
	}

	private bool IsRequiredStepActive()
	{
		if (string.IsNullOrWhiteSpace(RequiredStepId))
		{
			return true;
		}

		QuestManager? questManager = QuestManager.Instance;
		if (questManager == null)
		{
			GD.PrintErr("QuestManager was not available when checking blossom encounter step.");
			return false;
		}

		return questManager.IsCurrentStep(RequiredStepId);
	}

	private bool IsValidPlayerBody(Node body)
	{
		return body is CharacterBody2D || body.Name == "Player" || body.IsInGroup("player");
	}

	private void SetInteractionLabelVisible(bool isVisible)
	{
		if (_interactionLabel == null)
		{
			return;
		}

		_interactionLabel.Visible = isVisible;
	}
}
