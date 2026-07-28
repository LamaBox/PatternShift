using Godot;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using static Main;
using System.Reflection;

public partial class Main : Node2D
{
	private List<Card> TheDeck = new();
	private Timer _roundTimer;
	private Timer _timerover;
	private Label _timeLable;
	private Button _button;
	Card[] CardField = new Card[12];
	int[] CardCount = new int[12];
	int count = 0;
	int CardObserver = 12;
	int _time = 300;
	bool InfinityModeTrue = false;
	public class Card
	{
		public int Color;
		public int Symbol;
		public int NumberOfSymbols;
		public int Fill;
		public void SetCardData(int Color, int Symbol, int Fill, int NumberOfSymbols)
		{
			this.Color = Color;
			this.Symbol = Symbol;
			this.Fill = Fill;
			this.NumberOfSymbols = NumberOfSymbols;
		}
	}
	private string GetCardCode(Card _card)
	{
		return $"{_card.Color}{_card.Symbol}{_card.Fill}{_card.NumberOfSymbols}";
	}

	//Старт раунда
	private void TimeMode()
	{
		_roundTimer = GetNode<Timer>("RoundTimer");
		_timerover = GetNode<Timer>("TimerGameOver");
		_timeLable = GetNode<Label>("TimeLabel");
		InitializeDeck();
		StartRound();
		_roundTimer.Start();
		_timerover.Start();
		while (ChekPatternAvailable() == false)
		{ RefillDeck(); GD.Print("CardField Update"); }
	}
	private void ClassicMode()
	{
		InitializeDeck();
		StartRound();
		while (ChekPatternAvailable() == false)
		{ RefillDeck(); GD.Print("CardField Update"); }
	}
	private void InfinityMode()
	{
		InfinityModeTrue = true;
		InitializeDeck();
		StartRound();
		while (ChekPatternAvailable() == false)
		{ RefillDeck(); GD.Print("CardField Update"); }
	}
	private void Restart()
	{
		_time = 300;
		_roundTimer.Stop();
		_timerover.Stop();
		_timeLable.Text = "";
	}

	//Создание колоды
	private void InitializeDeck()
	{
		TheDeck.Clear();
		for (int c = 0; c < 3; c++)
			for (int s = 0; s < 3; s++)
				for (int n = 0; n < 3; n++)
					for (int f = 0; f < 3; f++)
					{
						TheDeck.Add(new Card { Color=c, Symbol=s, NumberOfSymbols = n, Fill=f});
					}
		Shuffle<Card>(TheDeck);
	}

	//Тосовка колоды
	private void Shuffle<T>(IList<T> list)
	{
		var rng = new Random();
		int n = list.Count;
		while (n > 1)
		{
			n--;
			int k = rng.Next(n + 1); // случайный индекс от 0 до n
			T value = list[k];
			list[k] = list[n];
			list[n] = value;
		}
	}

	//Расположние первых кард на поле
	private void StartRound()
	{
		for (int c=0; c < 12; c++)
		{
			CardField[c] = TheDeck[c];
			GD.Print("Card - ", c + 1, " - ", CardField[c].Color, CardField[c].Symbol, CardField[c].Fill, CardField[c].NumberOfSymbols);
		}
	}

