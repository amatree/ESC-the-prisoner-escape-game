using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footstep : MonoBehaviour
{
    public PlayerController playerController;
    public AudioSource audioSource;
    public AudioClip WalkSFX;
    public AudioClip RunSFX;

    float raydistance = 1.0f;
    float cliplength = 1.0f;
    AudioClip prev;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = 0.1f;
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit; 
        Physics.Raycast(new Ray(transform.position, Vector3.down), out hit, raydistance);
        if (hit.transform != null) {
            if (hit.transform.tag == "Ground" && playerController.movementSpeed > 0 && playerController.isGrounded) {
                Invoke("PlayFSSFX", cliplength);
            }
            else
                audioSource.Stop();
        }

        if (!playerController.isGrounded)
        {
            raydistance = 0.3f;
            audioSource.Stop();
            audioSource.clip = null;
        } else 
            raydistance = 1.0f;
        
    }
    
    void PlayFSSFX() {
        prev = audioSource.clip;
        audioSource.clip = playerController.isSprinting ? RunSFX : WalkSFX;
        cliplength = audioSource.clip.length;

        if (prev != audioSource.clip) {
            audioSource.Stop();
            audioSource.Play();
        }
        if (!audioSource.isPlaying)
            audioSource.Play();
    }
}
