using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using Oculus.Interaction;

public class Experiment : MonoBehaviour
{
    public GameObject canvas; // Standard canvas
    public GameObject instructions; // Experimental Instructions
    public SceneGeneration sceneGeneration;
    public Order orderSript;
    private float[] A; // Independent variable: Amplitude/Angle
    private float[] W; // Independent variable: Target width (W), i.e., the size of the buttons
    private int btnNum; // Number of buttons to click in a single trial

    public bool experimentBegin = false; //Status: Experiment officially started
    public bool experimentIng = false; // Status: Experiment in progress
    public bool experimentPause = false; // Status: Experiment on break
    public bool experimentEnd = false; // Status: Experiment ended

    public int No_A; // Index of the current A independent variable
    public int No_W; // Index of the current W independent variable
    public int No_Btn; // Index of the current target button click within the trial (how many buttons have been clicked)

    // public int[,] order;// Records the presentation order of each trial
    // Replaced int[,] order, using the balanced sequence returned by the Order script
    private Order.TrialCondition[] trialOrder;
    public int repetitions = 10; // **New: Number of repetitions for each balanced sequence**
    private int totalTrials;     // **New: Total number of trials (T * repetitions)**
    
    public int x_order; // Records the current trial number (zero-based index)

    public int targetNum; // The name of the current target button to click, e.g., Button X (zero-based index)
    public GameObject target; // The current target button object to click

    public string path;
    public List<string> str;

    private Color royalBlue = new Color(0.25f, 0.41f, 0.88f, 1.0f);

    private Color ErrorColor = new Color(1.0f, 0.31f, 0.0f, 1.0f);

    private int errorNumber;

    [SerializeField]
    public RayInteractor _rayInteractor;

    private string tableHeader;
    private string tableHeader_offset;
    private string path_offset;
    public List<string> str_offset;


    // Start is called before the first frame update
    void Start()
    {
        A = sceneGeneration.A;
        W = sceneGeneration.W;
        btnNum = sceneGeneration.btnNum;
        
        trialOrder = orderSript.currentTrialCondition;
        totalTrials = trialOrder.Length * repetitions;
        // Check if the sequence loaded successfully
        if (trialOrder == null || trialOrder.Length == 0)
        {
            // If Order.Start() hasn't run, manually call to generate it
            trialOrder = orderSript.GetParticipantOrder();
            if (trialOrder == null || trialOrder.Length == 0)
            {
                Debug.LogError("Trial order failed to load or generate.");
                return;
            }
        }

        path = $"/ExperimentData/{ParticipantData.ParticipantID}-{ParticipantData.InteractionMethod}-{ParticipantData.ParticipantName}";
        path = Application.dataPath + path + ".csv";
        tableHeader = "Participant ID,Participant Name,Interaction Method," +
                        "Trial No.,A,W,Trial Start Time," +
                        "Selection Time(Btn 1),Selection Time(Btn 2),Selection Time(Btn 3),Selection Time(Btn 4),Selection Time(Btn 5),Selection Time(Btn 6),Selection Time(Btn 7)," +
                        "Number of Error Selections";
        CSVs.Exist(path, tableHeader);
        str = new List<string>();

        path_offset = $"/ExperimentData/{ParticipantData.ParticipantID}-{ParticipantData.InteractionMethod}-Offset-{ParticipantData.ParticipantName}";
        path_offset = Application.dataPath + path_offset + ".csv";
        tableHeader_offset = "Participant ID,Participant Name,Interaction Method," +
                                "Trial No.,A,W,Trial Start Time," +
                                "Offset(Btn 1),Selection X(Btn 1),Selection Y(Btn 1),Selection Z(Btn 1),Target X(Btn 1),Target Y(Btn 1),Target Z(Btn 1)," +
                                "Offset(Btn 2),Selection X(Btn 2),Selection Y(Btn 2),Selection Z(Btn 2),Target X(Btn 2),Target Y(Btn 2),Target Z(Btn 2)," +
                                "Offset(Btn 3),Selection X(Btn 3),Selection Y(Btn 3),Selection Z(Btn 3),Target X(Btn 3),Target Y(Btn 3),Target Z(Btn 3)," +
                                "Offset(Btn 4),Selection X(Btn 4),Selection Y(Btn 4),Selection Z(Btn 4),Target X(Btn 4),Target Y(Btn 4),Target Z(Btn 4)," +
                                "Offset(Btn 5),Selection X(Btn 5),Selection Y(Btn 5),Selection Z(Btn 5),Target X(Btn 5),Target Y(Btn 5),Target Z(Btn 5)," +
                                "Offset(Btn 6),Selection X(Btn 6),Selection Y(Btn 6),Selection Z(Btn 6),Target X(Btn 6),Target Y(Btn 6),Target Z(Btn 6)," +
                                "Offset(Btn 7),Selection X(Btn 6),Selection Y(Btn 7),Selection Z(Btn 7),Target X(Btn 7),Target Y(Btn 7),Target Z(Btn 7)," +
                                "Number of Error Selections";
        CSVs.Exist(path_offset, tableHeader_offset);
        str_offset = new List<string>();
    }

