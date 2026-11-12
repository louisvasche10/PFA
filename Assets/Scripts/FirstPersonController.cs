using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    [SerializeField] Transform playerCamera;
    [SerializeField] float walkSpeed = 5f;
    [SerializeField] float runSpeed = 9f;
    [SerializeField] float jumpHeight = 1.5f;
    [SerializeField] float gravity = -20f;
    [SerializeField] float mouseSensitivity = 0.1f;
    CharacterController controller;
    Vector3 spawnPosition;
    float verticalSpeed;
    float cameraPitch;
    void Awake()
    {
        controller = GetComponent<CharacterController>();
        spawnPosition = transform.position;
    }
    void Start()
    {
        LockCursor();
    }
    void Update()
    {
        Keyboard keyboard = Keyboard.current;
        Mouse mouse = Mouse.current;
        if (keyboard == null || mouse == null || !Application.isFocused) return;
        if (keyboard.escapeKey.wasPressedThisFrame)
        {
            UnlockCursor();
            return;
        }
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            if (mouse.leftButton.wasPressedThisFrame) LockCursor();
            return;
        }
        Vector2 look = mouse.delta.ReadValue() * mouseSensitivity;
        transform.Rotate(Vector3.up * look.x);
        cameraPitch = Mathf.Clamp(cameraPitch - look.y, -85f, 85f);
        playerCamera.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
        Vector2 input = new Vector2(
            (keyboard.dKey.isPressed ? 1f : 0f) - (keyboard.aKey.isPressed ? 1f : 0f),
            (keyboard.wKey.isPressed ? 1f : 0f) - (keyboard.sKey.isPressed ? 1f : 0f));
        input = Vector2.ClampMagnitude(input, 1f);
        float speed = keyboard.leftShiftKey.isPressed || keyboard.rightShiftKey.isPressed ? runSpeed : walkSpeed;
        Vector3 movement = (transform.right * input.x + transform.forward * input.y) * speed;
        if (controller.isGrounded && verticalSpeed < 0f) verticalSpeed = -2f;
        if (controller.isGrounded && keyboard.spaceKey.wasPressedThisFrame)
            verticalSpeed = Mathf.Sqrt(jumpHeight * -2f * gravity);
        verticalSpeed += gravity * Time.deltaTime;
        movement.y = verticalSpeed;
        CollisionFlags collisions = controller.Move(movement * Time.deltaTime);
        if ((collisions & CollisionFlags.Above) != 0 && verticalSpeed > 0f) verticalSpeed = 0f;
        if (transform.position.y < -20f)
        {
            controller.enabled = false;
            transform.position = spawnPosition;
            controller.enabled = true;
            verticalSpeed = 0f;
        }
    }
    void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus) UnlockCursor();
    }
    void OnDisable()
    {
        UnlockCursor();
    }
}
