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
	
	[Header("Previous Data")]
	[ReadOnly] public float prevValue;
	[ReadOnly] public bool onOptionsGUI = false;

    // Start is called before the first frame update
    void Start()
    {
		if (gameConfiguration is null)
			gameConfiguration = GameObject.FindGameObjectWithTag("GameConfiguration").GetComponent<GameConfiguration>();
			
        gameDataManager = gameConfiguration.gameDataManager;
        audioSlider.value = gameDataManager.audioVolume;
        valueText.text = "Audio: " + audioSlider.value.ToString("F2");
    }

	void OnGUI()
	{
		if (!onOptionsGUI)
		{
			onOptionsGUI = true;
			prevValue = audioSlider.value;
		}
	}

	public void Revert()
	{
        gameDataManager.audioVolume = prevValue;
        audioSlider.value = gameDataManager.audioVolume;
        valueText.text = "Audio: " + prevValue.ToString("F2");
		onOptionsGUI = false;
	}

	public void Save()
	{
        gameDataManager.audioVolume = audioSlider.value;
        gameDataManager.Save();
		onOptionsGUI = false;
	}

    public void UpdateSliderText()
    {
        valueText.text = "Audio: " + audioSlider.value.ToString("F2");
    }
}
