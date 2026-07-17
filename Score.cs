using Godot;
using System;

[GlobalClass]
public partial class Score : Node
{
	[Signal] public delegate void ValueChangedEventHandler(int newValue);

    [Export] public int maxValue = int.MaxValue; //РњР°РєСЃРёРјР°Р»СЊРЅРѕРµ Р·РЅР°С‡РµРЅРёРµ РѕС‡РєРѕРІ, РЅР°СЃС‚СЂР°РёРІР°РµС‚СЃСЏ РІ СЂРµРґР°РєС‚РѕСЂРµ Godot
	[Export] public int minValue = 0; //РњРёРЅРёРјР°Р»СЊРЅРѕРµ Р·РЅР°С‡РµРЅРёРµ РѕС‡РєРѕРІ, РЅР°СЃС‚СЂР°РёРІР°РµС‚СЃСЏ РІ СЂРµРґР°РєС‚РѕСЂРµ Godot
	[Export] public int startValue = 0; //РЎС‚Р°СЂС‚РѕРІРѕРµ Р·РЅР°С‡РµРЅРёРµ РѕС‡РєРѕРІ, РЅР°СЃС‚СЂР°РёРІР°РµС‚СЃСЏ РІ СЂРµРґР°РєС‚РѕСЂРµ Godot

	public int CurrentValue { get; set; } //РўРµРєСѓС‰РµРµ Р·РЅР°С‡РµРЅРёРµ РѕС‡РєРѕРІ, РґРѕСЃС‚СѓРїРЅРѕ РґР»СЏ С‡С‚РµРЅРёСЏ Рё Р·Р°РїРёСЃРё
	public int HighScore { get; set; } //РњР°РєСЃРёРјР°Р»СЊРЅРѕРµ Р·РЅР°С‡РµРЅРёРµ РѕС‡РєРѕРІ, РґРѕСЃС‚СѓРїРЅРѕ РґР»СЏ С‡С‚РµРЅРёСЏ Рё Р·Р°РїРёСЃРё

	public override void _Ready()
	{
		InitializeScore();
	}

	private void InitializeScore()
	{
        CurrentValue = startValue;
        HighScore = startValue;
	}

	//РњРµС‚РѕРґ РёР·РјРµРЅРµРЅРёСЋ СЃС‡РµС‚С‡РёРєР° РѕС‡РєРѕРІ
	public void Modify(int amount)
	{
        CurrentValue = Mathf.Clamp(CurrentValue + amount, minValue, maxValue); //РџСЂРёСЃРІР°РёРІР°РЅРёРµ Р·РЅР°С‡РµРЅРёРµ СЃ СѓС‡С‘С‚РѕРј РѕРіСЂР°РЅРёС‡РµРЅРёР№ minValue Рё maxValuРµ,
																			   //РµСЃР»Рё С‚РµРєСѓС‰РµРµ Р·РЅР°С‡РµРЅРёРµ РІС‹С…РѕРґРёС‚ Р·Р° РїСЂРµРґРµР»С‹,
																			   //С‚Рѕ РѕРЅРѕ Р±СѓРґРµС‚ СѓСЃС‚Р°РЅРѕРІР»РµРЅРѕ РІ Р±Р»РёР¶Р°Р№С€РµРµ РѕРіСЂР°РЅРёС‡РµРЅРёРµ

		if (CurrentValue > HighScore)
		{
            HighScore = CurrentValue; //РћР±РЅРѕРІР»СЏРµС‚СЃСЏ СЂРµРєРѕСЂРґ, РµСЃР»Рё С‚РµРєСѓС‰РµРµ Р·РЅР°С‡РµРЅРёРµ РѕС‡РєРѕРІ РїСЂРµРІС‹С€Р°РµС‚ СЂРµРєРѕСЂРґРЅРѕРµ Р·РЅР°С‡РµРЅРёРµ
		}

        EmitSignal(SignalName.ValueChanged, CurrentValue);
    }

	public void ResetCurrentValue() => CurrentValue = startValue; //РЎР±СЂРѕСЃ С‚РµРєСѓС‰РµРіРѕ Р·РЅР°С‡РµРЅРёСЏ РѕС‡РєРѕРІ Рє СЃС‚Р°СЂС‚РѕРІРѕРјСѓ Р·R
	public void ResetHighScore() => HighScore = CurrentValue; //РЎР±СЂРѕСЃ СЂРµРєРѕСЂРґНё Р·R
}
