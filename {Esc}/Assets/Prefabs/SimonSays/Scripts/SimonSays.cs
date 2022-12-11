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

		public MeshRenderer MeshRenderer(SimonSaysButtonID __btn_id)
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
	public class SimonSaysButtonSequence
	{
		public float blinkTime = 0.25f;
		public float delayToNextBlink = 0.1f;
		public float delayBeforeStart = 0.2f;
		public float delayAfterPlaying = 0.5f;
		[ReadOnly] public bool isSequencePlaying;
		public SimonSaysButtons simonSaysPlates;
		public List<SimonSaysButtonID> Sequence;

		private Queue<IEnumerator> coroutineQueue;
		private MonoBehaviour __mono;

		public void DefaultSettings(MonoBehaviour mono = null)
		{
			if (mono is not null)
				__mono = mono;
			blinkTime = 0.25f;
			delayToNextBlink = 0.1f;
			delayBeforeStart = 0.2f;
			delayAfterPlaying = 0.5f;
		}

		public bool Check(List<SimonSaysButtonID> __button_sequence)
		{
			return __button_sequence.SequenceEqual(Sequence);
		}

		public void PlayAll(MonoBehaviour mono = null)
		{
			isSequencePlaying = true;
			if (coroutineQueue is null) coroutineQueue = new Queue<IEnumerator>();
			else coroutineQueue.Clear();
			foreach (SimonSaysButtonID __btn_id_go in Sequence)
			{
				MeshRenderer __meshRenderer = simonSaysPlates.MeshRenderer(__btn_id_go);
				coroutineQueue.Enqueue(ToggleEmission(__meshRenderer, true));
			}

			if (mono is null) mono = __mono;
			mono.StartCoroutine(DoCoroutineQueue(mono));
		}

		public void TogglePlate(SimonSaysButtonID buttonID, MonoBehaviour mono = null)
		{
			MeshRenderer __meshRenderer = simonSaysPlates.MeshRenderer(buttonID);
			if (mono is null) mono = __mono;
			mono.StartCoroutine(ToggleEmission(__meshRenderer, true));
		}

		IEnumerator DoCoroutineQueue(MonoBehaviour mono = null)
		{
			yield return new WaitForSeconds(delayBeforeStart);
			if (mono is null) mono = __mono;
			while (coroutineQueue.Count > 0) 
			{
				yield return mono.StartCoroutine(coroutineQueue.Dequeue());
				yield return new WaitForSeconds(delayToNextBlink);
			}
			yield return new WaitForSeconds(delayAfterPlaying);
			isSequencePlaying = false;
		}

		IEnumerator ToggleEmission(MeshRenderer __meshRenderer, bool release = true)
		{
			__meshRenderer.material.EnableKeyword("_EMISSION");
			yield return new WaitForSeconds(blinkTime);
			if (release) __meshRenderer.material.DisableKeyword("_EMISSION");
		}
		
		public void EnableEmission(SimonSaysButtonID buttonID)
		{
			EnableEmission(simonSaysPlates.MeshRenderer(buttonID));
		}

		public void EnableEmission(SimonSaysButtonHandle buttonHandle)
		{
			EnableEmission(simonSaysPlates.MeshRenderer(buttonHandle.buttonID));
		}

		public void DisableEmission(SimonSaysButtonID buttonID)
		{
			DisableEmission(simonSaysPlates.MeshRenderer(buttonID));
		}

		public void DisableEmission(SimonSaysButtonHandle buttonHandle)
		{
			DisableEmission(simonSaysPlates.MeshRenderer(buttonHandle.buttonID));
		}

		public void EnableEmission(MeshRenderer __meshRenderer)
		{
			if (__mono is not null)
				__mono.StartCoroutine(ToggleEmission(__meshRenderer, false));
			else
				__meshRenderer.material.EnableKeyword("_EMISSION");

		}

		public void DisableEmission(MeshRenderer __meshRenderer)
		{
			__meshRenderer.material.DisableKeyword("_EMISSION");
		}
	}
}