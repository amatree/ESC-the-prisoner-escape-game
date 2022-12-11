using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationHandle : MonoBehaviour
{
	public Animator animator;
	public string isSprintingParameterName = "isSprinting";
	public string isWalkingParameterName = "isWalking";
	public string isSlowWalkingParameterName = "isSlowWalking";
	public string isIdlingParameterName = "isIdling";
	public string isGoingBackwardParameterName = "isGoingBackward";
	public string isStrafingLeftParameterName = "isStrafingLeft";
	public string isStrafingRightParameterName = "isStrafingRight";
	public string isJumpingParameterName = "isJumping";

	public string jumpingSpeedParameterName = "jumpingAnimationSpeed";
	public string slowWalkingSpeedMultiplierParameterName = "slowWalkingSpeedMultiplier";
	public float slowWalkingSpeedMultiplier = 1.0f;
	public string walkingSpeedMultiplierParameterName = "walkingSpeedMultiplier";
	public float walkingSpeedMultiplier = 3.0f;
	public string runningSpeedMultiplierParameterName = "runningSpeedMultiplier";
	public float runningSpeedMultiplier = 2.0f;
	public AnimationCurve jumpSpeedVSHeight;
	public PlayerController playerController;

    // Start is called before the first frame update
    void Start()
    {
		if (playerController is null)
			playerController = GameObject.FindObjectOfType<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool(isSprintingParameterName, playerController.isSprinting);
        animator.SetBool(isWalkingParameterName, !playerController.isSprinting && playerController.finalSpeed > 0f && !playerController.isWalkingSlow);
        animator.SetBool(isSlowWalkingParameterName, !playerController.isSprinting && playerController.finalSpeed > 0f && playerController.isWalkingSlow);
        animator.SetBool(isIdlingParameterName, playerController.finalSpeed == 0f);
        animator.SetBool(isGoingBackwardParameterName, playerController.verticalAxis < 0f);

		animator.SetBool(isStrafingLeftParameterName, playerController.horizontalAxis < 0f);
		animator.SetBool(isStrafingRightParameterName, playerController.horizontalAxis > 0f);

		animator.SetFloat(slowWalkingSpeedMultiplierParameterName, slowWalkingSpeedMultiplier);
		animator.SetFloat(walkingSpeedMultiplierParameterName, walkingSpeedMultiplier);
		animator.SetFloat(runningSpeedMultiplierParameterName, runningSpeedMultiplier);

		if (playerController.isJumping)
		{
			animator.SetTrigger(isJumpingParameterName);
			animator.SetFloat(jumpingSpeedParameterName, jumpSpeedVSHeight.Evaluate(playerController.transform.position.y));
		} else if (!playerController.isJumping && playerController.isGrounded)
			animator.ResetTrigger(isJumpingParameterName);
    }
}
