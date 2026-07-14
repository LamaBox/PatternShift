using Godot;
using System;

[GlobalClass]
public partial class Streak : Node
{
	[Export] public int maxValue = int.MaxValue; //Максимальное значение серии, настраивается в редакторе Godot
	[Export] public int minValue = 0; //Минимальное значение серии, настраивается в редакторе Godot
	[Export] public int startValue = 0; //Стартовое значение серии, настраивается в редакторе Godot

	public int currentValue; //Текущее значение серии, доступно для чтения и записи
	public int maxStreak; //Максимальное значение серии, доступно для чтения и записи

	public override void _Ready()
	{
		InitializeScore();
	}

	private void InitializeScore()
	{
		currentValue = startValue;
		maxStreak = startValue;
	}

	public void Increment()
	{
		currentValue = Mathf.Clamp(currentValue + 1, minValue, maxValue); //Увеличение серии на 1 в пределах ограничений

		if (currentValue > maxStreak)
		{
			maxStreak = currentValue; //Обновление рекорда, если текущее значение его превышает
		}
	}

	public void ResetCurrentValue() => currentValue = startValue; //Сброс текущего значения серии к стартовому значению

	public void ResetMaxStreak() => maxStreak = currentValue; //Сброс рекордного значения серии к текущему значению
}
