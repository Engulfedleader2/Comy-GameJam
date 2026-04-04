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
		
		if (_startButton == null || _optionsButton == null || _quitButton == null)
		{
			GD.PushError("MainMenu: missing one or more required button nodes.");
			return;
		}
		
		// Connect button press events to handlers
		_startButton.Pressed += OnStartPressed;
		_optionsButton.Pressed += OnOptionsPressed;
		_quitButton.Pressed += OnQuitPressed;
	}
	
	/// <summary>
	/// Called when the Start Game button is pressed.
	/// This will eventually load the main game scene.
	/// </summary>
	private void OnStartPressed()
	{
		GD.Print("Play pressed"); 
		//TODO: setup goTO new Scene
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
