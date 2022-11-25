using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Player")]
    public Rigidbody rigidBody;
    [Range(1f, 10f)] public float movementSpeed = 5f;
    [Range(1f, 15f)] public float sprintSpeed = 10f;
    [Range(1f, 10f)] public float jumpHeight = 3f;
    public bool enableDoubleJump = true;

    [Header("Animation")]
    public Animator playerAnimation;

    [Header("Camera")]
    public Camera playerCamera;
    [Range(0f, 50f)] public float mouseSensitivity = 3f;

    [Header("Physics")]
    public float gravity = Physics.gravity.y;
    [Range(0f, 2f)] public float accelerationMultiplier = 2.0f;
    [Range(0f, 1f)] public float frictionMultiplier = 0.7f;
    [Range(0f, 1f)] public float airAccelerationMultiplier = 0.7f;
    [Range(0f, 2f)] public float airFrictionMultiplier = 2.0f;
    [Range(0.01f, 1f)] public float airDrag = 0.5f;

    [Header("Other")]
    public Transform groundCheck;
    public LayerMask groundMask;
    public float groundDistace = 0.03f;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode jumpKey = KeyCode.Space;

    float mouseX;
    float mouseY;

    float finalSpeed;
    float verticalAxis;
    float horizontalAxis;

    public bool isGrounded;
    public bool isSprinting;

    int jumpCount;
    bool isJumpKeyReleased;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = rigidBody == null ? GetComponentInChildren<Rigidbody>() : rigidBody;
        rigidBody.useGravity = false;
        rigidBody.drag = 0f;
        playerCamera = playerCamera == null ? GetComponentInChildren<Camera>() : playerCamera;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        // state checks
        isSprinting = Input.GetKey(sprintKey);
        finalSpeed = isSprinting ? sprintSpeed : movementSpeed;

        RaycastHit ray = RayCast(Vector3.down);
        Debug.DrawRay(transform.position, new Vector3(0, -10f, 0), Color.cyan);
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistace, groundMask);

        // rotation
        mouseX += Input.GetAxis("Mouse X") * mouseSensitivity * 100f * Time.fixedDeltaTime;
        mouseY += Input.GetAxis("Mouse Y") * mouseSensitivity * 100f * Time.fixedDeltaTime;
        mouseY = Mathf.Clamp(mouseY, -90f, 90f);

        transform.localRotation = Quaternion.Euler(-mouseY, 0f, 0f);
        playerCamera.transform.localRotation = Quaternion.Euler(-mouseY, 0f, 0f);
        transform.rotation = Quaternion.Euler(0f, mouseX, 0f);

        // position
        verticalAxis = Input.GetAxis("Vertical") * finalSpeed * 1000f * Time.fixedDeltaTime;
        horizontalAxis = Input.GetAxis("Horizontal") * finalSpeed * 1000f * Time.fixedDeltaTime;
        Vector3 move = transform.right * horizontalAxis + transform.forward * verticalAxis;
        
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
        
        if (isGrounded && rigidBody.velocity.magnitude != 0)
            AddForce(move * accelerationMultiplier - move * frictionMultiplier, ForceMode.Acceleration);
        else if (!isGrounded && rigidBody.velocity.magnitude != 0)
            AddForce((move * airAccelerationMultiplier - move * airFrictionMultiplier) / -airDrag, ForceMode.Acceleration);
        
        // animation
        playerAnimation.SetBool("isRunning", isSprinting && move.magnitude != 0);
        playerAnimation.SetBool("isWalking", move.magnitude != 0);

        // gravity
        AddForce(new Vector3(0, gravity, 0), ForceMode.Acceleration);
    }

    void AddForce(Vector3 force, ForceMode mode, bool resetVelocity = true)
    {
        if (resetVelocity)
            rigidBody.velocity = new Vector3(0, rigidBody.velocity.y, 0);
        rigidBody.AddForce(force, mode);
    }

    public RaycastHit RayCast(Vector3 direction)
    {
        Physics.Raycast(groundCheck.position, direction, out RaycastHit hit, 100f);
        return hit;
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer == groundMask)
        {
            //isGrounded = true;
        }
    }
}
