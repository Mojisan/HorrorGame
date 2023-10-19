using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float maxStamina = 100f;
    public float currentStamina;
    public float staminaRegenRate = 2f;
    public float sprintStaminaCost = 10f;

    PlayerInput playerInput;
    InputAction moveAction;
    InputAction sprintAction;

    [SerializeField] float speed = 5;
    [SerializeField] float sprintSpeed = 8;

    bool isSprinting = false;
    bool wasSprinting = false;

    // Start is called before the first frame update
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions.FindAction("Move");
        sprintAction = playerInput.actions.FindAction("Sprint");
        sprintAction.performed += SprintAction_performed;
        sprintAction.canceled += SprintAction_canceled;

        currentStamina = maxStamina;
    }

    // Update is called once per frame
    void Update()
    {
        float currentSpeed = speed;

        if (isSprinting && !wasSprinting)
        {
            ReduceStamina(sprintStaminaCost);
        }

        wasSprinting = isSprinting;

        if (isSprinting)
        {
            currentSpeed = isSprinting ? sprintSpeed : speed;
        }
        else
        {
            RegenerateStamina(staminaRegenRate * Time.deltaTime);
        }

        MovePlayer(currentSpeed);
        print(currentSpeed);
    }

    private void OnDestroy()
    {
        sprintAction.performed -= SprintAction_performed;
        sprintAction.canceled -= SprintAction_canceled;
    }

    private void SprintAction_performed(InputAction.CallbackContext obj)
    {
        isSprinting = true;
    }

    private void SprintAction_canceled(InputAction.CallbackContext obj)
    {
        isSprinting = false;
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
}
