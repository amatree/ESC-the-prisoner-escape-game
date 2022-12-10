using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDSettings : MonoBehaviour
{
	[Header("Game Configuration")]
    public GameConfiguration gameConfiguration;

    [Header("Text Components")]
    public TMP_Text tooltipTextComponent;
    [ReadOnly] public string currentTooltipText;
	[ReadOnly] public bool isLocked = false;

    [Header("Crosshair Settings")]
    public Image crossHair;
    [Range(0.2f, 1.0f)] public float crossHairSize = 0.2f;

	float prevCrosshairSize = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        prevCrosshairSize = crossHairSize;
    }

    // Update is called once per frame
    void Update()
    {
        // if (tooltipTextComponent is not null)
        //     currentTooltipText = tooltipTextComponent.text;
		if (crossHair is not null && crossHairSize != prevCrosshairSize)
		{
			prevCrosshairSize = crossHairSize;
			crossHair.rectTransform.localScale = new Vector3(crossHairSize, crossHairSize, crossHairSize);
		}
    }

	public void AquireLock()
	{
		if (!isLocked)
			isLocked = true;
	}

	public void ReleaseLock()
	{
		if (isLocked)
			isLocked = false;
	}

    public bool GetState()
    {
        return tooltipTextComponent.enabled;
    }

    public void MoveTextPosition(float x = 0f, float y = 0f, float z = 0f)
    {
        if (tooltipTextComponent is not null)
            tooltipTextComponent.rectTransform.anchoredPosition3D += new Vector3(x, y, z);

    }

    public void SetTextPosition(Vector3 position)
    {
        if (tooltipTextComponent is not null)
            tooltipTextComponent.rectTransform.anchoredPosition3D = position;
    }

    public void SetTextColor(Color color)
    {
        if (tooltipTextComponent is not null)
            tooltipTextComponent.color = color;
    }

    public void SetTooltipText(string setText = "")
    {
		if (!isLocked)
		{
			if (!GetState())
				SetTooltipTextState(true);

			if (setText == "")
				SetTextColor(Color.white);

			if (tooltipTextComponent is not null)
				tooltipTextComponent.text = setText;
		}
    }

    public void SetTooltipText(string setText, Color color)
    {
        SetTextColor(color);
        SetTooltipText(setText);
    }

    public void SetTooltipTextState(bool enabled)
    {
        if (tooltipTextComponent is not null)
            tooltipTextComponent.enabled = enabled;
    }

    public void ToggleTooltip(string text, Color color, float duration = 2.0f)
    {
        StartCoroutine(ToggleTooltipCoroutine(text, color, duration));
    }

    public void ToggleTooltip(string text, float duration = 2.0f)
    {
        StartCoroutine(ToggleTooltipCoroutine(text, Color.white, duration));
    }

    IEnumerator ToggleTooltipCoroutine(string text, Color color, float duration = 2.0f)
    {
        SetTooltipText(text, color);
        SetTooltipTextState(true);
        yield return new WaitForSeconds(duration);
        SetTooltipTextState(false);
        SetTooltipText();
        yield break;
    }
}