	//Проверка выборах трех карт
	private void CheckSet() 
	{
		if (InfinityModeTrue == false)
		{
			if (CardObserver < 69)
			{
				if (count == 3)
				{
					List<int> list = new List<int>();
					for (int c = 0; c < 12; c++) { if (CardCount[c] == 1) list.Add(c); }
					if (ChekPattern(CardField[list[0]], CardField[list[1]], CardField[list[2]]))
					{
						count = 0;
						int ic = 0;
						for (int c = 0; c < 12; c++)
						{
							if (CardCount[c] == 1)
							{
								CardCount[c] = 0;
								ic += 1;
								CardField[c] = TheDeck[CardObserver];
								GD.Print("New Card - ", TheDeck[CardObserver].Color, TheDeck[CardObserver].Symbol, TheDeck[CardObserver].Fill, TheDeck[CardObserver].NumberOfSymbols);
								CardObserver += 1;
								var CardDeck1 = ResourceLoader.Load<Texture2D>("D:/Projects/godot/pattern-shift-v-3/CardTexture/card2.png");
								var textureRect = GetNode<TextureRect>($"CardField/TextureRect{c + 1}");
								textureRect.Texture = CardDeck1;
							}
							if (ic == 3)
							{
								if (ChekPatternAvailable())
								{ break; }
								else
								{
									GD.Print("Peretasovka");
									RefillDeck();
								}
							}
						}
					}
					else
					{
						GD.Print("Is not set");
						_time = Math.Max(1, _time-30);
						_timerover.WaitTime = _time;
						_timerover.Start();
						count = 0;
						for (int c = 0; c < 3; c++)
						{
							CardCount[list[c]] = 0;
							var CardDeck1 = ResourceLoader.Load<Texture2D>("D:/Projects/godot/pattern-shift-v-3/CardTexture/card2.png");
							var textureRect = GetNode<TextureRect>($"CardField/TextureRect{list[c] + 1}");
							textureRect.Texture = CardDeck1;
						}
					}
				}
			}
			else
			{
				GD.Print("Последние карты на поле");
				if (count == 3)
				{
					List<int> list = new List<int>();
					for (int c = 0; c < 12; c++) { if (CardCount[c] == 1) list.Add(c); }
					if (ChekPattern(CardField[list[0]], CardField[list[1]], CardField[list[2]]))
					{
						GD.Print("Pattern set");
						count = 0;
						for (int c = 0; c < 12; c++)
						{
							if (CardCount[c] == 1)
							{
								CardCount[c] = 0;
								var textureRect = GetNode<TextureRect>($"CardField/TextureRect{c + 1}");
								textureRect.Texture = null;
								var _button = GetNode<Button>($"CardField/TextureRect{c + 1}/Button{c + 1}");
								_button.Disabled = true;
								CardField[c].SetCardData(9, 9, 9, 9);
							}
						}
						if (ChekPatternAvailable() == false)
						{ GD.Print("Паттернов больше нет"); _roundTimer.Stop(); CardDesabled(); for (int f = 0; f < 12; f++) GD.Print("Card - ", GetCardCode(CardField[f])); }
					}
					else
					{
						GD.Print("Is not set");
						_time = Math.Max(1, _time - 30);
						_timerover.WaitTime = _time;
						_timerover.Start();
						count = 0;
						for (int c = 0; c < 3; c++)
						{
							CardCount[list[c]] = 0;
							var CardDeck1 = ResourceLoader.Load<Texture2D>("D:/Projects/godot/pattern-shift-v-3/CardTexture/card2.png");
							var textureRect = GetNode<TextureRect>($"CardField/TextureRect{list[c] + 1}");
							textureRect.Texture = CardDeck1;
						}
					}
				}
			}
		}
		else
		{
			if (CardObserver < 69)
			{
				if (count == 3)
				{
					List<int> list = new List<int>();
					for (int c = 0; c < 12; c++) { if (CardCount[c] == 1) list.Add(c); }
					if (ChekPattern(CardField[list[0]], CardField[list[1]], CardField[list[2]]))
					{
						count = 0;
						int ic = 0;
						for (int c = 0; c < 12; c++)
						{
							if (CardCount[c] == 1)
							{
								CardCount[c] = 0;
								ic += 1;
								CardField[c] = TheDeck[CardObserver];
								GD.Print("New Card - ", TheDeck[CardObserver].Color, TheDeck[CardObserver].Symbol, TheDeck[CardObserver].Fill, TheDeck[CardObserver].NumberOfSymbols);
								CardObserver += 1;
								var CardDeck1 = ResourceLoader.Load<Texture2D>("D:/Projects/godot/pattern-shift-v-3/CardTexture/card2.png");
								var textureRect = GetNode<TextureRect>($"CardField/TextureRect{c + 1}");
								textureRect.Texture = CardDeck1;
							}
							if (ic == 3)
							{
								if (ChekPatternAvailable())
								{ break; }
								else
								{
									GD.Print("Peretasovka");
									RefillDeck();
								}
							}
						}
					}
					else
					{
						GD.Print("Is not set");
						count = 0;
						for (int c = 0; c < 3; c++)
						{
							CardCount[list[c]] = 0;
							var CardDeck1 = ResourceLoader.Load<Texture2D>("D:/Projects/godot/pattern-shift-v-3/CardTexture/card2.png");
							var textureRect = GetNode<TextureRect>($"CardField/TextureRect{list[c] + 1}");
							textureRect.Texture = CardDeck1;
						}
					}
				}
			}
			else
			{
				if (count == 3)
				{
					List<int> list = new List<int>();
					for (int c = 0; c < 12; c++) { if (CardCount[c] == 1) list.Add(c); }
					if (ChekPattern(CardField[list[0]], CardField[list[1]], CardField[list[2]]))
					{
						GD.Print("Deck Update");
						List<Card> NewDeck = new List<Card>();
						for (int c = 0; c < 69; c++) { NewDeck.Add(TheDeck[c]); }
						Shuffle<Card>(NewDeck);
						CardObserver = 12;
						for (int c = 0; c < 12; c++) { TheDeck[c] = CardField[c]; }
						for (int c = 0; c < 69; c++) { TheDeck[c + 12] = NewDeck[c]; }

						count = 0;
						int ic = 0;
						for (int c = 0; c < 12; c++)
						{
							if (CardCount[c] == 1)
							{
								CardCount[c] = 0;
								ic += 1;
								CardField[c] = TheDeck[CardObserver];
								GD.Print("New Card - ", TheDeck[CardObserver].Color, TheDeck[CardObserver].Symbol, TheDeck[CardObserver].Fill, TheDeck[CardObserver].NumberOfSymbols);
								CardObserver += 1;
								var CardDeck1 = ResourceLoader.Load<Texture2D>("D:/Projects/godot/pattern-shift-v-3/CardTexture/card2.png");
								var textureRect = GetNode<TextureRect>($"CardField/TextureRect{c + 1}");
								textureRect.Texture = CardDeck1;
							}
							if (ic == 3)
							{
								if (ChekPatternAvailable())
								{ break; }
								else
								{
									GD.Print("Peretasovka");
									RefillDeck();
								}
							}
						}
					}
					else
					{
						GD.Print("Is not set");
						count = 0;
						for (int c = 0; c < 3; c++)
						{
							CardCount[list[c]] = 0;
							var CardDeck1 = ResourceLoader.Load<Texture2D>("D:/Projects/godot/pattern-shift-v-3/CardTexture/card2.png");
							var textureRect = GetNode<TextureRect>($"CardField/TextureRect{list[c] + 1}");
							textureRect.Texture = CardDeck1;
						}
					}
				}
			}
		}
	}

