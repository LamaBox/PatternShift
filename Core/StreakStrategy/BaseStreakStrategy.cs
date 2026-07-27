using Godot;
using System;

[GlobalClass]
public abstract partial class BaseStreakStrategy : Node
{
    public abstract int CalculateScore(Score score, Streak streak);
}