using Godot;
using System;

[GlobalClass]
public partial class LinearStreakStrategy : BaseStreakStrategy
{
    [Export] public int PointsPerStreakStep { get; set; } = 5; //Прибавка за увеличение серии
    [Export] public int MaxMultiplier { get; set; } = 5; //Максимально возможное число прибавок от количества серии

    public override int CalculateScore(Score score, Streak streak)
    {
        if(streak.CurrentValue < 1) return score.CurrentValue;

        int resultMultiplier = Mathf.Clamp(streak.CurrentValue, 1, MaxMultiplier);

        return score.CurrentValue + (PointsPerStreakStep * resultMultiplier);
    }
}