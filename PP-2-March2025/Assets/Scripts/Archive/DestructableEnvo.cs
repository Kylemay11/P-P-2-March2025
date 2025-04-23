using UnityEngine;

public class DestructableEnvo : MonoBehaviour, IDamage
{
    [Range(1, 100)][SerializeField] int HP;

   
    public void takeDamage(int amount)
    {
        HP -= amount;

        if (HP <= 0)
            Destroy(gameObject);
    }
}
