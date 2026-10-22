using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class GameController : Node
{
    [Signal] public delegate void SetCheckSuccessEventHandler(Godot.Collections.Array<CardData> checkedCards);
    [Signal] public delegate void SetCheckFailedEventHandler();
    [Signal] public delegate void TimeChangedEventHandler(int remainingTime);
    [Signal] public delegate void GameFinishedEventHandler();

    [Export] public int ScoreForOneSet { get; set; } = 0;
    [Export] public int GameDuration { get; set; } = 300;

    public int RemainingTime { get; private set; }

    private bool _isTimerPaused = false;

    [Export] public Score GameScore { get; set; }
    [Export] public Streak GameStreak { get; set; }
    [Export] public Timer GameTimer { get; set; }

    public int SetsThisGame { get; private set; } = 0;
    public int MistakesThisGame { get; private set; } = 0;

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

        GameTimer.Timeout += OnTimerTimeout;

        StartGameTimer();
    }

    public void ProcessSelectedSet(List<CardData> cards)
    {
        if (cards == null || cards.Count != 3)
        {
            GD.PrintErr($"[GameController]: The number of cards is insufficient to check for a set. Cards transferred: {(cards?.Count ?? 0)}");
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

        SetsThisGame++;
        SaveData.TotalSets++;

        gameAchievementSystem.CheckTotalSets(SaveData.TotalSets);
        gameAchievementSystem.CheckScore(GameScore.CurrentValue);
        gameAchievementSystem.CheckStreak(GameStreak.CurrentValue);

        if (SaveData.TotalSets == 1)
        {
            gameAchievementSystem.UnlockByType(AchievementType.FirstSet);
        }

        if (RemainingTime >= (GameDuration - 5))
        {
            gameAchievementSystem.CheckAchievements(AchievementType.FastSet);
        }
        if (RemainingTime >= (GameDuration - 15))
        {
            gameAchievementSystem.CheckAchievements(AchievementType.FastSet, SetsThisGame);
        }

        var cardArray = new Godot.Collections.Array<CardData>(cards);
        EmitSignal(SignalName.SetCheckSuccess, cardArray);
    }

    private void HandleFailedSet()
    {
        int penalty = PenaltyStrategy.CalculatePenalty(GameScore, GameStreak);
        GameScore.Modify(-penalty);
        GameStreak.ResetCurrentValue();

        MistakesThisGame++;
        SaveData.TotalMistakes++;
        SaveController.SaveGameData(SaveData);

        EmitSignal(SignalName.SetCheckFailed);
    }

    public (bool scoreRecord, bool streakRecord, bool PatternsRecord) FinishGame()
    {
        bool scoreRecord = GameScore.CurrentValue > SaveData.BestScore;
        bool streakRecord = GameStreak.MaxStreak > SaveData.BestStreak;
        bool patternsRecord = SetsThisGame > SaveData.BestPatterns;

        SaveData.TotalGames++;
        SaveData.TotalScore += GameScore.CurrentValue;

        if (scoreRecord)
            SaveData.BestScore = GameScore.CurrentValue;

        if (streakRecord)
            SaveData.BestStreak = GameStreak.MaxStreak;

        if (patternsRecord)
            SaveData.BestPatterns = SetsThisGame;

        SaveData.BestScore = Math.Max(SaveData.BestScore, GameScore.CurrentValue);
        SaveData.BestStreak = Math.Max(SaveData.BestStreak, GameStreak.MaxStreak);
        SaveData.BestPatterns = Math.Max(SaveData.BestPatterns, SetsThisGame);

        if (SaveData.TotalGames >= 1)
        {
            gameAchievementSystem.UnlockByType(AchievementType.FirstGame);
        }

        if (MistakesThisGame == 0)
        {
            gameAchievementSystem.UnlockByType(AchievementType.NoMistakes);
        }

        SaveController.SaveGameData(SaveData);
        return (scoreRecord, streakRecord, patternsRecord);
    }

    private void ValidateDependencies()
    {
        if (GameScore == null) GD.PrintErr($"[GameController] Error: Score node not connected in the Inspector {Name}");
        if (GameStreak == null) GD.PrintErr($"[GameController] Error: Streak node not connected in the Inspector {Name}");
        if (CardValidator == null) GD.PrintErr($"[GameController] Error: CardValidator node not connected in the Inspector {Name}");
        if (StreakStrategy == null) GD.PrintErr($"[GameController] Error: StreakStrategy node not connected in the Inspector {Name}");
        if (PenaltyStrategy == null) GD.PrintErr($"[GameController] Error: PenaltyStrategy node not connected in the Inspector {Name}");
    }
    public void StartGameTimer()
    {
        RemainingTime = GameDuration;

        GameTimer.Start();

        EmitSignal( SignalName.TimeChanged, RemainingTime);
    }

    private void OnTimerTimeout()
    {
        RemainingTime--;

        if (RemainingTime <= 0)
        {
            RemainingTime = 0;
            GameTimer.Stop();

            EmitSignal(SignalName.TimeChanged, RemainingTime);

            EmitSignal(SignalName.GameFinished);
            return;
        }

        EmitSignal(SignalName.TimeChanged, RemainingTime);
    }

    public void PauseGameTimer()
    {
        if (GameTimer == null || GameTimer.IsStopped())
            return;

        _isTimerPaused = true;
        GameTimer.Paused = true;
    }

    public void ResumeGameTimer()
    {
        if (GameTimer == null)
            return;

        _isTimerPaused = false;
        GameTimer.Paused = false;
    }
}