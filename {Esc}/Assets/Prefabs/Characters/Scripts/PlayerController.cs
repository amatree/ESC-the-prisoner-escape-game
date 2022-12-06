using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Player")]
    public Rigidbody rigidBody;
	public CapsuleCollider playerCollider;
	public GameObject playerModelPrefab;

    [ReadOnly] public float currentSpeed = 0f;
    [Range(1f, 10f)] public float movementSpeed = 5f;
    [Range(1f, 15f)] public float sprintSpeed = 10f;
    [Range(1f, 10f)] public float jumpHeight = 3f;
    [Tooltip("Disabled cuz it broke animation :3")] [ReadOnly] public bool enableDoubleJump = false;

	[Header("Slope Handling")]
	[ReadOnly] public bool isOnSlope;
	public float maxSlopeAngle = 35.0f;
	[ReadOnly] public float currSlopeAngle;
	[ReadOnly] public RaycastHit slopeHit;

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

    [Header("Audio")]
    public AudioSource audioSource;

    [Header("Other")]
    public Transform groundCheck;
    public LayerMask groundMask;
    public float groundDistance = 0.03f;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode jumpKey = KeyCode.Space;

	[Header("Debug")]
    float mouseX;
    float prev_mouseX;
    float mouseY;

	float dGroundDistance;

	[ReadOnly] public Vector3 moveVector;
    [ReadOnly] public float moveMagnitude;
    [ReadOnly] public float finalSpeed;
    [ReadOnly] public float verticalAxis;
    [ReadOnly] public float horizontalAxis;

    public bool isGrounded;
    public bool isSprinting;

    [ReadOnly] public int jumpCount;
    [ReadOnly] public bool isJumpKeyPressed;
    [ReadOnly] public bool isJumpKeyReleased;

    int collisionContactCount = 0;

    public bool hasCameraControl = true;
    public bool hasAllControl = true;

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
            finalSpeed = isSprinting ? sprintSpeed : movementSpeed;
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
			if (Input.GetKey(jumpKey) && isGrounded)
			{
				StartCoroutine(ToggleJump(jumpHeight + (moveVector.magnitude / 320f)));
			}



			// slope handling
			isOnSlope = OnSlope();
			if (isOnSlope)
			{
				// TODO: fix slope bounce
				groundDistance = slopeHit.distance;
				if (isGrounded) rigidBody.AddForce(GetSlopeMoveDirection() * finalSpeed * 2.5f, ForceMode.Acceleration);
				rigidBody.AddForce(new Vector3(0, - currSlopeAngle * finalSpeed / maxSlopeAngle, 0), ForceMode.Acceleration);
			} else if (isGrounded && rigidBody.velocity.magnitude != 0)
			{
                AddForce(moveVector * accelerationMultiplier - moveVector * frictionMultiplier, ForceMode.Acceleration);
			} else if (!isGrounded && rigidBody.velocity.magnitude != 0)
			{
                AddForce((moveVector * airAccelerationMultiplier - moveVector * airFrictionMultiplier) / -airDrag, ForceMode.Acceleration); 
			} else
				groundDistance = dGroundDistance;
            
            // animation
            // playerAnimation.SetBool("isRunning", isSprinting && move.magnitude != 0);
            // playerAnimation.SetBool("isWalking", move.magnitude != 0);
        }

        // gravity
		rigidBody.AddForce(new Vector3(0, gravity, 0), ForceMode.Acceleration);
    }

	bool OnSlope()
	{
		if (Physics.Raycast(playerModelPrefab.transform.position, Vector3.down, out slopeHit, playerCollider.height * 0.5f + 0.3f))
		{
			currSlopeAngle = Vector3.Angle(transform.forward , slopeHit.normal);
			return currSlopeAngle < maxSlopeAngle && currSlopeAngle != 0;
		}

		return false;
	}

	Vector3 GetSlopeMoveDirection() {
		return Vector3.ProjectOnPlane(moveVector, slopeHit.normal).normalized;
	}

	IEnumerator ToggleJump(float jumpForceMultiplier = 3.0f, float delay = 0.1f)
	{
		if (!isGrounded)
			yield return new WaitForSeconds(delay);
		if (isOnSlope)
			jumpForceMultiplier = 3.0f;
		AddForce(Vector3.up * Mathf.Sqrt(-2f * jumpHeight / gravity) * jumpForceMultiplier, ForceMode.Impulse);
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
        rigidBody.velocity = new Vector3(0, 0, 0);
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
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

    public void PlayFX(AudioClip clip, float volume = 1f, bool resetVolumeAfter = true)
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
    
}
