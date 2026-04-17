using Godot;
using System;

public partial class MainMenu : Control
{
	// Base path to menu buttons
	private const string MenuPath = "CenterContainer/VBoxContainer/";
	
	// References to menu buttons in the scene
	private Button _startButton;
	private Button _optionsButton;
	private Button _quitButton;
	
	public override void _Ready()
	{
		// Get button nodes from MainMenu nodes (UI hierachy)
		_startButton = GetNodeOrNull<Button>(MenuPath + "StartButton");
		_optionsButton = GetNodeOrNull<Button>(MenuPath + "OptionsButton");
		_quitButton = GetNodeOrNull<Button>(MenuPath + "QuitButton");

		// Connect button press events to handlers
		if (_startButton != null)
		{
			_startButton.Pressed += OnStartPressed;
		}
		else
		{
			GD.PushWarning("MainMenu: StartButton node not found at path: " + MenuPath + "StartButton");
		}

		if (_optionsButton != null)
		{
			_optionsButton.Pressed += OnOptionsPressed;
		}
		else
		{
			GD.PushWarning("MainMenu: OptionsButton node not found at path: " + MenuPath + "OptionsButton");
		}

		if (_quitButton != null)
		{
			_quitButton.Pressed += OnQuitPressed;
		}
		else
		{
			GD.PushWarning("MainMenu: QuitButton node not found at path: " + MenuPath + "QuitButton");
		}
	}
	
	/// <summary>
	/// Called when the Start Game button is pressed.
	/// This will eventually load the main game scene.
	/// </summary>
	private void OnStartPressed()
	{
		GD.Print("Play pressed"); 
		GetTree().ChangeSceneToFile("res://Scenes/Main.tscn");
	}

   	/// <summary>
	/// Called when the Options button is pressed.
	/// This will eventually open the options menu.
	/// </summary>
	private void OnOptionsPressed()
	{
		GD.Print("Options pressed");
	}
	
	/// <summary>
	/// Called when the Quit button is pressed.
	/// Exits the game application.
	/// </summary>
	private void OnQuitPressed()
	{
		GetTree().Quit();
	}
}
