using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System;

public class ProgramManager : MonoBehaviour
{
    public EmissionController emissionController;
    public QuestionnaireManager questionnaireManager;
    public TextMeshProUGUI guideText;
    public Image timeFeedback;
    public GameObject timeFeedbackUI;
    public GameObject finishUI;

    private Phase currentPhase = Phase.RightOnLeftOff;
    private State currentState = State.Waiting;
    private float timer = 0f;
    private float phaseDuration = 10f;
    private float coolDownDuration = 40f;

    public GameObject leftControllerPrefab, rightControllerPrefab;

    public GameObject uiLine;

    void Start()
    {
        // OVRInput.SetControllerVibration(1, 1, OVRInput.Controller.LTouch);
        // OVRInput.SetControllerVibration(1, 1, OVRInput.Controller.RTouch);
        
        // Vector3 leftControllerPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch);
        // Quaternion leftControllerRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.LTouch);
        // Vector3 rightControllerPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
        // Quaternion rightControllerRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);

        // Debug.Log("Left Controller Position: " + leftControllerPosition);
        // Debug.Log("Right Controller Position: " + rightControllerPosition);

        // leftControllerPrefab.transform.position = leftControllerPosition;
        // leftControllerPrefab.transform.rotation = leftControllerRotation;

        // rightControllerPrefab.transform.position = rightControllerPosition;
        // rightControllerPrefab.transform.rotation = rightControllerRotation;

        // OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.LTouch);
        // OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);
        guideText.text = "Please put both hands inside the heaters.";
        
    }

    void Update()
    {
        switch (currentState)
        {
            case State.Waiting:
                HandleWaitingState();
                break;
            case State.CoolDown:
                HandleCoolDownState();
                break;
            case State.Questionnaire:
                HandleQuestionnaireState();
                
                break;
        }
    }

    private void HandleWaitingState()
    {
        uiLine.GetComponent<LineRenderer>().enabled = false;
        if (emissionController.AreBothHandsIn())
        {
            timer += Time.deltaTime;
            timeFeedback.fillAmount = timer / phaseDuration;
            guideText.text = "Do not take your hands out until the time passes.";

            if (timer >= phaseDuration)
            {
                timer = 0f;
                currentState = State.Questionnaire;
                guideText.text = "Take your hands out and pick up the controller.";
            }
        }
    }

    private void HandleCoolDownState()
    {
        timer += Time.deltaTime;
        timeFeedback.fillAmount = timer / coolDownDuration;

        uiLine.GetComponent<LineRenderer>().enabled = false;

        if (timer >= coolDownDuration)
        {
            timer = 0f;
            currentPhase++;
            if (currentPhase == Phase.Finished)
            {
                questionnaireManager.ResetQuestionnaire();
                finishUI.SetActive(true);
                timeFeedbackUI.SetActive(false);
            }
            else
            {
                guideText.text = "Please put both hands inside the heaters.";
                currentState = State.Waiting;
                questionnaireManager.ResetQuestionnaire();
            }
        }
    }

    private void HandleQuestionnaireState()
    {
        if (emissionController.AreBothHandsOut())
        {
            questionnaireManager.ShowQuestionnaire();
            uiLine.GetComponent<LineRenderer>().enabled = true;
            currentState = State.CoolDown;
        }
    }

    public Phase GetCurrentPhase()
    {
        return currentPhase;
    }

    public State GetCurrentState() {
        return currentState;
    }
}

public enum Phase
{
    RightOnLeftOff,
    RightOffLeftOn,
    RightOnLeftOn,
    Finished
}

public enum State
{
    Waiting,
    CoolDown,
    Questionnaire
}