using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UIElements;

public class IntroUI : MonoBehaviour {
    public MenuUIButtons UIButtons;
    [ReadOnly] public GameObject currentHoveredElement;
    [ReadOnly] public TMP_Text currentHoveredBtnText;

    [Header("Options UI")]
    [SerializeField]
    public OptionsUI optionsUI;

    [Header("Intro Camera Animation")]
    [SerializeField]
    public IntroCameraAnimation introCameraAnimation;    
    
    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        
    }

    public void OptionsButtonClick()
    {
        introCameraAnimation.AnimateOptionsMenu();
    }

    public void ToggleButtonHoverOn(int buttonID)
    {
        UIButtons.ToggleHoveredOn(buttonID);
    }

    public void ToggleButtonHoverOff(int buttonID)
    {
        UIButtons.ToggleHoveredOff(buttonID);
    }
}