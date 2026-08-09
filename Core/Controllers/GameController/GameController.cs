using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class GameController : Node
{
    [Signal] public delegate void SetCheckSuccessEventHandler(Godot.Collections.Array<CardData> checkedCards);
    [Signal] public delegate void SetCheckFailedEventHandler();

    [Export] public int ScoreForOneSet { get; set; } = 0;

    [Export] public Score GameScore { get; set; }
    [Export] public Streak GameStreak { get; set; }

    [Export] public BaseCardValidator CardValidator { get; set; }
    [Export] public BaseStreakStrategy StreakStrategy { get; set; }
    [Export] public BasePenaltyStrategy PenaltyStrategy { get; set; }

    private AchievementSystem gameAchievementSystem;

    private GameSaveData SaveData;

    public override void _Ready()
    {
        ValidateDependencies();

        SaveData = SaveController.GameData;

        gameAchievementSystem = GetNode<AchievementSystem>("/root/Achievements");

        gameAchievementSystem.Initialize(GameScore, GameStreak);
    }

    public void ProcessSelectedSet(List<CardData> cards)
    {
        if (cards == null || cards.Count != 3)
        {
            GD.PrintErr($"[GameController]: Р С›РЎв‚¬Р С‘Р В±Р С”Р В°! Р СџРЎР‚Р С•Р Р†Р ВµРЎР‚РЎРЏРЎРЏРЎРѓРЎРЉ Р Т‘Р С•Р В»Р В¶Р Р…Р С• Р В±РЎвЂ№РЎвЂљРЎРЉ 3 Р С”Р В°РЎР‚РЎвЂљРЎвЂ№, Р С—Р С•Р В»РЎС“РЎвЂЎР ВµР Р…Р С•: {(cards?.Count ?? 0)}");
            return;
        }

        bool isSetValid = CardValidator.Validate(cards);

        if (isSetValid)
        {
            HandleSuccessfulSet(cards);
        }
        else
        {
            HandleFailedSet();
        }
    }

    private void HandleSuccessfulSet(List<CardData> cards)
    {
        GameStreak.Increment();
        int streakBonus = StreakStrategy.CalculateScore(GameScore, GameStreak);
        GameScore.Modify(ScoreForOneSet + streakBonus);

        SaveData.TotalSets++;

        gameAchievementSystem.CheckTotalSets(SaveData.TotalSets);

        if (SaveData.TotalSets == 1)
        {
            gameAchievementSystem.UnlockByType(AchievementType.FirstSet);
        }

        SaveData.BestScore = Math.Max(SaveData.BestScore, GameScore.HighScore);
        SaveData.BestStreak = Math.Max(SaveData.BestStreak, GameStreak.MaxStreak);
        SaveController.SaveGameData(SaveData);

        var cardArray = new Godot.Collections.Array<CardData>(cards);
        EmitSignal(SignalName.SetCheckSuccess, cardArray);
    }

    private void HandleFailedSet()
    {
        int penalty = PenaltyStrategy.CalculatePenalty(GameScore, GameStreak);
        GameScore.Modify(-penalty);
        GameStreak.ResetCurrentValue();

        SaveData.TotalMistakes++;
        SaveController.SaveGameData(SaveData);

        EmitSignal(SignalName.SetCheckFailed);
    }

    public (bool scoreRecord, bool streakRecord) FinishGame()
    {
        bool scoreRecord = GameScore.CurrentValue > SaveData.BestScore;
        bool streakRecord = GameStreak.MaxStreak > SaveData.BestStreak;

        SaveData.TotalGames++;
        SaveData.TotalScore += GameScore.CurrentValue;

        SaveData.BestScore = Math.Max(SaveData.BestScore, GameScore.CurrentValue);
        SaveData.BestStreak = Math.Max(SaveData.BestStreak, GameStreak.MaxStreak);

        if (SaveData.TotalGames >= 1)
        {
            gameAchievementSystem.UnlockByType(AchievementType.FirstGame);
            GD.Print($"[GameController] unlock first game achievement");
        }

        SaveController.SaveGameData(SaveData);
        return (scoreRecord, streakRecord);
    }

    private void ValidateDependencies()
    {
        if (GameScore == null) GD.PrintErr($"[GameController] Р С›РЎв‚¬Р С‘Р В±Р С”Р В°: Р СњР Вµ Р С—РЎР‚Р С‘Р С”РЎР‚Р ВµР С—Р В»Р ВµР Р… РЎС“Р В·Р ВµР В» Score Р Р† Р С‘Р Р…РЎРѓР С—Р ВµР С”РЎвЂљР С•РЎР‚Р Вµ {Name}");
        if (GameStreak == null) GD.PrintErr($"[GameController] Р С›РЎв‚¬Р С‘Р В±Р С”Р В°: Р СњР Вµ Р С—РЎР‚Р С‘Р С”РЎР‚Р ВµР С—Р В»Р ВµР Р… РЎС“Р В·Р ВµР В» Streak Р Р† Р С‘Р Р…РЎРѓР С—Р ВµР С”РЎвЂљР С•РЎР‚Р Вµ {Name}");
        if (CardValidator == null) GD.PrintErr($"[GameController] Р С›РЎв‚¬Р С‘Р В±Р С”Р В°: Р СњР Вµ Р С—РЎР‚Р С‘Р С”РЎР‚Р ВµР С—Р В»Р ВµР Р… РЎС“Р В·Р ВµР В» CardValidator Р Р† Р С‘Р Р…РЎРѓР С—Р ВµР С”РЎвЂљР С•РЎР‚Р Вµ {Name}");
        if (StreakStrategy == null) GD.PrintErr($"[GameController] Р С›РЎв‚¬Р С‘Р В±Р С”Р В°: Р СњР Вµ Р С—РЎР‚Р С‘Р С”РЎР‚Р ВµР С—Р В»Р ВµР Р… РЎС“Р В·Р ВµР В» StreakStrategy Р Р† Р С‘Р Р…РЎРѓР С—Р ВµР С”РЎвЂљР С•РЎР‚Р Вµ {Name}");
        if (PenaltyStrategy == null) GD.PrintErr($"[GameController] Р С›РЎв‚¬Р С‘Р В±Р С”Р В°: Р СљР Вµ Р С—РЎР‚Р С‘Р С”РЎР‚Р ВµР С—Р В»Р ВµР Р… РЎС“Р В·Р ВµР В» PenaltyStrategy Р Р† Р С‘Р Р…РЎРѓР С—Р ВµР С”РЎвЂљР С•РЎР‚Р Вµ {Name}");
    }
}