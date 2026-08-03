using Godot;
using System.Collections.Generic;

public partial class Card : Control
{
	private Panel _background;
	private VBoxContainer _shapesContainer;
	private TextureRect _shapeTemplate;
	private Panel _highlight;
	private Button _clickArea;

	private static readonly Dictionary<CardColor, Color> ColorMap = new()
	{
		{ CardColor.Orange, new Color("#FF9800") },
		{ CardColor.Blue, new Color("#4FC3F7") },
		{ CardColor.Purple, new Color("#9C27B0") }
	};

	private static readonly Dictionary<(CardFigure, CardFill), string> ShapePaths = new()
	{
		{ (CardFigure.Rectangle, CardFill.Empty), "res://assets/images/cards/shapes/shape_rect_empty.png" },
		{ (CardFigure.Rectangle, CardFill.Striped), "res://assets/images/cards/shapes/shape_rect_striped.png" },
		{ (CardFigure.Rectangle, CardFill.Solid), "res://assets/images/cards/shapes/shape_rect_solid.png" },
		
		{ (CardFigure.Triangle, CardFill.Empty), "res://assets/images/cards/shapes/shape_triangle_empty.png" },
		{ (CardFigure.Triangle, CardFill.Striped), "res://assets/images/cards/shapes/shape_triangle_striped.png" },
		{ (CardFigure.Triangle, CardFill.Solid), "res://assets/images/cards/shapes/shape_triangle_solid.png" },
		
		{ (CardFigure.Hexagon, CardFill.Empty), "res://assets/images/cards/shapes/shape_hexagon_empty.png" },
		{ (CardFigure.Hexagon, CardFill.Striped), "res://assets/images/cards/shapes/shape_hexagon_striped.png" },
		{ (CardFigure.Hexagon, CardFill.Solid), "res://assets/images/cards/shapes/shape_hexagon_solid.png" }
	};

	private CardData cardData;
	private bool _isSelected = false;
	private float _currentScale = 1.0f;

	public override void _Ready()
	{
		_background = GetNode<Panel>("Background");
		_shapesContainer = GetNode<VBoxContainer>("ShapesContainer");
		_shapeTemplate = GetNode<TextureRect>("ShapesContainer/Shape");
		_highlight = GetNode<Panel>("Highlight");
		_clickArea = GetNode<Button>("ClickArea");

		if (_clickArea != null)
			_clickArea.Pressed += OnCardPressed;

		UpdateCard();
	}

	public void Setup(CardData data)
	{
		cardData = data;
		UpdateCard();
	}

	public void SetScale(float scale)
	{
		_currentScale = scale;
		UpdateCard();
	}

	private void UpdateCard()
	{
		if (_shapeTemplate == null) return;

		foreach (Node child in _shapesContainer.GetChildren())
		{
			if (child != _shapeTemplate)
				child.QueueFree();
		}

		string shapePath = ShapePaths[(cardData.Figure, cardData.Fill)];
		var texture = (Texture2D)GD.Load(shapePath);
		if (texture == null)
		{
			GD.PrintErr($"Texture not loaded: {shapePath}");
			return;
		}

		_shapeTemplate.Texture = texture;

		float baseWidth = 70f;
		float baseHeight = 25f;
		_shapeTemplate.Size = new Vector2(baseWidth * _currentScale, baseHeight * _currentScale);
		_shapeTemplate.StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered;

		Color color = ColorMap[cardData.Color];
		_shapeTemplate.Modulate = color;

		for (int i = 1; i < (int)cardData.Count; i++)
		{
			var duplicate = _shapeTemplate.Duplicate() as TextureRect;
			if (duplicate != null)
				_shapesContainer.AddChild(duplicate);
		}
	}

	public void SetSelected(bool selected)
	{
		_isSelected = selected;
		if (_highlight != null)
			_highlight.Visible = selected;
	}

	private void OnCardPressed()
	{
		SetSelected(!_isSelected);
		GD.Print($"Card clicked: {cardData.Figure}, {cardData.Color}, {cardData.Fill}, {cardData.Count}");
	}
}
