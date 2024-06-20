using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PointCounter pointCounter = FindObjectOfType<PointCounter>();
            if (pointCounter != null)
            {
                pointCounter.AddPoints(1);
            }
            Destroy(gameObject);
        }
    }
}