	//Проверка на наличие паттерна
	private bool ChekPatternAvailable()
	{
		for(int c = 0; c < 6; c++)
			for(int f = c+1; f < 9; f++)
				for(int g = f+1; g < 12; g++)
				{
					if (ChekPattern(CardField[c], CardField[f], CardField[g]))
					{ GD.Print($"Pattern - {c+1}, {f+1}, {g+1}"); return true;}
				}
		return false;
	}
	private bool ChekPattern(Card card1, Card card2, Card card3)
	{
		if(card1.Color == 9 || card2.Color == 9 || card3.Color == 9) {  return false; }
		if ((card1.Color == card2.Color && card2.Color == card3.Color && card1.Color == card3.Color || card1.Color != card2.Color && card2.Color != card3.Color && card1.Color != card3.Color) &&
		   (card1.Symbol == card2.Symbol && card2.Symbol == card3.Symbol && card1.Symbol == card3.Symbol || card1.Symbol != card2.Symbol && card2.Symbol != card3.Symbol && card1.Symbol != card3.Symbol) &&
		   (card1.Fill == card2.Fill && card2.Fill == card3.Fill && card1.Fill == card3.Fill || card1.Fill != card2.Fill && card2.Fill != card3.Fill && card1.Fill != card3.Fill) &&
		   (card1.NumberOfSymbols == card2.NumberOfSymbols && card2.NumberOfSymbols == card3.NumberOfSymbols && card1.NumberOfSymbols == card3.NumberOfSymbols || card1.NumberOfSymbols != card2.NumberOfSymbols && card2.NumberOfSymbols != card3.NumberOfSymbols && card1.NumberOfSymbols != card3.NumberOfSymbols))
		{ return true; }
		else return false;
	}

