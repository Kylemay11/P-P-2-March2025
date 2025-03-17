using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using static BarricadeDoor;
using static UnityEngine.GraphicsBuffer;

public class enemyAI : MonoBehaviour, IDamage, IZombie
{
    public event System.Action OnZombieDeath;
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
    // [SerializeField] int animTranSpeed;

    // [SerializeField] Transform player;
    // [SerializeField] Transform spitterTarget;
    // [SerializeField] Transform attackPOS;
    // [SerializeField] GameObject zombieBile;
    [Range(0.5f, 5)][SerializeField] float attackRate;
    [Range(1, 30)][SerializeField] float sightRange, attackRange;

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
        agent.avoidancePriority = Random.Range(30, 70);

        if (type == enemyType.spitter)
            agent.stoppingDistance = attackRange;

        if (type == enemyType.tank)
            transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);

        if (barrierDoor == null)
        {
            barrierDoor = GetComponentInParent<BarricadeDoor>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Kyle Added for barricade door
        if (agent == null || !agent.isActiveAndEnabled) return;

        attackTimer += Time.deltaTime;
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            playerDir = barrierDoor.GetAttackPoint() - transform.position;
          
            faceTarget();
        }

        switch (currentTargetState)
        {
            case ZombieTargetState.AttackingDoor:
                if (barrierDoor != null && barrierDoor.CurrentState != BarricadeDoor.DoorState.Destroyed)
                {
                   agent.SetDestination(barrierDoor.GetAttackPoint());
                }
                else
                {
                    currentTargetState = ZombieTargetState.AttackingPlayer;
                    agent.SetDestination(gameManager.instance.player.transform.position);
                }
                break;

            case ZombieTargetState.AttackingPlayer:
                agent.SetDestination(gameManager.instance.player.transform.position);
                break;
        }

        float distanceToPlayer = Vector3.Distance(gameManager.instance.player.transform.position, transform.position);
        playerInAttackRange = distanceToPlayer <= attackRange;

        if (playerInSightRange && type == enemyType.runner)
            agent.speed = enemySpeed * runMultiplyer;

        if (attackTimer >= attackRate)
            enemyAttack();

        if (agent.remainingDistance <= agent.stoppingDistance)
            faceTarget();
    }

    public void takeDamage(int amount)
    {
        HP -= amount;
        //Kyle added
        animator.SetTrigger("TakeDamage");
        StartCoroutine(DamageAnimationCooldown());
        StartCoroutine(FlashRed());
        if (HP <= 0)
        {
            CurrencySystem.instance.AddMoney(moneyOnDeath);
            OnZombieDeath?.Invoke();
            if (barrierDoor != null)
                barrierDoor.RemoveAttacker(this);
            Destroy(gameObject);
        }
    }

    private void enemyAttack()
    {
        attackTimer = 0;
        //Kyle added for demo
        if (playerInAttackRange)
        {
            float distanceToDoor = Vector3.Distance(transform.position, barrierDoor.GetAttackPoint());
            bool doorInAttackRange = distanceToDoor <= attackRange;

            if (barrierDoor != null && barrierDoor.CurrentState != BarricadeDoor.DoorState.Destroyed)
            {
                if (barrierDoor.CanZombieAttack(this))
                {
                    barrierDoor.ApplyDamage(enemyDamage);
                    Debug.Log($"{gameObject.name} attacked the barricade door for {enemyDamage} damage.");
                }
                else
                {
                    // Optional: idle or wander near door if not allowed to attack
                    agent.SetDestination(barrierDoor.GetAttackPoint()); // just hang out
                }
                return;
            }
            else
            {
                //Kyle added for only damage the player when the door is gone
                Debug.Log($"[Zombie Attack] {gameObject.name} attacked player for {enemyDamage} damage!");
                playerController player = gameManager.instance.player.GetComponent<playerController>();
                if (player != null)
                {
                    player.takeDamage((int)enemyDamage);
                }
            }
            // do animation for melee attack

            //if(type == enemyType.spitter)
            //{
            //    // temp variable
            //    GameObject projectile = Instantiate(zombieBile, attackPOS.position, transform.rotation);
            //}//if(type == enemyType.spitter)
            //{
            //    // temp variable
            //    GameObject projectile = Instantiate(zombieBile, attackPOS.position, transform.rotation);
            //}

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
    public void ForcePathUpdate()
    {
        if (agent != null && agent.isActiveAndEnabled)
        {
            switch (currentTargetState)
            {
                case ZombieTargetState.AttackingDoor:
                    if (barrierDoor != null && barrierDoor.CurrentState != BarricadeDoor.DoorState.Destroyed)
                    {
                        agent.SetDestination(barrierDoor.transform.position);
                    }
                    break;
                case ZombieTargetState.AttackingPlayer:
                    agent.SetDestination(gameManager.instance.player.transform.position);
                    break;
            }
        }
    }

    // Kyle added for barricadeDoor
    public void SetBarricadeDoor(BarricadeDoor door)
    {
        this.barrierDoor = door;
    }
    // Kyle added for barricadeDoor
    public void SetTargetState(ZombieTargetState newState)
    {
        if (hasEnteredRoom && newState == ZombieTargetState.AttackingDoor)
            return;

        if (currentTargetState == ZombieTargetState.AttackingDoor && newState == ZombieTargetState.AttackingPlayer && barrierDoor != null)
        {
            barrierDoor.RemoveAttacker(this);
        }

        currentTargetState = newState;
        if (newState == ZombieTargetState.AttackingPlayer)
            hasEnteredRoom = true;

        ForcePathUpdate();
    }
}
