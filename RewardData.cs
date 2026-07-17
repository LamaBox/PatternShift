using Godot;
using System;

[GlobalClass]
public partial class RewardData : Resource
{
    [Export] public string RewardName { get; set; } = "Название награды";
    [Export] public string RewardDescription { get; set; } = "Описание условия получения награды";

    [Export] public int RequiredScore { get; set; } = 0;
    [Export] public int RequiredStreak { get; set; } = 0;

    public bool IsUnlocked { get; set; } = false;
}