using UnityEngine;
using UnityEngine.InputSystem;

public class HandColliderTrigger : MonoBehaviour
{
    public EmissionController emissionController;

    private bool isRightHandPresent = false;
    private bool isLeftHandPresent = false;

    private bool isEmissionEnded = false; // Track if emission phase is finished

    private bool isTimerActive = false;

    void Update()
    {
        // Simulate right hand enter/exit using keyboard (for testing)
        if (Keyboard.current.tKey.wasPressedThisFrame) // Simulate right hand enter
        {
            Debug.Log("Right hand entered.");
            isRightHandPresent = true;
        }
        if (Keyboard.current.hKey.wasPressedThisFrame) // Simulate right hand exit
        {
            Debug.Log("Right hand exited.");
            isRightHandPresent = false;
        }

        // Simulate left hand enter/exit using keyboard (for testing)
        if (Keyboard.current.yKey.wasPressedThisFrame) // Simulate left hand enter
        {
            Debug.Log("Left hand entered.");
            isLeftHandPresent = true;
        }
        if (Keyboard.current.gKey.wasPressedThisFrame) // Simulate left hand exit
        {
            Debug.Log("Left hand exited.");
            isLeftHandPresent = false;
        }

        HandlePhase();
    }

    private void HandlePhase()
    {   
        if (emissionController.currentPhase == 0 && isRightHandPresent && !isTimerActive) // Phase 1: Right hand only
        {
            Debug.Log("Phase 0: Right hand detected. Starting timer.");
            emissionController.StartTimer();
            isTimerActive = true;
        }
        else if (emissionController.currentPhase == 1 && isLeftHandPresent && !isTimerActive) // Phase 2: Left hand only
        {
            Debug.Log("Phase 1: Left hand detected. Starting timer.");
            emissionController.StartTimer();
            isTimerActive = true;
        }
        else if (emissionController.currentPhase == 2 && isRightHandPresent && isLeftHandPresent && !isTimerActive) // Phase 3: Both hands
        {
            Debug.Log("Phase 2: Both hands detected. Starting timer.");
            emissionController.StartTimer();
            isTimerActive = true;
        }
        else if (!isRightHandPresent && !isLeftHandPresent && isTimerActive)
        {
            Debug.Log("No hands detected. Stopping timer.");
            emissionController.StopTimer();
            isTimerActive = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("RightHand")) // Assuming the hand has a tag "Hand"
        {
            Debug.Log("Right hand entered.");
            isRightHandPresent = true;
            // HandlePhase();
        }
        if (other.CompareTag("LeftHand")) // Assuming the hand has a tag "Hand"
        {
            Debug.Log("Left hand entered.");
            isLeftHandPresent = true;
            // HandlePhase();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("RightHand")) // Assuming the hand has a tag "Hand"
        {
            Debug.Log("Right hand exited.");
            isRightHandPresent = false;
            // HandlePhase();
        }
        if (other.CompareTag("LeftHand")) // Assuming the hand has a tag "Hand"
        {
            Debug.Log("Left hand exited.");
            isLeftHandPresent = false;
            // HandlePhase();
        }
    }

    // This method is called when the emission phase ends
    public void EmissionEnded()
    {
        isEmissionEnded = true; // Set emission end flag
    }
}
