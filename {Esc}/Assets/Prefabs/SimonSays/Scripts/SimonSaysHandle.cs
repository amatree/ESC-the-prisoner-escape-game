using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimonSays;
using MouseToColliderDetection;

public class SimonSaysHandle : MonoBehaviour
{
    [Header("General")]
    public string deviceName = "SimonSays#001";
    public KeyCode accessKey = KeyCode.E;
    [Tooltip("add {0} in to be filled in with `accessKey`")]
    public string deviceTooltipText = "Press {0} to use Device";
    public string correctTooltipText = "Access GRANTED!!!";
    public string errorTooltipText = "Wrong sequence please try again or press ESC to exit.";
    public string outOfAttempTooltipText = "Reached maximum attempts allowed! Please try again later.";
	public Vector3 errorTTOffset = new Vector3(0f, -70f, 0f);
    [ReadOnly] public bool wasUnlocked = false;

	[Header("Configuration")]
	public bool playOnStart = false;
	public bool playOnFirstAccess = true;
	public bool playOnAccess = false;
	public bool defaultSettings = false;
	[ReadOnly] public int currentNumAttempts = 0;
	public int maximumAttempts = 3;
	public bool enableAttemptsTimeLimit = true;
	public float resetAttemptsDuration = 3.0f;
	[Space(20)]
	public SimonSaysButtonSequence lockingButtonSequence;
	[ReadOnly] public List<SimonSaysButtonID> inputSequence;

	[Header("SFX")]
	public AudioClip buttonPushSFX;
	public AudioClip buttonReleaseSFX;
	public AudioClip correctSFX;
	public AudioClip incorrectSFX;

    [Header("Interactions")]
    public PlayerInteraction playerInteraction;
    public HUDSettings hudSettings;
	[Tooltip("Recommended to turn this on")] public bool disableCharacterCollider = true;
	public Vector3 cameraPositionFocusOffset = new Vector3(0f, 0f, 2.16f);
    [ReadOnly] public bool isLookedAt = false;
    [ReadOnly] public bool isFocused = false;
    
	[Header("Debug")]
    [ReadOnly] public SimonSaysHandle lookingAtDevice;
    [ReadOnly] public PlayerController playerController;
    [ReadOnly] public bool canTry = true;
    [ReadOnly] public bool isMouseButton0Hold = false;

	[ReadOnly] public Vector3 oldCamLocalPos;
	[ReadOnly] public string deviceTText;
	
    // Start is called before the first frame update
    void Start()
    {
		if (playerInteraction is null)
			playerInteraction = GameObject.FindObjectOfType<PlayerInteraction>();
		if (hudSettings is null)
			hudSettings = GameObject.FindObjectOfType<HUDSettings>();
		playerController = playerInteraction.playerController;
		deviceTText = deviceTooltipText.Contains("{0}") ? string.Format(deviceTooltipText, accessKey) : deviceTooltipText;
		if (defaultSettings) lockingButtonSequence.DefaultSettings(this);
		if (playOnStart) PlaySequence();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInteraction.handRaycastHitTransform is not null && !isFocused)
        {
            lookingAtDevice = playerInteraction.handRaycastHitTransform.GetComponent<SimonSaysHandle>();
            isLookedAt = lookingAtDevice is not null && lookingAtDevice.deviceName == "SimonSays#001";
            if (isLookedAt && hudSettings.currentTooltipText != deviceTooltipText)
            {
                // check if camera's foward.x and device's forward.x is opposite
                float forwardVectorXs = lookingAtDevice.transform.forward.x + playerInteraction.playerCamera.transform.forward.x;
                if (forwardVectorXs < 0.61f)
                {
                    // player is looking at the device
                    if (!wasUnlocked)
					{
                        hudSettings.SetTooltipText(deviceTText);
					}
                }
            } else if (!isLookedAt && hudSettings.currentTooltipText == deviceTText) {
                hudSettings.SetTooltipText();
            }
        }
        
