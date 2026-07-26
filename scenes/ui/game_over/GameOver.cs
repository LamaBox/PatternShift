using Godot;

public partial class GameOver : Control
{
	[Export] private Label _scoreLabel;
	[Export] private Label _streakLabel;
	[Export] private Label _patternsLabel;
	[Export] private Label _recordLabel1;
	[Export] private Label _recordLabel2;
	[Export] private Label _recordLabel3;
	[Export] private Button _quitButton;

	public override void _Ready()
	{
		_quitButton.Pressed += OnQuitPressed;
	}

	public void SetData(int score, int streak, int patterns, bool isNewRecord)
	{
		_scoreLabel.Text = score.ToString("D4");
		_streakLabel.Text = $"x{streak}";
		_patternsLabel.Text = patterns.ToString();

		string recordText = isNewRecord ? "Новый рекорд!" : "";
		
		_recordLabel1.Text = recordText;
		_recordLabel2.Text = recordText;
		_recordLabel3.Text = recordText;
	}

	private void OnQuitPressed()
	{
		GetNode<SceneManager>("/root/SceneManager").GoToMainMenu();
	}
}
