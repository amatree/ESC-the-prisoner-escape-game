using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AudioSliderHandle : MonoBehaviour
{
    [Header("Game Data Configuration")]
    [SerializeField] public GameConfiguration gameConfiguration;
    [SerializeField] [ReadOnly] public GameSettingsManager gameDataManager;

    [Header("UI")]
    public Slider audioSlider;
    public TMP_Text valueText;

    // Start is called before the first frame update
    void Start()
    {
        gameDataManager = gameConfiguration.gameDataManager;
        // audioSlider.onValueChanged.AddListener(delegate {SaveData();});
        audioSlider.value = gameDataManager.audioVolume;
        valueText.text = "Audio: " + audioSlider.value.ToString("F1");
    }

    public void SaveData()
    {
        gameDataManager.audioVolume = audioSlider.value;
        valueText.text = "Audio: " + audioSlider.value.ToString("F1");
        gameDataManager.Save();
    }
}
