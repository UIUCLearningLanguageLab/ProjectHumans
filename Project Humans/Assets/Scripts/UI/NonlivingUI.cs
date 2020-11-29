using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NonlivingUI : MonoBehaviour
{
    GameObject selectedObject = null;
    GameObject panel;
    GameObject mainCam;
    public GameObject halo;
    public Text OGName;

    bool togglePanel = true;
    
    private void Start()
    {
        mainCam = GameObject.Find("Main Camera");
        panel = GameObject.Find("NonlivingPanel");
    }
    
    private void Update(){
        if (Input.GetMouseButtonDown(0)) {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if(Physics.Raycast(ray, out hit)) {
                halo.SetActive(true);
                Vector3 haloPos = new Vector3 (hit.transform.position.x, 0.0f, hit.transform.position.z);
                halo.transform.position = haloPos;

               ReceiveObject(hit.transform.gameObject); 
            }
        }
    }

    public void ReceiveObject(GameObject clicked) {
        if(clicked.tag == "Apple" || clicked.tag == "Water") {
            Debug.Log ("Got a thing!");
            selectedObject = clicked;

            selectedObject = World.GetObject(clicked.name).gameObject;
            DisplayInfo();
        }
    }

    public void TogglePanel() {
        togglePanel = !togglePanel;
        panel.SetActive(togglePanel);
    }

    public void PassObjectTransform() {
       Transform to_send =  selectedObject.transform;
       MainCamera.CenterObject(to_send);
    }


    public void DisplayInfo(){
        OGName.text = selectedObject.name;
    }

    public void ExitPanel() {
        halo.SetActive(false);
        panel.SetActive(false);
    }
}