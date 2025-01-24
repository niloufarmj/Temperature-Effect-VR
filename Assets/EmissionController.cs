using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmissionController : MonoBehaviour
{
    public Material materialRight; // Assign your material for the right heater in the Inspector
    public Material materialLeft; // Assign your material for the left heater in the Inspector
    public Color startColor = new Color(1.0f, 0.64f, 0.0f); // Orangish yellow
    public Color endColor = Color.red; // Red
    public float maxIntensity = 1.0f; // Maximum emission intensity
    public float duration = 180.0f; // Duration in seconds (3 minutes)
    public GameObject lightRight; // Light for the right heater
    public GameObject lightLeft; // Light for the left heater
    public HandColliderTrigger handColliderTriggerRight; // Reference to HandColliderTrigger script for right hand
    public HandColliderTrigger handColliderTriggerLeft; // Reference to HandColliderTrigger script for left hand
    public QuestionnaireManager questionnaireManager; // Reference to QuestionnaireManager script

    private float elapsedTime = 0.0f;
    private bool isTimerRunning = false;
    private bool isRightHandIn = false;
    private bool isLeftHandIn = false;

    void Update()
    {
        if (isTimerRunning)
        {
            elapsedTime += Time.deltaTime;

            if (elapsedTime <= duration)
            {
                // Calculate the emission color and intensity based on the elapsed time
                float t = elapsedTime / duration;
                Color currentColor = Color.Lerp(startColor, endColor, t);
                float currentIntensity = Mathf.Lerp(0.0f, maxIntensity, t);

                // Apply the emission color and intensity to the materials
                Color finalColor = currentColor * Mathf.LinearToGammaSpace(currentIntensity);
                int currentPhase = questionnaireManager.GetCurrentPhase();

                if (currentPhase == 1 && isRightHandIn)
                {
                    materialRight.SetColor("_EmissionColor", finalColor);
                    materialRight.EnableKeyword("_EMISSION");
                }
                else if (currentPhase == 2 && isLeftHandIn)
                {
                    materialLeft.SetColor("_EmissionColor", finalColor);
                    materialLeft.EnableKeyword("_EMISSION");
                }
                else if (currentPhase == 3)
                {
                    if (isRightHandIn)
                    {
                        materialRight.SetColor("_EmissionColor", finalColor);
                        materialRight.EnableKeyword("_EMISSION");
                    }
                    if (isLeftHandIn)
                    {
                        materialLeft.SetColor("_EmissionColor", finalColor);
                        materialLeft.EnableKeyword("_EMISSION");
                    }
                }

                if (lightRight != null)
                {
                    lightRight.SetActive(isRightHandIn); // Use SetActive instead of enabled for GameObject
                }
                if (lightLeft != null)
                {
                    lightLeft.SetActive(isLeftHandIn); // Use SetActive instead of enabled for GameObject
                }
            }
            else
            {
                // Turn off the emission after the entire duration has passed
                materialRight.SetColor("_EmissionColor", Color.black);
                materialRight.DisableKeyword("_EMISSION");
                materialLeft.SetColor("_EmissionColor", Color.black);
                materialLeft.DisableKeyword("_EMISSION");

                if (lightRight != null)
                {
                    lightRight.SetActive(false); // Turn off the light
                }
                if (lightLeft != null)
                {
                    lightLeft.SetActive(false); // Turn off the light
                }

                handColliderTriggerRight.EmissionEnded(); // Notify HandColliderTrigger for right hand
                handColliderTriggerLeft.EmissionEnded(); // Notify HandColliderTrigger for left hand
                isTimerRunning = false; // Stop the timer
            }
        }
    }

    public void StartTimer()
    {
        if (isRightHandIn && isLeftHandIn)
        {
            isTimerRunning = true;
            elapsedTime = 0.0f; // Reset the timer
            questionnaireManager.StartWaitingPhase(); // Notify QuestionnaireManager to start the waiting phase
        }
    }

    public void StopTimer()
    {
        isTimerRunning = false;
    }

    public void SetRightHandIn(bool isIn)
    {
        isRightHandIn = isIn;
        StartTimer();
    }

    public void SetLeftHandIn(bool isIn)
    {
        isLeftHandIn = isIn;
        StartTimer();
    }
}