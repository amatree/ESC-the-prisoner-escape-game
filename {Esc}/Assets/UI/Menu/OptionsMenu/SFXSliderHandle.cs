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

    // Start is called before the first frame update
    void Start()
    {
		if (gameConfiguration is null)
			gameConfiguration = GameObject.FindObjectOfType<GameConfiguration>();

        gameDataManager = gameConfiguration.gameDataManager;
        SFXSlider.value = gameDataManager.SFXVolume;
        valueText.text = "SFX: " + SFXSlider.value.ToString("F1");
    }

    public void SaveData()
    {
        gameDataManager.SFXVolume = SFXSlider.value;
        valueText.text = "SFX: " + SFXSlider.value.ToString("F1");
        gameDataManager.Save();
    }
}
