using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField]
    Transform NetworkVisualizationPanel;
    [SerializeField]
    NetworkVisualization networkVisualization;
    public Entity selectedEntity;
    // Start is called before the first frame update
    void Start()
    {
        NetworkVisualizationPanel.transform.localScale = new Vector3(1, 0, 1);
    }

    // Update is called once per frame
    void Update()
    {
        SelectObject();
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
                if (hitInfo.transform.gameObject.tag == "Human")
                {
                    NetworkVisualizationPanel.transform.localScale = new Vector3(1, 1, 1);
                    NetworkVisualizationPanel.GetComponent<Performance_Graph>().enabled = true;
                    selectedEntity = World.entityDict[hitInfo.transform.gameObject.transform.root.name];
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
}
