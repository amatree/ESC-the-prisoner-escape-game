using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Player")]
    public Rigidbody rigidBody;
	public CapsuleCollider playerCollider;
	public GameObject playerModelPrefab;

    [Header("Speed & Key Configuration")]
    [ReadOnly] public float currentSpeed = 0f;
    [Range(1f, 10f)] public float slowWalkSpeed = 2f;
    public KeyCode slowWalkKey = KeyCode.LeftControl;
    [Range(1f, 10f)] public float movementSpeed = 5f;
    [Range(1f, 15f)] public float sprintSpeed = 10f;
    public KeyCode sprintKey = KeyCode.LeftShift;
    [Range(1f, 10f)] public float jumpHeight = 3f;
    public KeyCode jumpKey = KeyCode.Space;
    [Tooltip("Disabled cuz it broke animation :3")] [ReadOnly] public bool enableDoubleJump = false;

	[Header("Slope Handling (integrated with Stair Handling)")]
	[ReadOnly] public bool isOnSlope;
	[ReadOnly, Range(0f, 90f)] public float maxSlopeAngle = 35.0f;
	[ReadOnly] public float currSlopeAngle;
	[ReadOnly] public RaycastHit slopeHit;
	[ReadOnly] public bool groundDistanceChanged = false;

	[Header("Stair Handling")]
	public bool stairDebugRays = true;
	[Range(0f, 1f)] public float maxStepHeight = 0.3f;
	[Range(0f, 1f)] public float maxStepSize = 0.25f;

    [Header("Animation")]
    public Animator playerAnimation;

    [Header("Camera")]
    public Camera playerCamera;
    [Range(0f, 50f)] public float mouseSensitivity = 5f;

    [Header("Physics")]
    public float gravity = Physics.gravity.y;
    [Range(0f, 2f)] public float accelerationMultiplier = 2.0f;
    [Range(0f, 1f)] public float frictionMultiplier = 0.7f;
    [Range(0f, 1f)] public float airAccelerationMultiplier = 0.7f;
    [Range(0f, 2f)] public float airFrictionMultiplier = 2.0f;
    [Range(0.01f, 1f)] public float airDrag = 0.5f;

    [Header("Ground")]
    public Transform groundCheck;
    public LayerMask groundMask;
    public float groundDistance = 0.03f;

    [Header("Audio")]
    public AudioSource audioSource;

	[Header("Debug")]
	[ReadOnly] public Vector3 moveVector;
    [ReadOnly] public float moveMagnitude;
    [ReadOnly] public float finalSpeed;
    [ReadOnly] public float verticalAxis;
    [ReadOnly] public float horizontalAxis;

    [ReadOnly] public bool isGrounded;
    [ReadOnly] public bool isJumping;
    [ReadOnly] public bool isWalkingSlow;
    [ReadOnly] public bool isSprinting;

    [ReadOnly] public int jumpCount;
    [ReadOnly] public bool isJumpKeyPressed;
    [ReadOnly] public bool isJumpKeyReleased;

    [ReadOnly] public int collisionContactCount = 0;

    [ReadOnly] public bool hasCameraControl = true;
    [ReadOnly] public bool hasAllControl = true;

    [ReadOnly] public float mouseX;
    [ReadOnly] public float prev_mouseX;
    [ReadOnly] public float mouseY;

	[ReadOnly] public float pGroundDistance;
	[ReadOnly] public float dGroundDistance;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = rigidBody == null ? GetComponent<Rigidbody>() : rigidBody;
        rigidBody.useGravity = false;
        rigidBody.drag = 0f;
        rigidBody.centerOfMass = Vector3.zero;
        rigidBody.inertiaTensor = Vector3.zero;

        audioSource = GetComponent<AudioSource>();

        playerCamera = playerCamera == null ? GetComponentInChildren<Camera>() : playerCamera;
        Cursor.lockState = CursorLockMode.Locked;

		pGroundDistance = groundDistance;
		dGroundDistance = groundDistance;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        if (hasAllControl)
        {
            // state checks
            isSprinting = Input.GetKey(sprintKey);
			isWalkingSlow = Input.GetKey(slowWalkKey);
			
            finalSpeed = isSprinting ? sprintSpeed : isWalkingSlow ? slowWalkSpeed : movementSpeed;
            if (collisionContactCount > 1 && !isGrounded)
                finalSpeed = 0f;

            // RaycastHit ray = RayCastFromFeet(Vector3.down * groundDistace, groundDistace);
            // Debug.DrawRay(groundCheck.position, Vector3.down * groundDistace, Color.cyan);
            // isGrounded = ray.transform is not null && (int)Mathf.Pow(2, ray.transform.gameObject.layer) == groundMask;
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

            // rotation
            mouseX += Input.GetAxis("Mouse X") * mouseSensitivity * 20f * Time.fixedDeltaTime;
            mouseX = mouseX % 360;
            mouseY += Input.GetAxis("Mouse Y") * mouseSensitivity * 20f * Time.fixedDeltaTime;
            mouseY = Mathf.Clamp(mouseY, -90f, 90f);

            if (mouseX - prev_mouseX > 30f)
            {
                prev_mouseX += 30f;
                mouseX = prev_mouseX;
            }

            transform.localRotation = Quaternion.Euler(-mouseY, 0f, 0f);
            if (hasCameraControl) playerCamera.transform.localRotation = Quaternion.Euler(-mouseY, 0f, 0f);
            transform.rotation = Quaternion.Euler(0f, mouseX, 0f);

            // position
            moveVector = Vector3.zero;
			verticalAxis = Input.GetAxisRaw("Vertical") * finalSpeed * 1000f * Time.fixedDeltaTime;
			horizontalAxis = Input.GetAxisRaw("Horizontal") * finalSpeed * 1000f * Time.fixedDeltaTime;
			if (verticalAxis == 0 && horizontalAxis == 0)
				finalSpeed = 0f;
			moveVector = Vector3.ClampMagnitude(transform.right * horizontalAxis + transform.forward * verticalAxis, finalSpeed * 25f);
			moveMagnitude = moveVector.magnitude;

            // double jump
            // if (jumpCount > 0 && jumpCount < 2 && enableDoubleJump && !isGrounded)
            // {
            //     if (isJumpKeyReleased && Input.GetKeyDown(jumpKey))
            //     {
            //         AddForce(Vector3.up * Mathf.Sqrt(-2f * jumpHeight / gravity) * 1.0f, ForceMode.VelocityChange);
            //         jumpCount++;
            //     }
            // }
            // if (isGrounded && jumpCount > 0)
            //     jumpCount = 0;
            // isJumpKeyReleased = !Input.GetKey(jumpKey);
			// isJumpKeyPressed = Input.GetKeyDown(jumpKey);
            // if (!isJumpKeyReleased && isGrounded && jumpCount == 0)
            // {
            //     AddForce(Vector3.up * Mathf.Sqrt(-2f * jumpHeight / gravity) * 1.0f, ForceMode.VelocityChange);
            //     jumpCount++;
            // }

			// single jump
			if (Input.GetKey(jumpKey) && isGrounded && !isJumping)
			{
				StartCoroutine(ToggleJump(jumpHeight + (moveVector.magnitude / 320f)));
			}

			if (isGrounded && rigidBody.velocity.magnitude != 0)
			{
                AddForce(moveVector * accelerationMultiplier - moveVector * frictionMultiplier, ForceMode.Acceleration);
			} else if (!isGrounded && rigidBody.velocity.magnitude != 0)
			{
                AddForce((moveVector * airAccelerationMultiplier - moveVector * airFrictionMultiplier) / -airDrag, ForceMode.Acceleration); 
			} else if (groundDistanceChanged)
			{
				groundDistance = dGroundDistance;
				groundDistanceChanged = false;
			}
            
			// stair & slope handling
			StepHandle();

            // animation
            // playerAnimation.SetBool("isRunning", isSprinting && move.magnitude != 0);
            // playerAnimation.SetBool("isWalking", move.magnitude != 0);
        }

        // gravity
		if (transform.position.y > 0f) rigidBody.AddForce(new Vector3(0, gravity, 0), ForceMode.Acceleration);
		
		// if (transform.position.y < 0f) rigidBody.AddForce(new Vector3(0, -gravity, 0), ForceMode.Acceleration);
		// if (transform.position.y < 0f) transform.position = new Vector3(transform.position.x, 0, transform.position.z);
    }

	void StepHandle() {
		Vector3 movingDirection = moveVector.normalized;
		Vector3 stepVec = Quaternion.Euler(transform.eulerAngles) * Vector3.forward * maxStepSize;
		stepVec += transform.position + new Vector3(0, maxStepHeight, 0);
		float length = maxStepHeight - pGroundDistance;
		
		if (stairDebugRays)
		{
			Debug.DrawRay(movingDirection, Vector3.forward * length, Color.blue);
			Debug.DrawRay(movingDirection, Vector3.down * length, Color.black);
			Debug.DrawRay(stepVec, Vector3.down * length, Color.yellow);
		}
		if (Physics.Raycast(stepVec, Vector3.down, out RaycastHit stepHit, length))
		{
			Debug.DrawRay(stepHit.point, Vector3.down * length, Color.magenta);
			if (stepHit.point.y - transform.position.y <= maxStepHeight)
			{
				if (finalSpeed > 0f && !isJumping)
				{
					if (groundDistance < pGroundDistance + maxStepHeight)
						groundDistance = pGroundDistance + maxStepHeight;
					transform.position = new Vector3(transform.position.x, stepHit.point.y, transform.position.z);
					// rigidBody.AddForce(Vector3.up * Mathf.Sqrt(-2f * jumpHeight / gravity) * (maxStepHeight + (moveVector.magnitude / 240f)), ForceMode.Impulse);
				}
			} else
			{
				rigidBody.AddForce(new Vector3(0, gravity, 0), ForceMode.Impulse);
			}
		} else {
			if (groundDistance != pGroundDistance)
				groundDistance = pGroundDistance;
		}
	}

	bool OnSlope()
	{
		if (Physics.Raycast(playerModelPrefab.transform.position, Vector3.down, out slopeHit, playerCollider.height * 0.5f + 0.3f))
		{
			currSlopeAngle = Vector3.Angle(transform.forward , slopeHit.normal) - 90f;
			return currSlopeAngle < maxSlopeAngle && currSlopeAngle != 0;
		}

		return false;
	}

	Vector3 GetSlopeMoveDirection() {
		return Vector3.ProjectOnPlane(moveVector, slopeHit.normal).normalized;
	}

	IEnumerator ToggleJump(float jumpForceMultiplier = 3.0f)
	{
		if (!isJumping)
		{
			float timeout = 3.0f;
			float currTimeout = 0f;
			if (isOnSlope)
				jumpForceMultiplier = 3.0f;
			rigidBody.AddForce(Vector3.up * Mathf.Sqrt(-2f * jumpHeight / gravity) * jumpForceMultiplier, ForceMode.Impulse);

			isJumping = true;
			while (isGrounded && currTimeout < timeout) // wait until force takes transform up
			{
				currTimeout += Time.fixedDeltaTime;
				yield return new WaitForEndOfFrame();
			}

			while (!isGrounded)
				yield return new WaitForEndOfFrame();

			isJumping = false;
		}
	}

    void AddForce(Vector3 force, ForceMode mode, bool resetVelocity = true)
    {
        if (resetVelocity)
        {
            currentSpeed = rigidBody.velocity.x;
            rigidBody.velocity = new Vector3(0, rigidBody.velocity.y, 0);
        }
            rigidBody.AddForce(force, mode);
    }

    public RaycastHit RayCastFromFeet(Vector3 direction, float maxDistance)
    {
        Physics.Raycast(groundCheck.position, direction, out RaycastHit hit, maxDistance);
        return hit;
    }

    public void GiveUpAllControl()
    {
		verticalAxis = 0f;
		horizontalAxis = 0f;
		finalSpeed = 0f;
        rigidBody.velocity = new Vector3(0, 0, 0);

		isJumping = false;
		isGrounded = true;
		isSprinting = false;
        // transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        hasAllControl = false;
    }

    public void RestoreAllControl()
    {
        hasAllControl = true;
        hasCameraControl = true;
    }

    public void GiveUpCameraControl()
    {
        hasCameraControl = false;
    }

    public void RestoreCameraControl()
    {
        hasCameraControl = true;
    }

    void OnCollisionEnter(Collision collisionInfo)
    {
        // print(collisionInfo.contactCount);
    }

    void OnCollisionStay(Collision collision)
    {
        // collisionContactCount = collision.contacts.Length;
        if (collision.gameObject.layer == groundMask)
        {
            //isGrounded = true;
        }
        foreach (ContactPoint contact in collision.contacts)
        {
            Debug.DrawRay(contact.point, contact.normal * 10, Color.white);
        }
        // print(collision.contacts.Length);
    }

	public void PlaySFXOnce(AudioClip clip, float volumeScale = 0.3f)
	{
        this.audioSource.PlayOneShot(clip, volumeScale);
	}

    public void PlaySFX(AudioClip clip, float volume = 1f, bool resetVolumeAfter = true)
    {
        StartCoroutine(PlayFX_IE(clip, volume, resetVolumeAfter));
    }

    IEnumerator PlayFX_IE(AudioClip clip, float volume = 1f, bool waitTilFinish = false)
    {
        float prev_vol = this.audioSource.volume;
        this.audioSource.volume = Mathf.Clamp01(volume);
        this.audioSource.clip = clip;
        this.audioSource.Play();
        while (waitTilFinish && this.audioSource.isPlaying)
            yield return null;
        this.audioSource.volume = prev_vol;
    }

	public void EnableAllColliders()
	{
		foreach (Collider __c in transform.GetComponents<Collider>())
		{
			__c.enabled = true;
		}
		gravity = Physics.gravity.y;
	}

	public void DisableAllColliders()
	{
		foreach (Collider __c in transform.GetComponents<Collider>())
		{
			__c.enabled = false;
		}
		gravity = 0f;
	}
    
}
