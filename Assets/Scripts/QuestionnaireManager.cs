using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System;
using System.Linq;

public class QuestionnaireManager : MonoBehaviour
{
    [Header("Thermal Comfort Data")]
    public GameObject comfortUI;
    public ToggleGroup tgComfort;
    private string comfortAnswer;
    private bool isComfortAnswered = false;

    [Header("IPQ Data")]
    public GameObject ipqUi;
    public ToggleGroup tgIPQ;
    public TextMeshProUGUI textAnchorPositive;
    public TextMeshProUGUI textAnchorNegative;
    public TextMeshProUGUI questionHeader;
    private IPQ_Question[] questions;
    private string[] ipqItemNames;
    public TextAsset csvFile;
    private string[] ipqAnswers;
    private int currentQuestion = 0;
    public bool isIPQAnswered = false;

    private TextWriter tw;
    private int scenecounter;
    private int envIndex;
    private string filePath;

    public ProgramManager programManager;

    void Start()
    {
        loadIPQ();
        setIPQQuestion();
        scenecounter = PlayerPrefs.GetInt("scene counter");
        int userId = PlayerPrefs.GetInt("pid");
        envIndex = PlayerPrefs.GetInt("s" + scenecounter);
        filePath = Application.dataPath + "/CSV-Data/" + userId + "_count" + scenecounter + "_env" + envIndex + "_ipq_comfort.csv";
    }

    void Update()
    {
        if (comfortUI.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.A)) SelectComfortAnswer(0);
            else if (Input.GetKeyDown(KeyCode.S)) SelectComfortAnswer(1);
            else if (Input.GetKeyDown(KeyCode.D)) SelectComfortAnswer(2);
            else if (Input.GetKeyDown(KeyCode.F)) SelectComfortAnswer(3);
            else if (Input.GetKeyDown(KeyCode.G)) SelectComfortAnswer(4);
        }
        else if (ipqUi.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.A)) SelectIPQAnswer(0);
            else if (Input.GetKeyDown(KeyCode.S)) SelectIPQAnswer(1);
            else if (Input.GetKeyDown(KeyCode.D)) SelectIPQAnswer(2);
            else if (Input.GetKeyDown(KeyCode.F)) SelectIPQAnswer(3);
            else if (Input.GetKeyDown(KeyCode.G)) SelectIPQAnswer(4);
        }
    }

    public void ShowQuestionnaire()
    {
        comfortUI.SetActive(true);
    }

    private void SelectComfortAnswer(int index)
    {
        Toggle[] toggles = tgComfort.GetComponentsInChildren<Toggle>();
        if (index >= 0 && index < toggles.Length)
        {
            toggles[index].isOn = true;
            comfortAnswer = toggles[index].name;
            checkComfortQ();
        }
    }

    private void SelectIPQAnswer(int index)
    {
        Toggle[] toggles = tgIPQ.GetComponentsInChildren<Toggle>();
        if (index >= 0 && index < toggles.Length)
        {
            toggles[index].isOn = true;
            ipqAnswers[currentQuestion] = toggles[index].name;
            checkIPQ();
        }
    }

    public void checkComfortQ()
    {
        Toggle toggle = tgComfort.ActiveToggles().First();
        if (toggle != null)
        {
            comfortAnswer = toggle.name;
            isComfortAnswered = true;
            comfortUI.SetActive(false);
            ipqUi.SetActive(true);
        }
    }

    public void checkIPQ()
    {
        Toggle toggle = tgIPQ.ActiveToggles().First();

        if (toggle != null)
        {
            ipqAnswers[currentQuestion] = toggle.name;

            if (currentQuestion < ipqAnswers.Length - 1)
            {
                currentQuestion += 1;
                resetIPQToggles();
                setIPQQuestion();
            }
            else
            {
                writeQuestionnaireCSV();
                isIPQAnswered = true;
                ipqUi.SetActive(false);
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
        string header = "phase; time;" + "comfort;" + string.Join(";", ipqItemNames);
        tw = new StreamWriter(filePath, true);
        tw.WriteLine(header);

        string answers = (int)programManager.GetCurrentPhase() + ";" + DateTime.Now + ";" + comfortAnswer + ";" +
                            string.Join(";", ipqAnswers);

        tw.WriteLine(answers);
        tw.Close();
        Debug.Log("csv should be written now");
        scenecounter += 1;
        PlayerPrefs.SetInt("scene counter", scenecounter);
    }

    public void ResetQuestionnaire()
    {
        currentQuestion = 0;
        isComfortAnswered = false;
        isIPQAnswered = false;
        comfortUI.SetActive(false);
        ipqUi.SetActive(false);
        resetIPQToggles();
        setIPQQuestion();
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