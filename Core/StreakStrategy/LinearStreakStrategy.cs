using Godot;
using System;

[GlobalClass]
public partial class LinearStreakStrategy : BaseStreakStrategy
{
	[Export] public int PointsPerStreakStep { get; set; } = 5;
	[Export] public int MaxMultiplier { get; set; } = 5;

	public override int CalculateScore(Score score, Streak streak)
	{
		if(streak.CurrentValue <= 1) return score.CurrentValue;

		int resultMultiplier = Mathf.Clamp(streak.CurrentValue, 1, MaxMultiplier);

		return PointsPerStreakStep * resultMultiplier;
	}
}
