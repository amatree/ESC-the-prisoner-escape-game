using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameConfiguration : MonoBehaviour
{
    [ReadOnly] public string mainDataFile = "";
    [SerializeField] [ReadOnly] public GameSettingsManager gameDataManager;

    [SerializeField] public PlayerController playerController;
    [SerializeField] public HUDSettings hudSettings;

    void Awake()
    {
        mainDataFile = Application.persistentDataPath + "/config.dat";
        gameDataManager = new GameSettingsManager(mainDataFile);
        UpdateSettings();
    }

    // Start is called before the first frame update
    void Start()
    {
		
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
    public void StartGame()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName != "Game")
            SceneManager.LoadScene(1);
    }

    public void UpdateSettings()
    {
        if (playerController is not null)
        {
            playerController.audioSource.volume = gameDataManager.audioVolume;
            playerController.mouseSensitivity = gameDataManager.mouseSensitivity;
        }
		if (hudSettings is not null)
		{
			hudSettings.crossHairSize = gameDataManager.crossHairSize;
		}
    }
}
