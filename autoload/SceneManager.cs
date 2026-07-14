using Godot;

public partial class SceneManager : Node
{
	public const string MainMenu = "res://scenes/ui/main_menu/MainMenu.tscn";
	public const string ModeSelect = "res://scenes/ui/mode_select/ModeSelect.tscn";
	public const string Settings = "res://scenes/ui/settings/Settings.tscn";
	public const string GameUI = "res://scenes/ui/game_ui/GameUI.tscn";
	public const string GameOver = "res://scenes/ui/game_over/GameOver.tscn";

	private Node _currentScene;

	public override void _Ready()
	{
		GD.Print("SceneManager загружен");
		var root = GetTree().Root;
		_currentScene = root.GetChild(root.GetChildCount() - 1);
	}

	public void ChangeScene(string scenePath)
	{
		var newScene = (PackedScene)GD.Load(scenePath);
		var newSceneInstance = newScene.Instantiate();
		GetTree().Root.AddChild(newSceneInstance);

		if (_currentScene != null)
		{
			GetTree().Root.RemoveChild(_currentScene);
			_currentScene.QueueFree();
		}

		_currentScene = newSceneInstance;
	}

	public void QuitGame()
	{
		GetTree().Quit();
	}

	public void GoToMainMenu()
	{
		ChangeScene(MainMenu);
	}
}
