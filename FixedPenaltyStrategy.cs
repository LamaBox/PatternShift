using Godot;
using System;

[GlobalClass]
public partial class FixedPenaltyStrategy : BasePenaltyStrategy
{
    [Export] public int PenaltyPoints { get; set; } = 10; //Фиксированное количество очков штрафа

    public override int CalculatePenalty(Score score, Streak streak)
    {
        return PenaltyPoints;
    }
}