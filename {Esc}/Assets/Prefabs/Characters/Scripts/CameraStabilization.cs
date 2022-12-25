using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraStabilization : MonoBehaviour
{
	public PlayerController playerController;
	public Transform headTransform;

	[ReadOnly] public Camera playerCamera;
	[ReadOnly] public float timeCount;
	[ReadOnly] public Vector3 velocity = Vector3.zero;
	[ReadOnly] public float smoothTime = 0f;

	private bool isWaitingForCameraControl = false;

    // Start is called before the first frame update
    void Start()
    {
		playerCamera = playerController.playerCamera;
		playerController.GiveUpCameraControl();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
		if (playerController.hasAllControl)
		{
			Quaternion fromRot = playerCamera.transform.localRotation;
			Quaternion toRot = Quaternion.Euler(-playerController.mouseY, 0f, 0f);
			playerCamera.transform.localRotation = Quaternion.Slerp(fromRot, toRot, timeCount);
			timeCount += Time.fixedDeltaTime;

			Vector3 targetPos = headTransform.position;
			playerCamera.transform.position = Vector3.SmoothDamp(playerCamera.transform.position, targetPos, ref velocity, smoothTime);
		} else 
		{
			if (!isWaitingForCameraControl)
			{
				isWaitingForCameraControl = true;
				StartCoroutine(WaitForCameraControl());
			}
		}
    }

	IEnumerator WaitForCameraControl()
	{
		if (isWaitingForCameraControl)
		{
			while (!playerController.hasAllControl)
				yield return null;
			
			playerController.GiveUpCameraControl();
			isWaitingForCameraControl = false;
		}
	}
}
