using UnityEngine;

public class CRot : MonoBehaviour
{
    private float rotationSpeed = 15f; // Degrees per second
    private float rotationTimer = 5f; // Seconds

    private void Update()
    {
        // Rotate the cube around its up axis by the specified speed
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        // Decrease the timer
        rotationTimer -= Time.deltaTime;

        // Check if the timer has elapsed
        if (rotationTimer <= 0f)
        {
            // Reset the timer
            rotationTimer = 5f;

            // Rotate the cube by 15 degrees
            transform.Rotate(Vector3.up, 15f);
        }
    }
}
