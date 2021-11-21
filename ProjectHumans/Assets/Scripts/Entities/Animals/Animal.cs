using System.Collections.Generic;
using UnityEngine;
using System;
using MathNet.Numerics.LinearAlgebra;

public class Animal : Entity {

    public Camera visualInputCamera;
    public int cheatCommand;
    public float cheatArgs;
    public bool noCheats = true;

    private static AnimalBody animalBody;
    private static DriveSystem driveSystem;
    private static MotorSystem motorSystem;
    public static void SetMotorSystem(MotorSystem motor) { motorSystem = motor; }
    private static SensorySystem sensorySystem;
    private bool finishedUpdate = true;
    protected object activeAI;
    protected string action;

    public static List<int> timeList = new List<int>();
    public static List<string> timeEventList = new List<string>();

    public Animal(string objectType, string index, Genome motherGenome, Genome fatherGenome, Vector3 spawn) 
    : base (objectType, index, motherGenome, fatherGenome, spawn, true) {
        
        InitBodyControl(spawn);
        
        driveSystem = new DriveSystem(this);
        sensorySystem = new SensorySystem(this);
        InitBrain();
        
    }

    void InitBodyControl(Vector3 spawn) {
        if(species == "Human")
        {
            animalBody = new PrimateBody(this, spawn);
            motorSystem = new SimplePrimateMotorSystem(this);
        }
        else
        {
            animalBody = new AnimalBody(this, spawn);
            motorSystem = new SimpleMotorSystem(this);
        }
        body = animalBody;
        visualInputCamera = animalBody.GetGameObject().GetComponentInChildren<Camera>();
    }
    
    void InitBrain() {
        Type type = Type.GetType(World.aISelected);
        activeAI = Activator.CreateInstance(type, this, GetBody(), GetDriveSystem(), GetMotorSystem(), GetSensorySystem(), GetPhenotype());
    }

    public override void UpdateEntity() {
        GetDriveSystem().UpdateDrives();
        Matrix<float> visualInputMatrix = GetSensorySystem().GetVisualInput();
        Vector<float> temp = ((AI)activeAI).ChooseAction().Column(0);
        GetMotorSystem().TakeAction(temp);
        action = "In progress!";

        IncreaseAge(1);
    }
    public void ToggleBodyPart(string part, bool toggle) {
        GetBody().GetSkeletonDict()[part].gameObject.SetActive(toggle);
    }
    
    // getters and setters for body, drive system, motor system, sensory system, and action choice class
    public new AnimalBody  GetBody() { return animalBody; }

    public bool GetBodyState(string state) { return (GetBody().GetStateDict()[state] == 1f); }

    public DriveSystem GetDriveSystem() { return driveSystem; }

    public MotorSystem GetMotorSystem() { return motorSystem; }

    public SensorySystem GetSensorySystem() { return sensorySystem; }

    public AI GetAI() { return ((AI)activeAI); }

    public string GetAction() { return ((AI)activeAI).GetAction(); }

    public string GetSex() { 
        if(GetPhenotype().GetTrait("sex") == 1.0) {
            return "Male";
        } else { return "Female"; }
    }

    public void SetCommand(float command, float param) {
        //Debug.Log("Passed command B " + command + " with parameter of " + param);
        cheatCommand = (int) command;
        cheatArgs = param;
        noCheats = false;
    }


    public static void ResetEventTimes(){
        timeList.Clear();
        timeEventList.Clear();
    }
    
    public static void AddEventTime(string eventName) {
        int time = DateTime.Now.Millisecond + 1000*DateTime.Now.Second;

        timeList.Add(time);
        timeEventList.Add(eventName);
        //Debug.Log("Adding Event Time " + eventName + " " + timeList.Count + " " + timeEventList.Count);
    }

    public static void PrintEventTimes(){
        int numEvents = timeList.Count;
        
        if (numEvents > 3){
            Debug.Log(numEvents);
            string outputString = "";
            for (int i = 0; i < numEvents-1; i++){
                int timeSpan = timeList[i+1] - timeList[i];
                outputString += timeEventList[i+1] + ": " + timeSpan.ToString() + System.Environment.NewLine;
            }
            Debug.Log(outputString);
        }
    }
}