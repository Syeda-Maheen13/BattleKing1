using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public Image healthFill;   // Fill wali Image drag karo inspector me
    public float maxHealth = 200f;   // total health 200
    private float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    void Update()
    {
        // ✅ Test ke liye key press
        if (Input.GetKeyDown(KeyCode.M))   // M press = Damage
        {
            TakeDamage(20f);   // Example: 20 damage
        }

        if (Input.GetKeyDown(KeyCode.H))   // H press = Heal
        {
            Heal(15f);   // Example: 15 heal
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;
        UpdateHealthBar();
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
        UpdateHealthBar();
    }

    void UpdateHealthBar()
    {
        healthFill.fillAmount = currentHealth / maxHealth;
        Debug.Log("Health: " + currentHealth);
    }
}
