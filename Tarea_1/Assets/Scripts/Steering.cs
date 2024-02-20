using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Steering : MonoBehaviour
{
    public float followSpeed = 15f;
    public float slowdownDistance = 1f;

    Vector3 velocity = Vector3.zero;

    
    void Update()
    {
        Vector3 targetPosition= Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.y));
        Vector3 playerDistance = targetPosition - transform.position;
        Vector3 desiredVelocity = playerDistance.normalized * followSpeed;
        Vector3 steering = desiredVelocity - velocity;

        velocity += steering * Time.deltaTime;
        float slowDownFactor = Mathf.Clamp01(playerDistance.magnitude / slowdownDistance);
        velocity *= slowDownFactor;

        transform.position += (Vector3)velocity * Time.deltaTime;
    }
}
