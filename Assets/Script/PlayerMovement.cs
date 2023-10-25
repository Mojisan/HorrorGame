using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class PlayerMovement : MonoBehaviour
{
    public float maxStamina = 100f;
    public float currentStamina;
    public float staminaRegenRate = 2f;
    public float sprintStaminaCost = 10f;

    PlayerInput playerInput;
    InputAction moveAction;
    InputAction sprintAction;
    InputAction jumpAction;
    Rigidbody rb;

    [SerializeField] float speed = 5;
    [SerializeField] float sprintSpeed = 8;
    [SerializeField] float jumpForce = 8;

    bool isSprinting = false;
    bool isJumping = false;
    bool isOnGround = false;

    // Start is called before the first frame update
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();
        moveAction = playerInput.actions.FindAction("Move");
        sprintAction = playerInput.actions.FindAction("Sprint");
        jumpAction = playerInput.actions.FindAction("Jump");
        sprintAction.performed += SprintAction_performed;
        sprintAction.canceled += SprintAction_canceled;
        jumpAction.performed += JumpAction_performed;

        currentStamina = maxStamina;
    }

    // Update is called once per frame
    void Update()
    {
        float currentSpeed = speed;

        if (isSprinting)
        {
            ReduceStamina(sprintStaminaCost * Time.deltaTime);
            currentSpeed = isSprinting ? sprintSpeed : speed;
        }
        else
        {
            RegenerateStamina(staminaRegenRate * Time.deltaTime);
        }

        MovePlayer(currentSpeed);
    }

    private void OnDestroy()
    {
        sprintAction.performed -= SprintAction_performed;
        sprintAction.canceled -= SprintAction_canceled;
        jumpAction.performed -= JumpAction_performed;
    }

    private void SprintAction_performed(InputAction.CallbackContext obj)
    {
        isSprinting = true;
    }

    private void SprintAction_canceled(InputAction.CallbackContext obj)
    {
        isSprinting = false;
    }

    private void JumpAction_performed(InputAction.CallbackContext obj)
    {
        if (!isJumping && isOnGround)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            /*isJumping = true;*/
        }
    }

    void MovePlayer(float currentSpeed)
    {
        Vector2 direction = moveAction.ReadValue<Vector2>();
        transform.position += new Vector3(direction.x, 0, direction.y) * currentSpeed * Time.deltaTime;
    }

    public void ReduceStamina(float amount)
    {
        currentStamina = Mathf.Clamp(currentStamina - amount, 0, maxStamina);

        if (currentStamina == 0)
        {
            isSprinting = false;
        }
    }

    public void RegenerateStamina(float amount)
    {
        currentStamina = Mathf.Clamp(currentStamina + amount, 0, maxStamina);
    }

    void OnCollisionEnter(Collision hit)
    {
        if (hit.gameObject.CompareTag("Ground"))
        {
            isOnGround = true;
            Debug.Log("OnGround");
        }
        else
        {
            isOnGround = false;
        }
    }
    void OnCollisionExit(Collision hit)
    {
        isOnGround = false;
    }
}
