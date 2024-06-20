using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class LifeBar : MonoBehaviour
{
    public Slider slider;
    public float maxHealth = 100f;
    private float currentHealth;
    public GameObject GameOverScreen;
    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    // Function to set the current health of the life bar
    public void SetHealth(float health)
    {
        currentHealth = health;
        UpdateHealthBar();
    }

    // Function to take damage
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0)
            currentHealth = 0; // Prevent health from going below zero
        UpdateHealthBar();

        // Check if the player is dead
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Function to update the visual representation of the health bar
    void UpdateHealthBar()
    {
        slider.value = currentHealth / maxHealth;
    }

    // Function to handle death (override this function in subclasses if needed)
    protected virtual void Die()
    {
        Debug.Log("Player died!");
        GameOverScreen.SetActive(true);
        Invoke("ReloadScene", 5f);
        // Add any additional actions upon death here
    }
    void onCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            TakeDamage(10);
        }
    }
    void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
