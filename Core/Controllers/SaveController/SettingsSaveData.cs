using Godot;
using System;

[GlobalClass]
public partial class SettingsSaveData : Resource
{
    [Export] public int MusicVolume { get; set; } = 100;
    [Export] public int SoundsVolume { get; set; } = 100;
}