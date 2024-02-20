using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Steering : MonoBehaviour
{
    public float followSpeed = 15f;
    public float slowdownDistance = 1f;
    public Transform targetObject; // Reference to the game object to follow

    Vector3 velocity = Vector3.zero;

    void Update()
    {
        if (targetObject != null) // Ensure target object is not null
        {
            Vector3 targetPosition = targetObject.position; // Use the position of the target object
            Vector3 playerDistance = targetPosition - transform.position;
            Vector3 desiredVelocity = playerDistance.normalized * followSpeed;
            Vector3 steering = desiredVelocity - velocity;

            velocity += steering * Time.deltaTime;
            float slowDownFactor = Mathf.Clamp01(playerDistance.magnitude / slowdownDistance);
            velocity *= slowDownFactor;

            transform.position += (Vector3)velocity * Time.deltaTime;
        }
    }
}
