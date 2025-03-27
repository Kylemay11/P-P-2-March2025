using UnityEngine;
using System;
using TMPro;
using System.Collections;
using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public enum PlayerState
{
    Idle,
    Walking,
    Sprinting,
    Crouching,
    Sliding
}

public class playerController : MonoBehaviour, IDamage, IPickupable
{
    public static playerController instance;

    [Header("References")]
    [SerializeField] private CharacterController controller;
    [SerializeField] private LayerMask ignoreMask;
    [SerializeField] private TMP_Text speedMeter;
    [SerializeField] private Transform playerCamera;

    private PlayerState currentState;
    public PlayerState CurrentState => currentState;

    [Header("Player Stats")]
    [SerializeField] public int maxHP;
    [SerializeField] public int currentHP;

    [Header("Movement Settings")]
    [SerializeField] public float walkSpeed;
    [SerializeField] private float sprintMultiplier;

    [Header("Stamina Settings")]
    [SerializeField] private float maxStamina;
    [SerializeField] private float currentStamina;
    [SerializeField] private float staminaDrainRate;
    [SerializeField] private float staminaRegainRate;
    [SerializeField] private float staminaRegainDelay;
    [SerializeField] private float slideStaminaDrainMultiplier;

    [Header("Slide Settings")]
    [SerializeField] private float slideDuration;
    [SerializeField] private float slideSpeedBoost;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float gravity;
    [SerializeField] private int maxJumps;

    [Header("Crouch Settings")]
    [SerializeField] private float crouchSpeed;
    [SerializeField] private Vector3 crouchColliderSize;
    [SerializeField] private Vector3 normalColliderSize;
    [SerializeField] private float crouchCameraHeight;
    [SerializeField] private float normalCameraHeight;

    // Jacob added
    [Header("Weapon Settings")]
    [SerializeField] List<weaponStats> wepList = new List<weaponStats>();
    [SerializeField] GameObject wepModel;
    [SerializeField] private LayerMask hitMask;
    [SerializeField] private int wepDamage;
    [SerializeField] private int wepDist;
    [SerializeField] private float wepRate;
    [SerializeField] private float reloadTime;
    [SerializeField] private GameObject mFlashPos;
    [SerializeField] private Coroutine reloadTest;
    [SerializeField] private ParticleSystem mFlash;

    [Header("--- Audio ---")]
    [SerializeField] AudioClip[] audSteps;
    [Range(0, 1)][SerializeField] float audStepsVol;
    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip[] audJump;
    [Range(0, 1)][SerializeField] float audJumpVol;
    [SerializeField] AudioClip[] audHurt;
    [Range(0, 1)][SerializeField] float audHurtVol;

    private Vector3 moveDir;
    private Vector3 velocity;
    private int wepListPos;
    private float attackTimer;
    private bool isReloading;
    private int jumpCount;
    private float currentSpeed;
    private float staminaRegenTimer;
    private bool staminaFullyDrained;
    private bool canSprint;
    [SerializeField] private bool isGrounded;
    bool isplayingSteps;

    void Start()
    {
        currentHP = maxHP;
        currentStamina = maxStamina;
        currentSpeed = walkSpeed;
        currentState = PlayerState.Idle;

        if (playerCamera != null)
            UpdateCameraHeight(normalCameraHeight);

        if (speedMeter == null)
            speedMeter = GameObject.Find("SpeedMeter").GetComponent<TMP_Text>();

        controller.height = normalColliderSize.y;
        controller.center = new Vector3(0, normalColliderSize.y / 16f, 0);
        updatePlayerUI();
    }

    void Update()
    {
        HandleMovement();
        HandleSprint();
        HandleCrouch();
        HandleStamina();
        UpdateSpeedUI();
    }

