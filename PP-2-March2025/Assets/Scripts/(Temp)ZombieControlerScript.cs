using UnityEngine;

public class ZombieController : MonoBehaviour, IDamage
{
    [SerializeField] private int maxHealth = 50;
    [SerializeField] private int damageToPlayer = 10;
    [SerializeField] private float attackCooldown = 2f;

    private int currentHealth;
    private float lastAttackTime;

    private void Start()
    {
        currentHealth = maxHealth;
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