using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Drawing;
using static Godot.WebSocketPeer;

public partial class GameUI : Control
{
	[Export] private GameController gameController;

	[Export] private Label _streakLabel;
	[Export] private Label _timerLabel;
	[Export] private Label _scoreLabel;
	[Export] private Button _pauseButton;
	[Export] private Control _cardsContainer;
	private static AudioStreamPlayer sound1;
	private static AudioStreamPlayer sound2;
	private static Control _gameui;

	private float _time = 300f;
	public static bool _isPaused = false;
	private bool _isGameOver = false;
	private Random _random = new Random();
	private static List<int[]> TheDeck = new();
	private static List<int[]> Discard = new();
	public static int CardClickedCounter = 0;
	private static List<Card> CardsField = new();
	private static bool IsAddCards = false;

	public override void _Ready()
	{
		sound1 = GetNode<AudioStreamPlayer>("SoundsPlayer1");
		sound2 = GetNode<AudioStreamPlayer>("SoundsPlayer2");
		_cardsContainer = GetNode<Control>("Background/VBoxContainer/MarginContainer/HBoxContainer/GridOfCards/CardsContainer");
		_gameui = GetNode<Control>(".");
		_pauseButton.Pressed += OnPausePressed;

		gameController.GameScore.ValueChanged += OnScoreChanged;
		gameController.GameStreak.ValueChanged += OnStreakChanged;

		UpdateUI();
		InitializeDeck();
		CallDeferred(nameof(GenerateCards));
		GetViewport().SizeChanged += OnViewportResized;
		var screen = GetViewport().GetVisibleRect().Size;
		
	}

	private static void PlaySound1(string s) { sound1.Stream = ResourceLoader.Load<AudioStream>(s); sound1.Play(); }
	private static void PlaySound2(string s) { sound2.Stream = ResourceLoader.Load<AudioStream>(s); sound2.Play(); }
	public void CardSelected(){ CardClickedCounter += 1; CheckSet(); }
	public void CardDeselected() { CardClickedCounter -= 1; }

	private void CheckSet()
	{
		if (CardClickedCounter != 3) return;
	
		List<int> selectedCards = new List<int>();
		for (int i = 0; i < CardsField.Count; i++) if (CardsField[i]._isSelected) selectedCards.Add(i);
		GD.Print("ListCompiled");
		if (ChekPattern(CardsField[selectedCards[0]], CardsField[selectedCards[1]], CardsField[selectedCards[2]]))
		{
			GD.Print("It`s set!");
			PlaySound1("res://sounds/Р В Р вЂ Р В Р’ВµР РЋР вЂљР В Р вЂ¦Р В Р’В°Р РЋР РЏ Р В РЎвЂќР В РЎвЂўР В РЎВР В Р’В±Р В РЎвЂР В Р вЂ¦Р В Р’В°Р РЋРІР‚В Р В РЎвЂР РЋР РЏ 1.mp3");
			CardClickedCounter = 0;
			if (TheDeck.Count > 0)
			{
				if (IsAddCards == false)
				{
					for (int i = 0; i < 3; i++)
					{
						GenerateNewCard(selectedCards[i]);
					}
				}
				else
				{
					for (int i = 2; i > -1; i--)
					{
						CardsField[selectedCards[i]].QueueFree();
						CardsField.RemoveAt(selectedCards[i]);
						GD.Print("CardRemoved");
					}
					UpdateCardPosition();
					GD.Print("CradUpadtePosition");
				}
				if (ChekPatternAvailable()) return;
				AddAdditionalCards();
			}
			else
			{
				GD.Print("Last 12 card on field");
				for (int i = 2; i > -1; i--)
				{
					CardsField[selectedCards[i]].QueueFree();
					CardsField.RemoveAt(selectedCards[i]);
					GD.Print("CardRemoved");
				}
				if (ChekPatternAvailable()) return;
				EndGame();
			}
		}
		else
		{
			GD.Print("It isn`t set");
			PlaySound1("res://sounds/Р В Р вЂ¦Р В Р’ВµР В Р вЂ Р В Р’ВµР РЋР вЂљР В Р вЂ¦Р В Р’В°Р РЋР РЏ Р В РЎвЂќР В РЎвЂўР В РЎВР В Р’В±Р В РЎвЂР В Р вЂ¦Р В Р’В°Р РЋРІР‚В Р В РЎвЂР РЋР РЏ 1.mp3");
			CardClickedCounter = 0;
			for (int i = 0; i < 3; i++)
			{
				CardsField[selectedCards[i]].SetSelected(false);
				CardsField[selectedCards[i]].CCount = 0;
			}
			if (ChekPatternAvailable()) return;
			AddAdditionalCards();
		}	
	}

