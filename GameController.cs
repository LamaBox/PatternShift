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

    public override void _Ready()
    {
        ValidateDependencies();
    }

    public void ProcessSelectedSet(List<CardData> cards)
    {
        if (cards == null || cards.Count != 3)
        {
            GD.PrintErr($"[GameController]: Ошибка! Ожидалось 3 карты для проверки, получено: {(cards?.Count ?? 0)}");
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

        var cardArray = new Godot.Collections.Array<CardData>(cards);
        EmitSignal(SignalName.SetCheckSuccess, cardArray);
    }

    private void HandleFailedSet()
    {
        int penalty = PenaltyStrategy.CalculatePenalty(GameScore, GameStreak);
        GameScore.Modify(-penalty);
        GameStreak.ResetCurrentValue();
        EmitSignal(SignalName.SetCheckFailed);
    }

    private void ValidateDependencies()
    {
        if (GameScore == null) GD.PrintErr($"[GameController] Ошибка: Не прикреплен узел Score в инспекторе {Name}");
        if (GameStreak == null) GD.PrintErr($"[GameController] Ошибка: Не прикреплен узел Streak в инспекторе {Name}");
        if (CardValidator == null) GD.PrintErr($"[GameController] Ошибка: Не прикреплен узел CardValidator в инспекторе {Name}");
        if (StreakStrategy == null) GD.PrintErr($"[GameController] Ошибка: Не прикреплен узел StreakStrategy в инспекторе {Name}");
        if (PenaltyStrategy == null) GD.PrintErr($"[GameController] Ошибка: Не прикреплен узел PenaltyStrategy в инспекторе {Name}");
    }
}