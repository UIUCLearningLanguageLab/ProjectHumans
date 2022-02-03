using System;
using System.Linq;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using UnityEngine;

public abstract class MotorSystem 
{
    protected Animal thisAnimal;
    protected AnimalBody thisBody;
    protected List<Action> actionList;
    protected Vector<float> paramCopy;

    protected Vector<float> states;
    protected List<string> stateLabelList;
    protected Dictionary<string, int> stateIndexDict;
    protected Dictionary<string, float> stateDict;
    protected int numArgs;

    protected List<string> skeletonInUse = new List<string>();
    protected bool illigalAction;
    public Vector<float> GetStates() { return states; }
    public float GetState(string place) { return stateDict[place]; }
    public List<string> GetStateLabels() { return stateLabelList; }
    public Dictionary<string, int> GetStateIndices() { return stateIndexDict; }
    public Dictionary<string, float> GetStateDict() { return stateDict; }

    public bool isCrouching;
    public bool setAxis;
    public bool reached;
    public Transform rightHand;
    public Transform leftHand;

    public MotorSystem(Animal passed) {
        thisAnimal = passed;
        this.thisBody = thisAnimal.GetBody();

        stateLabelList = new List<string> {
            "take steps",   // 0
            "rotate",       // 1
            "sit down",     // 2
            "sit up",       // 3
            "stand up",     // 4
            "crouch",       // 5
            "lay down",     // 6
            "sleep",        // 7
            "rest",         // 8
            "look",         // 9
            "pick up",      // 10
            "consume",      // 11
            "index"
        };
        this.InitStates(stateLabelList);
        this.InitActionDict();
        this.numArgs = 4;
    }

    public void SetState(string label, float val) {
        stateDict[label] = val;
        int currentIndex = stateIndexDict[label];
        states[currentIndex] = val;
    }

    public void SetState(int index, float val) {
        string label  = stateLabelList[index];
        stateDict[label] = val;
        states[index] = val;
    }

    public void CheckActionLegality()
    {
        if (skeletonInUse.Any(o => o != skeletonInUse[0]))
        {
            illigalAction = true;
            Collapse();
        }
        else
        {
            illigalAction = false;
        }
    }

    public void TakeAction(Vector<float> actions) {
        for (int i = 0; i < states.Count; i++)
        {
            states[i] = actions[i];
        }
        if (states[stateIndexDict["take steps"]] != 0)
        {
            TakeSteps();
        }
        if (states[stateIndexDict["rotate"]] != 0)
        {
            Rotate();
        }
        if (states[stateIndexDict["crouch"]] != 0)
        {
            Crouch();
        }
        if (states[stateIndexDict["sit down"]] != 0)
        {
            SitDown();
        }
        if (states[stateIndexDict["sit up"]] != 0)
        {
           SitUp();
        }
        if (states[stateIndexDict["lay down"]] != 0)
        {
            LayDown();
        }
        if (states[stateIndexDict["stand up"]] != 0)
        {
            StandUp();
        }
        if (states[stateIndexDict["consume"]] != 0)
        {
            Consume();
        }
        if (states[stateIndexDict["sleep"]] != 0)
        {
            Sleep();
        }
        if (states[stateIndexDict["rest"]] != 0)
        {
            Rest();
        }
        if (states[stateIndexDict["look"]] != 0)
        {
            Look();
        }
        if (states[stateIndexDict["pick up"]] != 0)
        {
            PickUp();
        }

    }

    void InitStates(List<string> passedList) {
        states = Vector<float>.Build.Dense(passedList.Count);
        stateLabelList = passedList;
        stateIndexDict = new Dictionary<string, int>();
        stateDict = new Dictionary<string, float>();

        if (passedList != null){
            for (int i = 0; i < passedList.Count; i++) {
                states[i] = 0f;
                stateIndexDict[passedList[i]] = i;
                stateDict[passedList[i]] = 0f;
            }
        } else { Debug.Log("No actions passed to this animal"); }
    }

    void InitActionDict() {
        actionList = new List<Action>();

        actionList.Add(TakeSteps);
        actionList.Add(Rotate);
        actionList.Add(SitDown);
        actionList.Add(SitUp);
        actionList.Add(StandUp);
        actionList.Add(Crouch);
        actionList.Add(LayDown);
        actionList.Add(Sleep);
        actionList.Add(Rest);
        actionList.Add(Look);
        actionList.Add(PickUp);
        actionList.Add(Consume);

    }

    public abstract void TakeSteps();
    public abstract void Rotate();
    public abstract void SitDown();
    public abstract void SitUp();
    public abstract void StandUp();
    public abstract void Crouch();
    public abstract void LayDown();
    public abstract void Sleep();
    public abstract void Rest();
    public abstract void Look();
    public abstract void PickUp();
    public abstract void Consume();
    public abstract void Collapse();

    public abstract void Reset();
}