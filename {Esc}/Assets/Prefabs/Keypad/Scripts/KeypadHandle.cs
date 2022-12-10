using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
using Keypad;

public class KeypadHandle : MonoBehaviour
{
	// public enum CharacterModel { MainCharacter, CapsuleCharacter }

    [Header("General")]
    public string keypadName = "Room#001";
    public int passcode = 1234;
    public KeyCode accessKey = KeyCode.E;
    [Tooltip("add {0} in to be filled in with `accessKey`")]
    public string keypadTooltipText = "Press {0} to use Keypad";
    public string correctTooltipText = "Access GRANTED!!!";
    public string errorTooltipText = "Wrong passcode please try again or press ESC to exit.";
    public bool keepCorrectPasscode = true;
    [ReadOnly] public string inputPasscode = "";
    [ReadOnly] public bool wasUnlocked = false;
	// public CharacterModel characterModel = CharacterModel.MainCharacter;

    [Header("Keypad Configuration")]
    public float keypadDownTime = 2.0f;
    public MeshRenderer keypadMeshRenderer;
    public KeypadMaterials keypadMaterials;
    public KeypadSoundEffects keypadSoundEffects;
	public BoxCollider keypadCollider;

    [Header("Interactions")]
    public PlayerInteraction playerInteraction;
    public HUDSettings hudSettings;
    [ReadOnly] public bool isLookedAt = false;
    [ReadOnly] public bool isFocused = false;
    
    KeypadHandle lookingAtKeypad;
    List<List<Vector3>> keypadPositions;
    List<Material> allKeyClickedMaterials;
    PlayerController playerController;
    bool canTry = true;
    bool isMouseButton0Hold = false;

	Vector3 oldCamLocalPos;
	string keypadTText;
    
    // Start is called before the first frame update
    void Start()
    {
        playerController = playerInteraction.playerController;

        // if (characterModel == CharacterModel.CapsuleCharacter) GenerateKeypadPositions__C(); // for capsule character
        // else if (characterModel == CharacterModel.MainCharacter) GenerateKeypadPositions__M(); // for main character
        allKeyClickedMaterials = new List<Material>() {
            keypadMaterials.KeypadClicked000,
            keypadMaterials.KeypadClicked001,
            keypadMaterials.KeypadClicked002,
            keypadMaterials.KeypadClicked003,
            keypadMaterials.KeypadClicked004,
            keypadMaterials.KeypadClicked005,
            keypadMaterials.KeypadClicked006,
            keypadMaterials.KeypadClicked007,
            keypadMaterials.KeypadClicked008,
            keypadMaterials.KeypadClicked009
        };

		keypadTText = keypadTooltipText.Contains("{0}") ? string.Format(keypadTooltipText, accessKey) : keypadTooltipText;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInteraction.handRaycastHitTransform is not null && !isFocused)
        {
            lookingAtKeypad = playerInteraction.handRaycastHitTransform.GetComponent<KeypadHandle>();
            isLookedAt = lookingAtKeypad is not null && lookingAtKeypad.keypadName == "Room#001";
            if (isLookedAt && hudSettings.currentTooltipText != keypadTooltipText)
            {
                // check if camera's foward.x and keypad's forward.x is opposite
                float forwardVectorXs = lookingAtKeypad.transform.forward.x + playerInteraction.playerCamera.transform.forward.x;
                if (forwardVectorXs < 0.61f)
                {
                    // player is looking at a keypad
                    if (!wasUnlocked) 
                    {
                        hudSettings.SetTooltipText(keypadTText);
                    }
                } 
            } else if (!isLookedAt && hudSettings.currentTooltipText == keypadTText) {
                hudSettings.SetTooltipText();
            }
        }
        
