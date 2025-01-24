
using UnityEngine;

public class HandColliderTrigger : MonoBehaviour
{
    public EmissionController emissionController; // Assign the EmissionController script in the Inspector
    public Timer timer; // Reference to your Timer script
    private bool isEmissionEnded = false; // Track if emission has ended

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("RightHand")) // Assuming the right hand has a tag "RightHand"
        {
            Debug.Log("Right hand entered the heater collider.");
            emissionController.SetRightHandIn(true);
        }
        else if (other.CompareTag("LeftHand")) // Assuming the left hand has a tag "LeftHand"
        {
            Debug.Log("Left hand entered the heater collider.");
            emissionController.SetLeftHandIn(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("RightHand")) // Assuming the right hand has a tag "RightHand"
        {
            Debug.Log("Right hand exited the heater collider.");
            emissionController.SetRightHandIn(false);
        }
        else if (other.CompareTag("LeftHand")) // Assuming the left hand has a tag "LeftHand"
        {
            Debug.Log("Left hand exited the heater collider.");
            emissionController.SetLeftHandIn(false);
        }

        if (isEmissionEnded)
        {
            timer.timerEnded(); // Show questionnaire
        }
    }

    public void EmissionEnded()
    {
        isEmissionEnded = true; // Set emission ended flag
    }
}