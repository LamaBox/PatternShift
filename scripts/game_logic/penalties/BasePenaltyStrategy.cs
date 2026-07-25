using Godot;
using System;

[GlobalClass]
public abstract partial class BasePenaltyStrategy : Node
{
    public abstract int CalculatePenalty(Score score, Streak streak);
}