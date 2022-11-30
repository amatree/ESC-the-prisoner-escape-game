using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Player")]
    public Rigidbody rigidBody;

    [ReadOnly] public float currentSpeed = 0f;
    [Range(1f, 10f)] public float movementSpeed = 5f;
    [Range(1f, 15f)] public float sprintSpeed = 10f;
    [Range(1f, 10f)] public float jumpHeight = 3f;
    public bool enableDoubleJump = true;

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
    public float groundDistace = 0.03f;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode jumpKey = KeyCode.Space;
    public GameConfiguration gameConfiguration;

    float mouseX;
    float prev_mouseX;
    float mouseY;

    float finalSpeed;
    float verticalAxis;
    float horizontalAxis;

    public bool isGrounded;
    public bool isSprinting;

    int jumpCount;
    bool isJumpKeyReleased;

    int collisionContactCount = 0;

    bool hasCameraControl = true;
    bool hasAllControl = true;

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
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistace, groundMask);

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
            Vector3 move = Vector3.zero;
            if (rigidBody.SweepTest((Mathf.Sign(horizontalAxis) * Vector3.right), out RaycastHit hit1, 0.3f) && !isGrounded)
            {
                verticalAxis = 0f;
                horizontalAxis = 0f;
                rigidBody.velocity = new Vector3(0, rigidBody.velocity.y, 0);
            } else
            {
                verticalAxis = Input.GetAxis("Vertical") * finalSpeed * 1000f * Time.fixedDeltaTime;
                horizontalAxis = Input.GetAxis("Horizontal") * finalSpeed * 1000f * Time.fixedDeltaTime;
                move = transform.right * horizontalAxis + transform.forward * verticalAxis;
            }

            // double jump
            if (jumpCount > 0 && jumpCount < 2 && enableDoubleJump && !isGrounded)
            {
                if (isJumpKeyReleased && Input.GetKey(jumpKey))
                {
                    AddForce(Vector3.up * Mathf.Sqrt(-2f * jumpHeight / gravity) * 6f, ForceMode.VelocityChange);
                    jumpCount++;
                }
            }
            if (isGrounded && jumpCount > 0)
                jumpCount = 0;
            isJumpKeyReleased = !Input.GetKey(jumpKey);
            if (Input.GetKey(jumpKey) && isGrounded && jumpCount == 0)
            {
                AddForce(Vector3.up * Mathf.Sqrt(-2f * jumpHeight / gravity) * 6f, ForceMode.VelocityChange);
                jumpCount++;
            }

            // add moving force
            if (isGrounded && rigidBody.velocity.magnitude != 0)
                AddForce(move * accelerationMultiplier - move * frictionMultiplier, ForceMode.Acceleration);
            else if (!isGrounded && rigidBody.velocity.magnitude != 0)
                AddForce((move * airAccelerationMultiplier - move * airFrictionMultiplier) / -airDrag, ForceMode.Acceleration);
            
            // animation
            playerAnimation.SetBool("isRunning", isSprinting && move.magnitude != 0);
            playerAnimation.SetBool("isWalking", move.magnitude != 0);
        }

        // gravity
        rigidBody.AddForce(new Vector3(0, gravity, 0), ForceMode.Acceleration);
    }

    void AddForce(Vector3 force, ForceMode mode, bool resetVelocity = true)
    {
        if (resetVelocity)
        {
            currentSpeed = rigidBody.velocity.magnitude;
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
