using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterFootstep : MonoBehaviour
{
	[Header("Dependencies")]
	public Animator characterAnimator;
	public AudioSource audioSource;
	public ErikaArcherAnimationHandle animationHandler;
	public PlayerController playerController;

	[Header("SFX Settings")]
	public bool enable3D;

	[Header("Clips")]
	public AudioClip walkingClip;
	public AudioClip runningClip;
	public AudioClip landingClip;
	public AudioClip jumpingClip;

	Coroutine footstepClipC;

	bool didJump;
	bool wasOnGround;

    // Start is called before the first frame update
    void Start()
    {
        if (enable3D && audioSource is not null)
			audioSource.spatialBlend = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
		if (!wasOnGround && playerController.isGrounded)
			wasOnGround = true;

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


			if (!playerController.isGrounded && wasOnGround)
			{
				StartCoroutine(JumpAndLand());
			}
		}
    }

	IEnumerator JumpAndLand()
	{
		if (!playerController.isGrounded && !didJump)
		{
			didJump = true;
			audioSource.PlayOneShot(jumpingClip, 0.3f);
		}

		while (!playerController.isGrounded)
			yield return new WaitForEndOfFrame();

		didJump = false;
		audioSource.PlayOneShot(landingClip, 0.05f);
	}

	IEnumerator PlayClip(AudioClip clip)
	{
		if (audioSource.clip != clip) audioSource.clip = clip;
		// if (audioSource.isPlaying) audioSource.Stop();
		if (!audioSource.isPlaying) audioSource.Play();
		while (audioSource.isPlaying)
		{
			yield return new WaitForEndOfFrame();
		}
		audioSource.Stop();
	}
}
