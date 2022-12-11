using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterFootstep : MonoBehaviour
{
	[Header("Dependencies")]
	public Animator characterAnimator;
	public AudioSource sfxSource;
	public AudioSource audioSource;
	public CharacterAnimationHandle animationHandler;
	public PlayerController playerController;

	[Header("SFX Settings")]
	public bool enable3D;

	[Header("Clips")]
	public AudioClip walkingClip;
	public AudioClip runningClip;
	public AudioClip landingClip;
	public AudioClip jumpingClip;

	Coroutine footstepClipC;

	[Header("Settings")]
	public float landingSFXInterval = 1.0f;

	[Header("Debug")]
	[ReadOnly] public float timeInAirWithoutJumping = 0f;  
	[ReadOnly] public bool didJump;
	[ReadOnly] public bool isInAirWithoutJumping;

    // Start is called before the first frame update
    void Start()
    {
		if (playerController is null)
			playerController = GameObject.FindObjectOfType<PlayerController>();
        if (enable3D && sfxSource is not null)
		{
			audioSource.spatialBlend = 1.0f;
			sfxSource.spatialBlend = 1.0f;
		}
    }

    // Update is called once per frame
    void Update()
    {
        if (characterAnimator is not null)
		{
			if (playerController.finalSpeed > 0f && playerController.finalSpeed < playerController.sprintSpeed && playerController.isGrounded)
			{
				if (footstepClipC is not null) StopCoroutine(footstepClipC);
			 	footstepClipC = StartCoroutine(PlayClip(walkingClip));
			} else if (playerController.isSprinting && playerController.isGrounded)
			{ 
				if (footstepClipC is not null) StopCoroutine(footstepClipC);
				footstepClipC = StartCoroutine(PlayClip(runningClip));
			} else {
				if (footstepClipC is not null) StopCoroutine(footstepClipC);
				if (audioSource.clip == walkingClip || audioSource.clip == runningClip) audioSource.clip = null;
			}


			if (playerController.isJumping)
			{
				StartCoroutine(JumpAndLand());
			} else if (!playerController.isGrounded && !playerController.isJumping && !playerController.isOnSlope && !isInAirWithoutJumping)
			{
				StartCoroutine(Landing());
			}
		}
    }

	IEnumerator Landing()
	{
		if (!isInAirWithoutJumping && !didJump)
		{
			isInAirWithoutJumping = true;
			float __t = 0f;
			while (__t < landingSFXInterval)
			{
				if (playerController.isGrounded || didJump)
				{
					isInAirWithoutJumping = false;
					// print("aired but not enough time to land " + __t);
					yield break;
				}

				__t += Time.deltaTime;
				timeInAirWithoutJumping = __t;
				yield return null;
			}
			
			while (!playerController.isGrounded)
			{
				__t += Time.deltaTime;
				timeInAirWithoutJumping = __t;
				yield return null;
			}

			sfxSource.PlayOneShot(landingClip, 0.1f);
			// print("aired and landed " + __t);
			isInAirWithoutJumping = false;

		}
	}

	IEnumerator JumpAndLand()
	{
		if (!playerController.isGrounded && !didJump)
		{
			didJump = true;
			audioSource.Pause();
			sfxSource.PlayOneShot(jumpingClip, 0.2f);
		}

		while (!playerController.isGrounded)
			yield return new WaitForEndOfFrame();

		didJump = false;
		sfxSource.PlayOneShot(landingClip, 0.01f);
		audioSource.UnPause();
	}

	IEnumerator PlayClip(AudioClip clip)
	{
		if (audioSource.isPlaying && audioSource.clip != clip) audioSource.Stop();
		if (audioSource.clip != clip) audioSource.clip = clip;
		if (!audioSource.isPlaying) audioSource.Play();
		float t = 0f;
		while (t < clip.length)
		{
			t += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		audioSource.Stop();
	}
}
