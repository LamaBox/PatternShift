using Godot;

public partial class MainMenu : Control
{
	[Export] private Button _playButton;
	[Export] private Button _settingsButton;
	[Export] private Button _achievementsButton;
	[Export] private Button _collectionButton;
	[Export] private Button _quitButton;

	public override void _Ready()
	{
		_playButton.Pressed += OnPlayPressed;
		_settingsButton.Pressed += OnSettingsPressed;
		_achievementsButton.Pressed += OnAchievementsPressed;
		_collectionButton.Pressed += OnCollectionPressed;
		_quitButton.Pressed += OnQuitPressed;
	}

	private void OnPlayPressed()
	{
		GetNode<SceneManager>("/root/SceneManager").ChangeScene(SceneManager.ModeSelect);
	}

	private void OnSettingsPressed()
	{
		var settingsScene = (PackedScene)GD.Load("res://scenes/ui/settings/Settings.tscn");
		var settingsInstance = settingsScene.Instantiate<Control>();
		AddChild(settingsInstance);
	}

	private void OnAchievementsPressed() => GD.Print("Achievements (in development)");

	private void OnCollectionPressed() => GD.Print("Collection (in development)");

	private void OnQuitPressed()
	{
		GetNode<SceneManager>("/root/SceneManager").QuitGame();
	}
}
