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
            GameScore.ValueChanged += score => CheckAchievements(AchievementType.Score, score);

        if (GameStreak != null)
            GameStreak.ValueChanged += streak => CheckAchievements(AchievementType.Streak, streak);
    }

    private void UnlockAchievement(AchievementData achievement)
    {
        if (achievement.IsUnlocked)
            return;

        achievement.IsUnlocked = true;

        // Сохраняем факт разблокировки в сохранениях
        var save = SaveController.LoadGameData();
        if (save != null && !save.Achievements.Contains(achievement.AchievementName))
        {
            save.Achievements.Add(achievement.AchievementName);
            SaveController.SaveGameData(save);
        }

        EmitSignal(SignalName.AchievementUnlocked, achievement);
    }

    private void CheckAchievements(AchievementType type, int value)
    {
        foreach (var achievement in AllAchievements)
        {
            if (achievement.IsUnlocked)
                continue;

            if (achievement.Type != type)
                continue;

            if (value >= achievement.TargetValue)
                UnlockAchievement(achievement);
        }
    }

    public void CheckTotalSets(int totalSets)
    {
        CheckAchievements(AchievementType.TotalSets, totalSets);
    }

    public void CheckStreak(int streak)
    {
        CheckAchievements(AchievementType.Streak, streak);
    }

    public void CheckScore(int score)
    {
        CheckAchievements(AchievementType.Score, score);
    }

    public void UnlockByType(AchievementType type)
    {
        foreach (var achievement in AllAchievements)
        {
            if (!achievement.IsUnlocked && achievement.Type == type)
                UnlockAchievement(achievement);
        }
    }
}