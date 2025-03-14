using UnityEngine;
using UnityEngine.AI;

public class enemyAI : MonoBehaviour
{
    enum enemyType { walker, runner, spitter, tank };
    // [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] enemyType type;
    // [SerializeField] Animator anim;

    [SerializeField] int HP;
    [SerializeField] int faceTargetSpeed;
    // [SerializeField] int animTranSpeed;

    [SerializeField] Transform player;
    [SerializeField] Transform attackPOS;
    [SerializeField] GameObject zombieBile;
    [SerializeField] float attackRate;
    [SerializeField] float sightRange, attackRange;

    float attackTimer;
    Vector3 playerDir;
    bool playerInSightRange, playerInAttackRange;
    public float projectileSpeed;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        
    }

    // Update is called once per frame
    void Update()
    {
        attackTimer += Time.deltaTime;

        playerDir = gameManager.instance.player.transform.position - transform.position; // always know
        agent.SetDestination(gameManager.instance.player.transform.position);

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

        }

    }

    void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, transform.position.y, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }
}
