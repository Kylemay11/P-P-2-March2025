using UnityEngine;

public class ADS : MonoBehaviour
{
    [Header("Main Settings")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Transform weapon;
    [SerializeField] private KeyCode adsKey = KeyCode.Mouse1;

    [Header("ADS Settings")]
    [SerializeField] private float adsFOV;
    [SerializeField] private float normalFOV;
    [SerializeField] private Vector3 adsPosition;
    [SerializeField] private float transitionSpeed;

    private Vector3 originalWeaponPosition;
    private bool isAiming;

    private void Start()
    {
        // Store initial weapon position
        originalWeaponPosition = weapon.localPosition;

        // Set initial FOV
        playerCamera.fieldOfView = normalFOV;
    }

    private void Update()
    {
        // Toggle ADS when key is pressed/released
        if (Input.GetKeyDown(adsKey)) isAiming = true;
        if (Input.GetKeyUp(adsKey)) isAiming = false;
        adsPosition = playerController.instance.pcADSPosition;

        // Apply ADS effects
        if (isAiming)
        {
            AimDownSights();
        }
        else
        {
            ReturnToHip();
        }
    }

    private void AimDownSights()
    {
        // Smoothly transition camera FOV
        playerCamera.fieldOfView = Mathf.Lerp(
            playerCamera.fieldOfView,
            adsFOV,
            transitionSpeed * Time.deltaTime
        );

        // Move weapon to ADS position
        weapon.localPosition = Vector3.Lerp(
            weapon.localPosition,
            adsPosition,
            transitionSpeed * Time.deltaTime
        );
    }

    private void ReturnToHip()
    {
        // Return camera FOV to normal
        playerCamera.fieldOfView = Mathf.Lerp(
            playerCamera.fieldOfView,
            normalFOV,
            transitionSpeed * Time.deltaTime
        );

        // Return weapon to original position
        weapon.localPosition = Vector3.Lerp(
            weapon.localPosition,
            originalWeaponPosition,
            transitionSpeed * Time.deltaTime
        );
    }

    // Public access to check if aiming
    public bool IsAiming => isAiming;
}