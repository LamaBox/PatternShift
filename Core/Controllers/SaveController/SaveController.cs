using Godot;
using System;

[GlobalClass]
public partial class SaveController : Node
{
    private const string GameDataPath = "user://game_save.tres";
    private const string SettingsPath = "user://settings.cfg";

    private static GameSaveData gameData;

    public static GameSaveData GameData
    {
        get
        {
            if (gameData == null)
            {
                gameData = LoadGameDataFromFile();
            }

            return gameData;
        }
    }

    public static void SaveSettings(float volume, bool fullscreen)
    {
        var config = new ConfigFile();

        config.SetValue("Audio", "Volume", volume);
        config.SetValue("Video", "Fullscreen", fullscreen);

        Error result = config.Save(SettingsPath);

        if (result != Error.Ok)
        {
            GD.PrintErr("Couldn't save settings!");
        }
    }

    public static (float volume, bool fullscreen, int comboModeIndex) LoadSettings()
    {
        var config = new ConfigFile();
        Error result = config.Load(SettingsPath);

        if (result != Error.Ok)
        {
            return (1.0f, false, 0);
        }

        float volume = (float)config.GetValue("Audio", "Volume", 1.0f);
        bool fullscreen = (bool)config.GetValue("Video", "Fullscreen", false);
        int comboMode = (int)config.GetValue("Gameplay", "ComboMode", 0);

        return (volume, fullscreen, comboMode);
    }

    public static void SaveGameData(GameSaveData gameSaveData)
    {
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

    private static GameSaveData LoadGameDataFromFile()
    {
        if (!ResourceLoader.Exists(GameDataPath))
        {
            return new GameSaveData();
        }

        GameSaveData gameSaveData =
            ResourceLoader.Load<GameSaveData>(
                GameDataPath,
                "",
                ResourceLoader.CacheMode.Ignore
            );

        if (gameSaveData == null)
        {
            return new GameSaveData();
        }

        return gameSaveData;
    }
}
