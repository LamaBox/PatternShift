using Godot;

public partial class AchievementCard : PanelContainer
{
	[Export] private TextureRect icon;
	[Export] private Label nameLabel;
	[Export] private Label descriptionLabel;
	[Export] private Label statusLabel;

	public void Setup(AchievementData achievement)
	{
		if (achievement == null)
			return;

		icon.Texture = achievement.Icon;
		nameLabel.Text = achievement.AchievementName;
		descriptionLabel.Text = achievement.AchievementDescription;

		GD.Print(
			$"CARD: {achievement.Id}, " +
			$"Name={achievement.AchievementName}, " +
			$"Unlocked={achievement.IsUnlocked}"
		);

		if (achievement.IsUnlocked)
		{
			statusLabel.Text = "Разблокировано!";
			statusLabel.AddThemeColorOverride(
				"font_color",
				new Color("#4CFF6A")
			);

			Modulate = Colors.White;
		}
		else
		{
			statusLabel.Text = "Заблокировано!";
			statusLabel.AddThemeColorOverride(
				"font_color",
				Colors.White
			);

			Modulate = new Color(1f, 1f, 1f, 0.5f);
		}
	}
}
