using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimonSays {
    [System.Serializable]
    public struct SimonSaysButtons
    {
		public GameObject RedButton;
		public GameObject GreenButton;
		public GameObject BlueButton;
		public GameObject YellowButton;
		
		public static void SetEmissionStateOfButton(GameObject __button, bool __enabled = true)
		{
			if (__button is not null)
			{
				MeshRenderer __meshRenderer = __button.GetComponent<MeshRenderer>();
				if (__meshRenderer is not null)
					if(__enabled)
						__meshRenderer.material.EnableKeyword("_EMISSION");
					else
						__meshRenderer.material.DisableKeyword("_EMISSION");
			}
		}
	}
}