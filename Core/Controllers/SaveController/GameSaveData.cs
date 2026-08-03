using Godot;
using System;

[GlobalClass]
public partial class GameSaveData : Resource
{
    [Export] public int BestScore { get; set; } = 0;
    [Export] public int BestStreak { get; set; } = 0;

    [Export] public int TotalScore { get; set; } = 0;

    [Export] public int TotalGames { get; set; } = 0;
    [Export] public int TotalSets { get; set; } = 0;
    [Export] public int TotalMistakes { get; set; } = 0;

    [Export] public int UnlockedCardSkins { get; set; } = 0;

    [Export] public Godot.Collections.Array<string> Rewards { get; set; } = new();

    [Export] public Godot.Collections.Array<string> Achievements { get; set; } = new();
}
