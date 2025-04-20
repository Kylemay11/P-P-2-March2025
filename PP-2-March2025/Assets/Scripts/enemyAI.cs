using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static BarricadeDoor;
using static UnityEngine.GraphicsBuffer;

public class enemyAI : MonoBehaviour, IDamage, IZombie
{
    public System.Action OnZombieDeath;
    public ZombieSpawner originSpawner;
    //Kyle added for barricade
    public enum ZombieTargetState { AttackingDoor, AttackingPlayer };

    enum enemyType { walker, runner, spitter, tank };
    // [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    //Kyle added so you can gain money
    [SerializeField] int moneyOnDeath;
    [SerializeField] enemyType type;
    [SerializeField] SphereCollider sphereCollider;
    [SerializeField] private Renderer zombieRenderer;
    [SerializeField] private Color damageColor = Color.red;
    private Color originalColor;
    // [SerializeField] Rigidbody projectileRB;
    // [SerializeField] Animator anim;
    [Range(0.5f, 50)][SerializeField] float projectileSpeed;
    int HP;
    float enemyDamage;
    float enemySpeed;
    [Range(1, 30)][SerializeField] float faceTargetSpeed;
    [Range(1.1f, 5)][SerializeField] float runMultiplyer;

    //KyleAdded for wave scalling
    [Range(1, 500)][SerializeField] private float baseHealth;
    [Range(1, 20)][SerializeField] private float baseSpeed;
    [Range(1, 500)][SerializeField] private float baseDamage;


    //Kyle added for breaking the barrier door
    [SerializeField] public BarricadeDoor barrierDoor;
    private bool hasEnteredRoom = false;
    private Transform selectedAttackPoint;
    // [SerializeField] int animTranSpeed;

    // [SerializeField] Transform player;
    // [SerializeField] Transform spitterTarget;
    // [SerializeField] Transform attackPOS;
    // [SerializeField] GameObject zombieBile;
    [Range(0.5f, 5)][SerializeField] float attackRate;
    [Range(1, 30)][SerializeField] float sightRange, attackRange;

    [Header("--- Audio ---")]
    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip[] audSteps;
    [Range(0, 1)][SerializeField] float audStepsVol;
    [SerializeField] AudioClip[] audAttack;
    [Range(0, 1)][SerializeField] float audAttackVol; 
    [SerializeField] AudioClip[] audHurt;
    [Range(0, 1)][SerializeField] float audHurtVol;
    [SerializeField] AudioClip[] audDeath;
    [Range(0, 1)][SerializeField] float audDeathVol;





    float attackTimer;
    Vector3 playerDir;
    bool playerInSightRange, playerInAttackRange;
    private ZombieTargetState currentTargetState = ZombieTargetState.AttackingDoor;

    Animator animator;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        originalColor = zombieRenderer.material.color;
        agent.speed = enemySpeed;
        //Kyle added Helps the zombies avoid eachother whn they spawn
        agent.avoidancePriority = Random.Range(10, 65);
        float radiusVaule = Random.Range(0.5f, 0.8f);
        agent.radius = Mathf.Round(radiusVaule * 10) * 0.1f;

        if (type == enemyType.spitter)
            agent.stoppingDistance = attackRange;

