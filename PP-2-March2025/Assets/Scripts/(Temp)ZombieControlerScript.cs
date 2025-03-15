using UnityEngine;
using UnityEngine.AI;

public class ZombieController : MonoBehaviour, IDamage
{
    public event System.Action OnZombieDeath;
    [SerializeField] private int maxHealth;
    [SerializeField] private int damageToPlayer;
    [SerializeField] private float attackCooldown;
    private Transform player;
    private NavMeshAgent agent;

    [SerializeField] private int currentHealth;
    private float lastAttackTime;

    private void Start()
    {
        currentHealth = maxHealth;
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        if (player != null)
        {
            agent.SetDestination(player.position);
        }
    }

    public void SetTarget(Transform target)
    {
        player = target;
        if (agent != null)
            agent.SetDestination(player.position);
    }
    public void takeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Placeholder death behavior
        OnZombieDeath?.Invoke();
        Destroy(gameObject);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerController player = other.GetComponent<playerController>();
            if (player != null && Time.time >= lastAttackTime + attackCooldown)
            {
                player.takeDamage(damageToPlayer);
                lastAttackTime = Time.time;
            }
        }
    }
}