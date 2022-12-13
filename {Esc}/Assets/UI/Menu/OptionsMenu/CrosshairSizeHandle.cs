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
	
	[Header("Previous Data")]
	[ReadOnly] public float prevValue;
	[ReadOnly] public bool onOptionsGUI = false;

    // Start is called before the first frame update
    void Start()
    {
		if (gameConfiguration is null)
			gameConfiguration = GameObject.FindGameObjectWithTag("GameConfiguration").GetComponent<GameConfiguration>();

        gameDataManager = gameConfiguration.gameDataManager;
        crosshairSizeSlider.value = gameDataManager.crossHairSize;
        valueText.text = "Crosshair Size: " + crosshairSizeSlider.value.ToString("F2");
    }

	void OnGUI()
	{
		if (!onOptionsGUI)
		{
			onOptionsGUI = true;
			prevValue = crosshairSizeSlider.value;
		}
	}

	public void Revert()
	{
        gameDataManager.crossHairSize = prevValue;
        crosshairSizeSlider.value = gameDataManager.crossHairSize;
        valueText.text = "Crosshair Size: " + prevValue.ToString("F2");
		onOptionsGUI = false;
	}

	public void Save()
	{
        gameDataManager.crossHairSize = crosshairSizeSlider.value;
        gameDataManager.Save();
		onOptionsGUI = false;
	}
	
    public void UpdateSliderText()
    {
        valueText.text = "Crosshair Size: " + crosshairSizeSlider.value.ToString("F2");
    }
}
