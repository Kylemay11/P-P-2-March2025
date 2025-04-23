using UnityEngine;

public class HeadshotDetector : MonoBehaviour, IDamage
{
    [SerializeField] private enemyAI enemy;
    [SerializeField] private DummyZombie dummy;
    [SerializeField] private float multiplier = 2f;

    public void takeDamage(int amount)
    {
        int headshotDamage = Mathf.RoundToInt(amount * multiplier);

        if (enemy != null)
        {
            enemy.takeDamage(headshotDamage);
        }
        else if (dummy != null)
        {
            dummy.HandleHeadshot(headshotDamage);
        }
    }
}