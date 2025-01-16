using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System;
using UnityEditor.SearchService;
using UnityEngine.SceneManagement;

public class QuestionnaireManager : MonoBehaviour
{
    // timer stuff
    private float timer = 0f;
    private float minWaitTime = 60f; // TODO 3 minutes TODO
    private bool isWaiting = true;

    // Comfort
    [Header("Thermal Comfort Data")]
    public GameObject comfortUI;
    public ToggleGroup tgComfort;
    private string comfortAnswer;
    private bool isComfortAnswered = false;

    // IPQ
    [Header("IPQ Data")]
    public GameObject ipqUi;
    public ToggleGroup tgIPQ;
    public TextMeshProUGUI textAnchorPositive;
    public TextMeshProUGUI textAnchorNegative;
    public TextMeshProUGUI questionHeader;
    //private string filePath = "Assets/Scripts/comfort_test.cs";
    private IPQ_Question[] questions;
    private string[] ipqItemNames;
    public TextAsset csvFile;
    private string[] ipqAnswers;
    private int currentQuestion = 0; 
    public bool isIPQAnswered = false; 
    //public GameObject wait_ui;

    // csv things
    private TextWriter tw;
    private int scenecounter;
    private int envIndex;
    private string filePath = Application.dataPath + "/CSV-Data/qr.csv";

    //wait things
    [Header("Waiting Data")]
    public Image timeFeedback;
    public GameObject timeFeedbackUI;
    public GameObject finishUI;




    // Start is called before the first frame update
    void Start()
    {
        loadIPQ();
        setIPQQuestion(); //it's the first one
        //does this work with active = false?
        scenecounter = PlayerPrefs.GetInt("scene counter");
        int userId = PlayerPrefs.GetInt("pid");
        envIndex = PlayerPrefs.GetInt("s"+scenecounter);
        filePath = Application.dataPath + "/CSV-Data/" + userId + "_count" + scenecounter + "_env" + envIndex + "_ipq_comfort.csv" ;

        Debug.Log("Scene Counter: " + PlayerPrefs.GetInt("scene counter"));
        Debug.Log("PID: " + PlayerPrefs.GetInt("pid"));
        Debug.Log("Environment Index: " + PlayerPrefs.GetInt("s" + scenecounter));


    }

    // Update is called once per frame
    void Update()
    {
        if (isWaiting)
        {
            if (timer <= minWaitTime)
            {
                timer += Time.deltaTime;
                if (isIPQAnswered) {
                    timeFeedback.fillAmount = timer / minWaitTime;
                }
            }
            else
            {
                isWaiting = false;
            }
        }
        else 
        {
            if (isIPQAnswered && isComfortAnswered)
            {
                if (scenecounter < 7) {
                    string scenePref = "s" + scenecounter;
                    int sceneIndex = PlayerPrefs.GetInt(scenePref);
                    //Debug.Log("Loading " + sceneIndex);
                    SceneManager.LoadScene(sceneIndex);
                }
                
            }
        }
    }

    public void checkComfortQ()
    {
        Toggle toggle = tgComfort.ActiveToggles().First();
        if (toggle != null)
        {
            comfortAnswer = toggle.name;
            // Debug.Log("Comfort: " + comfortAnswer);
            isComfortAnswered = true;
            comfortUI.SetActive(false);
            ipqUi.SetActive(true);
        }
        // else show input text pls

    }

    public void checkIPQ()
    {
        Toggle toggle = tgIPQ.ActiveToggles().First();

        if (toggle != null)
        {

            // save answers
            //Debug.Log("current question: " + currentQuestion);
            ipqAnswers[currentQuestion] = toggle.name;

            // Debug.Log("Current: " + currentQuestion + "answer: " + ipqAnswers[currentQuestion]);


            if (currentQuestion < ipqAnswers.Length - 1) // < 13
            {
                currentQuestion += 1;
                resetIPQToggles();
                setIPQQuestion();
            }
            else
            {
                Debug.Log("IPQ_done");
                // writecsv
                writeQuestionnaireCSV();
                isIPQAnswered = true;
                ipqUi.SetActive(false);
                // if (scenecounter == 7) {
                //     finishUI.gameObject.SetActive(true);
                // } else {
                //     timeFeedbackUI.SetActive(true);
                // }       
                // Directly activate finish screen
                finishUI.gameObject.SetActive(true);
            }
        }
    }

    private void loadIPQ()
    {

        
        string[] lines = csvFile.text.Split('\n');

        questions = new IPQ_Question[lines.Length];
        ipqItemNames = new string[lines.Length];
        ipqAnswers = new string[lines.Length];

        for (int i = 0; i < lines.Length; i++)
        {
            string[] values = lines[i].Split(';');

            if (values.Length == 4)
            {
                ipqItemNames[i] = values[0];
                IPQ_Question ipq_question = new IPQ_Question(values[1], values[2], values[3]);
                questions[i] = ipq_question;
                //questions[i] = new IPQ_Question(values[0], values[1], values[2]);
            }
        }


        if (csvFile == null)
        {
            Debug.LogError("CSV file is not assigned!");
            return;
        }

        if (lines.Length == 0)
        {
            Debug.LogError("CSV file is empty!");
        }

    }

    private void setIPQQuestion()
    {
        questionHeader.text = questions[currentQuestion].get_question();
        textAnchorPositive.text = questions[currentQuestion].get_positive_anchor();
        textAnchorNegative.text = questions[currentQuestion].get_negative_anchor();
    }

    private void resetIPQToggles()
    {
        Toggle[] toggles = tgIPQ.GetComponentsInChildren<Toggle>();

        foreach (Toggle toggle in toggles)
        {
            toggle.isOn = false;
        }
    }

    private void writeQuestionnaireCSV()
    {
        string header = "scene; time;" + "comfort;" + string.Join(";", ipqItemNames);
        tw = new StreamWriter(filePath, true);
        tw.WriteLine(header);

        string answers = envIndex + ";" + DateTime.Now + ";" + comfortAnswer + ";" +
                            string.Join(";", ipqAnswers);

        tw.WriteLine(answers);
        tw.Close();
        Debug.Log("csv should be written now");
        scenecounter += 1;
        PlayerPrefs.SetInt("scene counter", scenecounter);
        // Debug.Log(PlayerPrefs.GetInt("scene counter"));
    }


}


public class IPQ_Question
{
    private string question;
    private string anchor_negative;
    private string anchor_positive;

    public IPQ_Question(string question, string anchor_negative, string anchor_positive)
    {
        this.question = question;
        this.anchor_negative = anchor_negative;
        this.anchor_positive = anchor_positive;
    }

    public string get_question()
    {
        return question;
    }

    public string get_positive_anchor()
    {
        return anchor_positive;
    }

    public string get_negative_anchor()
    {
        return anchor_negative;
    }
}