using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ErikaArcherAnimationHandle : MonoBehaviour
{
	public Animator animator;
	public string isSprintingParameterName = "isSprinting";
	public string isWalkingParameterName = "isWalking";
	public string isIdlingParameterName = "isIdling";
	public PlayerController playerController;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool(isSprintingParameterName, playerController.isSprinting);
        animator.SetBool(isWalkingParameterName, !playerController.isSprinting && playerController.finalSpeed > 0f);
        animator.SetBool(isIdlingParameterName, playerController.finalSpeed == 0f);
    }
}
