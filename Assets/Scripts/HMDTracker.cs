using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class HandTracker : MonoBehaviour
{
    public GameObject leftHand;
    public GameObject rightHand;
    private TextWriter tw;
    private string fileName;
    private WaitForSeconds freq = new WaitForSeconds(0.05f); // 30 fps

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
        while (true)
        {
            float leftHandX = leftHand.transform.position.x;
            float leftHandY = leftHand.transform.position.y;
            float leftHandZ = leftHand.transform.position.z;

            float rightHandX = rightHand.transform.position.x;
            float rightHandY = rightHand.transform.position.y;
            float rightHandZ = rightHand.transform.position.z;

            string dataPoint = DateTime.Now + ";" + leftHandX + ";" + leftHandY + ";" + leftHandZ + ";" + rightHandX + ";" + rightHandY + ";" + rightHandZ;
            tw = new StreamWriter(fileName, true);
            tw.WriteLine(dataPoint);
            tw.Close();
            yield return freq;
        }
    }
}