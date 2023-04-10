using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonMovement : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float turnSmoothTime = 0.1f;
    [SerializeField] private Transform cam;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float gravityScale = 2f;

    private Vector2 moveDirection;
    private CharacterController controller;
    private float turnSmoothVelocity;
    private float verticalVelocity = 0f;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    private void OnEnable()
    {
        // Enable the PlayerInput component
        GetComponent<PlayerInput>().enabled = true;
    }

    private void OnDisable()
    {
        // Disable the PlayerInput component
        GetComponent<PlayerInput>().enabled = false;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveDirection = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && controller.isGrounded)
        {
            verticalVelocity = jumpForce;
        }
    }

    private void Update()
    {
        // Move the player using the move input
        Vector3 movement = new Vector3(moveDirection.x, 0f, moveDirection.y).normalized;

        if (movement.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(movement.x, movement.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }

        // Apply gravity to the character controller
        verticalVelocity += Physics.gravity.y * gravityScale * Time.deltaTime;
        Vector3 gravityVector = new Vector3(0f, verticalVelocity, 0f);
        controller.Move(gravityVector * Time.deltaTime);
    }
}
