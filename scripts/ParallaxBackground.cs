using Godot;
using System.Collections.Generic;

public partial class ParallaxBackground : Control
{
	[Export] private float _smoothSpeed = 2.5f;
	[Export] private float _maxOffset = 250.0f;
	[Export] private float _layer1Speed = 0.05f;
	[Export] private float _layer2Speed = 0.1f;
	[Export] private float _layer3Speed = 0.2f;

	private List<TextureRect> _layers = new();
	private Vector2 _targetOffset = Vector2.Zero;
	private Vector2 _currentOffset = Vector2.Zero;
	private Vector2 _center = Vector2.Zero;

	public override void _Ready()
	{
		var viewportSize = GetViewport().GetVisibleRect().Size;
		_center = -viewportSize * 0.1f;

		var bg = new ColorRect();
		bg.Color = new Color("#0a0a1a");
		bg.Size = viewportSize * 10f;
		bg.Position = Vector2.Zero;
		AddChild(bg);
		MoveChild(bg, 0);

		foreach (Node child in GetChildren())
		{
			if (child is TextureRect textureRect && child != bg)
			{
				_layers.Add(textureRect);
				textureRect.StretchMode = TextureRect.StretchModeEnum.KeepAspectCovered;
				textureRect.Position = _center;
			}
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion motion)
		{
			var viewport = GetViewport().GetVisibleRect().Size;
			Vector2 mousePos = motion.Position;
			Vector2 center = viewport / 2;

			float offsetX = (mousePos.X - center.X) / center.X;
			float offsetY = (mousePos.Y - center.Y) / center.Y;

			_targetOffset = new Vector2(offsetX * _maxOffset, offsetY * _maxOffset);
		}
	}

	public override void _Process(double delta)
	{
		_currentOffset = _currentOffset.Lerp(_targetOffset, (float)delta * _smoothSpeed);

		for (int i = 0; i < _layers.Count; i++)
		{
			var layer = _layers[i];
			float speed = i == 0 ? _layer1Speed : (i == 1 ? _layer2Speed : _layer3Speed);

			Vector2 offset = _currentOffset * speed;

			float maxOffset = _maxOffset * speed;
			offset = new Vector2(
				Mathf.Clamp(offset.X, -maxOffset, maxOffset),
				Mathf.Clamp(offset.Y, -maxOffset, maxOffset)
			);

			layer.Position = _center + offset;
		}
	}

	public Vector2 GetCurrentMouseOffset()
	{
		return _currentOffset;
	}
}
