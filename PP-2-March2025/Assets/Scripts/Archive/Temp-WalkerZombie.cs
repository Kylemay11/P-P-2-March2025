using UnityEngine;

public class WalkerZombie : MonoBehaviour, IZombie
{
    private float health;
    private float speed;
    private float damage;

    public void InitializeZombie(float health, float speed, float damage)
    {
        this.health = health;
        this.speed = speed;
        this.damage = damage;

    }
}
