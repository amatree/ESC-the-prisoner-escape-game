using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SFXSliderHandle : MonoBehaviour
{
    [Header("Game Data Configuration")]
    [SerializeField] public GameConfiguration gameConfiguration;
    [SerializeField] [ReadOnly] public GameSettingsManager gameDataManager;

    [Header("UI")]
    public Slider SFXSlider;
    public TMP_Text valueText;
	
	[Header("Previous Data")]
	[ReadOnly] public float prevValue;
	[ReadOnly] public bool onOptionsGUI = false;

    // Start is called before the first frame update
    void Start()
    {
		if (gameConfiguration is null)
			gameConfiguration = GameObject.FindGameObjectWithTag("GameConfiguration").GetComponent<GameConfiguration>();

        gameDataManager = gameConfiguration.gameDataManager;
        SFXSlider.value = gameDataManager.SFXVolume;
        valueText.text = "SFX: " + SFXSlider.value.ToString("F2");
    }

	void OnGUI()
	{
		if (!onOptionsGUI)
		{
			onOptionsGUI = true;
			prevValue = SFXSlider.value;
		}
	}

	public void Revert()
	{
        gameDataManager.SFXVolume = prevValue;
        SFXSlider.value = gameDataManager.SFXVolume;
        valueText.text = "SFX: " + prevValue.ToString("F2");
		onOptionsGUI = false;
	}

	public void Save()
	{
        gameDataManager.SFXVolume = SFXSlider.value;
        gameDataManager.Save();
		onOptionsGUI = false;
	}

    public void UpdateSliderText()
    {
        valueText.text = "SFX: " + SFXSlider.value.ToString("F2");
    }
}
