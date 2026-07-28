using Godot;
using System.Collections.Generic;

public partial class Card : Control
{
	private Panel _background;
	private VBoxContainer _shapesContainer;
	private TextureRect _shapeTemplate;
	private Panel _highlight;
	private Button _clickArea;

	public enum ShapeType { Rectangle, Triangle, Hexagon }
	public enum FillType { Empty, Striped, Solid }
	public enum ColorType { Orange, Blue, Purple }

	private static readonly Dictionary<ColorType, Color> ColorMap = new()
	{
		{ ColorType.Orange, new Color("#FF9800") },
		{ ColorType.Blue, new Color("#4FC3F7") },
		{ ColorType.Purple, new Color("#9C27B0") }
	};

	private static readonly Dictionary<(ShapeType, FillType), string> ShapePaths = new()
	{
		{ (ShapeType.Rectangle, FillType.Empty), "res://assets/images/cards/shapes/shape_rect_empty.png" },
		{ (ShapeType.Rectangle, FillType.Striped), "res://assets/images/cards/shapes/shape_rect_striped.png" },
		{ (ShapeType.Rectangle, FillType.Solid), "res://assets/images/cards/shapes/shape_rect_solid.png" },
		
		{ (ShapeType.Triangle, FillType.Empty), "res://assets/images/cards/shapes/shape_triangle_empty.png" },
		{ (ShapeType.Triangle, FillType.Striped), "res://assets/images/cards/shapes/shape_triangle_striped.png" },
		{ (ShapeType.Triangle, FillType.Solid), "res://assets/images/cards/shapes/shape_triangle_solid.png" },
		
		{ (ShapeType.Hexagon, FillType.Empty), "res://assets/images/cards/shapes/shape_hexagon_empty.png" },
		{ (ShapeType.Hexagon, FillType.Striped), "res://assets/images/cards/shapes/shape_hexagon_striped.png" },
		{ (ShapeType.Hexagon, FillType.Solid), "res://assets/images/cards/shapes/shape_hexagon_solid.png" }
	};

	private ShapeType _shape = ShapeType.Rectangle;
	private ColorType _color = ColorType.Blue;
	private FillType _fill = FillType.Empty;
	private int _count = 1;
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

	public void Setup(ShapeType shape, ColorType color, FillType fill, int count)
	{
		_shape = shape;
		_color = color;
		_fill = fill;
		_count = count;
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

		string shapePath = ShapePaths[(_shape, _fill)];
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

		Color color = ColorMap[_color];
		_shapeTemplate.Modulate = color;

		for (int i = 1; i < _count; i++)
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
		GD.Print($"Card clicked: {_shape}, {_color}, {_fill}, {_count}");
	}
}
