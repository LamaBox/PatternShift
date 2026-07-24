using Godot;
using System;

[GlobalClass]
public partial class AchievementSystem : Node
{
    [Signal] public delegate void AchievementUnlockedEventHandler(AchievementData achievement);

    [Export] public Score GameScore { get; set; }
    [Export] public Streak GameStreak { get; set; }

    [Export] public Godot.Collections.Array<AchievementData> AllAchievements { get; set; } = new();

    public override void _Ready()
    {
        if (GameScore != null)
        {
            GameScore.ValueChanged += CheckScoreAchievements;
        }
        if (GameStreak != null)
        {
            GameStreak.ValueChanged += CheckStreakAchievements;
        }
    }

    private void UnlockAchievement(AchievementData achievement)
    {
        achievement.IsUnlocked = true;

        EmitSignal(SignalName.AchievementUnlocked, achievement);
    }

    private void CheckScoreAchievements(int currentScore)
    {
        foreach (var achievement in AllAchievements)
        {
            if (!achievement.IsUnlocked && achievement.RequiredScore > 0 && currentScore >= achievement.RequiredScore)
            {
                UnlockAchievement(achievement);
            }
        }
    }

    private void CheckStreakAchievements(int currentStreak)
    {
        foreach (var achievement in AllAchievements)
        {
            if (!achievement.IsUnlocked && achievement.RequiredStreak > 0 && currentStreak >= achievement.RequiredStreak)
            {
                UnlockAchievement(achievement);
            }
        }
    }
}