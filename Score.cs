using Godot;
using System;

[GlobalClass]
public partial class Score : Node
{
	[Export] public int maxValue = int.MaxValue; //Максимальное значение очков, настраивается в редакторе Godot
	[Export] public int minValue = 0; //Минимальное значение очков, настраивается в редакторе Godot
	[Export] public int startValue = 0; //Стартовое значение очков, настраивается в редакторе Godot

	public int currentValue; //Текущее значение очков, доступно для чтения и записи
	public int highScore; //Максимальное значение очков, доступно для чтения и записи

	public override void _Ready()
	{
		InitializeScore();
	}

	private void InitializeScore()
	{
		currentValue = startValue;
		highScore = startValue;
	}

	//Метод изменению счетчика очков
	public void Modify(int amount)
	{
		currentValue = Mathf.Clamp(currentValue + amount, minValue, maxValue); //Присваивание значение с учётом ограничений minValue и maxValuе,
																			   //если текущее значение выходит за пределы,
																			   //то оно будет установлено в ближайшее ограничение

		if (currentValue > highScore)
		{
			highScore = currentValue; //Обновляется рекорд, если текущее значение очков превышает рекордное значение
		}
	}

	public void ResetCurrentValue() => currentValue = startValue; //Сброс текущего значения очков к стартовому значению

	public void ResetHighScore() => highScore = currentValue; //Сброс рекордного значения очков к текущему значению
}
