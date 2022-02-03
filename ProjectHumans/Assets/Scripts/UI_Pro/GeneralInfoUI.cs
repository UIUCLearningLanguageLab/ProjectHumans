using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GeneralInfoUI : MonoBehaviour
{
    [SerializeField] Text health, thirst, hunger, fatigue, sleepiness;
    [SerializeField] UIController uiController;
    Entity selectedEntity;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.localScale == new Vector3(1,1,1))
        {
            selectedEntity = uiController.selectedEntity;
            health.text = ((Animal)selectedEntity).GetDriveSystem().GetStateDict()["health"].ToString("F2");
            thirst.text = ((Animal)selectedEntity).GetDriveSystem().GetStateDict()["thirst"].ToString("F2");
            hunger.text = ((Animal)selectedEntity).GetDriveSystem().GetStateDict()["hunger"].ToString("F2");
            fatigue.text = ((Animal)selectedEntity).GetDriveSystem().GetStateDict()["fatigue"].ToString("F2");
            sleepiness.text = ((Animal)selectedEntity).GetDriveSystem().GetStateDict()["sleepiness"].ToString("F2");
        }
    }
}
