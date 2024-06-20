using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickeringLight : MonoBehaviour
{
    public Light flickeringLight;
    public float minIntensity = 0.1f;
    public float maxIntensity = 1.0f;
    public float flickerSpeed = 0.1f;

    private float targetIntensity;
    private float currentIntensity;

    void Start()
    {
        // Inicializar la intensidad actual y el objetivo de la intensidad
        currentIntensity = flickeringLight.intensity;
        targetIntensity = Random.Range(minIntensity, maxIntensity);

        // Empezar la corrutina para el parpadeo de la luz
        StartCoroutine(Flicker());
    }

    IEnumerator Flicker()
    {
        while (true)
        {
            // Cambiar gradualmente la intensidad actual hacia el objetivo
            currentIntensity = Mathf.MoveTowards(currentIntensity, targetIntensity, flickerSpeed * Time.deltaTime);
            flickeringLight.intensity = currentIntensity;

            // Si la intensidad actual alcanza el objetivo, establecer un nuevo objetivo aleatorio
            if (Mathf.Approximately(currentIntensity, targetIntensity))
            {
                targetIntensity = Random.Range(minIntensity, maxIntensity);
            }

            // Esperar un pequeño tiempo antes de actualizar de nuevo la intensidad
            yield return new WaitForSeconds(Random.Range(0.05f, 0.2f));
        }
    }
}
