using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage = 10f; // Amount of damage the bullet inflicts

   void OnCollisionEnter(Collision collision)
    {
        // Check if the collision is with the player
        if (collision.gameObject.CompareTag("Player"))
        {
            // Get the LifeBar component attached to the player
            LifeBar playerLife = collision.gameObject.GetComponent<LifeBar>();
            
            // Check if the player has a LifeBar component
            if (playerLife != null)
            {
                // Deal damage to the player
                playerLife.TakeDamage(damage);
            }
            
            // Destroy the bullet upon hitting the player
            Destroy(gameObject);
        }
        else{
            // Destroy the bullet upon hitting anything else
            Destroy(gameObject);
        }
        
    }
}
