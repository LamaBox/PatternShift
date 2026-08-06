using Godot;
using System.Collections.Generic;

public partial class Collection : Control
{
	[Export] private Label _styleNameLabel;
	[Export] private GridContainer _gridContainer;
	[Export] private Button _prevButton;
	[Export] private Button _nextButton;
	[Export] private Button _selectButton;
	[Export] private Button _backButton;

	private List<CardStyle> _styles = new() { CardStyle.Default, CardStyle.Neon };
	private int _currentStyleIndex = 0;

	public override void _Ready()
	{
		_prevButton.Pressed += OnPrevPressed;
		_nextButton.Pressed += OnNextPressed;
		_selectButton.Pressed += OnSelectPressed;
		_backButton.Pressed += OnBackPressed;

		UpdateCollection();
	}

	private void UpdateCollection()
	{
		_styleNameLabel.Text = _styles[_currentStyleIndex].ToString();
		CardStyleManager.CurrentStyle = _styles[_currentStyleIndex];
		GenerateCards();
		_selectButton.Text = $"Выбрать {_styles[_currentStyleIndex]}";
	}

	private void GenerateCards()
	{
		foreach (Node child in _gridContainer.GetChildren())
			child.QueueFree();

		for (int shape = 0; shape < 3; shape++)
		{
			for (int color = 0; color < 3; color++)
			{
				for (int fill = 0; fill < 3; fill++)
				{
					for (int count = 1; count <= 3; count++)
					{
						var card = CreateCard(
							(Card.ShapeType)shape,
							(Card.ColorType)color,
							(Card.FillType)fill,
							count
						);
						_gridContainer.AddChild(card);
					}
				}
			}
		}
	}

	private Card CreateCard(Card.ShapeType shape, Card.ColorType color, Card.FillType fill, int count)
	{
		var cardScene = (PackedScene)GD.Load("res://scenes/game/cards/Card.tscn");
		var card = cardScene.Instantiate<Card>();

		card.Setup(shape, color, fill, count);
		card.SetScale(1.0f);
		card.SetSelected(false);
		card.MouseFilter = MouseFilterEnum.Ignore;

		return card;
	}

	private void OnPrevPressed()
	{
		_currentStyleIndex = (_currentStyleIndex - 1 + _styles.Count) % _styles.Count;
		UpdateCollection();
	}

	private void OnNextPressed()
	{
		_currentStyleIndex = (_currentStyleIndex + 1) % _styles.Count;
		UpdateCollection();
	}

	private void OnSelectPressed()
	{
		CardStyle selectedStyle = _styles[_currentStyleIndex];
		CardStyleManager.CurrentStyle = selectedStyle;
		GD.Print($"Style selected: {selectedStyle}");
		_selectButton.Text = $"{selectedStyle} выбран";
	}

	private void OnBackPressed()
	{
		GetNode<SceneManager>("/root/SceneManager").GoToMainMenu();
	}
}
