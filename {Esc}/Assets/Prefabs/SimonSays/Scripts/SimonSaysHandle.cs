using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimonSays;

public class SimonSaysHandle : MonoBehaviour
{
    [Header("General")]
    public string deviceName = "SimonSays#001";
    public KeyCode accessKey = KeyCode.E;
    [Tooltip("add {0} in to be filled in with `accessKey`")]
    public string deviceTooltipText = "Press {0} to use Device";
    public string correctTooltipText = "Access GRANTED!!!";
    public string errorTooltipText = "Wrong sequence please try again or press ESC to exit.";
    [ReadOnly] public bool wasUnlocked = false;
	public SimonSaysButtonSequence simonSaysButtonSequence;

    [Header("Interactions")]
    public PlayerInteraction playerInteraction;
    public HUDSettings hudSettings;
    [ReadOnly] public bool isLookedAt = false;
    [ReadOnly] public bool isFocused = false;
    
    SimonSaysHandle lookingAtDevice;
    PlayerController playerController;
    bool canTry = true;
    // bool isMouseButton0Hold = false;

	Vector3 oldCamLocalPos;
	string deviceTText;
	
    // Start is called before the first frame update
    void Start()
    {
		deviceTText = deviceTooltipText.Contains("{0}") ? string.Format(deviceTooltipText, accessKey) : deviceTooltipText;
		simonSaysButtonSequence.Play(this);
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

	bool CheckSequence()
	{
		return false;
	}

	void CheckForAccessToggle()
	{
        if (isLookedAt && !isFocused && Input.GetKeyDown(accessKey))
        {
			simonSaysButtonSequence.Play(this);
			hudSettings.SetTooltipTextState(false);
			isFocused = true;
			wasUnlocked = false;
			// toggle keypad UI here
			oldCamLocalPos = playerInteraction.playerCamera.transform.localPosition;
			playerInteraction.playerController.GiveUpAllControl();
			playerInteraction.playerCamera.transform.SetParent(lookingAtDevice.transform);
			playerInteraction.playerCamera.transform.LookAt(lookingAtDevice.transform);
			playerInteraction.playerCamera.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
			playerInteraction.playerCamera.transform.localPosition = new Vector3(0f, 0f, 1.08f);
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
			while (simonSaysButtonSequence.isSequencePlaying);
        }
	}

	void CheckForAccess()
	{
        if (isFocused && canTry)
        {
            if (Input.GetKeyDown(KeyCode.Escape) || CheckSequence() || wasUnlocked) {
                playerInteraction.playerController.RestoreAllControl();
                playerInteraction.playerCamera.transform.SetParent(playerInteraction.transform);
                playerInteraction.playerCamera.transform.localPosition = oldCamLocalPos;
                isFocused = false;
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }

            if (wasUnlocked)
            {
                // correct actions here
                hudSettings.ToggleTooltip(correctTooltipText, Color.green);
            }
        }
	}
}
