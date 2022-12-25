using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ButtonTextEffect : MonoBehaviour
{
    public TMP_Text btnText;
	public Vector2 holdOffset = new Vector2(0.1f, 0.1f);
	[ReadOnly] public bool wasHeld = false;

    // Start is called before the first frame update
    void Start()
    {
        if (btnText is null)
            btnText = transform.GetComponentInChildren<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void HoldOn()
	{
		if (!wasHeld)
		{
			wasHeld = true;
			btnText.rectTransform.anchoredPosition += holdOffset / 10f;
			HoverOff();
		}
	}

	public void HoldOff()
	{
		if (wasHeld)
		{
			wasHeld = false;
			btnText.rectTransform.anchoredPosition -= holdOffset / 10f;
			HoverOn();
		}
	}

    public void ToggleHover()
    {
        if (btnText.text.Contains("{  ") || btnText.text.Contains("  }"))
            HoverOff();
        else
            HoverOn();
    }

    public void HoverOn()
    {
        if (!btnText.text.Contains("{  ") && !btnText.text.Contains("  }"))
            btnText.text = "{  " + btnText.text + "  }";
    }

    public void HoverOff()
    {
        btnText.text = btnText.text.Replace("{  ", "").Replace("  }", "");
    }
}
