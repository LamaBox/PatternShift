using Godot;
using System;

[GlobalClass]
public partial class AchievementData : Resource
{
    [Export] public string AchievementName { get; set; } = "Name of the Achievement";
    [Export] public string AchievementDescription { get; set; } = "Description of the achievement";
    
    [Export] public AchievementType Type { get; set; }

    [Export] public int TargetValue { get; set; }

    [Export] public bool IsUnlocked { get; set; }

    [Export] public Texture2D Icon { get; set; }
}