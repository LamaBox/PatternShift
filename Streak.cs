using Godot;
using System;

[GlobalClass]
public partial class Streak : Node
{
    [Signal] public delegate void ValueChangedEventHandler(int newValue);

    [Export] public int maxValue = int.MaxValue; //РњР°РєСЃРёРјР°Р»СЊРЅРѕРµ Р·РЅР°С‡РµРЅРёРµ СЃРµСЂРёРё, РЅР°СЃС‚СЂР°РёРІР°РµС‚СЃСЏ РІ СЂРµРґР°РєС‚РѕСЂРµ Godot
	[Export] public int minValue = 0; //РњРёРЅРёРјР°Р»СЊРЅРѕРµ Р·РЅР°С‡РµРЅРёРµ СЃРµСЂРёРё, РЅР°СЃС‚СЂР°РёРІР°РµС‚СЃСЏ РІ СЂРµРґР°РєС‚РѕСЂРµ Godot
	[Export] public int startValue = 0; //РЎС‚Р°СЂС‚РѕРІРѕРµ Р·РЅР°С‡РµРЅРёРµ СЃРµСЂРёРё, РЅР°СЃС‚СЂР°РёРІР°РµС‚СЃСЏ РІ СЂРµРґР°РєС‚РѕСЂРµ Godot

	public int CurrentValue { get; set; } //РўРµРєСѓС‰РµРµ Р·РЅР°С‡РµРЅРёРµ СЃРµСЂРёРё, РґРѕСЃС‚СѓРїРЅРѕ РґР»СЏ С‡С‚РµРЅРёСЏ Рё Р·Р°РїРёСЃРё
	public int MaxStreak { get; set; } //РњР°РєСЃРёРјР°Р»СЊРЅРѕРµ Р·РЅР°С‡РµРЅРёРµ СЃРµСЂРёРё, РґРѕСЃС‚СѓРїРЅРѕ РґР»СЏ С‡С‚РµРЅРёСЏ Рё Р·Р°РїРёСЃРё

	public override void _Ready()
	{
		InitializeScore();
	}

	private void InitializeScore()
	{
        CurrentValue = startValue;
        MaxStreak = startValue;
	}

	public void Increment()
	{
        CurrentValue = Mathf.Clamp(CurrentValue + 1, minValue, maxValue); //РЈРІРµР»РёС‡РµРЅРёРµ СЃРµСЂРёРё РЅР° 1 РІ РїСЂРµРґРµР»Р°С… РѕРіСЂР°РЅРёС‡РµРЅРёР№

		if (CurrentValue > MaxStreak)
		{
            MaxStreak = CurrentValue; //РћР±РЅРѕРІР»РµРЅРёРµ СЂРµРєРѕСЂРґР°, РµСЃР»Рё С‚РµРєСѓС‰РµРµ Р·РЅР°С‡РµРЅРёРµ РµРіРѕ РїСЂРµРІС‹С€Р°РµС‚
		}

        EmitSignal(SignalName.ValueChanged, CurrentValue);
    }

	public void ResetCurrentValue() => CurrentValue = startValue; //РЎР±СЂРѕСЃ С‚РµРєСѓС‰РµРіРѕ Р·РЅР°С‡РµРЅРёСЏ СЃРµСЂРёРё Рє СЃС‚Р°СЂС‚РѕРІРѕРјСѓ Р·РЅР°С‡РµРЅРёСЋ

	public void ResetMaxStreak() => MaxStreak = CurrentValue; //РЎР±СЂРѕСЃ СЂРµРєРѕСЂРґРЅРѕРіРѕ Р·РЅР°С‡РµРЅРёСЏ СЃРµСЂРёРё Рє С‚РµРєСѓС‰РµРјСѓ Р·РЅР°С‡РµРЅРёСЋ
}
