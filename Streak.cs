using Godot;
using System;

[GlobalClass]
public partial class Streak : Node
{
    [Signal] public delegate void ValueChangedEventHandler(int newValue);

    [Export] public int maxValue = int.MaxValue;
	[Export] public int minValue = 0;
	[Export] public int startValue = 0;

	public int CurrentValue { get; set; }
	public int MaxStreak { get; set; }

	public override void _Ready()
	{
		InitializeScore();
	}

	private void InitializeScore()
	{
        CurrentValue = startValue;
        MaxStreak = startValue;
	}

	public void Increment()
	{
        CurrentValue = Mathf.Clamp(CurrentValue + 1, minValue, maxValue);

		if (CurrentValue > MaxStreak)
		{
            MaxStreak = CurrentValue;
		}

        EmitSignal(SignalName.ValueChanged, CurrentValue);
    }

	public void ResetCurrentValue() => CurrentValue = startValue;

	public void ResetMaxStreak() => MaxStreak = CurrentValue;
}
