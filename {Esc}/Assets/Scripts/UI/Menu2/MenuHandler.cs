using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class MenuHandler : MonoBehaviour
{
    public KeyCode pauseMenuAccess = KeyCode.Escape;
    public GameObject mainMenuUI;
    [ReadOnly] public Camera menuCamera;
    [ReadOnly] public Camera mainCamera;
    
    [Header("Menus")]
    [ReadOnly] public GameObject pauseMenu;
    [ReadOnly] public GameObject optionsMenu;
    [ReadOnly] public GameObject mainMenu;

    // Start is called before the first frame update
    void Start()
    {
        mainMenuUI.SetActive(true);
        mainCamera = Camera.main;

        // get all menu game objects
        foreach (Transform c in mainMenuUI.GetComponentsInChildren<Transform>())
        {
            if (c.name == "PauseMenu")
                pauseMenu = c.gameObject;
            else if (c.name == "OptionsMenu")
                optionsMenu = c.gameObject;
            else if (c.name == "MainMenu")
                mainMenu = c.gameObject;
            else if (c.name == "PauseMenuCamera (No PostFX)")
                menuCamera = c.GetComponent<Camera>();
            else if (c.name == "MMCamera (No PostFX)")
                c.gameObject.SetActive(true); // make sure menu camera is active
        }

        // get pause menu's buttons
        foreach (Button b in pauseMenu.GetComponentsInChildren<Button>())
        {
            if (b.name == "ContinueButton")
                b.onClick.AddListener(ContinueGame);
            else if (b.name == "MainMenuButton")
                b.onClick.AddListener(ReturnToMainMenu);
        }

        mainMenuUI.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(pauseMenuAccess))
        {
            if (mainMenuUI.activeInHierarchy)
            {
                ContinueGame();
            } else 
            {
                string sceneName = SceneManager.GetActiveScene().name;
                if (sceneName == "Game")
                {
                    Time.timeScale = 0f;
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    OpenPauseMenu();
                }
            }
        } 
    }

    public void OpenPauseMenu()
    {
        mainCamera.enabled = false;
        menuCamera.enabled = true;
        mainMenuUI.SetActive(true);

        pauseMenu.SetActive(true);
        optionsMenu.SetActive(false);
        mainMenu.SetActive(false);
    }

    public void ContinueGame()
    {
        menuCamera.enabled = false;
        mainCamera.enabled = true;
        mainMenuUI.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1.0f;
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