    private void HandleMovement()
    {
        attackTimer += Time.deltaTime;

        if (controller.isGrounded)
        {
            isGrounded = true;
            velocity.y = 0;
            jumpCount = 0;

            if (currentState != PlayerState.Sprinting && currentState != PlayerState.Crouching && currentState != PlayerState.Sliding)
            {
                currentState = PlayerState.Walking;
                currentSpeed = walkSpeed;
            }

            if (currentState == PlayerState.Crouching)
                currentSpeed = crouchSpeed;
        }

        moveDir = (Input.GetAxis("Horizontal") * transform.right) + (Input.GetAxis("Vertical") * transform.forward);
        controller.Move(moveDir * currentSpeed * Time.deltaTime);

        HandleJump();

        velocity.y -= gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        if (Input.GetButton("Fire1") && wepList.Count > 0 && wepList[wepListPos].ammoCur > 0 && attackTimer >= wepRate)
            Shoot();

        selectWeapon();
        reloadWeapon();
    }

    private void HandleSprint()
    {
        if (Input.GetButtonDown("Sprint") && isGrounded && canSprint)
        {
            staminaRegenTimer = 0f;
            currentState = PlayerState.Sprinting;
            currentSpeed = walkSpeed * sprintMultiplier;
        }
        else if (Input.GetButtonUp("Sprint") && currentState == PlayerState.Sprinting)
        {
            currentState = PlayerState.Walking;
            currentSpeed = walkSpeed;
        }
    }

    private void HandleCrouch()
    {
        if (Input.GetButtonDown("Crouch"))
        {
            if (currentState == PlayerState.Sprinting && isGrounded)
            {
                EnterSlide();
                return;
            }
            if (currentState != PlayerState.Crouching)
                EnterCrouch();
        }

        if (Input.GetButton("Crouch") && currentState != PlayerState.Crouching && currentState != PlayerState.Sliding)
        {
            EnterCrouch();
        }
        else if (!Input.GetButton("Crouch") && currentState == PlayerState.Crouching)
        {
            ExitCrouch();
        }
    }

    private void EnterCrouch()
    {
        currentState = PlayerState.Crouching;
        currentSpeed = crouchSpeed;
        controller.height = crouchColliderSize.y;
        controller.center = new Vector3(0, crouchColliderSize.y / 2f, 0);
        UpdateCameraHeight(crouchCameraHeight);
    }

    private void ExitCrouch()
    {
        currentState = PlayerState.Walking;
        currentSpeed = walkSpeed;
        controller.height = normalColliderSize.y;
        controller.center = new Vector3(0, normalColliderSize.y / 16f, 0);
        UpdateCameraHeight(normalCameraHeight);
    }

    private void EnterSlide()
    {
        currentState = PlayerState.Sliding;
        currentSpeed = walkSpeed * sprintMultiplier * slideSpeedBoost;
        controller.height = crouchColliderSize.y;
        controller.center = new Vector3(0, crouchColliderSize.y / 2f, 0);
        UpdateCameraHeight(crouchCameraHeight);
        StartCoroutine(SlideTimer());
    }

    private IEnumerator SlideTimer()
    {
        yield return new WaitForSeconds(slideDuration);
        ExitSlide();
    }

