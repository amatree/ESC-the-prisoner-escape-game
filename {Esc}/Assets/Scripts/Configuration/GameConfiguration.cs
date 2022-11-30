using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameConfiguration : MonoBehaviour
{
    [ReadOnly] public string mainDataFile = "";
    [SerializeField] [ReadOnly] public GameSettingsManager gameDataManager;

    // Start is called before the first frame update
    void Start()
    {
        mainDataFile = Application.persistentDataPath + "/config.dat";
        gameDataManager = new GameSettingsManager(mainDataFile);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
