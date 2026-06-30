using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ExperimentLoader : MonoBehaviour
{
    [Header("Participant Information")]
    public TMP_InputField participantIDInput;
    public TMP_InputField participantNameInput;
    public TMP_Dropdown interactionMethodDropdown;

    public string practiceSceneName = "VR_ExperimentScene";
    public string controllerBasedRaySceneName = "VR_ExperimentScene";
    public string virtualHandRaySceneName = "VR_ExperimentScene";

    public void LoadDataAndStartExperiment()
    {
        // 1. Parse and store ParticipantID
        if (int.TryParse(participantIDInput.text, out int id))
        {
            ParticipantData.ParticipantID = id;
        }
        else
        {
            // Error handling if ID is not a valid number
            Debug.LogError("Error: Participant ID must be a number! Please check the input.");
            return;
        }

        // 2. Store ParticipantName
        string name = participantNameInput.text.Trim();
        if (!string.IsNullOrEmpty(name))
        {
            ParticipantData.ParticipantName = name;
        }
        else
        {
            // Error handling if the input field is left empty
            Debug.LogError("Error: Participant Name is empty! Please check the input.");
            return;
        }
        // 3. Get and store InteractionMethod from the Dropdown
        if (interactionMethodDropdown != null)
        {
            int index = interactionMethodDropdown.value; // Get the index of the selected option
            
            // Get the text string corresponding to that index
            string method = interactionMethodDropdown.options[index].text; 
            
            ParticipantData.InteractionMethod = method;
        }
        else
        {
            Debug.LogError("Error: Interaction Method Dropdown is not linked in the Inspector!");
            return;
        }

        // Log stored static data for debugging purposes
        Debug.Log("Data successfully loaded into ParticipantData:");
        Debug.Log($"ID: {ParticipantData.ParticipantID}, Name: {ParticipantData.ParticipantName}, Method: {ParticipantData.InteractionMethod}");

        // 4. Load the next scene to start the experiment
        string sceneToLoad = "";
        switch (interactionMethodDropdown.value)
        {
            case 0:
                sceneToLoad = practiceSceneName;
                break;
            case 1:
                sceneToLoad = controllerBasedRaySceneName;
                break;
            case 2:
                sceneToLoad = virtualHandRaySceneName;
                break;
        }

        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            // This should not happen if the switch statement is complete, but is a safety check.
            Debug.LogError("Scene name is empty. Cannot load scene.");
        }
    }
}