	//Р В РЎСџР РЋР вЂљР В РЎвЂўР В Р вЂ Р В Р’ВµР РЋР вЂљР В РЎвЂќР В Р’В° Р В Р вЂ¦Р В Р’В° Р В Р вЂ¦Р В Р’В°Р В Р’В»Р В РЎвЂР РЋРІР‚РЋР В РЎвЂР В Р’Вµ Р В РЎвЂ”Р В Р’В°Р РЋРІР‚С™Р РЋРІР‚С™Р В Р’ВµР РЋР вЂљР В Р вЂ¦Р В Р’В°
	private static bool ChekPatternAvailable()
	{
		for (int c = 0; c < CardsField.Count-2; c++)
			for (int f = c + 1; f < CardsField.Count-1; f++)
				for (int g = f + 1; g < CardsField.Count; g++)
				{
					if (ChekPattern(CardsField[c], CardsField[f], CardsField[g]))
					{ GD.Print($"Pattern - {c + 1}, {f + 1}, {g + 1}"); return true; }	
				}
		GD.Print("The pattern does not exist");
		return false;
	}
	private static bool ChekPattern(Card card1, Card card2, Card card3)
	{
		if ((card1.Color == card2.Color && card2.Color == card3.Color && card1.Color == card3.Color || card1.Color != card2.Color && card2.Color != card3.Color && card1.Color != card3.Color) &&
		   (card1.Shape == card2.Shape && card2.Shape == card3.Shape && card1.Shape == card3.Shape || card1.Shape != card2.Shape && card2.Shape != card3.Shape && card1.Shape != card3.Shape) &&
		   (card1.Fill == card2.Fill && card2.Fill == card3.Fill && card1.Fill == card3.Fill || card1.Fill != card2.Fill && card2.Fill != card3.Fill && card1.Fill != card3.Fill) &&
		   (card1.Count == card2.Count && card2.Count == card3.Count && card1.Count == card3.Count || card1.Count != card2.Count &&	card2.Count != card3.Count &&	card1.Count !=	card3.Count))
		{ return true; }
		else return false;
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
		_streakLabel.Text = $"x{gameController.GameStreak.CurrentValue}";
		_scoreLabel.Text = $"{gameController.GameScore.CurrentValue:D4}";

		_timerLabel.Text = $"{Mathf.Floor(_time / 60):00}:{Mathf.Floor(_time % 60):00}";
	}

