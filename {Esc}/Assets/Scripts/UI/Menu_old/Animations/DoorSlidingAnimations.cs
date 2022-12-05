using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSlidingAnimations : MonoBehaviour
{
    public Transform leftDoor;
    public Transform rightDoor;
    public float actionDuration = 1.5f;
    private float oldActionDuration = 1.5f;

    public IntroCameraAnimation introCameraAnimation;

    [SerializeField]
    [ReadOnly] public bool startOpenningLeftDoorAnimation = false;
    [SerializeField]
    [ReadOnly] public bool startOpenningRightDoorAnimation = false;

    [SerializeField]
    [ReadOnly] public bool startClosingLeftDoorAnimation = false;
    [SerializeField]
    [ReadOnly] public bool startClosingRightDoorAnimation = false;

    // Start is called before the first frame update
    void Start()
    {
        oldActionDuration = actionDuration;
    }

    // Update is called once per frame
    void Update()
    {
        if (introCameraAnimation.isSkipped)
            StartCoroutine(ToggleActionSpeed());

        if (startOpenningLeftDoorAnimation)
            StartCoroutine(AnimateOpenLeftDoor());
        if (startOpenningRightDoorAnimation)
            StartCoroutine(AnimateOpenRightDoor());

        if (startClosingLeftDoorAnimation)
            StartCoroutine(AnimateOpenLeftDoor());
        if (startClosingRightDoorAnimation)
            StartCoroutine(AnimateOpenRightDoor());
    }

    public void OpenLeftDoor()
    {
        StartCoroutine(AnimateOpenLeftDoor());
    }

    public void CloseLeftDoor()
    {
        StartCoroutine(AnimateCloseLeftDoor());
    }

    public void OpenRightDoor()
    {
        StartCoroutine(AnimateOpenRightDoor());
    }

    public void CloseRightDoor()
    {
        StartCoroutine(AnimateCloseRightDoor());
    }

    IEnumerator ToggleActionSpeed()
    {
        actionDuration = 999999f;
        yield return new WaitForSeconds(0.1f);
        actionDuration = oldActionDuration;
    }

    public IEnumerator AnimateOpenRightDoor(float delay = 1.0f)
    {
        yield return new WaitForSeconds(delay);
        startOpenningRightDoorAnimation = false;
        while (rightDoor.position.y < 5.0f)
        {
            rightDoor.position += new Vector3(0f, Time.deltaTime * actionDuration, 0f);
            yield return null;
        }
    }

    public IEnumerator AnimateOpenLeftDoor(float delay = 1.0f)
    {
        yield return new WaitForSeconds(delay);
        startOpenningLeftDoorAnimation = false;
        while (leftDoor.position.y < 5.0f)
        {
            leftDoor.position += new Vector3(0f, Time.deltaTime * actionDuration, 0f);
            yield return null;
        }
    }

    public IEnumerator AnimateCloseRightDoor(float delay = 1.0f)
    {
        yield return new WaitForSeconds(delay);
        startClosingRightDoorAnimation = false;
        while (rightDoor.position.y > 0.0f)
        {
            rightDoor.position -= new Vector3(0f, Time.deltaTime * actionDuration, 0f);
            yield return null;
        }
    }

    public IEnumerator AnimateCloseLeftDoor(float delay = 1.0f)
    {
        yield return new WaitForSeconds(delay);
        startClosingLeftDoorAnimation = false;
        while (leftDoor.position.y > 0.0f)
        {
            leftDoor.position -= new Vector3(0f, Time.deltaTime * actionDuration, 0f);
            yield return null;
        }
    }
}
