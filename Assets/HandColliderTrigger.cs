using UnityEngine;

public class HandColliderTrigger : MonoBehaviour
{
    public EmissionController emissionController; // Assign the EmissionController script in the Inspector
    public Timer timer; // Reference to your Timer script
    private bool isEmissionEnded = false; // Track if emission has ended

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hand")) // Assuming the hand has a tag "Hand"
        {
            Debug.Log("Hand entered the heater collider.");
            emissionController.StartTimer();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Hand")) // Assuming the hand has a tag "Hand"
        {
            Debug.Log("Hand exited the heater collider.");
            emissionController.StopTimer();
            
            if (isEmissionEnded)
            {
                timer.timerEnded(); // Show questionnaire
            }
        }
    }

    public void EmissionEnded()
    {
        isEmissionEnded = true; // Set emission ended flag
    }
}
