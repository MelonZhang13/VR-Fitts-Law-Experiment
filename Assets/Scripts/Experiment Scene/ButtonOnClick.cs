using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonOnClick : MonoBehaviour
{
    public bool onClick = false;
    public Experiment experiment;
    public void TargetButtonOnClick()
    {
        onClick = true;
        if(experiment.target)
        {
            if (experiment.target == this.gameObject)
            {
                this.GetComponent<Button>().interactable = false;
                this.GetComponent<Image>().color = Color.black;
            }
            else onClick = false;
        }
    }
}
