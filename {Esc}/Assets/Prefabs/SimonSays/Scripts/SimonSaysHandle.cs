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
    public string deviceTooltipText = "Press {0} to use device";
    public string correctTooltipText = "Access GRANTED!!!";
    public string errorTooltipText = "Wrong sequence please try again or press ESC to exit.";
    [ReadOnly] public bool wasUnlocked = false;
	public SimonSaysButtons simonSaysButtons;

    [Header("Interactions")]
    public PlayerInteraction playerInteraction;
    public HUDSettings hudSettings;
    [ReadOnly] public bool isLookedAt = false;
    [ReadOnly] public bool isFocused = false;
    
    SimonSaysHandle lookingAtDevice;
    PlayerController playerController;
    // bool canTry = true;
    // bool isMouseButton0Hold = false;
	
    // Start is called before the first frame update
    void Start()
    {
		SimonSaysButtons.SetEmissionStateOfButton(simonSaysButtons.BlueButton);
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
                        hudSettings.SetTooltipText(deviceTooltipText.Contains("{0}") ? string.Format(deviceTooltipText, accessKey) : deviceTooltipText);
					}
                }
            } else if (lookingAtDevice is null && hudSettings.currentTooltipText != "") {
                hudSettings.SetTooltipText();
            }
        }
        
    }
}
