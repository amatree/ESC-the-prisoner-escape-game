using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SimonSays {
	public enum SimonSaysButtonID { RED, GREEN, BLUE, YELLOW }

    [System.Serializable]
    public class SimonSaysButtons
    {
		public GameObject RedButton;
		public SimonSaysButtonID RedButtonID = SimonSaysButtonID.RED;
		public GameObject GreenButton;
		public SimonSaysButtonID GreenButtonID = SimonSaysButtonID.GREEN;
		public GameObject BlueButton;
		public SimonSaysButtonID BlueButtonID = SimonSaysButtonID.BLUE;
		public GameObject YellowButton;
		public SimonSaysButtonID YellowButtonID = SimonSaysButtonID.YELLOW;
		
		public void SetEmissionStateOfButton(GameObject __button, bool __enabled = true)
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

		public MeshRenderer GetButtonMR(SimonSaysButtonID __btn_id)
		{
			if (__btn_id == SimonSaysButtonID.RED)
				return RedButton.GetComponent<MeshRenderer>();
			if (__btn_id == SimonSaysButtonID.GREEN)
				return GreenButton.GetComponent<MeshRenderer>();
			if (__btn_id == SimonSaysButtonID.BLUE)
				return BlueButton.GetComponent<MeshRenderer>();
			return YellowButton.GetComponent<MeshRenderer>();
		}
	}

    [System.Serializable]
	public struct SimonSaysButtonSequence
	{
		public float blinkTime;
		public float delayToNextBlink;
		public SimonSaysButtons simonSaysButtons;
		public List<SimonSaysButtonID> Sequence;
		[ReadOnly] public bool isSequencePlaying;

		private Queue<IEnumerator> coroutineQueue;

		public bool Check(List<SimonSaysButtonID> __button_sequence)
		{
			return __button_sequence.SequenceEqual(Sequence);
		}

		public void Play(MonoBehaviour mono)
		{
			coroutineQueue = new Queue<IEnumerator>();
			foreach (SimonSaysButtonID __btn_id_go in Sequence)
			{
				MeshRenderer __meshRenderer = simonSaysButtons.GetButtonMR(__btn_id_go);
				coroutineQueue.Enqueue(ToggleEmission(__meshRenderer));
			}

			mono.StartCoroutine(DoCoroutineQueue(mono));
		}

		IEnumerator DoCoroutineQueue(MonoBehaviour mono)
		{
			isSequencePlaying = true;
			while (coroutineQueue.Count > 0) 
			{
				yield return mono.StartCoroutine(coroutineQueue.Dequeue());
				yield return new WaitForSeconds(delayToNextBlink);
			}
			isSequencePlaying = false;
		}

		IEnumerator ToggleEmission(MeshRenderer __meshRenderer)
		{
			__meshRenderer.material.EnableKeyword("_EMISSION");
			yield return new WaitForSeconds(blinkTime);
			__meshRenderer.material.DisableKeyword("_EMISSION");
		}
	}
}