using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimonSays;

public class SimonSaysButtonHandle : MonoBehaviour
{
	public SimonSaysButtonID buttonID;
	public Animator animator;
	public float animationSpeed = 2.0f;
	[ReadOnly] public bool isPushing = false;

    // Start is called before the first frame update
    void Start()
    {
        if (animator is null) 
			animator = GetComponent<Animator>();
		animator.SetFloat("animationSpeed", animationSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void PushAndHold()
	{
		StartCoroutine(ButtonPush(false));
	}

	public void Push()
	{
		StartCoroutine(ButtonPush());
	}

	public void Release()
	{
		StartCoroutine(ButtonRelease());
	}

	IEnumerator ButtonPush(bool release = true)
	{
		if (!isPushing)
		{
			isPushing = true;
			animator.Play("Push");
			yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
			if (release)
				yield return StartCoroutine(ButtonRelease());
		}
	}

	IEnumerator ButtonRelease()
	{
		if (isPushing)
		{
			animator.Play("Release");
			yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
			isPushing = false;
		}
	}
}
