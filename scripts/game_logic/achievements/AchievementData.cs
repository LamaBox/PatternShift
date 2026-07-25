using Godot;
using System;

[GlobalClass]
public partial class AchievementData : Resource
{
    [Export] public string AchievementName { get; set; } = "Name of the Achievement";
    [Export] public string AchievementDescription { get; set; } = "Description of the achievement";

    [Export] public int RequiredScore { get; set; } = 0;
    [Export] public int RequiredStreak { get; set; } = 0;

    public bool IsUnlocked { get; set; } = false;
}