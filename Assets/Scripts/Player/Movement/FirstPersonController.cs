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
    int jumpsUsed;
    Vector3 wallVelocity;
    [Header("Sauts et murs")]
    [SerializeField] float wallJumpForce = 7f;
    [SerializeField] float wallDetectionDistance = 0.65f;
    float nextWallJump;
    float cameraPitch;
    int lockFrame;
    public bool IsControlling => enabled && Cursor.lockState == CursorLockMode.Locked && Time.frameCount > lockFrame;
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
        cameraPitch = FirstPersonLook.Rotate(transform, playerCamera, mouse.delta.ReadValue(), mouseSensitivity, cameraPitch);
        Vector2 input = new Vector2(
            (keyboard.dKey.isPressed ? 1f : 0f) - (keyboard.aKey.isPressed ? 1f : 0f),
            (keyboard.wKey.isPressed ? 1f : 0f) - (keyboard.sKey.isPressed ? 1f : 0f));
        input = Vector2.ClampMagnitude(input, 1f);
        float speed = keyboard.leftShiftKey.isPressed || keyboard.rightShiftKey.isPressed ? runSpeed : walkSpeed;
        Vector3 movement = (transform.right * input.x + transform.forward * input.y) * speed;
        if (controller.isGrounded && verticalSpeed < 0f) { verticalSpeed = -2f; jumpsUsed = 0; wallVelocity = Vector3.zero; }
        if (keyboard.spaceKey.wasPressedThisFrame)
        {
            Vector3 normal;
            if (!controller.isGrounded && Time.time >= nextWallJump && WallJumpDetector.FindWall(transform.position + Vector3.up, wallDetectionDistance, out normal))
            {
                verticalSpeed = Mathf.Sqrt(jumpHeight * -2f * gravity);
                wallVelocity = normal * wallJumpForce;
                jumpsUsed = 1;
                nextWallJump = Time.time + 0.25f;
            }
            else if (jumpsUsed < 2)
            {
                verticalSpeed = Mathf.Sqrt(jumpHeight * -2f * gravity);
                jumpsUsed++;
            }
        }
        // L’impulsion horizontale éloigne le joueur du mur puis décroît.
        movement += wallVelocity;
        wallVelocity = Vector3.MoveTowards(wallVelocity, Vector3.zero, 12f * Time.deltaTime);
        verticalSpeed += gravity * Time.deltaTime;
        movement.y = verticalSpeed;
        CollisionFlags collisions = controller.Move(movement * Time.deltaTime);
        if ((collisions & CollisionFlags.Above) != 0 && verticalSpeed > 0f) verticalSpeed = 0f;
        if (transform.position.y < -20f)
        {
            PFACombat.PlayerHealth health = GetComponent<PFACombat.PlayerHealth>();
            if (health != null) health.TakeDamage(1000);
            else ResetPlayer();
        }
    }
    public void ResetPlayer()
    {
        controller.enabled = false;
        transform.position = spawnPosition;
        transform.rotation = Quaternion.identity;
        playerCamera.localRotation = Quaternion.identity;
        controller.enabled = true;
        verticalSpeed = 0f;
        cameraPitch = 0f;
        jumpsUsed = 0;
        wallVelocity = Vector3.zero;
        LockCursor();
    }
    void LockCursor()
    {
        lockFrame = Time.frameCount;
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
