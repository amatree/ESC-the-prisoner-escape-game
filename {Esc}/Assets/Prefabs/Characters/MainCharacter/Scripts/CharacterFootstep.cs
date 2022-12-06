using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterFootstep : MonoBehaviour
{
	[Header("Dependencies")]
	public Animator characterAnimator;
	public AudioSource audioSource;
	public ErikaArcherAnimationHandle animationHandler;

	[Header("Clips")]
	public AudioClip walkingClip;
	public AudioClip runningClip;

	Coroutine footstepClipC;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (characterAnimator is not null)
		{
			if (characterAnimator.GetBool(animationHandler.isWalkingParameterName))
			{
			 	footstepClipC = StartCoroutine(PlayClip(walkingClip));
			} else if (characterAnimator.GetBool(animationHandler.isSprintingParameterName))
			{ 
				footstepClipC = StartCoroutine(PlayClip(runningClip));
			} else {
				if  (footstepClipC is not null) StopCoroutine(footstepClipC);
				if (audioSource.clip == walkingClip || audioSource.clip == runningClip) audioSource.clip = null;
			}
		}
    }

	IEnumerator PlayClip(AudioClip clip)
	{
		if (audioSource.clip != clip) audioSource.clip = clip;
		if (audioSource.isPlaying) audioSource.Stop();
		audioSource.Play();
		while (audioSource.isPlaying)
			yield return null;
		audioSource.Stop();
	}
}
