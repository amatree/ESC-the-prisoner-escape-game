using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDSettings : MonoBehaviour
{
    [Header("Text Components")]
    public TMP_Text tooltipTextComponent;
    [ReadOnly] public string currentTooltipText;

    [Header("Crosshair Settings")]
    public Image crossHair;
    [Range(0.2f, 1.0f)] public float crossHairSize = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (tooltipTextComponent is not null)
            currentTooltipText = tooltipTextComponent.text;
        
    }

    public void SetTooltipText(string setText)
    {
        if (tooltipTextComponent is not null)
            tooltipTextComponent.text = setText;
    }

    public void SetTooltipTextState(bool enabled)
    {
        if (tooltipTextComponent is not null)
            tooltipTextComponent.enabled = enabled;

    }
}
