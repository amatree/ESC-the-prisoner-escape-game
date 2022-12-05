using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CrosshairSizeHandle : MonoBehaviour
{
    [Header("Game Data Configuration")]
    [SerializeField] public GameConfiguration gameConfiguration;
    [SerializeField] [ReadOnly] public GameSettingsManager gameDataManager;

    [Header("UI")]
    public Slider crosshairSizeSlider;
    public TMP_Text valueText;

    // Start is called before the first frame update
    void Start()
    {
        gameDataManager = gameConfiguration.gameDataManager;
        // crosshairSizeSlider.onValueChanged.AddListener(delegate {SaveData();});
        crosshairSizeSlider.value = gameDataManager.crossHairSize;
        valueText.text = "Crosshair Size: " + crosshairSizeSlider.value.ToString("F1");
    }

    public void SaveData()
    {
        gameDataManager.crossHairSize = crosshairSizeSlider.value;
        valueText.text = "Crosshair Size: " + crosshairSizeSlider.value.ToString("F1");
        gameDataManager.Save();
    }
}
