using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DaylightCycle : MonoBehaviour
{
    [ReadOnly] public bool EditorTestStarted = false;

    [Range(0.0f, 1.0f)] public float time;
    [Tooltip("The real world value is 86400s")] public float FullDayLength = 120.0f;
    public bool UseRealDayLength = false;
    public float StartTime = 0.4f;
    public bool RandomStartTime = false;
    private float timeRate;
    public Vector3 Noon = new Vector3(90.0f, 0.0f, 0.0f);

    [Header("Date & Time")]
    public bool UseSystemTime = false;
    [ReadOnly] public int Day = 1;
    [ReadOnly] public int Month = 1;
    [ReadOnly] public int Year = 1;
    [ReadOnly] public bool IsLeapYear;
    [ReadOnly] public string CurrentTime = "hh:mm:ss";

    [Header("Skybox Settings")]
    public Gradient SkyColor;
    public Gradient HorizonColor;

    [Header("Fog Settings")]
    public bool EnableFog = true;
    public float MaximumFogDensity = 0.01f;
    public AnimationCurve FogDensity;

    [Header("Sun")]
    public Light Sun;
    public Gradient SunColor;
    public AnimationCurve SunIntensity;

    [Header("Moon")]
    public Light Moon;
    public Gradient MoonColor;
    public AnimationCurve MoonIntensity;

    [Header("Other Lighting Settings")]
    public AnimationCurve LightingIntensityMultiplier;
    public AnimationCurve ReflectionsIntensityMultiplier;
    
    // other vars
    List<int> knuckles = new List<int>{1, 3, 5, 7, 8, 10, 12};
    List<int> grooves = new List<int>{4, 6, 9, 11};

    void Awake()
    {
        EditorTestStarted = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (UseRealDayLength) {
            FullDayLength = 86400.0f;
        }
        if (UseSystemTime) {
            DateTime systime = DateTime.Now;
            Day = systime.Day;
            Month = systime.Month;
            Year = systime.Year;
            StartTime = (systime.Hour * 3600.0f + systime.Minute * 60.0f + systime.Second + systime.Millisecond * 0.001f) / 86400.0f;
        } else {
            Day = Month = Year = 1;
            StartTime = RandomStartTime ? UnityEngine.Random.Range(0.0f, 2.0f) : StartTime;
        }
        
        timeRate = 1.0f / FullDayLength;
        time = StartTime;

        RenderSettings.fog = EnableFog;
        
    }

    // Update is called once per frame
    void Update()
    {
        // time increment
        if (!EditorTestStarted)
        {
            time += timeRate * Time.deltaTime;
            time = time >= 1.0f ? IncrementDay() : time;
        }
        GetCurrentTime();
        IsLeapYear = DateTime.IsLeapYear(Year);

        // fog
        RenderSettings.fogDensity = MaximumFogDensity * FogDensity.Evaluate(time);

        // light rotation
        Sun.transform.eulerAngles = (time - 0.25f) * Noon * 4.0f;
        Moon.transform.eulerAngles = (time - 0.75f) * Noon * 4.0f;

        // light intensity
        Sun.intensity = SunIntensity.Evaluate(time);
        Moon.intensity = MoonIntensity.Evaluate(time);

        // light color
        Sun.color = SunColor.Evaluate(time);
        Moon.color = MoonColor.Evaluate(time);

        // toggle sun
        if (Sun.intensity == 0 && Sun.gameObject.activeInHierarchy)
            Sun.gameObject.SetActive(false);
        else if (Sun.intensity > 0 && !Sun.gameObject.activeInHierarchy)
            Sun.gameObject.SetActive(true);

        // toggle moon
        if (Moon.intensity == 0 && Moon.gameObject.activeInHierarchy)
            Moon.gameObject.SetActive(false);
        else if (Moon.intensity > 0 && !Moon.gameObject.activeInHierarchy)
            Moon.gameObject.SetActive(true);

        // lighting & reflections intensity
        RenderSettings.ambientIntensity = LightingIntensityMultiplier.Evaluate(time);
        RenderSettings.reflectionIntensity = ReflectionsIntensityMultiplier.Evaluate(time);

        // skybox 
        RenderSettings.skybox.SetColor("_SkyTint", SkyColor.Evaluate(time));
        RenderSettings.skybox.SetColor("_GroundColor", HorizonColor.Evaluate(time));
    }

    public void Reset() {
        if (EditorTestStarted)
        {
            this.Start();
            EditorTestStarted = false;
            print("Editor Test stopped!");
        }
    }

    public void UpdateCall()
    {
        if (!EditorTestStarted)
        {
            this.Start();
            EditorTestStarted = true;
            print("Editor Test started! Changes in the Inspector will now effect the Scene View.");
        }
        this.Update();
    }

    float IncrementDay() 
    {
        if (Day + 1 > MaximumDayInThisMonth()) {
            Day = 0;
            if (Month + 1 > 12) {
                Year++;
                Month = 0;
            }
            Month++;
        }

        Day++;
        return 0.0f;
    }

    int MaximumDayInThisMonth() {
        if (knuckles.Contains(Month))
            return 31;
        if (grooves.Contains(Month))
            return 30;
        return DateTime.IsLeapYear(Year) ? 29 : 28;
    }

    void GetCurrentTime() {
        float h = time * 24.0f;
        float m = h % 1 * 60.0f;
        float s = m % 1 * 60.0f;
        float ms = s % 1 * 1000.0f;
        CurrentTime = string.Join(":", Mathf.FloorToInt(h).ToString("D2"), Mathf.FloorToInt(m).ToString("D2"), 
                    Mathf.FloorToInt(s).ToString("D2")) + "." + Mathf.FloorToInt(ms).ToString("D3");
    }

}
