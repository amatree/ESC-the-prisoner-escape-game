using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Enums;

public class Flashlight : MonoBehaviour
{

    [Header("General")]
    public AudioSource audioSource;
    public AudioClip ToggleSFX;
    public KeyCode ToggleKey = KeyCode.F;
    public Camera playerCam;
    public Transform player;

    [Header("Battery Info")]
    public bool enableInfo;
    public GameObject BatteryFill;
    public GameObject BatteryText;
    [ReadOnly] public bool Status = false;
    [ReadOnly] public Difficulties Difficulty;
    [ReadOnly] public int MaximumBatteryCapacity = 0;
    [ReadOnly] public int CurrentBatteryCount = 0;
    [ReadOnly] public float BatteryHP = 0f;
    [ReadOnly][Tooltip("in second")] public int BatteryLifespan = 0;
    [ReadOnly] public bool isOutOfBattery = false;
    [ReadOnly] public float BatteryDropRate;

    [HideInInspector] public bool Enabled = false;
    
    private bool isOutOfBatteryCall = false;
    private bool isBlinking = false;
    private YieldInstruction fadeInstruction = new YieldInstruction();

    // Start is called before the first frame update
    void Start()
    {
        if (Difficulty == Difficulties.Easy) {
            MaximumBatteryCapacity = 7;
            BatteryLifespan = 300;
        } else if (Difficulty == Difficulties.Insane) {
            MaximumBatteryCapacity = 3;
            BatteryLifespan = 180;
        } else if (Difficulty == Difficulties.Nightmare) {
            MaximumBatteryCapacity = 2;
            BatteryLifespan = 45;
        } else { // default difficulty = normal
            MaximumBatteryCapacity = 5;
            BatteryLifespan = 180;
        }
        GetComponent<Light>().enabled = Enabled;
        CurrentBatteryCount = MaximumBatteryCapacity;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(ToggleKey))
            Toggle();

        if (BatteryHP <= 0) {
            if (!isOutOfBattery) {
                BatteryHP = 100.0f;
                CurrentBatteryCount -= 1;
            }
        } else if (!isOutOfBattery && Enabled)
            BatteryHP -= GetDropRate();

        // position
        transform.LookAt(playerCam.transform.position + playerCam.transform.forward * GetComponent<Light>().range);
        
        // HUD update
        if (enableInfo)
        {
            BatteryText.GetComponent<TextMeshProUGUI>().text = CurrentBatteryCount + "/" + MaximumBatteryCapacity;
            if (!isOutOfBattery) 
            {
                BatteryFill.GetComponent<Image>().fillAmount = BatteryHP / 100;
                if (BatteryHP < 30.0f) 
                    BatteryFill.GetComponent<Image>().color = Color.red;
                else 
                    BatteryFill.GetComponent<Image>().color = Color.white;
            }
        }

        // Inspector values
        Status = Enabled;
        BatteryDropRate = GetDropRate();
        if (isOutOfBattery = CurrentBatteryCount <= 0) {
            if (!isOutOfBatteryCall)
            {
                BatteryHP = 0.0f;
                GetComponent<Light>().enabled = false;
                isOutOfBatteryCall = true;
                if (enableInfo)
                {
                    BatteryFill.GetComponent<Image>().color = new Color(1, 0, 0, 0);
                    InvokeRepeating("BlinkingRed", 0.5f, 4.0f);
                }
            }
        }

        if (CurrentBatteryCount > 0 && isOutOfBatteryCall)
        {
            isOutOfBatteryCall = false;
            CancelInvoke();
            StopCoroutine("Fade");
            StopCoroutine("FadeIn");
            StopCoroutine("FadeOut");
        }

        if (Input.GetKeyUp(KeyCode.Comma)) {
            AddBattery();
        }
    }

    float GetDropRate() {
        return 100.0f / BatteryLifespan / (1.0f / Time.deltaTime);
    }

    void BlinkingRed() {
        StartCoroutine(Fade(BatteryFill.GetComponent<Image>()));
        isBlinking = !isBlinking;
    }

    IEnumerator Fade(Image image, float fadeTime = 1.0f)
    {
        yield return FadeIn(image);
        yield return FadeOut(image);
    }

    IEnumerator FadeOut(Image image, float fadeTime = 1.0f)
    {
        float elapsedTime = 0.0f;
        Color c = Color.red;
        while (elapsedTime < fadeTime)
        {
            yield return fadeInstruction;
            elapsedTime += Time.deltaTime ;
            c.a = 1.0f - Mathf.Clamp01(elapsedTime / fadeTime);
            image.color = c;
        }
    }

    IEnumerator FadeIn(Image image, float fadeTime = 1.0f)
    {
        float elapsedTime = 0.0f;
        Color c = Color.red;
        while (elapsedTime < fadeTime)
        {
            yield return fadeInstruction;
            elapsedTime += Time.deltaTime ;
            c.a = Mathf.Clamp01(elapsedTime / fadeTime);
            image.color = c;
        }
    }

    void Toggle() 
    {
        audioSource.PlayOneShot(ToggleSFX);
        Enabled = !Enabled;
        if (!isOutOfBattery) 
            GetComponent<Light>().enabled = Enabled;
    }

    public void AddBattery(int amount = 1) {
        if (CurrentBatteryCount < MaximumBatteryCapacity)
            CurrentBatteryCount += amount + 1;
        if (BatteryHP <= 0) {
            GetComponent<Light>().enabled = Enabled;
            CurrentBatteryCount--;
            BatteryHP = 100.0f;
        }
    }
}
