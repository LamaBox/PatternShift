using Godot;

public partial class ModeSelect : Control
{
	[Export] private Button _classicButton;
	[Export] private Button _standardButton;
	[Export] private Button _timeTrialButton;
	[Export] private Button _dailyButton;
	[Export] private Button _endlessButton;

	[Export] private Label _classicDesc;
	[Export] private Label _standardDesc;
	[Export] private Label _timeTrialDesc;
	[Export] private Label _dailyDesc;
	[Export] private Label _endlessDesc;

	[Export] private Button _startButton;
	[Export] private Button _backButton;

	private string _selectedMode = "classic";

	public override void _Ready()
	{
		_classicButton.Pressed += () => SelectMode("classic");
		_standardButton.Pressed += () => SelectMode("standard");
		_timeTrialButton.Pressed += () => SelectMode("time_trial");
		_dailyButton.Pressed += () => SelectMode("daily");
		_endlessButton.Pressed += () => SelectMode("endless");

		_backButton.Pressed += OnBackPressed;
		_startButton.Pressed += OnStartPressed;

		SelectMode("classic");
	}

	private void SelectMode(string mode)
	{
		_selectedMode = mode;
		ResetDescriptions();

		switch (mode)
		{
			case "classic":
				_classicDesc.Modulate = Colors.White;
				break;
			case "standard":
				_standardDesc.Modulate = Colors.White;
				break;
			case "time_trial":
				_timeTrialDesc.Modulate = Colors.White;
				break;
			case "daily":
				_dailyDesc.Modulate = Colors.White;
				break;
			case "endless":
				_endlessDesc.Modulate = Colors.White;
				break;
		}
	}

	private void ResetDescriptions()
	{
		_classicDesc.Modulate = new Color(1, 1, 1, 0.5f);
		_standardDesc.Modulate = new Color(1, 1, 1, 0.5f);
		_timeTrialDesc.Modulate = new Color(1, 1, 1, 0.5f);
		_dailyDesc.Modulate = new Color(1, 1, 1, 0.5f);
		_endlessDesc.Modulate = new Color(1, 1, 1, 0.5f);
	}

	private void OnBackPressed()
	{
		QueueFree();
	}

	private void OnStartPressed()
	{
		GetNode<SceneManager>("/root/SceneManager").ChangeScene(SceneManager.GameUI);
	}
}
