using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class HandTracker : MonoBehaviour
{
    public GameObject leftHand;
    public GameObject rightHand;

    public GameObject leftTouch;
    public GameObject rightTouch;

    private TextWriter tw;
    private string fileName;
    private WaitForSeconds freq = new WaitForSeconds(0.05f); // 30 fps

    public ProgramManager programManager;

    void Start()
    {
        int currentScene = PlayerPrefs.GetInt("scene counter"); // 1 - Feuer, 2 - Eis
        int userId = PlayerPrefs.GetInt("pid");
        int envIndex = PlayerPrefs.GetInt("s" + currentScene);
        fileName = Application.dataPath + "/CSV-Data/" + userId + "_count" + currentScene + "_env" + envIndex + "_hands.csv";

        // Ensure the directory exists
        Directory.CreateDirectory(Path.GetDirectoryName(fileName));

        tw = new StreamWriter(fileName, true);
        string header = "timestamp;leftHandX;leftHandY;leftHandZ;rightHandX;rightHandY;rightHandZ";
        tw.WriteLine(header);
        tw.Close();

        StartCoroutine(collectHandData());
    }

    private IEnumerator collectHandData()
    {
        float leftHandX;
        float leftHandY;
        float leftHandZ;

        float rightHandX;
        float rightHandY;
        float rightHandZ;
        while (true)
        {

            if (programManager.GetCurrentState() == State.Questionnaire) {
                leftHandX = leftHand.transform.position.x;
                leftHandY = leftHand.transform.position.y;
                leftHandZ = leftHand.transform.position.z;

                rightHandX = rightHand.transform.position.x;
                rightHandY = rightHand.transform.position.y;
                rightHandZ = rightHand.transform.position.z;
            } else {
                leftHandX = leftTouch.transform.position.x;
                leftHandY = leftTouch.transform.position.y;
                leftHandZ = leftTouch.transform.position.z;

                rightHandX = rightTouch.transform.position.x;
                rightHandY = rightTouch.transform.position.y;
                rightHandZ = rightTouch.transform.position.z;
            }
            

            string dataPoint = DateTime.Now + ";" + leftHandX + ";" + leftHandY + ";" + leftHandZ + ";" + rightHandX + ";" + rightHandY + ";" + rightHandZ;
            tw = new StreamWriter(fileName, true);
            tw.WriteLine(dataPoint);
            tw.Close();
            yield return freq;
        }
    }
}