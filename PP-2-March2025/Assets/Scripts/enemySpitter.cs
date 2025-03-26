using UnityEngine;


public class enemySpitter : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float attackRange;
    [SerializeField] private float attackCooldown;
    [SerializeField] private float projectileSpeed;
    [SerializeField] private float arcHeight;

    private Transform player;
    private float lastAttackTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        lastAttackTime = -attackCooldown; // Allow immediate first attack
    }

    // Update is called once per frame
    void Update()
    {
        if (CanAttack())
        {
            AttackPlayer();
        }
    }

    bool CanAttack()
    {
        return Vector3.Distance(transform.position, player.position) <= attackRange &&
               Time.time - lastAttackTime >= attackCooldown;
    }

    void AttackPlayer()
    {
        lastAttackTime = Time.time;

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        zombieBile projectileScript = projectile.GetComponent<zombieBile>();

        if (projectileScript != null)
        {
            projectileScript.Launch(player.position, projectileSpeed, arcHeight);
        }
    }
}
