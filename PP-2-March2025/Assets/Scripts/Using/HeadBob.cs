using UnityEngine;

public class HeadBob : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform playerCamera;
    [SerializeField] private CharacterController characterController;

    [Header("Settings")]
    [SerializeField] private float walkBobSpeed;
    [SerializeField] private float walkBobAmount;
    [SerializeField] private float runBobSpeed;
    [SerializeField] private float runBobAmount;
    [SerializeField] private float transitionSpeed;
    [SerializeField][Range(0, 1)] private float horizontalBobFactor;
    [SerializeField] private float groundedTolerance;

    private Vector3 _originalLocalPos;
    private float _timer;
    private float _lastGroundedTime;
    private bool _wasMoving;

    private void Awake()
    {
        if (!playerCamera) playerCamera = transform;
        if (!characterController) characterController = GetComponentInParent<CharacterController>();

        _originalLocalPos = playerCamera.localPosition;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        HandleHeadBob();
        DebugInputValues(); 
    }

    private void HandleHeadBob()
    {
        bool isGrounded = Time.time - _lastGroundedTime < groundedTolerance;
        Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        bool isMoving = input.magnitude > 0.1f;
        bool isRunning = Input.GetKey(KeyCode.LeftShift);

        // Update grounded timer
        if (characterController.isGrounded) _lastGroundedTime = Time.time;

        if (isGrounded && isMoving)
        {
            float bobSpeed = isRunning ? runBobSpeed : walkBobSpeed;
            float bobAmount = isRunning ? runBobAmount : walkBobAmount;

            // Frame-rate independent timer using unscaled time
            _timer += Time.unscaledDeltaTime * bobSpeed;
            _timer %= Mathf.PI * 2;

            Vector3 bobOffset = new Vector3(
                Mathf.Sin(_timer * 0.5f) * bobAmount * horizontalBobFactor,
                Mathf.Sin(_timer) * bobAmount,
                0
            );

            playerCamera.localPosition = Vector3.Lerp(
                playerCamera.localPosition,
                _originalLocalPos + bobOffset,
                Time.unscaledDeltaTime * transitionSpeed
            );

            _wasMoving = true;
        }
        else if (_wasMoving)
        {
            ResetCameraPosition();
            _wasMoving = false;
        }
    }

    private void ResetCameraPosition()
    {
        _timer = 0;
        playerCamera.localPosition = Vector3.Lerp(
            playerCamera.localPosition,
            _originalLocalPos,
            Time.unscaledDeltaTime * transitionSpeed * 2f
        );
    }

    // Add this method to debug key values
    private void DebugInputValues()
    {
        Debug.Log($"Grounded: {characterController.isGrounded} | " +
                  $"Moving: {new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).magnitude > 0.1f} | " +
                  $"Timer: {_timer:F2} | " +
                  $"CamPos: {playerCamera.localPosition}");
    }
}