        CheckForAccessToggle();
		CheckForAccess();
    }

	void CheckForAccessToggle()
	{
        if (isLookedAt && !isFocused && Input.GetKeyDown(accessKey))
        {
            hudSettings.SetTooltipTextState(false);
            isFocused = true;
            wasUnlocked = false;
            // toggle keypad UI here
			oldCamLocalPos = playerInteraction.playerCamera.transform.localPosition;
            playerInteraction.playerController.GiveUpAllControl();
            playerInteraction.playerCamera.transform.SetParent(lookingAtKeypad.transform);
            playerInteraction.playerCamera.transform.LookAt(lookingAtKeypad.transform);
            playerInteraction.playerCamera.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
            playerInteraction.playerCamera.transform.localPosition = new Vector3(0f, 0f, 0.08f);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
			keypadCollider.enabled = false;
        }
	}

	void CheckForAccess()
	{
        if (isFocused && canTry)
        {
            if (Input.GetKeyDown(KeyCode.Escape) || CheckPasscode() || wasUnlocked) {
                playerInteraction.playerController.RestoreAllControl();
                playerInteraction.playerCamera.transform.SetParent(playerInteraction.transform);
                playerInteraction.playerCamera.transform.localPosition = oldCamLocalPos;
                isFocused = false;
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
				keypadCollider.enabled = true;
            }

            if (wasUnlocked)
            {
                // correct actions here
                hudSettings.ToggleTooltip(correctTooltipText, Color.green);

                // reset passcode if specified
                if (!keepCorrectPasscode) inputPasscode = "";
            }
        }
	}

    bool CheckPasscode()
    {
        if (!isMouseButton0Hold)
        {
            if (inputPasscode == passcode.ToString())
            {
                StartCoroutine(UnlockKeypad());
                return true;
            } else if (inputPasscode.Length >= passcode.ToString().Length)
            {
                inputPasscode = ""; // reset inputed passcode
                playerController.PlayFX(keypadSoundEffects.IncorrectSFX, 0.1f, true);
                hudSettings.MoveTextPosition(0, -50);
                hudSettings.ToggleTooltip(errorTooltipText, Color.red);
                hudSettings.MoveTextPosition(0, 50);
                StartCoroutine(WaitUntilNextTry(1.0f));
                return false;
            }
			
            Vector3 mousePos = Input.mousePosition;
			if (Input.GetMouseButtonDown(0))
			{
				Ray ray = playerInteraction.playerCamera.ScreenPointToRay(mousePos);
				if (Physics.Raycast(ray, out RaycastHit hit))
				{
					string hitName = hit.transform.name;
					if (hitName.StartsWith("Numpad"))
					{
						int key = hitName.ElementAt(hitName.Length - 1) - 48;
						inputPasscode += key;
                        StartCoroutine(KeySelection(key));
					}
				}
			}

            // mousePos.z = playerInteraction.playerCamera.nearClipPlane;
            // Vector3 screenPos = playerInteraction.playerCamera.ScreenToWorldPoint(mousePos);
            // if (Input.GetMouseButtonDown(0))
            // {
			// 	// print(screenPos.x + "f, " + screenPos.y + "f, " + screenPos.z + "f");
            //     int key = 0;
            //     foreach (List<Vector3> keyPositions in keypadPositions)
            //     {
            //         if (KeypadSelectionCheck(keyPositions[0], keyPositions[1], screenPos))
            //         {
            //             inputPasscode += key.ToString();
            //             StartCoroutine(KeySelection(key));
            //             break;
            //         } else
            //             key++;
            //     }
            // }
        }
        return false;
    }

    IEnumerator WaitUntilNextTry(float duration = 1.0f)
    {
        canTry = false;
        yield return new WaitForSeconds(duration);
        canTry = true;
    }

    IEnumerator KeySelection(int key)
    {
        playerController.PlayFX(keypadSoundEffects.KeypadClickHoldSFX, 0.3f);
        keypadMeshRenderer.material = allKeyClickedMaterials[key];
        while (Input.GetMouseButton(0))
        {
            isMouseButton0Hold = true;
            yield return null;
        }
        playerController.PlayFX(keypadSoundEffects.KeypadClickReleaseSFX, 0.4f);
        keypadMeshRenderer.material = keypadMaterials.KeypadDefault;
        isMouseButton0Hold = false;
        yield break;
    }

    IEnumerator UnlockKeypad(bool playSFX = true)
    {
        if (playSFX) 
            playerController.PlayFX(keypadSoundEffects.CorrectSFX, 0.1f);
        wasUnlocked = true;
        yield return new WaitForSeconds(keypadDownTime);
        wasUnlocked = false;
        yield break;
    }

    public bool KeypadSelectionCheck(Vector3 firstpos, Vector3 secondpos, Vector3 tocheck, float tolerance = 0.0085f)
    {
        bool c1 = Mathf.Abs(tocheck.x - firstpos.x) <= tolerance && Mathf.Abs(tocheck.x - secondpos.x) <= tolerance;
        bool c2 = Mathf.Abs(tocheck.y - firstpos.y) <= tolerance && Mathf.Abs(tocheck.y - secondpos.y) <= tolerance;
        return c1 && c2;
    }

	void GenerateKeypadPositions__M()
    {
        keypadPositions = new List<List<Vector3>>();

        List<Vector3> tmpPos0 = new List<Vector3>();
        tmpPos0.Add(new Vector3(-0.004032095f, 1.409144f, 0.7f));
        tmpPos0.Add(new Vector3(0.00438273f, 1.40143f, 0.7f));
        keypadPositions.Add(tmpPos0);

        List<Vector3> tmpPos1 = new List<Vector3>();
        tmpPos1.Add(new Vector3(-0.01490124f, 1.4407f, 0.7f));
        tmpPos1.Add(new Vector3(-0.006720162f, 1.432402f, 0.7f));
        keypadPositions.Add(tmpPos1);

        List<Vector3> tmpPos2 = new List<Vector3>();
        tmpPos2.Add(new Vector3(-0.004148967f, 1.440583f, 0.7f));
        tmpPos2.Add(new Vector3(0.00414898f, 1.432635f, 0.7f));
        keypadPositions.Add(tmpPos2);

        List<Vector3> tmpPos3 = new List<Vector3>();
        tmpPos3.Add(new Vector3(0.006603308f, 1.440466f, 0.7f));
        tmpPos3.Add(new Vector3(0.015135f, 1.432402f, 0.7f));
        keypadPositions.Add(tmpPos3);

        List<Vector3> tmpPos4 = new List<Vector3>();
        tmpPos4.Add(new Vector3(-0.01513498f, 1.430181f, 0.7f));
        tmpPos4.Add(new Vector3(-0.006720162f, 1.422117f, 0.7f));
        keypadPositions.Add(tmpPos4);

        List<Vector3> tmpPos5 = new List<Vector3>();
        tmpPos5.Add(new Vector3(-0.004499585f, 1.430532f, 0.7f));
        tmpPos5.Add(new Vector3(0.004032112f, 1.422467f, 0.7f));
        keypadPositions.Add(tmpPos5);

        List<Vector3> tmpPos6 = new List<Vector3>();
        tmpPos6.Add(new Vector3(0.006252691f, 1.430181f, 0.7f));
        tmpPos6.Add(new Vector3(0.01490126f, 1.422117f, 0.7f));
        keypadPositions.Add(tmpPos6);

        List<Vector3> tmpPos7 = new List<Vector3>();
        tmpPos7.Add(new Vector3(-0.01513498f, 1.419779f, 0.7f));
        tmpPos7.Add(new Vector3(-0.006837034f, 1.411832f, 0.7f));
        keypadPositions.Add(tmpPos7);

        List<Vector3> tmpPos8 = new List<Vector3>();
        tmpPos8.Add(new Vector3(-0.004148967f, 1.419546f, 0.7f));
        tmpPos8.Add(new Vector3(0.004032112f, 1.411949f, 0.7f));
        keypadPositions.Add(tmpPos8);

        List<Vector3> tmpPos9 = new List<Vector3>();
        tmpPos9.Add(new Vector3(0.006486434f, 1.419779f, 0.7f));
        tmpPos9.Add(new Vector3(0.01478438f, 1.411715f, 0.7f));
        keypadPositions.Add(tmpPos9);
    }

    void GenerateKeypadPositions__C()
    {
        keypadPositions = new List<List<Vector3>>();

        List<Vector3> tmpPos0 = new List<Vector3>();
        tmpPos0.Add(new Vector3(0.004548831f, 2.478772f, 0.7f));
        tmpPos0.Add(new Vector3(-0.004315535f, 2.487403f, 0.7f));
        keypadPositions.Add(tmpPos0);

        List<Vector3> tmpPos1 = new List<Vector3>();
        tmpPos1.Add(new Vector3(-0.006298352f, 2.510031f, 0.7f));
        tmpPos1.Add(new Vector3(-0.01516273f, 2.518662f, 0.7f));
        keypadPositions.Add(tmpPos1);

        List<Vector3> tmpPos2 = new List<Vector3>();
        tmpPos2.Add(new Vector3(0.004315559f, 2.510031f, 0.7f));
        tmpPos2.Add(new Vector3(-0.004315535f, 2.518662f, 0.7f));
        keypadPositions.Add(tmpPos2);

        List<Vector3> tmpPos3 = new List<Vector3>();
        tmpPos3.Add(new Vector3(0.01516275f, 2.510497f, 0.7f));
        tmpPos3.Add(new Vector3(0.006415013f, 2.518662f, 0.7f));
        keypadPositions.Add(tmpPos3);

        List<Vector3> tmpPos4 = new List<Vector3>();
        tmpPos4.Add(new Vector3(-0.006298352f, 2.499533f, 0.7f));
        tmpPos4.Add(new Vector3(-0.01516273f, 2.508514f, 0.7f));
        keypadPositions.Add(tmpPos4);

        List<Vector3> tmpPos5 = new List<Vector3>();
        tmpPos5.Add(new Vector3(0.004198928f, 2.5f, 0.7f));
        tmpPos5.Add(new Vector3(-0.004082263f, 2.508281f, 0.7f));
        keypadPositions.Add(tmpPos5);

        List<Vector3> tmpPos6 = new List<Vector3>();
        tmpPos6.Add(new Vector3(0.01527938f, 2.500117f, 0.7f));
        tmpPos6.Add(new Vector3(0.006531655f, 2.508048f, 0.7f));
        keypadPositions.Add(tmpPos6);

        List<Vector3> tmpPos7 = new List<Vector3>();
        tmpPos7.Add(new Vector3(-0.006531631f, 2.489619f, 0.7f));
        tmpPos7.Add(new Vector3(-0.01492945f, 2.497667f, 0.7f));
        keypadPositions.Add(tmpPos7);

        List<Vector3> tmpPos8 = new List<Vector3>();
        tmpPos8.Add(new Vector3(0.004315559f, 2.489386f, 0.7f));
        tmpPos8.Add(new Vector3(-0.004315535f, 2.4979f, 0.7f));
        keypadPositions.Add(tmpPos8);

        List<Vector3> tmpPos9 = new List<Vector3>();
        tmpPos9.Add(new Vector3(0.01504611f, 2.489386f, 0.7f));
        tmpPos9.Add(new Vector3(0.006531655f, 2.4979f, 0.7f));
        keypadPositions.Add(tmpPos9);
    }
}
