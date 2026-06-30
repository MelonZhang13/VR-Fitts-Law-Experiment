using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneGeneration : MonoBehaviour
{
    public GameObject standardBtn; // Standard button
    public GameObject canvas; // Standard canvas
    public float[] A; // Independent variable: Angle/Amplitude
    public float[] W; // Independent variable: Target width (W), i.e., the size of the buttons
    public int btnNum; // Number of buttons to click in a single trial
    public float zDistance; // Set the interface distance from the user
    public float yDistance; // Set the interface deviation distance downward from the user
    public float colliderThickness = 0.001f; // Set the thickness of each button's BoxCollider to 0.001m, i.e., 1mm
    public GameObject cameraVR;


    // Start is called before the first frame update
    void Start()
    {
        InitButton();
    }

    // Keep the scene facing the participant at all times
    // Update is called once per frame
    void Update()
    {
        canvas.transform.position = cameraVR.transform.position + cameraVR.transform.rotation * Vector3.forward * zDistance + cameraVR.transform.rotation * Vector3.down * yDistance;
        canvas.transform.rotation = cameraVR.transform.rotation;
    }

    public void InitButton()
    {
        //buttonTransform = new Transform[A.Length][][];
        for (int i = 0; i < A.Length; i++)
        {
            GameObject Aobj = new GameObject("A=" + A[i] + "��");
            Aobj.transform.SetParent(canvas.transform, false);
            Aobj.transform.SetSiblingIndex(i);
            //buttonTransform[i] = new Transform[W.Length][];

            for (int j = 0; j < W.Length; j++)
            {
                GameObject Wobj = new GameObject("W=" + W[j]);
                Wobj.transform.SetParent(Aobj.transform, false);
                //buttonTransform[i][j] = new Transform[btnNum];

                for (int k = 0; k < btnNum; k++)
                {
                    GameObject btnObj = GameObject.Instantiate(standardBtn);
                    btnObj.name = "Button" + k;
                    btnObj.transform.SetParent(Wobj.transform, false);
                    BtnTransform(zDistance, A[i], W[j], k, btnObj);
                    //btnObj.GetComponent<BoxCollider>().size = new Vector3(btnObj.GetComponent<RectTransform>().sizeDelta.x, btnObj.GetComponent<RectTransform>().sizeDelta.y, 0.001f);
                }
            }
        }

        canvas.SetActive(false);
    }

    public GameObject BtnTransform(float zDistance,float _A, float _W, int k, GameObject btn)
    {
        Vector3 xyz;
        float angle = 90 - k * 360f / btnNum;
        xyz.x = (float)(Mathf.Sin(_A * Mathf.Deg2Rad) * Mathf.Cos(angle * Mathf.Deg2Rad)) * zDistance;
        xyz.y = (float)(Mathf.Sin(_A * Mathf.Deg2Rad) * Mathf.Sin(angle * Mathf.Deg2Rad)) * zDistance;
        xyz.z = (float)(Mathf.Cos(_A * Mathf.Deg2Rad)) * zDistance - zDistance;
        btn.transform.localPosition = xyz;
        btn.GetComponent<RectTransform>().sizeDelta = Vector2.one * Mathf.Sin(_W * Mathf.Deg2Rad) * zDistance;
        Vector3 abc;
        abc.x = -_A * Mathf.Sin(angle * Mathf.Deg2Rad);
        abc.y = _A * Mathf.Cos(angle * Mathf.Deg2Rad);
        abc.z = 0f;
        btn.transform.Rotate(abc, Space.World);
        return btn;
    }
}
