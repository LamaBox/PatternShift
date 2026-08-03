using Godot;
using System.Collections.Generic;

public partial class Card : Control
{
	private Panel _background;
	private VBoxContainer _shapesContainer;
	private TextureRect _shapeTemplate;
	private Panel _highlight;
	private Button _clickArea;
	private Vector2 _baseScale = Vector2.One;
	private int _originalZIndex = 0;

	public enum ShapeType { Rectangle, Triangle, Hexagon }
	public enum FillType { Empty, Striped, Solid }
	public enum ColorType { Orange, Blue, Purple }

	public class CardStyleParams
	{
		public Vector2 ShapeSize { get; set; }
		public int Separation { get; set; }
		public CardStyleParams(float width, float height, int separation)
		{
			ShapeSize = new Vector2(width, height);
			Separation = separation;
		}
	}

	private static readonly Dictionary<CardStyle, CardStyleParams> StyleParams = new()
	{
		{ CardStyle.Default, new CardStyleParams(70f, 27f, 8) },
		{ CardStyle.Neon, new CardStyleParams(76f, 32f, 3) }
	};

	private static readonly Dictionary<ColorType, Color> ColorMap = new()
	{
		{ ColorType.Orange, new Color("#FF9800") },
		{ ColorType.Blue, new Color("#4FC3F7") },
		{ ColorType.Purple, new Color("#9C27B0") }
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
		{
			_clickArea.Pressed += OnCardPressed;
			_clickArea.MouseEntered += OnHover;
			_clickArea.MouseExited += OnUnhover;
		}

		UpdateCard();
	}

	public void SetBaseScale(Vector2 scale)
	{
		_baseScale = scale;
		Scale = scale;
		_originalZIndex = ZIndex;
		PivotOffset = Size / 2;
	}

	private void OnHover()
	{
		ZIndex = 10;

		var tween = CreateTween();
		tween.TweenProperty(this, "scale", _baseScale * 1.1f, 0.15f);

		var shakeTween = CreateTween();
		shakeTween.TweenProperty(this, "rotation", 0.02f, 0.05f);
		shakeTween.TweenProperty(this, "rotation", -0.02f, 0.05f);
		shakeTween.TweenProperty(this, "rotation", 0.0f, 0.05f);
	}

	private void OnUnhover()
	{
		ZIndex = _originalZIndex;

		var tween = CreateTween();
		tween.TweenProperty(this, "scale", _baseScale, 0.15f);
		tween.Parallel().TweenProperty(this, "rotation", 0.0f, 0.1f);
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

	private string GetShapePath(ShapeType shape, FillType fill)
	{
		string styleFolder = CardStyleManager.CurrentStyle == CardStyle.Default ? "default" : "neon";

		string shapeName = shape switch
		{
			ShapeType.Rectangle => "rect",
			ShapeType.Triangle => "triangle",
			ShapeType.Hexagon => "hexagon",
			_ => "rect"
		};

		string fillName = fill.ToString().ToLower();
		return $"res://assets/images/cards/shapes/{styleFolder}/{shapeName}_{fillName}.png";
	}

	private void UpdateCard()
	{
		if (_shapeTemplate == null) return;

		var styleParams = StyleParams[CardStyleManager.CurrentStyle];

		foreach (Node child in _shapesContainer.GetChildren())
		{
			if (child != _shapeTemplate)
				child.QueueFree();
		}

		string shapePath = GetShapePath(_shape, _fill);
		var texture = (Texture2D)GD.Load(shapePath);
		if (texture == null)
		{
			GD.PrintErr($"Texture not loaded: {shapePath}");
			return;
		}

		_shapeTemplate.Texture = texture;
		_shapeTemplate.Size = styleParams.ShapeSize * _currentScale;
		_shapeTemplate.StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered;

		Color color = ColorMap[_color];
		_shapeTemplate.Modulate = color;

		for (int i = 1; i < _count; i++)
		{
			var duplicate = _shapeTemplate.Duplicate() as TextureRect;
			if (duplicate != null)
				_shapesContainer.AddChild(duplicate);
		}

		_shapesContainer.AddThemeConstantOverride("separation", styleParams.Separation);
	}

	public void SetSelected(bool selected)
	{
		_isSelected = selected;
		if (_highlight != null)
		{
			_highlight.Visible = selected;
			if (selected)
			{
				var tween = CreateTween();
				tween.SetLoops();
				tween.TweenProperty(_highlight, "modulate:a", 0.9f, 0.3f);
				tween.TweenProperty(_highlight, "modulate:a", 0.4f, 0.3f);
			}
			else
			{
				_highlight.Modulate = new Color(1, 1, 1, 0);
				_highlight.Visible = false;
			}
		}
	}

	private void OnCardPressed()
	{
		SetSelected(!_isSelected);
		GD.Print($"Card clicked: {_shape}, {_color}, {_fill}, {_count}");
	}
}
