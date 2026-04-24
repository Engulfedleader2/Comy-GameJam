using Godot;
using System;

public partial class MainMenu : Control
{
	private Button _startButton;
	private Button _optionsButton;
	private Button _quitButton;
	private Control _mainButtons;
	private Control _optionsButtons;
	private Button _backButton;
	private Label _loadingLabel;

	public override void _Ready()
	{
		GetNodeReferences();
		ConnectSignals();
		SetupStartingState();
		CreateLoadingLabel();
	}

	private void GetNodeReferences()
	{
		_mainButtons = GetNodeOrNull<Control>("UI/MainButtons");
		_optionsButtons = GetNodeOrNull<Control>("UI/OptionsButtons");
		_backButton = GetNodeOrNull<Button>("UI/BackButton");

		_startButton = GetNodeOrNull<Button>("UI/MainButtons/StartButton");
		_optionsButton = GetNodeOrNull<Button>("UI/MainButtons/OptionsButton");
		_quitButton = GetNodeOrNull<Button>("UI/MainButtons/QuitButton");
	}

	private void ConnectSignals()
	{
		SetupMenuButton(_startButton, "Start");
		SetupMenuButton(_optionsButton, "Options");
		SetupMenuButton(_quitButton, "Quit");

		if (_backButton != null)
		{
			_backButton.MouseFilter = MouseFilterEnum.Stop;
			DisableMouseInputForChildren(_backButton);
			_backButton.MouseEntered += () => OnButtonMouseEntered(_backButton);
			_backButton.MouseExited += () => OnButtonMouseExited(_backButton);
			_backButton.FocusEntered += () => OnButtonMouseEntered(_backButton);
			_backButton.FocusExited += () => OnButtonMouseExited(_backButton);
			_backButton.Pressed += OnBackPressed;
		}
	}

	private void SetupStartingState()
	{
		if (_mainButtons != null)
			_mainButtons.Visible = true;

		if (_optionsButtons != null)
			_optionsButtons.Visible = false;

		if (_backButton != null)
			_backButton.Visible = false;

		SetButtonVisualState(_startButton, false);
		SetButtonVisualState(_optionsButton, false);
		SetButtonVisualState(_quitButton, false);
		SetButtonVisualState(_backButton, false);
	}

	private void SetupMenuButton(Button button, string buttonName)
	{
		if (button == null)
			return;

		button.MouseFilter = MouseFilterEnum.Stop;
		button.ActionMode = BaseButton.ActionModeEnum.Press;
		button.FocusMode = FocusModeEnum.All;
		DisableMouseInputForChildren(button);

		button.MouseEntered += () => OnButtonMouseEntered(button);
		button.MouseExited += () => OnButtonMouseExited(button);

		button.FocusEntered += () => OnButtonMouseEntered(button);
		button.FocusExited += () => OnButtonMouseExited(button);

		button.Pressed += () => OnMenuButtonPressed(buttonName);
	}

	private void OnButtonMouseEntered(Button button)
	{
		SetButtonVisualState(button, true);
	}

	private void OnButtonMouseExited(Button button)
	{
		SetButtonVisualState(button, false);
	}

	private void SetButtonVisualState(Button button, bool showHoverRow)
	{
		if (button == null)
			return;

		var normalRow = button.GetNodeOrNull<Control>("NormalRow");
		var hoverRow = button.GetNodeOrNull<Control>("HoverRow");

		if (normalRow != null)
			normalRow.Visible = !showHoverRow;

		if (hoverRow != null)
			hoverRow.Visible = showHoverRow;
	}

	private void DisableMouseInputForChildren(Node node)
	{
		foreach (Node child in node.GetChildren())
		{
			if (child is Control control)
				control.MouseFilter = MouseFilterEnum.Ignore;

			DisableMouseInputForChildren(child);
		}
	}

	private void OnMenuButtonPressed(string buttonName)
	{
		GD.Print($"{buttonName} button was clicked");

		switch (buttonName)
		{
			case "Start":
				StartGame();
				break;
			case "Options":
				ShowOptionsMenu();
				break;
			case "Quit":
				GetTree().Quit();
				break;
		}
	}
	
	private async void StartGame()
	{
		if (_startButton != null)
		{
			_startButton.Disabled = true;
			_startButton.MouseFilter = MouseFilterEnum.Ignore;
		}

		if (_mainButtons != null)
		{
			_mainButtons.Visible = false;
		}

		if (_optionsButtons != null)
		{
			_optionsButtons.Visible = false;
		}

		if (_backButton != null)
		{
			_backButton.Visible = false;
		}

		if (_loadingLabel != null)
		{
			_loadingLabel.Visible = true;
			_loadingLabel.MoveToFront();
		}

		GD.Print("Start button accepted. Loading game scene...");

		await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
		await ToSignal(GetTree().CreateTimer(0.35), SceneTreeTimer.SignalName.Timeout);

		GetTree().ChangeSceneToFile("res://Scenes/World.tscn");
	}

	private void ShowOptionsMenu()
	{
		if (_mainButtons != null)
			_mainButtons.Visible = false;

		if (_optionsButtons != null)
			_optionsButtons.Visible = true;

		if (_backButton != null)
			_backButton.Visible = true;
	}

	private void OnBackPressed()
	{
		GD.Print("Back button was clicked");

		if (_mainButtons != null)
			_mainButtons.Visible = true;

		if (_optionsButtons != null)
			_optionsButtons.Visible = false;

		if (_backButton != null)
			_backButton.Visible = false;
	}

	private void CreateLoadingLabel()
	{
		_loadingLabel = new Label
		{
			Name = "LoadingLabel",
			Text = "Entering the world...",
			Visible = false,
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center,
			MouseFilter = MouseFilterEnum.Ignore,
			ZIndex = 100
		};

		_loadingLabel.SetAnchorsPreset(LayoutPreset.FullRect);
		_loadingLabel.OffsetLeft = 0;
		_loadingLabel.OffsetTop = 0;
		_loadingLabel.OffsetRight = 0;
		_loadingLabel.OffsetBottom = 0;
		_loadingLabel.AddThemeFontSizeOverride("font_size", 48);
		_loadingLabel.AddThemeColorOverride("font_color", new Color(0.95f, 0.9f, 0.78f, 1f));
		_loadingLabel.AddThemeColorOverride("font_outline_color", Colors.Black);
		_loadingLabel.AddThemeConstantOverride("outline_size", 4);

		AddChild(_loadingLabel);
	}
}
