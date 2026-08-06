using Godot;
using System;

public partial class GameUI : Control
{
	[Export] private GameController gameController;

	[Export] private Label _streakLabel;
	[Export] private Label _timerLabel;
	[Export] private Label _scoreLabel;
	[Export] private Button _pauseButton;
	[Export] private Control _cardsContainer;

	private int _score = 0;
	private int _streak = 0;
	private float _time = 300f;
	private bool _isPaused = false;
	private bool _isGameOver = false;
	private Random _random = new Random();

	public override void _Ready()
	{
		_pauseButton.Pressed += OnPausePressed;
		UpdateUI();
		CallDeferred(nameof(GenerateCards));
		GetViewport().SizeChanged += OnViewportResized;
	}

	public override void _Process(double delta)
	{
		if (_isPaused || _isGameOver)
			return;

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

		foreach (Node child in _cardsContainer.GetChildren())
		{
			child.QueueFree();
		}

		var screen = GetViewport().GetVisibleRect().Size;

		float baseCardWidth = 100f;
		float baseCardHeight = 140f;

		float scaleX = (screen.X / 4.5f) / baseCardWidth;
		float scaleY = (screen.Y / 3.5f) / baseCardHeight;
		float cardScale = Mathf.Min(scaleX, scaleY) * 0.75f;

		float startX = screen.X * -0.17f;
		float startY = screen.Y * -0.29f;

		for (int i = 0; i < 12; i++)
		{
			var card = cardScene.Instantiate<Card>();

			var figure = (CardFigure)_random.Next(0, 3);
			var color = (CardColor)_random.Next(0, 3);
			var fill = (CardFill)_random.Next(0, 3);
			var count = (CardCount)_random.Next(1, 4);

			card.Setup(shape, color, fill, count);
			card.SetScale(1.0f);

			card.Scale = new Vector2(cardScale, cardScale);
			card.SetBaseScale(card.Scale);

			int row = i / 4;
			int col = i % 4;
			float spacing = 8 * cardScale;
			float x = startX + col * (baseCardWidth * cardScale + spacing);
			float y = startY + row * (baseCardHeight * cardScale + spacing);

			card.Position = new Vector2(x, y);

			_cardsContainer.AddChild(card);
		}
			CardData data = new CardData(figure, color, fill, count);
			card.Setup(data);

			card.SetScale(1.2f);

		GD.Print($"Generated {_cardsContainer.GetChildCount()} cards");
	}

	private void OnViewportResized()
	{
		GenerateCards();
	}

	private void OnPausePressed()
	{
		var pauseScene = (PackedScene)GD.Load("res://scenes/ui/pause/PausePopup.tscn");
		var pauseInstance = pauseScene.Instantiate<PausePopup>();

		pauseInstance.CompleteGame += () =>
		{
			pauseInstance.QueueFree();
			EndGame();
		};

		AddChild(pauseInstance);
	}

	private void EndGame()
	{
		if (_isGameOver)
			return;

		_isGameOver = true;

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
