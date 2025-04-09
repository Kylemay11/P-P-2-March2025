using UnityEngine;

public class DummyZombie : MonoBehaviour, IDamage
{
    [SerializeField] private float maxHealth;
    private float currentHealth;

    [SerializeField] private GameObject damagePopupPrefab;
    [SerializeField] private Transform damagePopupSpawnPoint; // position above head

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void takeDamage(int damage)
    {
        currentHealth -= damage;
        ShowDamagePopup(damage);
    }

    private void ShowDamagePopup(float damage)
    {
        if (damagePopupPrefab == null || damagePopupSpawnPoint == null) return;

        GameObject popup = Instantiate(damagePopupPrefab, damagePopupSpawnPoint.position, Quaternion.identity);
        popup.GetComponent<DamagePopup>().Setup(damage);
    }
}

