using Godot;
using System.Collections.Generic;

public partial class Card : Control
{
	private GameUI _gameUI;

	private AudioStreamPlayer _sound;
	private AudioStreamPlayer _sound2;
	private Panel _background;
	private VBoxContainer _shapesContainer;
	private TextureRect _shapeTemplate;
	private Panel _highlight;
	private Button _clickArea;
	private GpuParticles2D _selectionParticles;
	private Vector2 _baseScale = Vector2.One;
	private int _originalZIndex = 0;
	public int CCount = 0;

	public enum ShapeType { Rectangle, Triangle, Hexagon }
	public enum FillType { Empty, Striped, Solid }
	public enum ColorType { Orange, Blue, Purple }

	public CardColor Color
	{
		get
		{
			return cardData.Color;
		}
		set
		{
			cardData.Color = value;
		}
	}
	public CardFigure Shape {
		get
		{
			return cardData.Figure;
		}
		set
		{
			cardData.Figure = value;
		}
	}
	public CardFill Fill {
		get
		{
			return cardData.Fill;
		}
		set
		{
			cardData.Fill = value;
		}
	}
	public CardCount Count {
		get
		{
			return cardData.Count;
		}
		set
		{
			cardData.Count = value;
		}
	}

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

	private static readonly Dictionary<CardColor, Color> ColorMap = new()
	{
		{ CardColor.Orange, new Color("#FF9800") },
		{ CardColor.Blue, new Color("#4FC3F7") },
		{ CardColor.Purple, new Color("#9C27B0") }
	};

	private CardData cardData;

	public bool _isSelected = false;
	private float _currentScale = 1.0f;

	public override void _Ready()
	{
		_gameUI = GetTree().GetFirstNodeInGroup("GameUI") as GameUI;

		_background = GetNode<Panel>("Background");
		_shapesContainer = GetNode<VBoxContainer>("ShapesContainer");
		_shapeTemplate = GetNode<TextureRect>("ShapesContainer/Shape");
		_highlight = GetNode<Panel>("Highlight");
		_clickArea = GetNode<Button>("ClickArea");
		_selectionParticles = GetNode<GpuParticles2D>("SelectionParticles");
		_sound = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
		_sound.Stream = ResourceLoader.Load<AudioStream>("res://sounds/Р В Р вЂ¦Р В Р’В°Р В Р вЂ Р В Р’ВµР В РўвЂР В Р’ВµР В Р вЂ¦Р В РЎвЂР В Р’Вµ Р В Р вЂ¦Р В Р’В° Р В РЎвЂќР В Р’В°Р РЋР вЂљР РЋРІР‚С™Р В РЎвЂўР РЋРІР‚РЋР В РЎвЂќР РЋРЎвЂњ 1.mp3");
		_sound2 = GetNode<AudioStreamPlayer>("AudioStreamPlayer2");
		_sound2.Stream = ResourceLoader.Load<AudioStream>("res://sounds/Р В РЎвЂќР В Р’В»Р В РЎвЂР В РЎвЂќ 1.mp3");

		if (_clickArea != null)
		{
			_clickArea.Pressed += OnCardPressed;
			_clickArea.MouseEntered += OnHover;
			_clickArea.MouseExited += OnUnhover;
		}

		if (_selectionParticles != null)
			_selectionParticles.Emitting = false;

		if (cardData != null)
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
		_sound.Play();
	}

	private void OnUnhover()
	{
		ZIndex = _originalZIndex;

		var tween = CreateTween();
		tween.TweenProperty(this, "scale", _baseScale, 0.15f);
		tween.Parallel().TweenProperty(this, "rotation", 0.0f, 0.1f);
	}

	public void Setup(CardData cardData)
	{
		this.cardData = cardData;
	}

	public void SetScale(float scale)
	{
		_currentScale = scale;
		if (IsNodeReady())
			UpdateCard();
	}

	private string GetShapePath(CardFigure shape, CardFill fill)
	{
		string styleFolder = CardStyleManager.CurrentStyle == CardStyle.Default ? "default" : "neon";

		string shapeName = shape switch
		{
			CardFigure.Rectangle => "rect",
			CardFigure.Triangle => "triangle",
			CardFigure.Hexagon => "hexagon",
			_ => "rect"
		};

		string fillName = fill.ToString().ToLower();
		return $"res://assets/images/cards/shapes/{styleFolder}/{shapeName}_{fillName}.png";
	}

	public void UpdateCard()
	{
		if (cardData == null)
			return;

		if (_shapeTemplate == null || _shapesContainer == null)
			return;

		var styleParams = StyleParams[CardStyleManager.CurrentStyle];

		foreach (Node child in _shapesContainer.GetChildren())
		{
			if (child != _shapeTemplate)
				child.QueueFree();
		}

		string shapePath = GetShapePath(cardData.Figure, cardData.Fill);
		var texture = (Texture2D)GD.Load(shapePath);
		if (texture == null)
		{
			GD.PrintErr($"Texture not loaded: {shapePath}");
			return;
		}

		_shapeTemplate.Texture = texture;
		_shapeTemplate.Size = styleParams.ShapeSize * _currentScale;
		_shapeTemplate.StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered;

		Color color = ColorMap[cardData.Color];
		_shapeTemplate.Modulate = color;

		for (int i = 1; i < (int)cardData.Count; i++)
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
				_selectionParticles.Emitting = true;

				var tween = CreateTween();
				tween.SetLoops();
				tween.TweenProperty(_highlight, "modulate:a", 0.9f, 0.3f);
				tween.TweenProperty(_highlight, "modulate:a", 0.4f, 0.3f);
			}
			else
			{
				_highlight.Modulate = new Color(1, 1, 1, 0);
				_highlight.Visible = false;
				_selectionParticles.Emitting = false;
			}
		}
	}

	private void OnCardPressed()
	{
		_sound2.Play();
		SetSelected(!_isSelected);
		GD.Print($"Card clicked: {cardData.Figure}, {cardData.Color}, {cardData.Fill}, {cardData.Count}");
		CCount += 1;
		if (CCount < 2) _gameUI.CardSelected();
		else { CCount = 0; _gameUI.CardDeselected(); }
	}
}
