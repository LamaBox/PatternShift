using Godot;
using System;

[GlobalClass]
public partial class AchievementSystem : Node
{
	[Signal] public delegate void AchievementUnlockedEventHandler(AchievementData achievement);

	public Score GameScore { get; set; }
	public Streak GameStreak { get; set; }

	[Export] public Godot.Collections.Array<AchievementData> AllAchievements { get; set; } = new();

	private GameSaveData saveData;

	public override void _Ready()
	{
		saveData = SaveController.LoadGameData();

		if (saveData == null)
		{
			saveData = new GameSaveData();
		}

		if (!saveData.Achievements.Contains("first_set"))
		{
			saveData.Achievements.Add("first_set");
		}
		
		if (!saveData.Achievements.Contains("points_1000"))
		{
			saveData.Achievements.Add("points_1000");
		}

		foreach (var achievement in AllAchievements)
		{
			achievement.IsUnlocked =
				saveData.Achievements.Contains(achievement.Id);
		}
	}

	public void Initialize(Score score, Streak streak)
	{
		if (GameScore != null)
			GameScore.ValueChanged -= OnScoreChanged;

		if (GameStreak != null)
			GameStreak.ValueChanged -= OnStreakChanged;

		GameScore = score;
		GameStreak = streak;

		if (GameScore != null)
			GameScore.ValueChanged += OnScoreChanged;

		if (GameStreak != null)
			GameStreak.ValueChanged += OnStreakChanged;
	}

	private void OnScoreChanged(int score)
	{
		CheckAchievements(AchievementType.Score, score);
	}

	private void OnStreakChanged(int streak)
	{
		CheckAchievements(AchievementType.Streak, streak);
	}

	private void UnlockAchievement(AchievementData achievement)
	{
		if (achievement.IsUnlocked)
			return;

		achievement.IsUnlocked = true;

		if (saveData != null && !saveData.Achievements.Contains(achievement.Id))
		{
			saveData.Achievements.Add(achievement.Id);
			SaveController.SaveGameData(saveData);
		}

		EmitSignal(SignalName.AchievementUnlocked, achievement);

		if (achievement.Type != AchievementType.AllAchievements)
		{
			CheckLegend();
		}
	}

	private void CheckAchievements(AchievementType type, int value = 1)
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

	public void CheckSkins(int totalSkins)
	{
		CheckAchievements(AchievementType.CardSkin, totalSkins);
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

	private void CheckLegend()
	{
		foreach (var achievement in AllAchievements)
		{
			if (achievement.Type == AchievementType.AllAchievements)
				continue;

			if (!achievement.IsUnlocked)
				return;
		}

		UnlockByType(AchievementType.AllAchievements);
	}
}
