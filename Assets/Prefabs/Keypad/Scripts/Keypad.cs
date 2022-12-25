using UnityEngine;

namespace Keypad
{
    [System.Serializable]
    public struct KeypadMaterials {
        public Material KeypadDefault;
        public Material KeypadClicked000;
        public Material KeypadClicked001;
        public Material KeypadClicked002;
        public Material KeypadClicked003;
        public Material KeypadClicked004;
        public Material KeypadClicked005;
        public Material KeypadClicked006;
        public Material KeypadClicked007;
        public Material KeypadClicked008;
        public Material KeypadClicked009;
    }

    [System.Serializable]
    public struct KeypadSoundEffects {
        public AudioClip KeypadClickHoldSFX;
        public AudioClip KeypadClickReleaseSFX;
        public AudioClip CorrectSFX;
        public AudioClip IncorrectSFX;
    }

}