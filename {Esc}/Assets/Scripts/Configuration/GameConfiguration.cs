using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameConfiguration : MonoBehaviour
{
    [ReadOnly] public string mainDataFile = "";
    [SerializeField] [ReadOnly] public GameSettingsManager gameDataManager;

    [SerializeField] public PlayerController playerController;
    [SerializeField] public CharacterFootstep characterFootstep;
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
		if (playerController is null)
			playerController = GameObject.FindObjectOfType<PlayerController>();
		if (characterFootstep is null)
			characterFootstep = GameObject.FindObjectOfType<CharacterFootstep>();
		if (hudSettings is null)
			hudSettings = GameObject.FindObjectOfType<HUDSettings>();
        UpdateSettings();
		
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

			playerController.slowWalkKey = gameDataManager.slowWalkKey;
			playerController.jumpKey = gameDataManager.jumpKey;
			playerController.sprintKey = gameDataManager.sprintKey;
        }
		if (hudSettings is not null)
		{
			hudSettings.crossHairSize = gameDataManager.crossHairSize;
		}
		if (characterFootstep is not null)
		{
			characterFootstep.sfxSource.volume = gameDataManager.SFXVolume;
		}
    }
}
