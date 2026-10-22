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

	private AudioStreamPlayer _achievementSound;

	public override void _Ready()
	{
		saveData = SaveController.GameData;

		_achievementSound = new AudioStreamPlayer();
		_achievementSound.Stream = ResourceLoader.Load<AudioStream>("res://sounds/unlock_achievement.mp3");

		AddChild(_achievementSound);

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

		if (_achievementSound != null)
		{
			_achievementSound.Play();
			GD.Print(achievement.AchievementName);
		}

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

	public void CheckAchievements(AchievementType type, int value = 1)
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
		GD.Print($"=== UnlockByType: {type} ===");
		GD.Print($"AllAchievements count: {AllAchievements.Count}");

		foreach (var achievement in AllAchievements)
		{
			GD.Print(
				$"Achievement: Id={achievement.Id}, " +
				$"Type={achievement.Type}, " +
				$"Unlocked={achievement.IsUnlocked}"
			);

			if (!achievement.IsUnlocked &&
				achievement.Type == type)
			{
				GD.Print($"FOUND! Unlocking: {achievement.Id}");

				UnlockAchievement(achievement);
			}
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
