using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

[Serializable]
public class GameSettingsManager
{
    public float audioVolume = 0.3f;
    public float mouseSensitivity = 5.0f;

    private string dataFile;

    [NonSerialized]
    public bool justSaved = false;

    public GameSettingsManager(string dataFile)
    {
        this.dataFile = dataFile;
        if (!File.Exists(dataFile))
		{
            File.Create(dataFile).Close();
			Save();
		} else
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

    IEnumerator UpdateSaveStatus(float duration = 0.1f)
    {
        justSaved = true;
        yield return new WaitForSeconds(duration);
        justSaved = false;
    }
}
