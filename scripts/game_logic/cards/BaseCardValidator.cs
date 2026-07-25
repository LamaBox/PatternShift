using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public abstract partial class BaseCardValidator : Node
{
    public abstract bool Validate(List<CardData> cardList);
}
