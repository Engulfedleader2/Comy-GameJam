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

	private Button _hoveredButton;
	private float _hoverFlashTimer = 0f;
	private bool _showHoverState = true;

	private const float HoverFlashInterval = 0.2f;

	public override void _Ready()
	{
		GetNodeReferences();
		ConnectSignals();
		SetupStartingState();
	}

	public override void _Process(double delta)
	{
		if (_hoveredButton == null)
			return;

		_hoverFlashTimer += (float)delta;

		if (_hoverFlashTimer >= HoverFlashInterval)
		{
			_hoverFlashTimer = 0f;
			_showHoverState = !_showHoverState;
			SetButtonVisualState(_hoveredButton, _showHoverState);
		}
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
			_backButton.Pressed += OnBackPressed;
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
	}

	private void SetupMenuButton(Button button, string buttonName)
	{
		if (button == null)
			return;

		button.MouseEntered += () => OnButtonMouseEntered(button);
		button.MouseExited += () => OnButtonMouseExited(button);

		button.FocusEntered += () => OnButtonMouseEntered(button);
		button.FocusExited += () => OnButtonMouseExited(button);

		button.Pressed += () => OnMenuButtonPressed(buttonName);
	}

	private void OnButtonMouseEntered(Button button)
	{
		_hoveredButton = button;
		_hoverFlashTimer = 0f;
		_showHoverState = true;
		SetButtonVisualState(button, true);
	}

	private void OnButtonMouseExited(Button button)
	{
		if (_hoveredButton == button)
			_hoveredButton = null;

		_hoverFlashTimer = 0f;
		_showHoverState = true;
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
		}
	}
	
	private void StartGame()
	{
		GD.Print("Starting Game");
		GetTree().ChangeSceneToFile("res://Scenes/Player.tscn");
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
}
