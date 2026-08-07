using Godot;

public partial class Achievements : Control
{
	[Export] private Button backButton;
	[Export] private VBoxContainer achievementsContainer;

	private AchievementSystem achievementSystem;

	public override void _Ready()
	{
		backButton.Pressed += OnBackButtonPressed;

		achievementSystem =
			GetNode<AchievementSystem>("/root/Achievements");

		CreateAchievementCards();
	}

	private void CreateAchievementCards()
	{
		var cardScene = GD.Load<PackedScene>("res://scenes/ui/achievements/AchievementCard.tscn");

		var achievements = new System.Collections.Generic.List<AchievementData>();

		foreach (var achievement in achievementSystem.AllAchievements)
		{
			achievements.Add(achievement);
		}

		achievements.Sort((a, b) => b.IsUnlocked.CompareTo(a.IsUnlocked));

		foreach (var achievement in achievements)
		{
			var card = cardScene.Instantiate<AchievementCard>();

			achievementsContainer.AddChild(card);
			card.Setup(achievement);
		}
	}

	private void OnBackButtonPressed()
	{
		QueueFree();
	}
}
