using UnityEngine;
using UnityEngine.AI;

public class enemyAI : MonoBehaviour, IDamage, IZombie
{
    public event System.Action OnZombieDeath;

    enum enemyType { walker, runner, spitter, tank };
    // [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] enemyType type;
    [SerializeField] SphereCollider sphereCollider;
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


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent.speed = enemySpeed;
        sphereCollider.radius = sightRange;
        if (type == enemyType.spitter)
            agent.stoppingDistance = attackRange;

        if (type == enemyType.tank)
        {
            transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        attackTimer += Time.deltaTime;

        playerDir = gameManager.instance.player.transform.position - transform.position; // always know
        agent.SetDestination(gameManager.instance.player.transform.position);

        // 
        if (playerInSightRange && type == enemyType.runner)
        {
            agent.speed = enemySpeed * runMultiplyer;
            faceTargetSpeed = (enemySpeed * runMultiplyer);
        }


        if (attackTimer >= attackRate)
            enemyAttack();

        if (agent.remainingDistance <= agent.stoppingDistance)
            faceTarget();
    }

    public void takeDamage(int amount)
    {
        HP -= amount;
        if (HP <= 0)
        {
            OnZombieDeath?.Invoke();
            Destroy(gameObject);
        }
    }

    private void enemyAttack()
    {
        attackTimer = 0;
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

}
