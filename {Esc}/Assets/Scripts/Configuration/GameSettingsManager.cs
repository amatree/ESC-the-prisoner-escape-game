using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

[Serializable]
public class GameSettingsManager
{
    public float audioVolume = 0.3f;
    public float SFXVolume = 0.7f;
    public float mouseSensitivity = 5.0f;
	public float crossHairSize = 0.2f;
	public KeyCode slowWalkKey = KeyCode.LeftControl;
	public KeyCode sprintKey = KeyCode.LeftShift;
	public KeyCode jumpKey = KeyCode.Space;

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
        if (!float.IsNaN(newData.audioVolume)) this.audioVolume = newData.audioVolume;
		if (!float.IsNaN(newData.SFXVolume)) this.SFXVolume = newData.SFXVolume;
        if (!float.IsNaN(newData.mouseSensitivity)) this.mouseSensitivity = newData.mouseSensitivity;
		if (!float.IsNaN(newData.crossHairSize)) this.crossHairSize = newData.crossHairSize;
		if (newData.slowWalkKey is not KeyCode.None) this.slowWalkKey = newData.slowWalkKey;
		if (newData.sprintKey is not KeyCode.None) this.sprintKey = newData.sprintKey;
		if (newData.jumpKey is not KeyCode.None) this.jumpKey = newData.jumpKey;
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
