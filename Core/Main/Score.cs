using Godot;
using System;

[GlobalClass]
public partial class Score : Node
{
	[Signal] public delegate void ValueChangedEventHandler(int newValue);

    [Export] public int maxValue = int.MaxValue;
	[Export] public int minValue = 0;
	[Export] public int startValue = 0;

	public int CurrentValue { get; set; }
	public int HighScore { get; set; }

	public override void _Ready()
	{
		InitializeScore();
	}

	private void InitializeScore()
	{
        CurrentValue = startValue;
        HighScore = startValue;
	}

	public void Modify(int amount)
	{
        CurrentValue = Mathf.Clamp(CurrentValue + amount, minValue, maxValue);

		if (CurrentValue > HighScore)
		{
            HighScore = CurrentValue;
		}

        EmitSignal(SignalName.ValueChanged, CurrentValue);
    }

	public void ResetCurrentValue() => CurrentValue = startValue;
	public void ResetHighScore() => HighScore = CurrentValue;
}