    // Update is called once per frame
    void Update()
    {
        if (experimentBegin)
        {
            ExperimentBegin();
        }
        if (experimentIng)
        {
            ExperimentIng();
        }
        if (experimentPause)
        {
            ExperimentPause();
        }
        if (experimentEnd)
        {
            ExperimentEnd();
        }
    }

    /// <summary>
    /// Initializes the experiment scene when the formal experiment begins
    /// </summary>
    public void ExperimentBegin()
    {
        canvas.SetActive(true);
        // Hide the "Next Trial" button
        canvas.transform.Find("NextTrial").transform.SetAsLastSibling();
        canvas.transform.Find("NextTrial").gameObject.SetActive(false);
        // Hide the standard button used as a template
        canvas.transform.Find("Button").transform.SetAsLastSibling();
        canvas.transform.Find("Button").gameObject.SetActive(false);
        // Initialize the display status of the btnNum buttons in each trial
        for (int i = 0; i < A.Length; i++)
        {
            for (int j = 0; j < W.Length; j++)
            {
                for (int k = 0; k < btnNum; k++)
                {
                    canvas.transform.GetChild(i).GetChild(j).GetChild(k).gameObject.SetActive(false);
                }
            }
        }
        // Update status
        experimentPause = true;
        experimentBegin = false;

        x_order = 0; // Initialize to the 1st trial (index 0)
        Order.TrialCondition current = trialOrder[x_order % trialOrder.Length];
        // No_A = order[x_order, 0];// Initialize the A index corresponding to the 1st trial
        // No_W = order[x_order, 1];// Initialize the W index corresponding to the 1st trial
        No_A = current.A_Index; 
        No_W = current.W_Index;
        No_Btn = 0; // Initialize: 0 buttons clicked in the 1st trial
        targetNum = 0; // Initialize: The starting button for the 1st trial is Button 0 (zero-based index)
        errorNumber = 0;
    }

    /// <summary>
    /// Formal experiment process (Trial in progress)
    /// </summary>
    public void ExperimentIng()
    {
        // Display the current experiment scene
        for (int i = 0; i < A.Length; i++)
        {
            for (int j = 0; j < W.Length; j++)
            {
                if (i == No_A && j == No_W)
                {
                    for (int k = 0; k < btnNum; k++)
                    {
                        canvas.transform.GetChild(i).GetChild(j).GetChild(k).gameObject.SetActive(true);
                        if (k == targetNum)
                        {
                            canvas.transform.GetChild(i).GetChild(j).GetChild(k).gameObject.GetComponent<Image>().color = royalBlue;
                            target = canvas.transform.GetChild(i).GetChild(j).GetChild(k).gameObject;
                        }
                    }
                }
                else
                {
                    for (int k = 0; k < btnNum; k++)
                    {
                        canvas.transform.GetChild(i).GetChild(j).GetChild(k).gameObject.SetActive(false);
                    }
                }
            }
        }
    }


    /// <summary>
    /// Calculation formula: Calculate the next target to click
    /// </summary>
    public int NextButton(int curNum)
    {
        curNum++; // Convert from array index (e.g., Button 0) to button number (e.g., 1st button)
        curNum += btnNum / 2;
        if (curNum > btnNum)
        {
            curNum -= btnNum;
        }
        curNum--; // Convert from button number (e.g., 1st button) back to array index (e.g., Button 0)
        return curNum;
    }

    /// <summary>
    /// Experiment pause process: Pause the experiment after a single trial, allowing the participant to rest
    /// </summary>
    public void ExperimentPause()
    {
        canvas.transform.Find("NextTrial").gameObject.SetActive(true);
        // Current scene display
        for (int i = 0; i < A.Length; i++)
        {
            for (int j = 0; j < W.Length; j++)
            {
                for (int k = 0; k < btnNum; k++)
                {
                    canvas.transform.GetChild(i).GetChild(j).GetChild(k).gameObject.GetComponent<Button>().interactable = true;
                    canvas.transform.GetChild(i).GetChild(j).GetChild(k).gameObject.GetComponent<Image>().color = Color.white;
                    canvas.transform.GetChild(i).GetChild(j).GetChild(k).gameObject.SetActive(false);
                }
            }
        }
        // If the "Next Trial" button is clicked, prepare to start the next trial
        if (canvas.transform.Find("NextTrial").GetComponent<NextTrialButton>().onClick)
        {
            experimentIng = true;
            experimentPause = false;
            canvas.transform.Find("NextTrial").GetComponent<NextTrialButton>().onClick = false;
            canvas.transform.Find("NextTrial").gameObject.SetActive(false);
            // Record initial data at the start of a single trial
            str.Add(ParticipantData.ParticipantID.ToString());
            str.Add(ParticipantData.ParticipantName.ToString());
            str.Add(ParticipantData.InteractionMethod.ToString());
            str.Add((x_order + 1).ToString()); // Header: Current Trial
            str.Add(A[No_A].ToString()); // Header: A
            str.Add(W[No_W].ToString()); // Header: W
            str.Add(Time.time.ToString()); // Header: Trial Start Time

            // Record offset data at the start of a single trial
            str_offset.Add(ParticipantData.ParticipantID.ToString());
            str_offset.Add(ParticipantData.ParticipantName.ToString());
            str_offset.Add(ParticipantData.InteractionMethod.ToString());
            str_offset.Add((x_order + 1).ToString()); // Header: Current Trial
            str_offset.Add(A[No_A].ToString()); // Header: A
            str_offset.Add(W[No_W].ToString()); // Header: W
            str_offset.Add(Time.time.ToString()); // Header: Trial Start Time
        }
    }

