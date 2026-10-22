using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class RewardSystem : Node
{
    [Signal] public delegate void RewardUnlockedEventHandler(RewardData reward);

    [Export] public Score GameScore { get; set; }
    [Export] public Streak GameStreak { get; set; }

    [Export] public Godot.Collections.Array<RewardData> AllRewards { get; set; } = new();

    public override void _Ready()
    {
        if (GameScore != null)
        {
            GameScore.ValueChanged += CheckScoreRewards;
        }
        if (GameStreak != null)
        {
            GameStreak.ValueChanged += CheckStreakRewards;
        }
    }

    private void UnlockReward(RewardData reward)
    {
        if (reward.IsUnlocked)
            return;

        reward.IsUnlocked = true;

        var save = SaveController.LoadGameData();
        if (save != null && !save.Rewards.Contains(reward.RewardName))
        {
            save.Rewards.Add(reward.RewardName);
            SaveController.SaveGameData(save);
        }

        EmitSignal(SignalName.RewardUnlocked, reward);
    }

    private void CheckScoreRewards(int currentScore)
    {
        foreach (var reward in AllRewards)
        {
            if (!reward.IsUnlocked && reward.RequiredScore > 0 && currentScore >= reward.RequiredScore)
            {
                UnlockReward(reward);
            }
        }
    }

    private void CheckStreakRewards(int currentStreak)
    {
        foreach (var reward in AllRewards)
        {
            if (!reward.IsUnlocked && reward.RequiredStreak > 0 && currentStreak >= reward.RequiredStreak)
            {
                UnlockReward(reward);
            }
        }
    }
}