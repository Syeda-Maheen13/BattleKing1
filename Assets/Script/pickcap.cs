using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public float healAmount = 15f; // kitna heal kare

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger enter: " + other.name); // ✅ dekhna console me
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.Heal(healAmount);
            }
            Destroy(gameObject);
        }
    }


}
