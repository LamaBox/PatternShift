using Godot;
using System;

public partial class GameUI : Control
{
	[Export] private Label _streakLabel;
	[Export] private Label _timerLabel;
	[Export] private Label _scoreLabel;
	[Export] private Button _pauseButton;
	[Export] private GridContainer _gridContainer;

	private int _score = 0;
	private int _streak = 0;
	private float _time = 300f;
	private bool _isPaused = false;
	private Random _random = new Random();

	public override void _Ready()
	{
		_pauseButton.Pressed += OnPausePressed;
		UpdateUI();
		GenerateCards();
	}

	public override void _Process(double delta)
	{
		if (_isPaused) return;

		_time -= (float)delta;
		if (_time <= 0)
		{
			_time = 0;
			EndGame();
		}
		UpdateUI();
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventKey key && key.Pressed && key.Keycode == Key.Escape)
		{
			EndGame();
		}
	}

	private void UpdateUI()
	{
		_streakLabel.Text = $"x{_streak}";
		_scoreLabel.Text = $"{_score:D4}";
		_timerLabel.Text = $"{Mathf.Floor(_time / 60):00}:{Mathf.Floor(_time % 60):00}";
	}

	private void GenerateCards()
	{
		var cardScene = (PackedScene)GD.Load("res://scenes/game/cards/Card.tscn");

		foreach (Node child in _gridContainer.GetChildren())
		{
			child.QueueFree();
		}

		for (int i = 0; i < 12; i++)
		{
			var card = cardScene.Instantiate<Card>();

			var shape = (Card.ShapeType)_random.Next(0, 3);
			var color = (Card.ColorType)_random.Next(0, 3);
			var fill = (Card.FillType)_random.Next(0, 3);
			int count = _random.Next(1, 4);

			card.Setup(shape, color, fill, count);
			
			card.SetScale(1.2f);

			_gridContainer.AddChild(card);
		}
	}

	private void OnPausePressed()
	{
		var pauseScene = (PackedScene)GD.Load("res://scenes/ui/pause/PausePopup.tscn");
		var pauseInstance = pauseScene.Instantiate<Control>();
		AddChild(pauseInstance);
	}

	private void EndGame()
	{
		var gameOverScene = (PackedScene)GD.Load("res://scenes/ui/game_over/GameOver.tscn");
		var gameOverInstance = gameOverScene.Instantiate<GameOver>();
		
		gameOverInstance.SetData(_score, _streak, 0, _score > 0);
		
		AddChild(gameOverInstance);

		var save = SaveController.LoadGameData();
		if (save == null) save = new GameSaveData();

		save.TotalGames++;
		save.BestScore = Math.Max(save.BestScore, _score);
		save.BestStreak = Math.Max(save.BestStreak, _streak);

		SaveController.SaveGameData(save);
	}

	public void AddScore(int points)
	{
		_score += points;
		UpdateUI();
	}

	public void SetStreak(int value)
	{
		_streak = value;
		UpdateUI();
	}

	public void ResetTimer()
	{
		_time = 300f;
		UpdateUI();
	}
}
