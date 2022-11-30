using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ButtonTextHoverEffect : MonoBehaviour
{
    public TMP_Text btnText;

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

    public void ToggleHover()
    {
        if (btnText.text.Contains(" { ") || btnText.text.Contains(" } "))
            HoverOff();
        else
            HoverOn();
    }

    public void HoverOn()
    {
        if (!btnText.text.Contains(" { ") && !btnText.text.Contains(" } "))
            btnText.text = " { " + btnText.text + " } ";
    }

    public void HoverOff()
    {
        btnText.text = btnText.text.Replace(" { ", "").Replace(" } ", "");
    }
}
