using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using IntroCameraAnimationStructs;

public class IntroCameraAnimation : MonoBehaviour
{
    [Header("Camera Movement Settings")]
    public Camera mainCamera;
    public IntroCameraAnimations introCameraAnimations;
    public float moveSeconds = 3.0f;
    public bool skippable = true;
    [ReadOnly] public bool isSkipped = false;

    [Header("Daylight Cycle")]
    public DaylightCycle daylightCycle;
    public float timeLapseDuration = 3.0f;

    [Header("Main Menu Lightings")]
    public float daylightStartTimeMainMenu = 0.3f;
    public float daylightEndTimeMainMenu = 1.0f;

    [Header("Options Menu Lightings")]
    public float daylightStartTimeOptMenu = 0.0f;
    public float daylightEndTimeOptMenu = 0.5f;

    [Header("UI Settings")]
    public Animator mainMenuButtonAnimator;
    public Canvas mainMenuCanvas;
    public Animator OptionsMenuButtonAnimator;
    public Canvas optionsMenuCanvas;

    Vector3 cameraOriginalPosition;
    Quaternion cameraOriginalRotation;

    // Start is called before the first frame update
    void Start()
    {
        mainMenuCanvas.gameObject.SetActive(false);
        optionsMenuCanvas.gameObject.SetActive(false);
        cameraOriginalPosition = mainCamera.transform.position;
        cameraOriginalRotation = mainCamera.transform.rotation;
        AnimateMainMenu();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isSkipped && skippable && Input.GetKeyDown(KeyCode.Escape))
        {
            StartCoroutine(SkipAnimation());
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
    
    public void AnimateOptionsMenu()
    {
        StartCoroutine(InitOptionsView());
    }

    public IEnumerator SkipAnimation(float delay = 0.1f)
    {
        isSkipped = true;
        yield return new WaitForSeconds(delay);
        isSkipped = false;
    }


    public IEnumerator InitOptionsView()
    {
        mainMenuButtonAnimator.Play("OptionsButtonClicked");
        StartCoroutine(DaylightCycleLapse(1.0f / timeLapseDuration, daylightStartTimeOptMenu, daylightEndTimeOptMenu));
        yield return StartCoroutine(MoveOverSeconds(mainCamera.transform, introCameraAnimations.optionView01, moveSeconds));
        yield return StartCoroutine(MoveOverSeconds(mainCamera.transform, introCameraAnimations.optionView02, moveSeconds));

        mainMenuCanvas.gameObject.SetActive(false);
        optionsMenuCanvas.gameObject.SetActive(true);
        InitOptGUI();
    }

    public void AnimateMainMenu()
    {
        StartCoroutine(InitMainView());
    }

    public void InitOptGUI()
    {
        isSkipped = false;
        OptionsMenuButtonAnimator.Play("ButtonIdle");
    }

    public void InitMainMenuUI()
    {
        isSkipped = false;
        mainMenuButtonAnimator.Play("AnimStart");
    }

    public IEnumerator InitMainView()
    {
        StartCoroutine(DaylightCycleLapse(1.0f / timeLapseDuration, daylightStartTimeMainMenu, daylightEndTimeMainMenu));
        yield return StartCoroutine(MoveOverSeconds(mainCamera.transform, introCameraAnimations.mainView01, moveSeconds));
        yield return StartCoroutine(MoveOverSeconds(mainCamera.transform, introCameraAnimations.mainView02, moveSeconds));

        mainMenuCanvas.gameObject.SetActive(true);
        optionsMenuCanvas.gameObject.SetActive(false);
        InitMainMenuUI();
    }

    public IEnumerator MoveOverSeconds(Transform objectToMove, Transform target, float seconds)
    {
        float elapsedTime = 0;
        Vector3 startingPos = objectToMove.position;
        while (elapsedTime < seconds && !isSkipped)
        {
            objectToMove.rotation = Quaternion.Lerp(objectToMove.rotation, target.rotation, (elapsedTime*0.1f / seconds));
            objectToMove.position = Vector3.Lerp(startingPos, target.position, (elapsedTime / seconds));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        objectToMove.position = target.position;
        objectToMove.rotation = target.rotation;
    }

    public IEnumerator DaylightCycleLapse(float speed, float startTime = 0.3f, float toTime = 1f)
    {
        daylightCycle.TakeControl();
        daylightCycle.time = startTime;
        float tolerance = speed * Time.deltaTime / toTime;
        while (daylightCycle.time <= toTime - tolerance && !isSkipped)
        {
            daylightCycle.time += speed * Time.deltaTime / toTime;
            yield return null;
        }
        daylightCycle.ReturnControl();
        daylightCycle.Stop();
    }
}
