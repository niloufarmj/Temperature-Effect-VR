using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmissionController : MonoBehaviour
{
    public Material material; // Assign your material in the Inspector
    public Color startColor = new Color(1.0f, 0.64f, 0.0f); // Orangish yellow
    public Color endColor = Color.red; // Red
    public float maxIntensity = 1.0f; // Maximum emission intensity
    public float duration = 30.0f; // Duration in seconds
    public GameObject light; // Corrected typo
    public HandColliderTrigger handColliderTrigger; // Reference to HandColliderTrigger script

    private float elapsedTime = 0.0f;
    private bool isTimerRunning = false;

    void Update()
    {
        if (isTimerRunning)
        {
            elapsedTime += Time.deltaTime;

            if (elapsedTime <= duration * 0.75f)
            {
                // Calculate the emission color and intensity based on the elapsed time
                float t = elapsedTime / (duration * 0.75f);
                Color currentColor = Color.Lerp(startColor, endColor, t);
                float currentIntensity = Mathf.Lerp(0.0f, maxIntensity, t);

                // Apply the emission color and intensity to the material
                Color finalColor = currentColor * Mathf.LinearToGammaSpace(currentIntensity);
                material.SetColor("_EmissionColor", finalColor);
                material.EnableKeyword("_EMISSION");

                if (light != null)
                {
                    light.SetActive(false); // Use SetActive instead of enabled for GameObject
                }
            }
            else if (elapsedTime <= duration)
            {
                // Maintain the maximum emission color and intensity
                if (light != null)
                {
                    light.SetActive(true); // Use SetActive instead of enabled for GameObject
                }

                Color finalColor = endColor * Mathf.LinearToGammaSpace(maxIntensity);
                material.SetColor("_EmissionColor", finalColor);
                material.EnableKeyword("_EMISSION");
            }
            else
            {
                // Turn off the emission after the entire duration has passed
                material.SetColor("_EmissionColor", Color.black);
                material.DisableKeyword("_EMISSION");

                if (light != null)
                {
                    light.SetActive(false); // Turn off the light
                }

                handColliderTrigger.EmissionEnded(); // Notify HandColliderTrigger
                isTimerRunning = false; // Stop the timer
            }
        }
    }

    public void StartTimer()
    {
        isTimerRunning = true;
        elapsedTime = 0.0f; // Reset the timer
    }

    public void StopTimer()
    {
        isTimerRunning = false;
    }
}