        if (type == enemyType.tank)
            transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);

        if (barrierDoor == null)
        {
            barrierDoor = GetComponentInParent<BarricadeDoor>();
        }
        if (barrierDoor != null)
            selectedAttackPoint = barrierDoor.GetRandomAttackPoint();
    }

    // Update is called once per frame
    void Update()
    {
        if (agent == null || !agent.isActiveAndEnabled) return;

        attackTimer += Time.deltaTime;

        HandleTargeting();
        HandleMovement();
        HandleAttack();
    }

    public void takeDamage(int amount)
    {
        HP -= amount;
        aud.PlayOneShot(audHurt[Random.Range(0, audHurt.Length)], audHurtVol);
        //Kyle added
        animator.SetTrigger("TakeDamage");
        StartCoroutine(DamageAnimationCooldown());
        StartCoroutine(FlashRed());
        if (HP <= 0)
        {
            CurrencySystem.instance.AddMoney(moneyOnDeath);
            OnZombieDeath?.Invoke();
            aud.PlayOneShot(audDeath[Random.Range(0, audDeath.Length)], audDeathVol);
            if (barrierDoor != null)
                barrierDoor.RemoveAttacker(this);
            Destroy(gameObject);
        }
    }

    private void enemyAttack()
    {
        attackTimer = 0;
        aud.PlayOneShot(audAttack[Random.Range(0, audAttack.Length)], audAttackVol);
        // 2. Attack barricade door (if still exists)
        if (barrierDoor != null && barrierDoor.CurrentState != BarricadeDoor.DoorState.Destroyed)
        {
            if (selectedAttackPoint == null)
                selectedAttackPoint = barrierDoor.GetRandomAttackPoint();

            float distanceToAttackPoint = Vector3.Distance(transform.position, selectedAttackPoint.position);

            if (distanceToAttackPoint <= attackRange)
            {
                if (barrierDoor.CanZombieAttack(this))
                {
                    barrierDoor.ApplyDamage(enemyDamage);
                    Debug.Log($"{gameObject.name} attacked the barricade door for {enemyDamage} damage.");
                }
            }
            else
            {
                agent.SetDestination(selectedAttackPoint.position);
            }

            return;
        }

        // 3. Attack player
        float distanceToPlayer = Vector3.Distance(transform.position, gameManager.instance.player.transform.position);
        if (distanceToPlayer <= attackRange)
        {
            Debug.Log($"[Zombie Attack] {gameObject.name} attacked player for {enemyDamage} damage!");
            playerController player = gameManager.instance.player.GetComponent<playerController>();
            if (player != null)
            {
                player.takeDamage((int)enemyDamage);
            }
        }
    }


    private float CalculateLaunchAngle(float initialVel, float x, float y, float gravity)
    {
        float iVelSquared = (initialVel * initialVel);
        float underSqrRoot = (iVelSquared * iVelSquared) - (gravity * (gravity * x * x + 2 * y * iVelSquared));

        if (underSqrRoot >= 0f)
        {
            float root = Mathf.Sqrt(underSqrRoot);
            float aimAngle = Mathf.Atan((iVelSquared + root) / (gravity * x));
            return aimAngle;
        }
        return initialVel;
    }

    void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, transform.position.y, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    //Kyle added for demo
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            float distance = Vector3.Distance(other.transform.position, transform.position);
            playerInAttackRange = distance <= attackRange;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInSightRange = true;
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInSightRange = false;
    }

    //Kyle added for build
    public void InitializeZombie(float waveHealthMult, float waveSpeedMult, float waveDamageMult)
    {
        HP = Mathf.RoundToInt(baseHealth * waveHealthMult);
        enemySpeed = baseSpeed * waveSpeedMult;
        enemyDamage = baseDamage * waveDamageMult;
        agent.speed = enemySpeed;

        Debug.Log($"[InitZombie] Final HP: {HP}, Speed: {enemySpeed}, Damage: {enemyDamage}");
    }

    //kyle added fopr demo
    private IEnumerator DamageAnimationCooldown()
    {
        agent.isStopped = true; // Stop movement while playing animation

        // Get the length of the animation
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        agent.isStopped = false; // Resume movement
    }

    private IEnumerator FlashRed()
    {
        zombieRenderer.material.color = damageColor;
        yield return new WaitForSeconds(0.2f);
        zombieRenderer.material.color = originalColor;
    }

    //Kyle added to force navmesh path to update
    //public void ForcePathUpdate()
    //{
    //    if (agent != null && agent.isActiveAndEnabled)
    //    {
    //        switch (currentTargetState)
    //        {
    //            case ZombieTargetState.AttackingDoor:
    //                if (barrierDoor != null && barrierDoor.CurrentState != BarricadeDoor.DoorState.Destroyed)
    //                {
    //                    agent.SetDestination(barrierDoor.transform.position);
    //                }
    //                break;
    //            case ZombieTargetState.AttackingPlayer:
    //                agent.SetDestination(gameManager.instance.player.transform.position);
    //                break;
    //        }
    //    }
    //}

    // Kyle added for barricadeDoor
    public void SetBarricadeDoor(BarricadeDoor door)
    {
        this.barrierDoor = door;
    }
    public void GoToTerminal(Vector3 targetPosition)
    {
        if (agent != null && agent.isActiveAndEnabled)
            agent.SetDestination(targetPosition);
    }
    //Kyle added for barricadeDoor
    public void SetTargetState(ZombieTargetState newState)
    {
        if (hasEnteredRoom && newState == ZombieTargetState.AttackingDoor)
            return;

        if (currentTargetState == ZombieTargetState.AttackingDoor &&
            newState == ZombieTargetState.AttackingPlayer &&
            barrierDoor != null)
        {
            barrierDoor.RemoveAttacker(this);
        }

        currentTargetState = newState;
        if (newState == ZombieTargetState.AttackingPlayer)
        {
            hasEnteredRoom = true;
            agent.SetDestination(gameManager.instance.player.transform.position);
            agent.isStopped = false;
        }
    }

    //private bool isbarrierDoor()
    //{
    //    if (!barrierDoor) // null
    //        return true;
    //    if (barrierDoor)
    //        return false;
    //}

    private void HandleTargeting()
    {
        if (currentTargetState == ZombieTargetState.AttackingDoor)
        {
            if (barrierDoor == null || barrierDoor.CurrentState == BarricadeDoor.DoorState.Destroyed)
            {
                currentTargetState = ZombieTargetState.AttackingPlayer;
                agent.SetDestination(gameManager.instance.player.transform.position);
            }
        }
    }

    private void HandleMovement()
    {
        aud.PlayOneShot(audSteps[Random.Range(0, audSteps.Length)], audStepsVol);

        switch (currentTargetState)
        {
            case ZombieTargetState.AttackingDoor:
                if (selectedAttackPoint == null) return;

                float distToPoint = Vector3.Distance(transform.position, selectedAttackPoint.position);
                if (distToPoint <= agent.stoppingDistance + 0.1f)
                {
                    agent.isStopped = true;

                    // Rotate to face the door
                    Vector3 lookDir = barrierDoor.transform.position - transform.position;
                    lookDir.y = 0f;
                    if (lookDir != Vector3.zero)
                    {
                        Quaternion rot = Quaternion.LookRotation(lookDir);
                        transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
                    }
                }
                else
                {
                    agent.SetDestination(selectedAttackPoint.position);
                    agent.isStopped = false;
                }
                break;

            case ZombieTargetState.AttackingPlayer:
                agent.SetDestination(gameManager.instance.player.transform.position);
                break;
        }

        if (playerInSightRange && type == enemyType.runner)
            agent.speed = enemySpeed * runMultiplyer;
    }

    private void HandleAttack()
    {
        if (attackTimer < attackRate) return;

        if (currentTargetState == ZombieTargetState.AttackingDoor && selectedAttackPoint != null)
        {
            float dist = Vector3.Distance(transform.position, selectedAttackPoint.position);
            if (dist <= agent.stoppingDistance + 0.1f)
                enemyAttack();
        }
        else if (currentTargetState == ZombieTargetState.AttackingPlayer && playerInAttackRange)
        {
            enemyAttack();
        }
    }
}
