using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField]
    World world;
    [SerializeField]
    Transform NetworkVisualizationPanel;
    [SerializeField]
    Transform GeneralInfoPanel;
    [SerializeField]
    NetworkVisualization networkVisualization;
    [SerializeField]
    Transform menu;
    public Entity selectedEntity;
    // Start is called before the first frame update
    void Start()
    {
        NetworkVisualizationPanel.transform.localScale = new Vector3(1, 0, 1);
        GeneralInfoPanel.transform.localScale = new Vector3(1, 0, 1);
    }

    // Update is called once per frame
    void Update()
    {
        SelectObject();
        ShowMenu();
    }
    void SelectObject()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Mouse is down");

            RaycastHit hitInfo = new RaycastHit();
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
            if (hit)
            {
                Debug.Log("Hit " + hitInfo.transform.gameObject.name);
                if (hitInfo.transform.parent.gameObject.tag == "Human")
                {
                    GeneralInfoPanel.transform.localScale = new Vector3(1, 1, 1);
                    //NetworkVisualizationPanel.GetComponent<Performance_Graph>().enabled = true;
                    selectedEntity = world.entityDict[hitInfo.transform.gameObject.transform.root.name];
                    networkVisualization.switchEntity = true;
                }
                else
                {
                    Debug.Log("nopz");
                }
            }
            else
            {
                Debug.Log("No hit");
            }
            Debug.Log("Mouse is down");
        }
    }
    void ShowMenu()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            menu.gameObject.SetActive(!menu.gameObject.activeSelf);
        }
    }
}
