using Godot;
using System;
using System.Runtime.CompilerServices;

[GlobalClass]
public partial class SaveController : Node
{
    private const string GameDataPath = "user://game_save.tres";
    private const string SettingsPath = "user://settings_save.tres";

    private static GameSaveData gameData;
    private static SettingsSaveData settingsData;

    public static GameSaveData GameData
    {
        get
        {
            if (gameData == null)
            {
                gameData = LoadGameData();
            }

            return gameData;
        }
    }

    public static SettingsSaveData SettingsData
    {
        get
        {
            if (settingsData == null)
                settingsData = LoadSettings();

            return settingsData;
        }
    }

    public static void SaveGameData(GameSaveData gameSaveData)
    {
        gameData = gameSaveData;

        Error result = ResourceSaver.Save(gameSaveData, GameDataPath);

        if (result != Error.Ok)
        {
            GD.PrintErr("Couldn't save game data!");
        }
    }

    public static GameSaveData LoadGameData()
    {
        if (!ResourceLoader.Exists(GameDataPath))
        {
            return new GameSaveData();
        }

        GameSaveData gameSaveData = ResourceLoader.Load<GameSaveData>(GameDataPath);

        if (gameSaveData == null)
        {
            return new GameSaveData();
        }

        return gameSaveData;
    }

    public static void SaveSettings(SettingsSaveData settingsSaveData)
    {
        settingsData = settingsSaveData;

        Error result = ResourceSaver.Save(settingsSaveData, SettingsPath);

        if (result != Error.Ok)
        {
            GD.PrintErr("Couldn't save settings!");
        }
    }

    public static SettingsSaveData LoadSettings()
    {
        if (!ResourceLoader.Exists(SettingsPath))
        {
            return new SettingsSaveData();
        }

        SettingsSaveData settingsSaveData = ResourceLoader.Load<SettingsSaveData>(SettingsPath);

        if (settingsSaveData == null)
        {
            return new SettingsSaveData();
        }

        return settingsSaveData;
    }
}
