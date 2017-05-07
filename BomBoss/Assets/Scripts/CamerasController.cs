using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CamerasController : MonoBehaviour {
    
    public GameObject[] cameras;
    public Text[] camerasUI;

    public float speedNormal = 1.0f;
    public float speedFast = 4.0f;

    public float mouseSensitivityX = 5.0f;
    public float mouseSensitivityY = 5.0f;

    private bool zoom;
    private bool sprint;

    private int selectedCamera;

    void Start()
    {
        zoom = false;
        sprint = false;

        selectedCamera = -1;

    }

    void Update()
    {

        GetCameras();

        if (selectedCamera == -1)
            return;

        float forward = Input.GetAxis("Vertical");
        float strafe = Input.GetAxis("Horizontal");
        //Debug.Log ("forward : " + forward + "  strafe : " + strafe);

        float run = forward * (Input.GetKey(KeyCode.LeftShift) ? speedFast : speedNormal);


        //zoom
        if (Input.GetKey(KeyCode.Mouse1))
        {
            zoom = true;
        }
        else
        {
            zoom = false;
        }


        float rotX = cameras[selectedCamera].transform.localEulerAngles.y + Input.GetAxis("Mouse X") * mouseSensitivityX;
        float roty = cameras[selectedCamera].transform.localEulerAngles.x - Input.GetAxis("Mouse Y") * mouseSensitivityX;

        cameras[selectedCamera].transform.localEulerAngles = new Vector3(roty, rotX, 0.0f);



        // move forwards/backwards
        if (forward != 0.0f)
        {
            float speed = Input.GetKey(KeyCode.LeftShift) ? speedFast : speedNormal;
            Vector3 trans = new Vector3(0.0f, 0.0f, forward * speed * Time.deltaTime);
            cameras[selectedCamera].transform.localPosition += cameras[selectedCamera].transform.localRotation * trans;
        }

        // strafe left/right
        if (strafe != 0.0f)
        {
            float speed = Input.GetKey(KeyCode.LeftShift) ? speedFast : speedNormal;
            Vector3 trans = new Vector3(strafe * speed * Time.deltaTime, 0.0f, 0.0f);
            cameras[selectedCamera].transform.localPosition += cameras[selectedCamera].transform.localRotation * trans;
        }
      }

    void GetCameras()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            selectedCamera++;
            if (selectedCamera >= cameras.Length)
            {
                selectedCamera = -1;
            }
            if(selectedCamera != -1)
            {
                camerasUI[selectedCamera].color = Color.red;
            }
            int prev = selectedCamera - 1;
            if(selectedCamera == -1)
            {
                camerasUI[cameras.Length - 1].color = Color.black;
            }
            else
            {
                if(prev>=0)
                 camerasUI[prev].color = Color.black;
            }
        }
    }
}