        CheckForAccessToggle();
		CheckForAccess();
    }

	public void PlaySequence()
	{
		if (!lockingButtonSequence.isSequencePlaying)
			lockingButtonSequence.PlayAll();
	}

	bool CheckSequence()
	{
		if (!isMouseButton0Hold)
		{
			if (Input.GetKeyDown(accessKey) && playOnAccess)
				PlaySequence();

			RaycastHit hit = ClickToCollider.WaitForHit(playerInteraction.playerCamera);
			if (hit.transform is not null)
			{
				if (hit.transform.name.EndsWith("PushButton"))
				{
					SimonSaysButtonHandle btnHandle = hit.transform.GetComponent<SimonSaysButtonHandle>();
					StartCoroutine(ButtonAnimation(btnHandle));
				}
			}

			if (lockingButtonSequence.Check(inputSequence))
			{
				// correct sequence
				playerController.PlaySFXOnce(correctSFX);
				return true;
			} else if (inputSequence.Count >= lockingButtonSequence.Sequence.Count)
			{
				inputSequence.Clear(); // reset input sequence
				playerController.PlaySFXOnce(incorrectSFX);
				StartCoroutine(WaitUntilNextTry());
				if (currentNumAttempts < maximumAttempts) hudSettings.ToggleTooltip(errorTooltipText, Color.red, errorTTOffset);
				return false;
			}
		}
		return false;
	}

	IEnumerator ButtonAnimation(SimonSaysButtonHandle buttonHandle)
	{
		buttonHandle.PushAndHold();
		lockingButtonSequence.EnableEmission(buttonHandle);
		playerController.PlaySFX(buttonPushSFX, 0.3f);
		while (Input.GetMouseButton(0)) // holding down left mouse
		{
			isMouseButton0Hold = true;
			yield return null;
		}
		isMouseButton0Hold = false;
		buttonHandle.Release();
		lockingButtonSequence.DisableEmission(buttonHandle);
		playerController.PlaySFX(buttonReleaseSFX, 0.3f);
		inputSequence.Add(buttonHandle.buttonID);
	}

	void CheckForAccessToggle()
	{
        if (isLookedAt && !isFocused && Input.GetKeyDown(accessKey) && canTry)
        {
			if (currentNumAttempts >= maximumAttempts) currentNumAttempts = 0;
			hudSettings.SetTooltipTextState(false);
			isFocused = true;
			wasUnlocked = false;
			// toggle keypad UI here
			oldCamLocalPos = playerInteraction.playerCamera.transform.localPosition;
			playerController.GiveUpAllControl();
			playerInteraction.playerCamera.transform.SetParent(lookingAtDevice.transform);
			playerInteraction.playerCamera.transform.LookAt(lookingAtDevice.transform);
			playerInteraction.playerCamera.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
			playerInteraction.playerCamera.transform.localPosition = cameraPositionFocusOffset;
			if (disableCharacterCollider) playerController.DisableAllColliders();
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
			if (playOnFirstAccess) PlaySequence();
        }
	}

    IEnumerator WaitUntilNextTry(float duration = 1.0f, bool increaseAttempt = true)
    {
        canTry = false;
		if (increaseAttempt) currentNumAttempts += 1;
        yield return new WaitForSeconds(duration);
        canTry = true;
    }

	void CheckForAccess()
	{
        if (isFocused && canTry)
        {
            if (Input.GetKeyDown(KeyCode.Escape) || CheckSequence() || wasUnlocked || currentNumAttempts >= maximumAttempts) {
                if (currentNumAttempts >= maximumAttempts)
				{
					hudSettings.ToggleTooltip(outOfAttempTooltipText, Color.red, errorTTOffset);
					if (enableAttemptsTimeLimit)
						StartCoroutine(WaitUntilNextTry(resetAttemptsDuration, false));
				}
				StartCoroutine(EndOfAttempt());
            }

            if (wasUnlocked)
            {
                // correct actions here
                hudSettings.ToggleTooltip(correctTooltipText, Color.green);
            }
        }
	}

	IEnumerator EndOfAttempt()
	{
		while (hudSettings.isLocked)
			yield return null;
		playerInteraction.playerController.RestoreAllControl();
		playerInteraction.playerCamera.transform.SetParent(playerInteraction.transform);
		playerInteraction.playerCamera.transform.localPosition = oldCamLocalPos;
		isFocused = false;
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
		if (disableCharacterCollider) playerController.EnableAllColliders();
		inputSequence.Clear();
		hudSettings.ResetText();
	}
}
