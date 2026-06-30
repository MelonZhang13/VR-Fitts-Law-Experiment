using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NextTrialButton : MonoBehaviour
{
    public bool onClick = false;
    public Experiment experiment;

    public void NextTrialButtonOnClick()
    {
        if(experiment.experimentPause)
        {
            onClick = true;
        }
    }
}
