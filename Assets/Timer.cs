using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Timer : MonoBehaviour
{
    public float targetTime = 60f;
    public GameObject uiQuestions;
    
    void Update()
    {
        targetTime -= Time.deltaTime; 
        
        if (targetTime <= 0.0f)
        {
            timerEnded();
        }       
    }

    

    void timerEnded()
    {
        uiQuestions.SetActive(true);
        Debug.Log("TimerEnded");
    }
}
