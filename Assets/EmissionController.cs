using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmissionController : MonoBehaviour
{
    public GameObject rightHeater;
    public GameObject leftHeater;
    public Material rightMaterial;
    public Material leftMaterial;

    public Color startColor = new Color(1.0f, 0.64f, 0.0f); // Orangish yellow
    public Color endColor = Color.red; // Red
    public float maxIntensity = 1.0f; // Maximum emission intensity
    public float phase0Duration = 60f; // Duration for Phase 0
    public float phase1Duration = 60f; // Duration for Phase 1
    public float phase2Duration = 60f; // Duration for Phase 2

    private float elapsedTime = 0.0f;
    private bool isTimerRunning = false;
    public int currentPhase = 0;

    public Timer timer;

    private bool emissionStarted = false;
    private bool emissionInProgress = false;

    private QuestionnaireManager questionnaireManager;
    public bool isQuestionnaireActive = false;

    void Start()
    {
        questionnaireManager = FindObjectOfType<QuestionnaireManager>();
        Debug.Log("EmissionController started. Waiting for phase triggers.");
    }

    void Update()
    {
        if (isTimerRunning && !isQuestionnaireActive)
        {
            elapsedTime += Time.deltaTime;
            Debug.Log($"Phase: {currentPhase}, Elapsed Time: {elapsedTime}");

            if (currentPhase == 0)
            {
                ControlHeater(rightHeater, rightMaterial);
            }
            else if (currentPhase == 1)
            {
                ControlHeater(leftHeater, leftMaterial);
            }
            else if (currentPhase == 2)
            {
                ControlHeater(rightHeater, rightMaterial);
                ControlHeater(leftHeater, leftMaterial);
            }

            if (CheckPhaseCompletion())
            {
                Debug.Log($"Phase {currentPhase} completed. Showing questionnaire.");
                ShowQuestionnaire();
            }
        }
    }

    private bool CheckPhaseCompletion()
    {
        float phaseDuration = GetPhaseDuration(currentPhase);
        return elapsedTime >= phaseDuration;
    }

    private float GetPhaseDuration(int phase)
    {
        if (phase == 0) return phase0Duration;
        if (phase == 1) return phase1Duration;
        if (phase == 2) return phase2Duration;
        return 0;
    }

    public void StartTimer()
    {
        isTimerRunning = true;
        elapsedTime = 0.0f; // Reset time on start
        emissionStarted = false; // Reset emission flag
        emissionInProgress = false;
        Debug.Log("Timer started. Elapsed time reset to 0.");
    }

    public void StopTimer()
    {
        isTimerRunning = false;
        elapsedTime = 0.0f; // Reset elapsed time
        Debug.Log("Timer stopped. Final elapsed time: " + elapsedTime);
    }

    public void ShowQuestionnaire()
    {
        isQuestionnaireActive = true;
        isTimerRunning = false;
        timer.timerEnded();
        Debug.Log("Questionnaire!");
    }

    private void AdvancePhase()
    {
        elapsedTime = 0.0f;
        currentPhase++;
        emissionStarted = false;
        emissionInProgress = false;
        isQuestionnaireActive = false;

        if (currentPhase > 2)
        {
            Debug.Log("All phases completed. Emission experiment finished.");
            StopTimer();
        }
        else
        {
            Debug.Log($"Starting Phase {currentPhase}.");
            StartTimer();
        }
    }

    private void ControlHeater(GameObject heater, Material heaterMaterial)
    {
        if (heater == null || heaterMaterial == null) return;

        // Turn on the emission in the first second of the phase
        if (!emissionStarted && elapsedTime <= 1.0f)
        {
            emissionStarted = true;
            emissionInProgress = true;
            heaterMaterial.SetColor("_EmissionColor", startColor * Mathf.LinearToGammaSpace(maxIntensity)); // Instant emission start
            heaterMaterial.EnableKeyword("_EMISSION");
            heater.SetActive(true); // Ensure the heater is active during the phase
        }
        
        // Once emission is on, keep it constant for the rest of the phase
        if (emissionInProgress && elapsedTime > 1.0f)
        {
            // The emission stays constant
            heaterMaterial.SetColor("_EmissionColor", startColor * Mathf.LinearToGammaSpace(maxIntensity));
        }
    }

    private void ResetEmission()
    {
        rightMaterial.SetColor("_EmissionColor", Color.black);
        leftMaterial.SetColor("_EmissionColor", Color.black);
        rightHeater.SetActive(false);
        leftHeater.SetActive(false);
    }
}
