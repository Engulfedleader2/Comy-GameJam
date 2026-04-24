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

	public override void _Ready()
	{
		GetNodeReferences();
		ConnectSignals();
		SetupStartingState();
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
	
	private void StartGame()
	{
		GD.Print("Starting Game");
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
}