    /// <summary>
    /// Experiment ends, stop the program
    /// </summary>
    public void ExperimentEnd()
    {
        for (int i = 0; i < A.Length; i++)
        {
            for (int j = 0; j < W.Length; j++)
            {
                for (int k = 0; k < btnNum; k++)
                {
                    canvas.transform.GetChild(i).GetChild(j).GetChild(k).gameObject.SetActive(false);
                }
            }
        }
        //Debug.Break();
        EditorApplication.isPlaying = false;
    }


    public void StartExperimentButtonClick(GameObject clickedButton)
    {
        clickedButton.SetActive(false);
        instructions.SetActive(false);
        experimentBegin = true;
    }
    
    
    public void HandleButtonClick(GameObject clickedButton)
    {
        if (!experimentIng) return;

        if (clickedButton == target)
        {
            str.Add(Time.time.ToString());

            target.GetComponent<Image>().color = Color.gray;

            if (ParticipantData.InteractionMethod != "0-Practice")
            {
                Vector3 worldHitPoint = _rayInteractor.CollisionInfo.Value.Point;
                Vector3 localHitPoint = sceneGeneration.cameraVR.transform.InverseTransformPoint(worldHitPoint);

                Vector3 targetWorldPosition = target.transform.position;
                Vector3 targetLocalPosition = sceneGeneration.cameraVR.transform.InverseTransformPoint(targetWorldPosition);

                float localDistance = Vector3.Distance(localHitPoint, targetLocalPosition);
            
                str_offset.Add(localDistance.ToString());

                str_offset.Add(localHitPoint.x.ToString());
                str_offset.Add(localHitPoint.y.ToString());
                str_offset.Add(localHitPoint.z.ToString());

                str_offset.Add(targetLocalPosition.x.ToString());
                str_offset.Add(targetLocalPosition.y.ToString());
                str_offset.Add(targetLocalPosition.z.ToString());
            }

            else
            {
                for (int i = 0; i < 7; i++) 
                {
                    str_offset.Add("NotVaild");
                }    
            }


                NextTargetLogic();
        }
        else
        {
            str.Add(Time.time.ToString());

            // clickedButton.GetComponent<Image>().color = ErrorColor;
            target.GetComponent<Image>().color = ErrorColor;
            errorNumber++;

            string errorText = "NotVaild";

            str_offset.Add(errorText);

            str_offset.Add(errorText);
            str_offset.Add(errorText);
            str_offset.Add(errorText);

            str_offset.Add(errorText);
            str_offset.Add(errorText);
            str_offset.Add(errorText);
            
            NextTargetLogic();
        }
    }


    /// <summary>
    /// Encapsulates the common logic for target switching and trial completion
    /// </summary>
    private void NextTargetLogic()
    {
        // Switch to the next button
        targetNum = NextButton(targetNum);
        No_Btn++;

        // Check if the trial is over
        if (No_Btn == btnNum) // Check if btnNum clicks have been reached (usually 9)
        {
            // Trial end logic (same as your original code)
            str.Add(errorNumber.ToString());
            CSVs.AddLineWithStringList(str, path);
            str.Clear();

            str_offset.Add(errorNumber.ToString());
            CSVs.AddLineWithStringList(str_offset, path_offset);
            str_offset.Clear();

            No_Btn = 0;
            targetNum = 0;
            x_order++;
            errorNumber = 0;

            if (x_order >= totalTrials)
            {
                experimentIng = false;
                experimentEnd = true;
            }
            else
            {
                Order.TrialCondition next = trialOrder[x_order % trialOrder.Length];
                // No_A = order[x_order % order.GetLength(0), 0];
                // No_W = order[x_order % order.GetLength(0), 1];
                No_A = next.A_Index;
                No_W = next.W_Index;
                experimentIng = false;
                experimentPause = true;
            }
        }
    }


}




