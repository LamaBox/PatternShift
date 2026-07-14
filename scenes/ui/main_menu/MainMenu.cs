using Godot;

public partial class MainMenu : Control
{
	private Button _playButton;
	private Button _settingsButton;
	private Button _achievementsButton;
	private Button _collectionButton;
	private Button _exitButton;

	public override void _Ready()
	{
		_playButton = GetNode<Button>("Panel/play");
		_settingsButton = GetNode<Button>("Panel/settings");
		_achievementsButton = GetNode<Button>("Panel/achievements");
		_collectionButton = GetNode<Button>("Panel/collection");
		_exitButton = GetNode<Button>("Panel/exit");

		_playButton.Pressed += OnPlayPressed;
		_settingsButton.Pressed += OnSettingsPressed;
		_achievementsButton.Pressed += OnAchievementsPressed;
		_collectionButton.Pressed += OnCollectionPressed;
		_exitButton.Pressed += OnExitPressed;
	}

	private void OnPlayPressed()
	{
		GD.Print("Играть");
		//GetNode<SceneManager>("/root/SceneManager").ChangeScene(SceneManager.ModeSelect);
	}

	private void OnSettingsPressed()
	{
		GD.Print("Настройки (пока в разработке)");
		//GetNode<SceneManager>("/root/SceneManager").ChangeScene(SceneManager.Settings);
	}

	private void OnAchievementsPressed()
	{
		GD.Print("Достижения (пока в разработке)");
		//GetNode<SceneManager>("/root/SceneManager").ChangeScene(Achievements.Settings)
	}

	private void OnCollectionPressed()
	{
		GD.Print("Коллекция (пока в разработке)");
		//GetNode<SceneManager>("/root/SceneManager").ChangeScene(Collection.Settings)
	}

	private void OnExitPressed()
	{
		GD.Print("Выход");
		GetNode<SceneManager>("/root/SceneManager").QuitGame();
	}
}