	private void GenerateCards()
	{
		var cardScene = (PackedScene)GD.Load("res://scenes/game/cards/Card.tscn");

		foreach (Node child in _cardsContainer.GetChildren())
		{
			child.QueueFree();
		}
		CardsField.Clear();

		var screen = GetViewport().GetVisibleRect().Size;

		float baseCardWidth = 100f;
		float baseCardHeight = 140f;

		float scaleX = (screen.X / 4.5f) / baseCardWidth;
		float scaleY = (screen.Y / 3.5f) / baseCardHeight;
		float cardScale = Mathf.Min(scaleX, scaleY) * 0.75f;

		float startX = screen.X * -0.17f; //Р В РўвЂР В Р’В»Р РЋР РЏ Р В РЎвЂ”Р РЋР РЏР РЋРІР‚С™Р В РЎвЂўР В РІвЂћвЂ“ Р В РЎвЂќР В РЎвЂўР В Р’В»Р В РЎвЂўР В Р вЂ¦Р В РЎвЂќР В РЎвЂ Р В РЎвЂ”Р В РЎвЂўР В РЎВР В Р’ВµР В Р вЂ¦Р РЋР РЏР РЋРІР‚С™Р РЋР Р‰ Р В Р вЂ¦Р В Р’В° -0.21
		float startY = screen.Y * -0.29f;

		for (int i = 0; i < 12; i++)
		{
			var card = cardScene.Instantiate<Card>();

			var figure = (CardFigure)_random.Next(0, 3);
			var color = (CardColor)_random.Next(0, 3);
			var fill = (CardFill)_random.Next(0, 3);
			var count = (CardCount)_random.Next(1, 4);

			Discard.Add(TheDeck[0]);
			TheDeck.RemoveAt(0);

			CardData data = new CardData(figure, color, fill, count);
			card.Setup(data);
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
			CardsField.Add(card);
		}
		
		GD.Print($"Generated {_cardsContainer.GetChildCount()} cards");
		if(ChekPatternAvailable()) return;
		AddAdditionalCards();
	}

	private static void GenerateNewCard(int i)
	{
		var shape = (CardFigure)TheDeck[0][0];
		var color = (CardColor)TheDeck[0][1];
		var fill = (CardFill)TheDeck[0][2];
		var count = (CardCount)TheDeck[0][3];

		Discard.Add(TheDeck[0]);
		TheDeck.RemoveAt(0);

		CardData data = new CardData(
		shape,
		color,
		fill,
		count
		);

		CardsField[i].Setup(data);
		CardsField[i].SetSelected(false);
		CardsField[i].CCount = 0;
		CardsField[i].UpdateCard();
	}

	private void AddAdditionalCards()
	{
		IsAddCards = true;
		var cardScene = (PackedScene)GD.Load("res://scenes/game/cards/Card.tscn");

		if (cardScene == null)
		{
			GD.PrintErr("[GameUI] РќРµ СѓРґР°Р»РѕСЃСЊ Р·Р°РіСЂСѓР·РёС‚СЊ Card.tscn");
			return;
		}

		Vector2 screen; screen.X = 1920; screen.Y = 1080;
		float baseCardWidth = 100f;
		float baseCardHeight = 140f;

		float scaleX = (screen.X / 4.5f) / baseCardWidth;
		float scaleY = (screen.Y / 3.5f) / baseCardHeight;
		float cardScale = Mathf.Min(scaleX, scaleY) * 0.75f;

		float startX = screen.X * -0.17f;
		float startY = screen.Y * -0.29f;

		for (int i = 0; i < 12; i++)
		{
			CardsField[i].Scale = new Vector2(cardScale, cardScale);
			CardsField[i].SetBaseScale(CardsField[i].Scale);

			int row = i / 4;
			int col = i % 4;
			float spacing = 8 * cardScale;
			float x = startX + col * (baseCardWidth * cardScale + spacing);
			

			CardsField[i].Position = new Vector2(CardsField[i].Position.X - 100, CardsField[i].Position.Y);
		}
		for (int i = 0; i < 3; i++)
		{
			int icol = 4;
			int irow = i;
			float ispacing = 8 * cardScale;
			float ix = startX + icol * (baseCardWidth * cardScale + ispacing);
			float iy = startY + irow * (baseCardHeight * cardScale + ispacing);
			var icard = cardScene.Instantiate<Card>();

			var ishape = (CardFigure)TheDeck[0][0];
			var icolor = (CardColor)TheDeck[0][1];
			var ifill = (CardFill)TheDeck[0][2];
			var icount = (CardCount)TheDeck[0][3];

			Discard.Add(TheDeck[0]);
			TheDeck.RemoveAt(0);

			CardData data = new CardData(ishape, icolor, ifill, icount);
			icard.Setup(data);

			icard.SetScale(1.0f);

			icard.Scale = new Vector2(cardScale, cardScale);
			icard.SetBaseScale(icard.Scale);
			icard.Position = new Vector2(ix - 100, iy);

			_cardsContainer.AddChild(icard);
			CardsField.Add(icard);
		}
		ChekPatternAvailable();
	}

