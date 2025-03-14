using UnityEngine;
using UnityEngine.AI;

public class enemyAI : MonoBehaviour
{
    enum enemyType { walker, runner, spitter, tank };
    // [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] enemyType type;
    [SerializeField] SphereCollider sphereCollider;
    // [SerializeField] Animator anim;

    [Range(1, 10)] [SerializeField] int HP;
    [Range(1, 30)] [SerializeField] float faceTargetSpeed;
    [Range(1, 20)] [SerializeField] float enemySpeed;
    [Range(1.1f, 5)] [SerializeField] float runMultiplyer;
    [Range(5, 30)] [SerializeField] float enemyRunSpeed;
    // [SerializeField] int animTranSpeed;

    [SerializeField] Transform player;
    [SerializeField] Transform attackPOS;
    [SerializeField] GameObject zombieBile;
    [Range(0.5f, 5)] [SerializeField] float attackRate;
    [Range(1, 30)] [SerializeField] float sightRange, attackRange;

    float attackTimer;
    Vector3 playerDir;
    bool playerInSightRange, playerInAttackRange;
    public float projectileSpeed;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent.speed = enemySpeed;
        sphereCollider.radius = sightRange;
        if (type == enemyType.spitter)
            agent.stoppingDistance = attackRange;
    }

    // Update is called once per frame
    void Update()
    {
        attackTimer += Time.deltaTime;

        playerDir = gameManager.instance.player.transform.position - transform.position; // always know
        agent.SetDestination(gameManager.instance.player.transform.position);

        // 
        if(playerInSightRange && type == enemyType.runner)
        {
            agent.speed = enemyRunSpeed;
            faceTargetSpeed = (enemyRunSpeed * 1.5f);
        }
        

        if (attackTimer >= attackRate)
            enemyAttack();

        if (agent.remainingDistance <= agent.stoppingDistance)
            faceTarget();
    }

    private void enemyAttack()
    {
        attackTimer = 0;
        // do animation for melee attack

        if(type == enemyType.spitter)
        {
            // temp variable
            GameObject projectile = Instantiate(zombieBile, attackPOS.position, transform.rotation);
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            Vector3 toPlayer = player.position - transform.position;
            Vector3 toPlayerXZ = new Vector3(toPlayer.x, 0, toPlayer.z);
            float x = toPlayerXZ.magnitude;
            float y = toPlayer.y - transform.position.y;

            // calculate


        }

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
}
