using UnityEngine;

public class DestructableEnvo : MonoBehaviour, IDamage
{
    [Range(1, 100)][SerializeField] int HP;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void takeDamage(int amount)
    {
        HP -= amount;

        if (HP <= 0)
            Destroy(gameObject);
    }
}
