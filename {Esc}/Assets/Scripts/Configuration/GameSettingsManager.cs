using UnityEngine;
using System;
using System.IO;

[Serializable]
public class GameSettingsManager
{
    public float audioVolume = 0.3f;
    public float mouseSensitivity = 3.0f;

    private string dataFile;

    public GameSettingsManager(string dataFile)
    {
        this.dataFile = dataFile;
        if (!File.Exists(dataFile))
            File.Create(dataFile).Close();
        
        Load();
    }

    public void Load()
    {
        GameSettingsManager newData = RetrieveData(this.dataFile);
        this.audioVolume = newData.audioVolume;
        this.mouseSensitivity = newData.mouseSensitivity;
    }
    
    public void Save()
    {
        SaveData(this.dataFile, this);
    }

    public static GameSettingsManager RetrieveData(string dataFile)
    {
        return JsonUtility.FromJson<GameSettingsManager>(File.ReadAllText(dataFile));
    }

    public static void SaveData(string dataFile, object data)
    {
        File.WriteAllText(dataFile, JsonUtility.ToJson(data));
    }
}
