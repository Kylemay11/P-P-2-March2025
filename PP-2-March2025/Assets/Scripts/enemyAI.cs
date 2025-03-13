using UnityEngine;
using UnityEngine.AI;

public class enemyAI : MonoBehaviour
{
    // [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    // [SerializeField] Animator anim;

    [SerializeField] int HP;
    [SerializeField] int faceTargetSpeed;
    // [SerializeField] int animTranSpeed;

    [SerializeField] Transform attackPOS;
    [SerializeField] GameObject zombieBile;
    [SerializeField] float attackRate;
    [SerializeField] float sightRange, attackRange;

    float attackTimer;
    Vector3 playerDir;
    bool playerInSightRange, playerInAttackRange; 



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
    }

    private void enemyAttack()
    {
        attackTimer = 0;
        // do animation for melee attack
        Instantiate(zombieBile, attackPOS.position, transform.rotation);
    }

}
