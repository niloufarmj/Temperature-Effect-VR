using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public GameObject uiQuestions;
    public GameObject laserPointer;

    public void timerEnded()
    {
        uiQuestions.SetActive(true);
        Debug.Log("TimerEnded");

        // Toggle the LineRenderer component
        ToggleLineRenderer();
    }

    private void ToggleLineRenderer()
    {
        LineRenderer lineRenderer = laserPointer.GetComponent<LineRenderer>();
        if (lineRenderer != null)
        {
            lineRenderer.enabled = !lineRenderer.enabled;
        }
        else
        {
            Debug.LogError("LineRenderer component not found on laserPointer.");
        }
    }
}
