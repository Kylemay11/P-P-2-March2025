using UnityEngine;

public class HealthPickups : MonoBehaviour, IPickupable
{
    [SerializeField] private int healAmount;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void OnPickup(playerController player)
    {
        player.Heal(healAmount);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            IPickupable pickup = GetComponent<IPickupable>();
            if (pickup != null)
            {
                pickup.OnPickup(other.GetComponent<playerController>());
            }
        }
    }
}
