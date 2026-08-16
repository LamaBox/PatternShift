using Godot;
using System;

public partial class GameUI : Control
{
	[Export] private Label _streakLabel;
	[Export] private Label _timerLabel;
	[Export] private Label _scoreLabel;
	[Export] private Button _pauseButton;
	[Export] private Control _cardsContainer;
	[Export] private Control _deckPile;
	[Export] private Control _setPile;

	private int _score = 0;
	private int _streak = 0;
	private float _time = 300f;
	private bool _isPaused = false;
	private Random _random = new Random();
	private Card _deckTopCard;

	public override void _Ready()
	{
		_pauseButton.Pressed += OnPausePressed;
		UpdateUI();
		
		SetupDeckPile();
		SetupSetPile();
		
		CallDeferred(nameof(GenerateCards));
		GetViewport().SizeChanged += OnViewportResized;
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

	private void SetupDeckPile()
	{
		var cardScene = (PackedScene)GD.Load("res://scenes/game/cards/Card.tscn");
		_deckTopCard = cardScene.Instantiate<Card>();
		_deckTopCard.Setup(Card.ShapeType.Rectangle, Card.ColorType.Blue, Card.FillType.Solid, 1);
		_deckTopCard.SetBaseScale(Vector2.One);
		_deckTopCard.Scale = new Vector2(0.5f, 0.5f);
		_deckTopCard.SetSelected(false);
		_deckTopCard.MouseFilter = MouseFilterEnum.Ignore;
		_deckPile.AddChild(_deckTopCard);
	}

	private void SetupSetPile()
	{
		var cardScene = (PackedScene)GD.Load("res://scenes/game/cards/Card.tscn");
		for (int i = 0; i < 5; i++)
		{
			var card = cardScene.Instantiate<Card>();
			card.Setup(Card.ShapeType.Rectangle, Card.ColorType.Blue, Card.FillType.Solid, 1);
			card.Scale = new Vector2(0.4f, 0.4f);
			card.Rotation = (float)(_random.NextDouble() * 0.2f - 0.1f);
			card.Position = new Vector2((float)(_random.NextDouble() * 10 - 5), (float)(_random.NextDouble() * 10 - 5));
			card.SetSelected(false);
			card.MouseFilter = MouseFilterEnum.Ignore;
			_setPile.AddChild(card);
		}
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

			var shape = (Card.ShapeType)_random.Next(0, 3);
			var color = (Card.ColorType)_random.Next(0, 3);
			var fill = (Card.FillType)_random.Next(0, 3);
			int count = _random.Next(1, 4);

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

		GD.Print($"Generated {_cardsContainer.GetChildCount()} cards");
	}

	private void SpawnCardFromDeck(Vector2 targetPosition)
	{
		var cardScene = (PackedScene)GD.Load("res://scenes/game/cards/Card.tscn");
		var card = cardScene.Instantiate<Card>();

		var shape = (Card.ShapeType)_random.Next(0, 3);
		var color = (Card.ColorType)_random.Next(0, 3);
		var fill = (Card.FillType)_random.Next(0, 3);
		int count = _random.Next(1, 4);

		card.Setup(shape, color, fill, count);
		card.Scale = new Vector2(0.3f, 0.3f);
		card.GlobalPosition = _deckPile.GlobalPosition + new Vector2(20, 20);
		card.SetBaseScale(new Vector2(0.3f, 0.3f));
		card.Rotation = 0;

		_cardsContainer.AddChild(card);

		var tween = card.CreateTween();
		tween.SetParallel(true);

		float finalScale = 0.75f;
		tween.TweenProperty(card, "scale", new Vector2(finalScale, finalScale), 0.4f)
			 .SetEase(Tween.EaseType.Out);
		tween.TweenProperty(card, "global_position", targetPosition, 0.4f)
			 .SetEase(Tween.EaseType.Out);
		tween.TweenProperty(card, "rotation", (float)(_random.NextDouble() * 0.2 - 0.1), 0.4f)
			 .SetEase(Tween.EaseType.Out);

		tween.Finished += () => card.SetBaseScale(card.Scale);
	}

	private void OnViewportResized()
	{
		GenerateCards();
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
