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
            GD.PrintErr($"[GameController]: Р В РЎвЂєР РЋРІвЂљВ¬Р В РЎвЂР В Р’В±Р В РЎвЂќР В Р’В°! Р В РЎСџР РЋР вЂљР В РЎвЂўР В Р вЂ Р В Р’ВµР РЋР вЂљР РЋР РЏР РЋР РЏР РЋР С“Р РЋР Р‰ Р В РўвЂР В РЎвЂўР В Р’В»Р В Р’В¶Р В Р вЂ¦Р В РЎвЂў Р В Р’В±Р РЋРІР‚в„–Р РЋРІР‚С™Р РЋР Р‰ 3 Р В РЎвЂќР В Р’В°Р РЋР вЂљР РЋРІР‚С™Р РЋРІР‚в„–, Р В РЎвЂ”Р В РЎвЂўР В Р’В»Р РЋРЎвЂњР РЋРІР‚РЋР В Р’ВµР В Р вЂ¦Р В РЎвЂў: {(cards?.Count ?? 0)}");
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
        if (GameScore == null) GD.PrintErr($"[GameController] Р В РЎвЂєР РЋРІвЂљВ¬Р В РЎвЂР В Р’В±Р В РЎвЂќР В Р’В°: Р В РЎСљР В Р’Вµ Р В РЎвЂ”Р РЋР вЂљР В РЎвЂР В РЎвЂќР РЋР вЂљР В Р’ВµР В РЎвЂ”Р В Р’В»Р В Р’ВµР В Р вЂ¦ Р РЋРЎвЂњР В Р’В·Р В Р’ВµР В Р’В» Score Р В Р вЂ  Р В РЎвЂР В Р вЂ¦Р РЋР С“Р В РЎвЂ”Р В Р’ВµР В РЎвЂќР РЋРІР‚С™Р В РЎвЂўР РЋР вЂљР В Р’Вµ {Name}");
        if (GameStreak == null) GD.PrintErr($"[GameController] Р В РЎвЂєР РЋРІвЂљВ¬Р В РЎвЂР В Р’В±Р В РЎвЂќР В Р’В°: Р В РЎСљР В Р’Вµ Р В РЎвЂ”Р РЋР вЂљР В РЎвЂР В РЎвЂќР РЋР вЂљР В Р’ВµР В РЎвЂ”Р В Р’В»Р В Р’ВµР В Р вЂ¦ Р РЋРЎвЂњР В Р’В·Р В Р’ВµР В Р’В» Streak Р В Р вЂ  Р В РЎвЂР В Р вЂ¦Р РЋР С“Р В РЎвЂ”Р В Р’ВµР В РЎвЂќР РЋРІР‚С™Р В РЎвЂўР РЋР вЂљР В Р’Вµ {Name}");
        if (CardValidator == null) GD.PrintErr($"[GameController] Р В РЎвЂєР РЋРІвЂљВ¬Р В РЎвЂР В Р’В±Р В РЎвЂќР В Р’В°: Р В РЎСљР В Р’Вµ Р В РЎвЂ”Р РЋР вЂљР В РЎвЂР В РЎвЂќР РЋР вЂљР В Р’ВµР В РЎвЂ”Р В Р’В»Р В Р’ВµР В Р вЂ¦ Р РЋРЎвЂњР В Р’В·Р В Р’ВµР В Р’В» CardValidator Р В Р вЂ  Р В РЎвЂР В Р вЂ¦Р РЋР С“Р В РЎвЂ”Р В Р’ВµР В РЎвЂќР РЋРІР‚С™Р В РЎвЂўР РЋР вЂљР В Р’Вµ {Name}");
        if (StreakStrategy == null) GD.PrintErr($"[GameController] Р В РЎвЂєР РЋРІвЂљВ¬Р В РЎвЂР В Р’В±Р В РЎвЂќР В Р’В°: Р В РЎСљР В Р’Вµ Р В РЎвЂ”Р РЋР вЂљР В РЎвЂР В РЎвЂќР РЋР вЂљР В Р’ВµР В РЎвЂ”Р В Р’В»Р В Р’ВµР В Р вЂ¦ Р РЋРЎвЂњР В Р’В·Р В Р’ВµР В Р’В» StreakStrategy Р В Р вЂ  Р В РЎвЂР В Р вЂ¦Р РЋР С“Р В РЎвЂ”Р В Р’ВµР В РЎвЂќР РЋРІР‚С™Р В РЎвЂўР РЋР вЂљР В Р’Вµ {Name}");
        if (PenaltyStrategy == null) GD.PrintErr($"[GameController] Р В РЎвЂєР РЋРІвЂљВ¬Р В РЎвЂР В Р’В±Р В РЎвЂќР В Р’В°: Р В РЎС™Р В Р’Вµ Р В РЎвЂ”Р РЋР вЂљР В РЎвЂР В РЎвЂќР РЋР вЂљР В Р’ВµР В РЎвЂ”Р В Р’В»Р В Р’ВµР В Р вЂ¦ Р РЋРЎвЂњР В Р’В·Р В Р’ВµР В Р’В» PenaltyStrategy Р В Р вЂ  Р В РЎвЂР В Р вЂ¦Р РЋР С“Р В РЎвЂ”Р В Р’ВµР В РЎвЂќР РЋРІР‚С™Р В РЎвЂўР РЋР вЂљР В Р’Вµ {Name}");
    }
}