	private static void UpdateCardPosition()
	{
		IsAddCards = false;

		Vector2 screen; screen.X = 1920; screen.Y = 1080;
		float baseCardWidth = 100f;
		float baseCardHeight = 140f;

		float scaleX = (screen.X / 4.5f) / baseCardWidth;
		float scaleY = (screen.Y / 3.5f) / baseCardHeight;
		float cardScale = Mathf.Min(scaleX, scaleY) * 0.75f;

		float startX = screen.X * -0.17f + 640;
		float startY = screen.Y * -0.29f + 350;

		for (int i = 0; i < 12; i++)
		{
			CardsField[i].Scale = new Vector2(cardScale, cardScale);
			CardsField[i].SetBaseScale(CardsField[i].Scale);

			int row = i / 4;
			int col = i % 4;
			float spacing = 8 * cardScale;
			float x = startX + col * (baseCardWidth * cardScale + spacing);
			float y = startY + row * (baseCardHeight * cardScale + spacing);

			CardsField[i].Position = new Vector2(x, y);
		}
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
		_isPaused = true;
	}

	private void EndGame()
	{
		if (_isGameOver)
			return;

		_isGameOver = true;

		int score = gameController.GameScore.CurrentValue;
		int streak = gameController.GameStreak.CurrentValue;

		var saveData = SaveController.GameData;

		bool isNewRecord = score > saveData.BestScore;

		gameController.FinishGame();

		var gameOverScene = (PackedScene)GD.Load("res://scenes/ui/game_over/GameOver.tscn");
		var gameOverInstance = gameOverScene.Instantiate<GameOver>();
		gameOverInstance.SetData(score, streak, 0, isNewRecord);

		AddChild(gameOverInstance);
	}

	public void ResetTimer()
	{
		_time = 300f;
		UpdateUI();
	}
	private void OnScoreChanged(int value)
	{
		_scoreLabel.Text = $"{value:D4}";
	}

	private void OnStreakChanged(int value)
	{
		_streakLabel.Text = $"x{value}";
	}

	//Р В Р Р‹Р В РЎвЂўР В Р’В·Р В РўвЂР В Р’В°Р В Р вЂ¦Р В РЎвЂР В Р’Вµ Р В РЎвЂќР В РЎвЂўР В Р’В»Р В РЎвЂўР В РўвЂР РЋРІР‚в„–
	private void InitializeDeck()
	{
		TheDeck.Clear();
		for (int c = 0; c < 3; c++)
			for (int s = 0; s < 3; s++)
				for (int n = 0; n < 3; n++)
					for (int f = 1; f < 4; f++)
					{
						int[] cardcode = new int[] { c, s, n, f };
						TheDeck.Add(cardcode);
					}
		Shuffle<int[]>(TheDeck);
	}
	private void Shuffle<T>(IList<T> list)
	{
		var rng = new Random();
		int n = list.Count;
		while (n > 1)
		{
			n--;
			int k = rng.Next(n + 1); // Р РЋР С“Р В Р’В»Р РЋРЎвЂњР РЋРІР‚РЋР В Р’В°Р В РІвЂћвЂ“Р В Р вЂ¦Р РЋРІР‚в„–Р В РІвЂћвЂ“ Р В РЎвЂР В Р вЂ¦Р В РўвЂР В Р’ВµР В РЎвЂќР РЋР С“ Р В РЎвЂўР РЋРІР‚С™ 0 Р В РўвЂР В РЎвЂў n
			T value = list[k];
			list[k] = list[n];
			list[n] = value;
		}
	}
}
