using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public PlayerController playerController;
    public Camera playerCamera;

    [Header("Raycast Info")]
    public float eyeSightDistance = 1.0f;
    public float handDistance = 0.6f;
    [ReadOnly] public RaycastHit cameraRayHit;
    [ReadOnly] public Transform eyeRaycastHitTransform;
    [ReadOnly] public string eyeRaycastHitName;

    [ReadOnly] public RaycastHit handRayHit;
    [ReadOnly] public Transform handRaycastHitTransform;
    [ReadOnly] public string handRaycastHitName;

    [Header("Debug")]
    public bool debugRays = true;
	public Color handRayColor = Color.blue;
	public Color eyeRayColor = Color.red;
    

    // Start is called before the first frame update
    void Start()
    {
        playerCamera = playerController.playerCamera is null ? Camera.main : playerController.playerCamera;
        eyeRaycastHitTransform = this.transform;
        handRaycastHitTransform = this.transform;
    }

    // Update is called once per frame
    void Update()
    {
        Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out cameraRayHit, eyeSightDistance);
        if (cameraRayHit.transform is not null)
        {
            eyeRaycastHitName = cameraRayHit.transform.name;
            eyeRaycastHitTransform = cameraRayHit.transform;
        } else 
        {
            eyeRaycastHitName = this.transform.name;
            eyeRaycastHitTransform = this.transform;
        }

        Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out handRayHit, handDistance);
        if (handRayHit.transform is not null)
        {
            handRaycastHitName = handRayHit.transform.name;
            handRaycastHitTransform = handRayHit.transform;
        } else 
        {
            handRaycastHitName = this.transform.name;
            handRaycastHitTransform = this.transform;
        }


        if (debugRays)
        {
            Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.forward.normalized * eyeSightDistance, eyeRayColor);
            Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.forward.normalized * handDistance, handRayColor);
        }
    }

    void FixedUpdate()
    {
        
    }
}
