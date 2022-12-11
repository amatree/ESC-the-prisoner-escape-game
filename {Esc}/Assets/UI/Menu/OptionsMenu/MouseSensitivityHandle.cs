using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MouseSensitivityHandle : MonoBehaviour
{
    [Header("Game Data Configuration")]
    [SerializeField] public GameConfiguration gameConfiguration;
    [SerializeField] [ReadOnly] public GameSettingsManager gameDataManager;

    [Header("UI")]
    public Slider mouseSensitivitySlider;
    public TMP_Text valueText;

    // Start is called before the first frame update
    void Start()
    {
		if (gameConfiguration is null)
			gameConfiguration = GameObject.FindObjectOfType<GameConfiguration>();

        gameDataManager = gameConfiguration.gameDataManager;
        mouseSensitivitySlider.value = gameDataManager.mouseSensitivity;
        valueText.text = "Mouse Sensitivity: " + mouseSensitivitySlider.value.ToString("F1");
    }

    public void SaveData()
    {
        gameDataManager.mouseSensitivity = mouseSensitivitySlider.value;
        valueText.text = "Mouse Sensitivity: " + mouseSensitivitySlider.value.ToString("F1");
        gameDataManager.Save();
    }
}