    private void ExitSlide()
    {
        if (Input.GetButton("Crouch"))
        {
            EnterCrouch();
        }
        else if (Input.GetButton("Sprint"))
        {
            currentState = PlayerState.Sprinting;
            currentSpeed = walkSpeed * sprintMultiplier;
        }
        else
        {
            currentState = PlayerState.Walking;
            currentSpeed = walkSpeed;
        }

        controller.height = normalColliderSize.y;
        controller.center = new Vector3(0, normalColliderSize.y / 16f, 0);
        UpdateCameraHeight(normalCameraHeight);
    }

    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < maxJumps)
        {
            isGrounded = false;
            velocity.y = jumpForce;
            jumpCount++;
        }
    }

    private void HandleStamina()
    {
        if (currentState == PlayerState.Sprinting)
        {
            currentStamina -= staminaDrainRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);

            if (currentStamina <= 0)
            {
                staminaFullyDrained = true;
                canSprint = false;
                currentState = PlayerState.Walking;
                currentSpeed = walkSpeed;
            }
            staminaRegenTimer = 0f;
        }

        if (currentState == PlayerState.Sliding)
        {
            currentStamina -= staminaDrainRate * slideStaminaDrainMultiplier * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
            staminaRegenTimer = 0f;
        }

        if (currentState != PlayerState.Sprinting && currentState != PlayerState.Sliding)
        {
            if (staminaFullyDrained)
            {
                if (staminaRegenTimer < staminaRegainDelay)
                {
                    staminaRegenTimer += Time.deltaTime;
                }
                else
                {
                    RegainStamina();
                }
            }
            else
            {
                RegainStamina();
            }
        }
    }

    private void RegainStamina()
    {
        if (staminaFullyDrained && currentStamina > 0)
            staminaFullyDrained = false;

        currentStamina += staminaRegainRate * Time.deltaTime;
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);

        if (currentStamina > 0)
            canSprint = true;
    }

    private void UpdateCameraHeight(float height)
    {
        if (playerCamera != null)
            playerCamera.localPosition = new Vector3(playerCamera.localPosition.x, height, playerCamera.localPosition.z);
    }

    private void UpdateSpeedUI()
    {
        if (speedMeter != null)
            speedMeter.text = $"Speed: {currentSpeed:F2}";
        gameManager.instance.playerStaminaBar.fillAmount = currentStamina / maxStamina;
    }

    public void takeDamage(int amount)
    {
        currentHP -= amount;
        updatePlayerUI();
        StartCoroutine(DamageFlash());

        if (currentHP <= 0)
            gameManager.instance.YouLose();
    }

    private IEnumerator DamageFlash()
    {
        gameManager.instance.playerDamageScreen.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gameManager.instance.playerDamageScreen.SetActive(false);
    }

    public void updatePlayerUI()
    {
        gameManager.instance.playerHPBar.fillAmount = (float)currentHP / maxHP;
    }

    public void Heal(int amount)
    {
        currentHP += amount;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
        updatePlayerUI();
    }

    // Jacob Added
    public void getWeaponStats(weaponStats wep)
    {
        // implement interaction with shop logic

        wepList.Add(wep);
        wepListPos = wepList.Count - 1;
        changeWeapon();


    }

    void selectWeapon()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && wepList.Count >= 1)
        {
            wepListPos = 0;
            changeWeapon();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && wepList.Count >= 2)
        {
            wepListPos = 1;
            changeWeapon();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) && wepList.Count >= 3)
        {
            wepListPos = 2;
            changeWeapon();
        }
        if (Input.GetKeyDown(KeyCode.Alpha4) && wepList.Count >= 4)
        {
            wepListPos = 3;
            changeWeapon();
        }

        //if (Input.GetAxis("Mouse ScrollWheel") > 0 && wepListPos < wepList.Count - 1)
        //{
        //    wepListPos++;
        //    changeWeapon();
        //}
        //else if (Input.GetAxis("Mouse ScrollWheel") < 0 && wepListPos > 0)
        //{
        //    wepListPos--;
        //    changeWeapon();

        //}

    }

    void changeWeapon()
    {
        if (isReloading) // true
        {
            //StopCoroutine(Reload());

            isReloading = false;
        }
        wepDamage = wepList[wepListPos].wepDamage;
        wepDist = wepList[wepListPos].wepDist;
        wepRate = wepList[wepListPos].wepRate;
        reloadTime = wepList[wepListPos].reloadTime;


        wepModel.GetComponent<MeshFilter>().sharedMesh = wepList[wepListPos].model.GetComponent<MeshFilter>().sharedMesh;
        wepModel.GetComponent<MeshRenderer>().sharedMaterial = wepList[wepListPos].model.GetComponent<MeshRenderer>().sharedMaterial;

        if (wepList[wepListPos] != null)
        {
            AmmoUI.instance?.UpdateAmmo(wepList[wepListPos].ammoCur, wepList[wepListPos].ammoMax);
            AmmoUI.instance?.Show(true);
            gameManager.instance.weaponNotification?.ShowWeaponName(wepList[wepListPos].name);
        }
        else
        {
            AmmoUI.instance?.Show(false);
        }
    }

    void reloadWeapon()
    {
        if (Input.GetButtonDown("Reload")) // add Timer for reload animation
        {

            reloadTest = StartCoroutine(Reload());
        }
    }

    private void Shoot()
    {
        attackTimer = 0;

        if (mFlashPos != null)
        {
            StartCoroutine(FlashMuzzle());
        }

        wepList[wepListPos].ammoCur--;
        //attackTimer = Time.deltaTime + wepRate;
        AmmoUI.instance.UpdateAmmo(wepList[wepListPos].ammoCur, wepList[wepListPos].ammoMax);

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Debug.DrawRay(ray.origin, ray.direction * wepDist, Color.red, 1.5f);

        if (Physics.Raycast(ray, out RaycastHit hit, wepDist, hitMask))
        {
            Debug.Log("Hit: " + hit.collider.name);
            Instantiate(wepList[wepListPos].hitEffect, hit.point, Quaternion.identity);

            IDamage target = hit.collider.GetComponentInParent<IDamage>();
            if (target != null)
            {
                target.takeDamage((int)wepDamage);
            }
        }
        else
        {
            Debug.Log("No Hit");
        }
    }

    private IEnumerator FlashMuzzle()
    {
        ParticleSystem psMFlash = Instantiate(mFlash, mFlashPos.transform.position, Quaternion.identity);

        psMFlash.transform.SetParent(mFlashPos.transform); // keeps particles inplace while moving

        if (mFlash != null)
            psMFlash.Play();

        yield return new WaitForSeconds(0.12f);

        if (mFlash != null)
            psMFlash.Stop();

    }

    private IEnumerator Reload()
    {
        // temp current wep
        int currentWepIndex = wepListPos;

        isReloading = true;

        if (wepList[wepListPos].ammoCur > 1)
        {
            wepList[wepListPos].ammoCur = 1; // 1 in chamber
            AmmoUI.instance.UpdateAmmo(wepList[wepListPos].ammoCur, wepList[wepListPos].ammoMax);
        }
        Debug.Log("Reloading...");

        AmmoUI.instance?.StartReload(reloadTime);

        //yield return new WaitForSeconds(reloadTime - 0.1f);
        float timer = 0f;
        while (timer < reloadTime)
        {
            if (wepList[wepListPos] != wepList[currentWepIndex])
            {
                reloadTest = null;
                AmmoUI.instance.StopReload();
                isReloading = false;

            }
            timer += Time.deltaTime;
            yield return null;
        }
        // current wep
        if (isReloading == true)
        {
            wepList[wepListPos].ammoCur = wepList[wepListPos].ammoMax;

            AmmoUI.instance?.UpdateAmmo(wepList[wepListPos].ammoCur, wepList[wepListPos].ammoMax);
        }
        else
            isReloading = false;
    }


    // logic for shops
    public void SpeedIncrease(float amount)
    {
        walkSpeed += amount;
    }

    public void BonusHealth(int amount)
    {
        maxHP += amount;
    }

    // replace the weapon the player has with their purchased weapon????
    public void ReplaceWeapon(weaponStats stats, GameObject weaponPrefab, int slot)
    {
        // Replace logic here, e.g., add stats to list, instantiate weapon model
        wepList[slot] = stats;
      //  EquipGunModel(Instantiate(weaponPrefab)); // Mount prefab properly
        changeWeapon();
    }

    //audio

    IEnumerator playSteps()
    {
        isplayingSteps = true;
        aud.PlayOneShot(audSteps[Random.Range(0, audSteps.Length)], audStepsVol);

        if (!canSprint)
            yield return new WaitForSeconds(0.05f);
        else
            yield return new WaitForSeconds(0.03f);

        isplayingSteps = false;
    }
}