using Godot;

public partial class PausePopup : Control
{
	[Export] private Button _resumeButton;
	[Export] private Button _settingsButton;
	[Export] private Button _quitButton;

	public override void _Ready()
	{
		_resumeButton.Pressed += OnResumePressed;
		_settingsButton.Pressed += OnSettingsPressed;
		_quitButton.Pressed += OnQuitPressed;
	}

	private void OnResumePressed()
	{
		GameUI._isPaused = false;
		QueueFree();
	}

	private void OnSettingsPressed()
	{
		var settingsScene = (PackedScene)GD.Load("res://scenes/ui/settings/Settings.tscn");
		var settingsInstance = settingsScene.Instantiate<Control>();
		AddChild(settingsInstance);
	}

	private void OnQuitPressed()
	{
		GameUI._isPaused = false;
		GameUI._isntTimeMode = true;
		GetNode<SceneManager>("/root/SceneManager").GoToMainMenu();
	}
}
