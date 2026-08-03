using Godot;

public partial class MainMenu : Control
{
	[Export] private Button _playButton;
	[Export] private Button _settingsButton;
	[Export] private Button _achievementsButton;
	[Export] private Button _collectionButton;
	[Export] private Button _quitButton;
	
	private Timer _spawnTimer;
	private Node2D _fallingCardsLayer;

	public override void _Ready()
	{
		_fallingCardsLayer = GetNode<Node2D>("FallingCardsLayer");

		_playButton.Pressed += OnPlayPressed;
		_settingsButton.Pressed += OnSettingsPressed;
		_achievementsButton.Pressed += OnAchievementsPressed;
		_collectionButton.Pressed += OnCollectionPressed;
		_quitButton.Pressed += OnQuitPressed;

		for (int i = 0; i < 8; i++)
		{
			SpawnInitialCard();
		}

		StartCardSpawning();
	}

	private void StartCardSpawning()
	{
		_spawnTimer = new Timer();
		_spawnTimer.WaitTime = 2.5f;
		_spawnTimer.Timeout += SpawnFallingCard;
		AddChild(_spawnTimer);
		_spawnTimer.Start();
	}

	private void SpawnInitialCard()
	{
		var card = CreateCard();
		var screen = GetViewport().GetVisibleRect().Size;

		float scale = GD.Randf() * 1.5f + 1.5f;
		card.Scale = new Vector2(scale, scale);

		float startX = screen.X * 0.5f + GD.Randf() * screen.X * 0.45f;
		float startY = GD.Randf() * screen.Y * 0.8f;
		card.Position = new Vector2(startX, startY);
		card.Rotation = GD.Randf() * Mathf.Pi * 2;

		_fallingCardsLayer.AddChild(card);
		ContinueFalling(card, startX);
	}

	private void SpawnFallingCard()
	{
		var card = CreateCard();
		var screen = GetViewport().GetVisibleRect().Size;

		float scale = GD.Randf() * 1.5f + 1.5f;
		card.Scale = new Vector2(scale, scale);

		float cardHeight = 140 * scale;
		float startX = screen.X * 0.5f + GD.Randf() * screen.X * 0.45f;
		float startY = -cardHeight - GD.Randf() * 200;

		card.Position = new Vector2(startX, startY);
		card.Rotation = GD.Randf() * Mathf.Pi * 2;

		_fallingCardsLayer.AddChild(card);
		ContinueFalling(card, startX);
	}

	private Card CreateCard()
	{
		var cardScene = (PackedScene)GD.Load("res://scenes/game/cards/Card.tscn");
		var card = cardScene.Instantiate<Card>();

		var shape = (Card.ShapeType)GD.RandRange(0, 2);
		var color = (Card.ColorType)GD.RandRange(0, 2);
		var fill = (Card.FillType)GD.RandRange(0, 2);
		int count = (int)GD.RandRange(1, 3);
		card.Setup(shape, color, fill, count);

		float scale = GD.Randf() * 1.5f + 1.5f;
		card.SetScale(scale);

		return card;
	}

	private void ContinueFalling(Card card, float startX)
	{
		var screen = GetViewport().GetVisibleRect().Size;
		var tween = card.CreateTween();
		tween.SetParallel(true);

		float endX = startX + GD.Randf() * 200 - 100;
		float endY = screen.Y + 200 + GD.Randf() * 400;
		float duration = GD.Randf() * 12 + 18;

		tween.TweenProperty(card, "position", new Vector2(endX, endY), duration)
			 .SetEase(Tween.EaseType.In);

		float rotationEnd = card.Rotation + GD.Randf() * 4 - 2;
		tween.TweenProperty(card, "rotation", rotationEnd, duration)
			 .SetEase(Tween.EaseType.Out);

		tween.Finished += () => card.QueueFree();
	}

	private void OnPlayPressed()
	{
		var modeSelectScene = (PackedScene)GD.Load("res://scenes/ui/mode_select/ModeSelect.tscn");
		var modeSelectInstance = modeSelectScene.Instantiate<Control>();
		AddChild(modeSelectInstance);
	}

	private void OnSettingsPressed()
	{
		var settingsScene = (PackedScene)GD.Load("res://scenes/ui/settings/Settings.tscn");
		var settingsInstance = settingsScene.Instantiate<Control>();
		AddChild(settingsInstance);
	}

	private void OnAchievementsPressed() 
	{
		var achievementsScene = (PackedScene)GD.Load("res://scenes/ui/achievements/Achievements.tscn");
		var achievementsInstance = achievementsScene.Instantiate<Control>();
		AddChild(achievementsInstance);
	}

	private void OnCollectionPressed() { }

	private void OnQuitPressed()
	{
		GetNode<SceneManager>("/root/SceneManager").QuitGame();
	}
}
