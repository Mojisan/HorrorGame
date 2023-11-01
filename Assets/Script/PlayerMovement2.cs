using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement2 : MonoBehaviour
{
    public float maxStamina = 100f;
    public float currentStamina;
    public float sprintStaminaCost = 10f;
    public float staminaRegenRate = 2f;

    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float jumpForce = 8f;
    [SerializeField] float sprintSpeed = 20f;

    private Rigidbody rb;
    private float verticalInput;
    private float horizontalInput;
    private float sprintInput;
    private bool isJumping = false;
    private bool isGrounded = true;
    private bool isSprinting = false;

    private float collisionDistance = 0.3f;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentStamina = maxStamina;
    }

    private void FixedUpdate()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        isSprinting = Input.GetButton("Sprint");
        isJumping = Input.GetButton("Jump");

        MovePlayer();
        Jump();
    }

    void Update()
    {
    }

    void MovePlayer()
    {
        float currentSpeed = moveSpeed;


        if (isSprinting && isGrounded)
        {
            ReduceStamina(sprintStaminaCost * Time.deltaTime);
            currentSpeed = sprintSpeed;
        }
        else 
        {
            RegenerateStamina(staminaRegenRate * Time.deltaTime);
        }


        Vector3 moveDirection = new Vector3(horizontalInput, 0, verticalInput).normalized;
        Vector3 movement = moveDirection * currentSpeed * Time.deltaTime;

        RaycastHit hit;
        if (!Physics.Raycast(transform.position, movement, out hit, collisionDistance))
        {
            rb.MovePosition(transform.position + movement);
        }
    }

    void Jump()
{
    if (isGrounded && isJumping)
    {
        rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
        isGrounded = false;
    }
}

    public void ReduceStamina(float amount)
    {
        currentStamina = Mathf.Clamp(currentStamina - amount, 0, maxStamina);

        if (currentStamina <= sprintStaminaCost)
        {
            isSprinting = false;
        }
    }

    public void RegenerateStamina(float amount)
    {
        currentStamina = Mathf.Clamp(currentStamina + amount, 0, maxStamina);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}

