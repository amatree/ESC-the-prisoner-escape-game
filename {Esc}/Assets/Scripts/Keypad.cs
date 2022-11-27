using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Keypad : MonoBehaviour
{
    [Header("Keypad Settings")]
    public string keypadName = "Room#001";
    public int passcode = 1234;
    public bool keepCorrectPasscode = true;

    [Header("Info")]
    [ReadOnly] public string inputPasscode = "";
    [ReadOnly] public bool isUnlocked = false;

    [Header("Interactions")]
    public PlayerInteraction playerInteraction;
    public HUDSettings hudSettings;
    [ReadOnly] public bool isLookedAt = false;
    [ReadOnly] public bool isFocused = false;
    
    Keypad lookingAtKeypad;
    List<List<Vector3>> keypadPositions;

    
    // Start is called before the first frame update
    void Start()
    {
        GenerateKeypadPositions();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInteraction.handRaycastHitTransform is not null && !isFocused)
        {
            lookingAtKeypad = playerInteraction.handRaycastHitTransform.GetComponent<Keypad>();
            isLookedAt = lookingAtKeypad is not null && lookingAtKeypad.keypadName == "Room#001";
            if (isLookedAt)
            {
                // check if camera's foward.x and keypad's forward.x is opposite
                float forwardVectorXs = lookingAtKeypad.transform.forward.x + playerInteraction.playerCamera.transform.forward.x;
                if (forwardVectorXs < 0.61f)
                {
                    // player is looking at a keypad
                    hudSettings.SetTooltipText("Press E to use Keypad");
                    hudSettings.SetTooltipTextState(true);
                } else {
                    hudSettings.SetTooltipTextState(false);
                    isLookedAt = false;
                }
            } else {
                    hudSettings.SetTooltipTextState(false);
                    isLookedAt = false;
                }
        } else {
            hudSettings.SetTooltipTextState(false);
            isLookedAt = false;
        }
        
        if (isLookedAt && !isFocused && Input.GetKeyDown(KeyCode.E))
        {
            isFocused = true;
            // toggle keypad UI here
            playerInteraction.playerController.GiveUpAllControl();
            playerInteraction.playerCamera.transform.SetParent(lookingAtKeypad.transform);
            playerInteraction.playerCamera.transform.LookAt(lookingAtKeypad.transform);
            playerInteraction.playerCamera.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
            playerInteraction.playerCamera.transform.localPosition = new Vector3(0f, 0f, 0.08f);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        if (isFocused)
        {
            if (Input.GetKeyDown(KeyCode.Escape) || CheckPasscode() || isUnlocked) {
                playerInteraction.playerController.RestoreAllControl();
                playerInteraction.playerCamera.transform.SetParent(playerInteraction.transform);
                playerInteraction.playerCamera.transform.localPosition = new Vector3(0f, 1.238f, 0.362f);
                isFocused = false;
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }

            if (isUnlocked)
            {
                // correct actions here
                print("ACCESS GRANTED!");


                // reset passcode if specified
                if (!keepCorrectPasscode) inputPasscode = "";
                isUnlocked = false;
            }
        }
    }

    bool CheckPasscode()
    {
        if (inputPasscode == passcode.ToString())
        {
            isUnlocked = true;
            return true;
        } else if (inputPasscode.Length >= passcode.ToString().Length)
        {
            inputPasscode = ""; // reset inputed passcode
            print("WRONG PASSCODE");
            return true;
        }
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = playerInteraction.playerCamera.nearClipPlane;

        Vector3 screenPos = playerInteraction.playerCamera.ScreenToWorldPoint(mousePos);
        if (Input.GetMouseButtonDown(0))
        {
            int key = 0;
            foreach (List<Vector3> keyPositions in keypadPositions)
            {
                if (SelectCheck(keyPositions[0], keyPositions[1], screenPos))
                {
                    inputPasscode += key.ToString();
                    break;
                } else
                    key++;
            }
        }

        return false;
    }
    public bool SelectCheck(Vector3 firstpos, Vector3 secondpos, Vector3 tocheck, float tolerance = 0.0085f)
    {
        bool c1 = Mathf.Abs(tocheck.x - firstpos.x) <= tolerance && Mathf.Abs(tocheck.x - secondpos.x) <= tolerance;
        bool c2 = Mathf.Abs(tocheck.y - firstpos.y) <= tolerance && Mathf.Abs(tocheck.y - secondpos.y) <= tolerance;
        return c1 && c2;
    }

    void GenerateKeypadPositions()
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
