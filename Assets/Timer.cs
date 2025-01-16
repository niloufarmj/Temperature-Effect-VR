using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public GameObject uiQuestions;

    

    public void timerEnded()
    {
        uiQuestions.SetActive(true);
        Debug.Log("TimerEnded");
    }
}
