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

    [Header("Melee Settings")]
    [SerializeField] private weaponStats meleeStats;

    [Header("Weapon Holders")]
    [SerializeField] GameObject gunHolder; 
    [SerializeField] GameObject meleeHolder;
    [SerializeField] GameObject itemHolder;

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
    [SerializeField] public Vector3 pcADSPosition;
    [SerializeField] private GameObject mFlashPos;
    [SerializeField] private Coroutine reloadTest;
    [SerializeField] private ParticleSystem mFlash;
    [SerializeField] private ParticleSystem currentMuzzleFlash;

    [Header("Throwable Settings")]
    [SerializeField] List<Throwables> itemList = new List<Throwables>();
    [SerializeField] GameObject itemModel;
    [SerializeField] private int itemDamage;
    [SerializeField] private int itemDist;
    [SerializeField] private float itemPrimeRate;
    [SerializeField] private float itemThrowForce;

    [Header("--- Audio ---")]
    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip[] audSteps;
    [Range(0, 1)][SerializeField] float audStepsVol;
    [SerializeField] AudioClip[] audJump;
    [Range(0, 1)][SerializeField] float audJumpVol;
    [SerializeField] AudioClip[] audHurt;
    [Range(0, 1)][SerializeField] float audHurtVol;
    [SerializeField] AudioClip[] audSlide;
    [Range(0, 1)][SerializeField] float audSlideVol;
    [SerializeField] AudioClip[] audStaminaOut;
    [Range(0, 1)][SerializeField] float audStaminaOutVol;
    [SerializeField] AudioClip[] audGunShot;
    [Range(0, 1)][SerializeField] float audGunShotVol;
    [SerializeField] AudioClip[] audReload;
    [Range(0, 1)][SerializeField] float audReloadVol;
    [SerializeField] AudioClip[] audEmptyGun;
    [Range(0, 1)][SerializeField] float audEmptyGunVol;
    [SerializeField] AudioClip[] audChangeGun;
    [Range(0, 1)][SerializeField] float audChangeGunVol;
    [SerializeField] AudioClip[] audCrouch;
    [Range(0, 1)][SerializeField] float audCrouchVol;
    [SerializeField] AudioClip[] audMelee;
    [Range(0, 1)][SerializeField] float audMeleeVol;






    public int zombiesKilled = 0;

    private Vector3 moveDir;
    private Vector3 velocity;
    public int wepListPos;
    private int itemListPos;
    private bool isThrowableEquipped;
    private float attackTimer;
    private bool isReloading;
    private int jumpCount;
    private float currentSpeed;
    private float staminaRegenTimer;
    private bool staminaFullyDrained;
    private bool canSprint;
    private bool isSprinting;
    private bool isSliding;
    [SerializeField] private bool isGrounded;
    bool isplayingSteps;
    public bool canMove = true;

    void Start()
    {
        instance = this;
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

        wepList.Add(meleeStats); // Add melee as default weapon
        wepListPos = 0;
        changeWeapon();
    }

    void Update()
    {
        if (canMove)
        {
            HandleMovement();
            HandleSprint();
            HandleCrouch();
            HandleStamina();
            UpdateSpeedUI();
        }
    }

    private void HandleMovement()
    {
        attackTimer += Time.deltaTime;

        if (controller.isGrounded)
        {
            if (!isplayingSteps && moveDir.magnitude > 0.3f)
            {
                StartCoroutine(playStep());
            }
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

        if (!gameManager.instance.isPaused && Input.GetButton("Fire1"))
        {
            if (isThrowableEquipped)
                ThrowItem();
            else if (wepList.Count > 0 && attackTimer >= wepRate)
                Shoot();
        }
        selectWeapon();
        reloadWeapon();
    }

    IEnumerator playStep()
    {
            isplayingSteps = true;
            aud.PlayOneShot(audSteps[Random.Range(0, audSteps.Length)], audStepsVol);
            if (!isSprinting)
            {
                yield return new WaitForSeconds(0.5f);
            }
            else
            {
                yield return new WaitForSeconds(0.3f);
            }
            isplayingSteps = false;
    }

    private void HandleSprint()
    {
        if (Input.GetButtonDown("Sprint") && isGrounded && canSprint)
        {
            staminaRegenTimer = 0f;
            currentState = PlayerState.Sprinting;
            currentSpeed = walkSpeed * sprintMultiplier;
            isSprinting = true;
        }
        else if (Input.GetButtonUp("Sprint") && currentState == PlayerState.Sprinting)
        {
            currentState = PlayerState.Walking;
            currentSpeed = walkSpeed;
            isSprinting = false;
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
        if (aud != null && audCrouch != null && !isSliding)
            aud.PlayOneShot(audCrouch[Random.Range(0, audCrouch.Length)], audCrouchVol);

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
        isSliding = true;
        currentState = PlayerState.Sliding;
        currentSpeed = walkSpeed * sprintMultiplier * slideSpeedBoost;
        controller.height = crouchColliderSize.y;
        controller.center = new Vector3(0, crouchColliderSize.y / 2f, 0);

        if (aud != null && audSlide != null)
            aud.PlayOneShot(audSlide[Random.Range(0, audSlide.Length)], audSlideVol);

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
        isSliding = false;
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
            if (aud != null && audJump != null)
                aud.PlayOneShot(audJump[Random.Range(0, audJump.Length)],audJumpVol);
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

                if (aud != null && audStaminaOut != null)
                    aud.PlayOneShot(audStaminaOut[Random.Range(0, audStaminaOut.Length)], audStaminaOutVol);

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
        aud.PlayOneShot(audHurt[Random.Range(0, audHurt.Length)], audHurtVol);
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
    public void getThrowables(Throwables item)
    {        
        itemList.Add(item);
        itemListPos = itemList.Count - 1;
        itemList[itemListPos].isPickedup = true;
        itemList[itemListPos].pickedup();
        ToggleThrowableEquipped();
    }
    private void ToggleThrowableEquipped()
    {
        isThrowableEquipped = !isThrowableEquipped;
        if (isThrowableEquipped)
        {
            // Hide weapon, show throwable
            if (gunHolder != null) gunHolder.SetActive(false);
            if (meleeHolder != null) meleeHolder.SetActive(false);
            if (itemHolder != null) itemHolder.SetActive(true);
            changeThrowable();
        }
        else if(!isThrowableEquipped)
        {
            // Show weapon, hide throwable
            if (itemHolder != null) itemHolder.SetActive(false);
            changeWeapon();
        }
    }

    void selectWeapon()
    {
        // Throwable selection
        if (Input.GetKeyDown(KeyCode.Q) && itemList.Count > 0)
        {
            ToggleThrowableEquipped();
        }

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
        if (isThrowableEquipped)
            return;
        // Clean up any active muzzle flash
        if (currentMuzzleFlash != null)
        {
            Destroy(currentMuzzleFlash.gameObject);
            currentMuzzleFlash = null;
        }

        if (isReloading && !isThrowableEquipped)
        {
            //StopCoroutine(reloadTest);
            isReloading = false;
        }

        // Play weapon switch sound
        if (aud != null && audChangeGun != null && audChangeGun.Length > 0)
            aud.PlayOneShot(audChangeGun[Random.Range(0, audChangeGun.Length)], audChangeGunVol);

        bool isMelee = wepList[wepListPos].isMelee;

        
        if (gunHolder != null) gunHolder.SetActive(!isMelee);
        if (meleeHolder != null) meleeHolder.SetActive(isMelee);

        // Update weapon model
        if (isMelee)
        {
            if (meleeHolder != null && wepList[wepListPos].model != null)
            {
                MeshFilter meleeMesh = meleeHolder.GetComponent<MeshFilter>();
                MeshRenderer meleeRenderer = meleeHolder.GetComponent<MeshRenderer>();
                if (meleeMesh != null) meleeMesh.sharedMesh = wepList[wepListPos].model.GetComponent<MeshFilter>()?.sharedMesh;
                if (meleeRenderer != null) meleeRenderer.sharedMaterial = wepList[wepListPos].model.GetComponent<MeshRenderer>()?.sharedMaterial;
            }
        }
        else
        {
            if (gunHolder != null && wepList[wepListPos].model != null)
            {
                MeshFilter gunMesh = gunHolder.GetComponent<MeshFilter>();
                MeshRenderer gunRenderer = gunHolder.GetComponent<MeshRenderer>();
                if (gunMesh != null) gunMesh.sharedMesh = wepList[wepListPos].model.GetComponent<MeshFilter>()?.sharedMesh;
                if (gunRenderer != null) gunRenderer.sharedMaterial = wepList[wepListPos].model.GetComponent<MeshRenderer>()?.sharedMaterial;
            }
        }

        // Update combat stats
        wepDamage = wepList[wepListPos].wepDamage;
        wepDist = wepList[wepListPos].wepDist;
        wepRate = wepList[wepListPos].wepRate;
        reloadTime = wepList[wepListPos].reloadTime;
        pcADSPosition = wepList[wepListPos].wepADSPosition;

        // Update UI elements
        UpdateAmmoUI();
        AmmoUI.instance?.Show(!isMelee);

        if (gameManager.instance != null && gameManager.instance.weaponNotification != null)
            gameManager.instance.weaponNotification.ShowWeaponName(wepList[wepListPos].weaponName);
    }

    private void changeThrowable()
    {
        if (itemHolder != null && itemList[itemListPos].model != null)
        {
            MeshFilter itemMesh = itemHolder.GetComponent<MeshFilter>();
            MeshRenderer itemRenderer = itemHolder.GetComponent<MeshRenderer>();
            if (itemMesh != null) itemMesh.sharedMesh = itemList[itemListPos].model.GetComponent<MeshFilter>()?.sharedMesh;
            if (itemRenderer != null) itemRenderer.sharedMaterial = itemList[itemListPos].model.GetComponent<MeshRenderer>()?.sharedMaterial;
        }

        itemDamage = itemList[itemListPos].itemDamage; // is necessary ?
        itemDist = itemList[itemListPos].itemDist;
        itemPrimeRate = itemList[itemListPos].itemPrimeRate;
        itemThrowForce = itemList[itemListPos].itemThrowForce;

        UpdateThrowablesUI();

        if (gameManager.instance != null && gameManager.instance.weaponNotification != null)
            gameManager.instance.weaponNotification.ShowWeaponName(itemList[itemListPos].itemName);
    }

    void reloadWeapon()
    {

        if (wepList[wepListPos].isMelee)
            return;

       if(!isThrowableEquipped)
       { 
            if (Input.GetButtonDown("Reload") && !isReloading) // add Timer for reload animation
            {

                reloadTest = StartCoroutine(Reload());
            }
       }
    }

    private void Shoot()
    {
        if (!canMove) return;
        //if (!wepList[wepListPos].CanFire()) // isempty
        //{
        //    aud.PlayOneShot(audEmptyGun[Random.Range(0, audEmptyGun.Length)], audEmptyGunVol);

        //    return; 
        //} // click sound
        if (wepList[wepListPos].isMelee)
        {
            MeleeAttack();
            return;
        }

        if (wepList[wepListPos].ammoCur <= 0)
        {
            //aud.PlayOneShot(audEmptyGun[Random.Range(0, audEmptyGun.Length)], audEmptyGunVol);
            return;
        }

        attackTimer = 0;

        if (mFlashPos != null)
        {
            StartCoroutine(FlashMuzzle());
        }

        
        aud.PlayOneShot(wepList[wepListPos].wepSound[Random.Range(0, wepList[wepListPos].wepSound.Length)], wepList[wepListPos].wepVolume);

        wepList[wepListPos].ammoCur--;
        //attackTimer = Time.deltaTime + wepRate;
        UpdateAmmoUI();

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

    private void MeleeAttack()
    {
        attackTimer = 0;

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, wepList[wepListPos].wepDist, hitMask))
        {
            IDamage target = hit.collider.GetComponentInParent<IDamage>();
            if (target != null)
            {
                target.takeDamage(wepList[wepListPos].wepDamage);
            }
            Instantiate(wepList[wepListPos].hitEffect, hit.point, Quaternion.identity);
        }

        if(aud != null && audMelee != null)
            aud.PlayOneShot(audMelee[Random.Range(0, audMelee.Length)], audMeleeVol);
    }

    void ThrowItem()
    {
        if (itemList.Count == 0 || !itemList[itemListPos].CanThrow()) return;

        // Implement throwable throwing logic here
        // Use itemDamage, itemDist, itemThrowSpeed, etc.
        GameObject throwable = Instantiate(itemList[itemListPos].itemPrefab, transform.position, transform.rotation);
        Rigidbody rb = throwable.GetComponent<Rigidbody>();
        rb.AddForce(transform.forward*itemThrowForce, ForceMode.VelocityChange);

        itemList[itemListPos].curInventory--;
        UpdateThrowablesUI();

        StartCoroutine(throwableExplode(throwable));
        // auto reload
        StartCoroutine(ReloadThrowable());
    }

    private IEnumerator FlashMuzzle()
    {
        // Clean up existing muzzle flash
        if (currentMuzzleFlash != null)
        {
            Destroy(currentMuzzleFlash.gameObject);
        }

        // Create new flash
        ParticleSystem psMFlash = Instantiate(mFlash, mFlashPos.transform.position, Quaternion.identity);
        psMFlash.transform.SetParent(mFlashPos.transform);
        currentMuzzleFlash = psMFlash; // Track the new flash

        if (psMFlash != null)
        {
            psMFlash.Play();
        }

        yield return new WaitForSeconds(0.12f);

        // Stop and destroy after delay
        if (psMFlash != null)
        {
            psMFlash.Stop();
            Destroy(psMFlash.gameObject, 1f); // Adjust time to match particle lifetime
        }
    }

    private IEnumerator Reload()
    {

        // temp current wep
        int currentWepIndex = wepListPos;

        isReloading = true;
        if(aud != null && audReload != null)
            aud.PlayOneShot(audReload[Random.Range(0, audReload.Length)], audReloadVol);

        if (wepList[wepListPos].ammoCur > 1)
        {
            wepList[wepListPos].ammoCur = 1; // 1 in chamber
            UpdateAmmoUI();
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
            wepList[wepListPos].Reload();
            UpdateAmmoUI();
            isReloading = false;
        }
        else
            isReloading = false;
    }

    IEnumerator ReloadThrowable()
    {
        isReloading = true;
        // Play reload animation, sound, etc.

        float timer = 0f;
        while (timer < itemList[itemListPos].reloadTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        itemList[itemListPos].Reload();
        UpdateThrowablesUI();
        isReloading = false;
    }

    IEnumerator throwableExplode(GameObject throwable)
    {
        float timer = 0f;
        while (timer < itemList[itemListPos].itemDelay)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        Instantiate(itemList[itemListPos].hitEffect, throwable.transform.position, throwable.transform.rotation);

        Collider[] nearbyColliders = Physics.OverlapSphere(throwable.transform.position, itemList[itemListPos].itemDamageRadius);

        foreach (Collider nearbyObject in nearbyColliders)
        {
            IDamage target = nearbyObject.GetComponentInParent<IDamage>();
            if (target != null)
                target.takeDamage((int)itemDamage);
            
        }

        Destroy(throwable.gameObject);
        aud.PlayOneShot(itemList[itemListPos].itemSound[0], itemList[itemListPos].itemVolume);
    }

    // logic for shops AND items
    public void SpeedIncrease(float amount)
    {
        walkSpeed += amount;
    }
    public void StaminaIncrease(float amount)
    {
        maxStamina += amount;
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
    }

    public void BonusHealth(int amount)
    {
        maxHP += amount;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP); 
        updatePlayerUI();
    }

    public void IncreaseWeaponDamage(int amount)
    {
        wepDamage += amount;
    }

    public void IncreaseWeaponFireRate(float amount)
    {
        wepRate -= amount;
    }

    public void IncreaseWeaponFireDistance(int amount)
    {
        wepDist += amount;
    }

    // replace the weapon the player has with their purchased weapon
    public void ReplaceWeapon(weaponStats newWeapon)
    {
        wepList.Remove(wepList[wepListPos]);
        wepList.Add(newWeapon);
        changeWeapon();
    }
    public void DamageIncrease(int amount)
    {
        wepDamage += amount;
    }
    public void UpdateAmmoUI()
    {

        if (wepList[wepListPos].isMelee)
        {
            AmmoUI.instance?.UpdateAmmo(0, 0); // Hide ammo for melee
        }
        else
        {
            AmmoUI.instance?.UpdateAmmo(wepList[wepListPos].ammoCur, wepList[wepListPos].curReserve);
        }
    }
    public void UpdateThrowablesUI()
    {
        if (itemList[itemListPos]) // !null
            AmmoUI.instance?.UpdateThrowable(itemList[itemListPos].curInventory, itemList[itemListPos].curReserve);
        else
            AmmoUI.instance?.UpdateThrowable(0, 0); // assume no throwable
    }


    // make the weapon change the weapon postion of the current weapon user has equipped

}