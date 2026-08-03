using Godot;

public partial class PausePopup : Control
{
	[Signal] public delegate void CompleteGameEventHandler();

	[Export] private Button _resumeButton;
	[Export] private Button _settingsButton;
	[Export] private Button _quitButton;
	[Export] private Button _completeButton;

	public override void _Ready()
	{
		_resumeButton.Pressed += OnResumePressed;
		_settingsButton.Pressed += OnSettingsPressed;
		_quitButton.Pressed += OnQuitPressed;
		_completeButton.Pressed += OnCompletePressed;
	}

	private void OnResumePressed()
	{
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
		GetNode<SceneManager>("/root/SceneManager").GoToMainMenu();
	}
	
	private void OnCompletePressed()
	{
		EmitSignal(SignalName.CompleteGame);
	}
}