	//Перетосовка неполной колоды
	private void RefillDeck()
	{
		while (ChekPatternAvailable() == false)
		{
			List<Card> cards = new List<Card>();
			for (int c = CardObserver - 12; c < 80; c++)
			{ cards.Add(TheDeck[c]); }
			Shuffle<Card>(cards);
			for (int c = CardObserver - 12; c < cards.Count-1; c++)
			{ TheDeck[c] = cards[c]; }
			for (int c = 0; c < 12; c++)
			{	CardField[c] = TheDeck[CardObserver - 12 + c];	}
		}
		GD.Print("New Field:");
		for(int c = 0; c < 12; c++)
		{GD.Print($"Card{c+1} - ", CardField[c].Color, CardField[c].Symbol, CardField[c].Fill, CardField[c].NumberOfSymbols);}
	}

	//Таймер
	private void OnTimerTimeout()
	{
		_time--;
		_timeLable.Text = $"Осталось: {_time/60}:{_time%60}";
	}

	private void TimerGameOverOut()
	{
		_roundTimer.Stop();
		CardDesabled();
		GD.Print("GameOver");
		//Вывод результатов
		//Выход в главное меню
	}

	//Отключение карточного поля
	private void CardDesabled()
	{
		for (int c = 0; c < 12; c++)
		{
			_button = GetNode<Button>($"CardField/TextureRect{c + 1}/Button{c + 1}");
			_button.Disabled = true;
		}
	}

