using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IntroCameraAnimationStructs;

public class IntroCameraAnimation : MonoBehaviour
{
    [Header("Camera Movement Settings")]
    public Camera mainCamera;
    public IntroCameraAnimations introCameraAnimations;
    public float moveSeconds = 3.0f;
    public float moveSpeed = 2.0f;

    [Header("Lightings")]
    public DaylightCycle daylightCycle;
    public float timeLapseDuration = 3.0f;

    [Header("UI Settings")]
    public Canvas mainMenuCanvas;

    Vector3 cameraOriginalPosition;
    Quaternion cameraOriginalRotation;

    // Start is called before the first frame update
    void Start()
    {
        mainMenuCanvas.enabled = false;
        cameraOriginalPosition = mainCamera.transform.position;
        cameraOriginalRotation = mainCamera.transform.rotation;
        StartCoroutine(InitMainView());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void InitUI()
    {
        mainMenuCanvas.enabled = true;
        // UI element animations
        // button configurations
    }

    public IEnumerator InitMainView()
    {
        StartCoroutine(DaylightCycleLapse(1.0f / timeLapseDuration));
        yield return StartCoroutine(MoveOverSeconds(mainCamera.transform, introCameraAnimations.mainView01, moveSeconds));
        yield return StartCoroutine(MoveOverSeconds(mainCamera.transform, introCameraAnimations.mainView02, moveSeconds));

        InitUI();
    }

    public IEnumerator MoveOverSeconds(Transform objectToMove, Transform target, float seconds)
    {
        float elapsedTime = 0;
        Vector3 startingPos = objectToMove.position;
        while (elapsedTime < seconds)
        {
            objectToMove.rotation = Quaternion.Lerp(objectToMove.rotation, target.rotation, (elapsedTime*0.1f / seconds));
            objectToMove.position = Vector3.Lerp(startingPos, target.position, (elapsedTime / seconds));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        // objectToMove.position = target.position;
        // objectToMove.rotation = target.rotation;
    }

    public IEnumerator DaylightCycleLapse(float speed, float toTime = 1f)
    {
        daylightCycle.TakeControl();
        daylightCycle.time = 0f;
        float tolerance = speed * Time.deltaTime / toTime;
        while (daylightCycle.time <= toTime - tolerance)
        {
            daylightCycle.time += speed * Time.deltaTime / toTime;
            yield return null;
        }
        daylightCycle.ReturnControl();
        daylightCycle.Stop();
    }
}
