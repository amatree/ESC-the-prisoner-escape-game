using UnityEngine;
using TMPro;

namespace UIElements {
    [System.Serializable]
    public struct MenuUIButtons
    {
        public TMP_Text ID_00StartButton;
        public TMP_Text ID_01OptionsButton;
        public TMP_Text ID_02ExitButton;

        [ReadOnly] public bool ID_00StartButtonHovered;
        [ReadOnly] public bool ID_01OptionsButtonHovered;
        [ReadOnly] public bool ID_02ExitButtonHovered;

        public TMP_Text GetTMP_TextFromID(int buttonID)
        {
            if (buttonID == 0) return ID_00StartButton;
            if (buttonID == 1) return ID_01OptionsButton;
            if (buttonID == 2) return ID_02ExitButton;

            return null;
        }
        
        public void ToggleHoveredOn(int buttonID)
        {
            ToggleHoveredOn(GetTMP_TextFromID(buttonID));
        }
        
        public void ToggleHoveredOff(int buttonID)
        {
            ToggleHoveredOff(GetTMP_TextFromID(buttonID));
        }

        public void ToggleHoveredOn(TMP_Text buttonTMP_Text)
        {
            if (buttonTMP_Text == ID_00StartButton && !ID_00StartButtonHovered)
            {
                ID_00StartButton.text = " > " + ID_00StartButton.text + " < ";
                ID_00StartButtonHovered = true;
            }
            if (buttonTMP_Text == ID_01OptionsButton && !ID_01OptionsButtonHovered) {
                ID_01OptionsButton.text = " < " + ID_01OptionsButton.text;
                ID_01OptionsButtonHovered = true;
            }
            if (buttonTMP_Text == ID_02ExitButton && !ID_02ExitButtonHovered) {
                ID_02ExitButton.text = ID_02ExitButton.text + " > ";
                ID_02ExitButtonHovered = true;
            }
        }

        public void ToggleHoveredOff(TMP_Text buttonTMP_Text)
        {
            if (buttonTMP_Text == ID_00StartButton && ID_00StartButtonHovered)
            {
                ID_00StartButton.text =  ID_00StartButton.text.Replace(" > ", "").Replace(" < ", "");
                ID_00StartButtonHovered = false;
            }
            if (buttonTMP_Text == ID_01OptionsButton && ID_01OptionsButtonHovered) {
                ID_01OptionsButton.text = ID_01OptionsButton.text.Replace(" > ", "").Replace(" < ", "");
                ID_01OptionsButtonHovered = false;
            }
            if (buttonTMP_Text == ID_02ExitButton && ID_02ExitButtonHovered) {
                ID_02ExitButton.text = ID_02ExitButton.text.Replace(" > ", "").Replace(" < ", "");
                ID_02ExitButtonHovered = false;
            }
        }
    }

    [System.Serializable]
    public struct OptionsUIButtons
    {
        public TMP_Text ID_00BackButton;
        [ReadOnly] public bool ID_00BackButtonHovered;

        public TMP_Text GetTMP_TextFromID(int buttonID)
        {
            if (buttonID == 0) return ID_00BackButton;

            return null;
        }

        public void ToggleHoveredOn(int buttonID)
        {
            ToggleHoveredOn(GetTMP_TextFromID(buttonID));
        }
        
        public void ToggleHoveredOff(int buttonID)
        {
            ToggleHoveredOff(GetTMP_TextFromID(buttonID));
        }

        public void ToggleHoveredOn(TMP_Text buttonTMP_Text)
        {
            if (buttonTMP_Text == ID_00BackButton && !ID_00BackButtonHovered)
            {
                ID_00BackButton.text = " > " + ID_00BackButton.text;
                ID_00BackButtonHovered = true;
            }
        }

        public void ToggleHoveredOff(TMP_Text buttonTMP_Text)
        {
            if (buttonTMP_Text == ID_00BackButton && ID_00BackButtonHovered)
            {
                ID_00BackButton.text =  ID_00BackButton.text.Replace(" > ", "").Replace(" < ", "");
                ID_00BackButtonHovered = false;
            }
        }
    }
}