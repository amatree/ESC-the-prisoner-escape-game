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
	
	[Header("Previous Data")]
	[ReadOnly] public float prevValue;
	[ReadOnly] public bool onOptionsGUI = false;

    // Start is called before the first frame update
    void Start()
    {
		if (gameConfiguration is null)
			gameConfiguration = GameObject.FindGameObjectWithTag("GameConfiguration").GetComponent<GameConfiguration>();

        gameDataManager = gameConfiguration.gameDataManager;
        mouseSensitivitySlider.value = gameDataManager.mouseSensitivity;
        valueText.text = "Mouse Sensitivity: " + mouseSensitivitySlider.value.ToString("F2");
    }

	void OnGUI()
	{
		if (!onOptionsGUI)
		{
			onOptionsGUI = true;
			prevValue = mouseSensitivitySlider.value;
		}
	}

	public void Revert()
	{
        gameDataManager.mouseSensitivity = prevValue;
        mouseSensitivitySlider.value = gameDataManager.mouseSensitivity;
        valueText.text = "Mouse Sensitivity: " + prevValue.ToString("F2");
		onOptionsGUI = false;
	}

	public void Save()
	{
        gameDataManager.mouseSensitivity = mouseSensitivitySlider.value;
        gameDataManager.Save();
		onOptionsGUI = false;
	}

    public void UpdateSliderText()
    {
        valueText.text = "Mouse Sensitivity: " + mouseSensitivitySlider.value.ToString("F2");
    }
}