	//Кнопки для карт
	private void _on_button1_down()
	{
		CardCount[0] += 1;
		if (CardCount[0] == 1)
		{
			count++;
			var CardDeck1 = ResourceLoader.Load<Texture2D>("D:/Projects/godot/pattern-shift-v-3/CardTexture/card3.png");
			var textureRect1 = GetNode<TextureRect>("CardField/TextureRect1");
			textureRect1.Texture = CardDeck1;
			CheckSet();
		}
		else
		{
			count--;
			CardCount[0] = 0;
			var CardDeck1 = ResourceLoader.Load<Texture2D>("D:/Projects/godot/pattern-shift-v-3/CardTexture/card2.png");
			var textureRect1 = GetNode<TextureRect>("CardField/TextureRect1");
			textureRect1.Texture = CardDeck1;
		}
	}
	private void _on_button2_down()
	{
		CardCount[1] += 1;
		if (CardCount[1] == 1)
		{ 
			count++;
			var CardDeck1 = ResourceLoader.Load<Texture2D>("D:/Projects/godot/pattern-shift-v-3/CardTexture/card3.png");
			var textureRect2 = GetNode<TextureRect>("CardField/TextureRect2");
			textureRect2.Texture = CardDeck1;
			CheckSet();
		}
		else 
		{
			count--;
			CardCount[1] = 0;
			var CardDeck1 = ResourceLoader.Load<Texture2D>("D:/Projects/godot/pattern-shift-v-3/CardTexture/card2.png");
			var textureRect2 = GetNode<TextureRect>("CardField/TextureRect2");
			textureRect2.Texture = CardDeck1;
		}
	}
	private void _on_button3_down()
	{
		CardCount[2] += 1;
		if (CardCount[2] == 1) 
		{ 
			count++;
			var CardDeck1 = ResourceLoader.Load<Texture2D>("D:/Projects/godot/pattern-shift-v-3/CardTexture/card3.png");
			var textureRect3 = GetNode<TextureRect>("CardField/TextureRect3");
			textureRect3.Texture = CardDeck1;
			CheckSet();
		}
		else 
		{ 
			count--;
			CardCount[2] = 0;
			var CardDeck1 = ResourceLoader.Load<Texture2D>("D:/Projects/godot/pattern-shift-v-3/CardTexture/card2.png");
			var textureRect3 = GetNode<TextureRect>("CardField/TextureRect3");
			textureRect3.Texture = CardDeck1;
		}
	}
	private void _on_button4_down()
	{
		CardCount[3] += 1;
		if (CardCount[3] == 1)
		{
			count++;
			var CardDeck1 = ResourceLoader.Load<Texture2D>("D:/Projects/godot/pattern-shift-v-3/CardTexture/card3.png");
			var textureRect4 = GetNode<TextureRect>("CardField/TextureRect4");
			textureRect4.Texture = CardDeck1;
			CheckSet() ;
		}
		else
		{
			count--;
			CardCount[3] = 0;
			var CardDeck1 = ResourceLoader.Load<Texture2D>("D:/Projects/godot/pattern-shift-v-3/CardTexture/card2.png");
			var textureRect4 = GetNode<TextureRect>("CardField/TextureRect4");
			textureRect4.Texture = CardDeck1;
		}
	}
	private void _on_button5_down()
	{
		CardCount[4] += 1;
		if (CardCount[4] == 1)
		{
			count++;
			var CardDeck1 = ResourceLoader.Load<Texture2D>("D:/Projects/godot/pattern-shift-v-3/CardTexture/card3.png");
			var textureRect5 = GetNode<TextureRect>("CardField/TextureRect5");
			textureRect5.Texture = CardDeck1;
			CheckSet() ;
		}
		else
		{
			count--;
			CardCount[4] = 0;
			var CardDeck1 = ResourceLoader.Load<Texture2D>("D:/Projects/godot/pattern-shift-v-3/CardTexture/card2.png");
			var textureRect5 = GetNode<TextureRect>("CardField/TextureRect5");
			textureRect5.Texture = CardDeck1;
		}
	}
	private void _on_button6_down()
	{
		CardCount[5] += 1;
		if (CardCount[5] == 1)
		{
			count++;
			var CardDeck1 = ResourceLoader.Load<Texture2D>("D:/Projects/godot/pattern-shift-v-3/CardTexture/card3.png");
			var textureRect6 = GetNode<TextureRect>("CardField/TextureRect6");
			textureRect6.Texture = CardDeck1;
			CheckSet() ;
		}
		else
		{
			count--;
			CardCount[5] = 0;
			var CardDeck1 = ResourceLoader.Load<Texture2D>("D:/Projects/godot/pattern-shift-v-3/CardTexture/card2.png");
			var textureRect6 = GetNode<TextureRect>("CardField/TextureRect6");
			textureRect6.Texture = CardDeck1;
		}
	}
	private void _on_button7_down()
	{
		CardCount[6] += 1;
		if (CardCount[6] == 1)
		{
			count++;
			var CardDeck1 = ResourceLoader.Load<Texture2D>("D:/Projects/godot/pattern-shift-v-3/CardTexture/card3.png");
			var textureRect7 = GetNode<TextureRect>("CardField/TextureRect7");
			textureRect7.Texture = CardDeck1;
			CheckSet() ;
		}
		else
		{
			count--;
			CardCount[6] = 0;
			var CardDeck1 = ResourceLoader.Load<Texture2D>("D:/Projects/godot/pattern-shift-v-3/CardTexture/card2.png");
			var textureRect7 = GetNode<TextureRect>("CardField/TextureRect7");
			textureRect7.Texture = CardDeck1;
		}
	}
	private void _on_button8_down()
	{
		CardCount[7] += 1;
		if (CardCount[7] == 1)
		{
			count++;
			var CardDeck1 = ResourceLoader.Load<Texture2D>("D:/Projects/godot/pattern-shift-v-3/CardTexture/card3.png");
			var textureRect8 = GetNode<TextureRect>("CardField/TextureRect8");
			textureRect8.Texture = CardDeck1;
			CheckSet() ;
		}
		else
		{
			count--;
			CardCount[7] = 0;
			var CardDeck1 = ResourceLoader.Load<Texture2D>("D:/Projects/godot/pattern-shift-v-3/CardTexture/card2.png");
			var textureRect8 = GetNode<TextureRect>("CardField/TextureRect8");
			textureRect8.Texture = CardDeck1;
		}
	}
	private void _on_button9_down()
	{
		CardCount[8] += 1;
		if (CardCount[8] == 1)
		{
			count++;
			var CardDeck1 = ResourceLoader.Load<Texture2D>("D:/Projects/godot/pattern-shift-v-3/CardTexture/card3.png");
			var textureRect9 = GetNode<TextureRect>("CardField/TextureRect9");
			textureRect9.Texture = CardDeck1;
			CheckSet() ;
		}
		else
		{
			count--;
			CardCount[8] = 0;
			var CardDeck1 = ResourceLoader.Load<Texture2D>("D:/Projects/godot/pattern-shift-v-3/CardTexture/card2.png");
			var textureRect9 = GetNode<TextureRect>("CardField/TextureRect9");
			textureRect9.Texture = CardDeck1;
		}
	}
	private void _on_button10_down()
	{
		CardCount[9] += 1;
		if (CardCount[9] == 1)
		{
			count++;
			var CardDeck1 = ResourceLoader.Load<Texture2D>("D:/Projects/godot/pattern-shift-v-3/CardTexture/card3.png");
			var textureRect10 = GetNode<TextureRect>("CardField/TextureRect10");
			textureRect10.Texture = CardDeck1;
			CheckSet() ;
		}
		else 
		{
			count--;
			CardCount[9] = 0;
			var CardDeck1 = ResourceLoader.Load<Texture2D>("D:/Projects/godot/pattern-shift-v-3/CardTexture/card2.png");
			var textureRect10 = GetNode<TextureRect>("CardField/TextureRect10");
			textureRect10.Texture = CardDeck1;
		}
	}
	private void _on_button11_down()
	{
		CardCount[10] += 1;
		if (CardCount[10] == 1)
		{
			count++;
			var CardDeck1 = ResourceLoader.Load<Texture2D>("D:/Projects/godot/pattern-shift-v-3/CardTexture/card3.png");
			var textureRect11 = GetNode<TextureRect>("CardField/TextureRect11");
			textureRect11.Texture = CardDeck1;
			CheckSet() ;
		}
		else 
		{
			count--;
			CardCount[10] = 0;
			var CardDeck1 = ResourceLoader.Load<Texture2D>("D:/Projects/godot/pattern-shift-v-3/CardTexture/card2.png");
			var textureRect11 = GetNode<TextureRect>("CardField/TextureRect11");
			textureRect11.Texture = CardDeck1;
		}
	}
	private void _on_button12_down()
	{
		CardCount[11] += 1;
		if (CardCount[11] == 1)
		{
			count++;
			var CardDeck1 = ResourceLoader.Load<Texture2D>("D:/Projects/godot/pattern-shift-v-3/CardTexture/card3.png");
			var textureRect12 = GetNode<TextureRect>("CardField/TextureRect12");
			textureRect12.Texture = CardDeck1;
			CheckSet() ;
		}
		else 
		{
			count--;;
			CardCount[11] = 0;
			var CardDeck1 = ResourceLoader.Load<Texture2D>("D:/Projects/godot/pattern-shift-v-3/CardTexture/card2.png");
			var textureRect12 = GetNode<TextureRect>("CardField/TextureRect12");
			textureRect12.Texture = CardDeck1;
		}
	}
}
