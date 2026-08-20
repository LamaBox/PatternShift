using Godot;
using System;

public partial class PlayGuide : Control
{
	[Export] private Button _closeButton;
	public override void _Ready()
	{
		_closeButton.Pressed += OnCloseButtonPressed;
	}

	private void OnCloseButtonPressed()
	{
		QueueFree();
	}
}
