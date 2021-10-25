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
            "crouch",      // 0, negative is down 
            "sit",         // 1, negative is down
            "lay",         // 2 -1 or 1 (or 0 if not switched)
            "stand",       // 3 -1 or 1 (or 0 if not switched)
            "rotate",      // 4, now a proportion
            "take steps",  // 5, now a proportion
            "consume",     // 6, set to consumable if ongoing
            "sleep",       // 7, awake/maintain/fall asleep
            "rest",        // 8 -1 or 1 (or 0 if not switched)
            "look",        // 9
            "use hands",   // 10 -1 left / 1 right
            "look vertically",   // 11 -1 or 1 (or 0 if not switched)
            "look horizontally", // 12 -1 or 1 (or 0 if not switched)
            "RP x",        // 13  -1 to 1, proportion of max range from start pos
            "RP y",        // 14
            "RP z",        // 15
            
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
        skeletonInUse.Clear();
        //Debug.Log(illigalAction);
        TakeSteps();
        
        if (Input.GetKey(KeyCode.Return))
        {
            Lay();
            //states[stateIndexDict["use hands"]] = -1;
            //states[stateIndexDict["RP x"]] = -1f;
            //states[stateIndexDict["RP y"]] = 0f;
            //states[stateIndexDict["RP z"]] = -0.5f;
            //UseHand();
        }
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            Reset();
            //states[stateIndexDict["use hands"]] = -1;
            //states[stateIndexDict["RP x"]] = 0f;
            //states[stateIndexDict["RP y"]] = 0f;
            //states[stateIndexDict["RP z"]] = 0f;
            //UseHand();
        }
        //for (int i = 0; i < states.Count; i++)
        //{
        //    states[i] = actions[i];
        //}
        //if (states[stateIndexDict["take steps"]] != 0)
        //{
        //    TakeSteps();
        //}
        //if (states[stateIndexDict["rotate"]] != 0)
        //{
        //    Rotate();
        //}
        //if (states[stateIndexDict["crouch"]] != 0)
        //{
        //    Crouch();
        //}
        //if (states[stateIndexDict["sit"]] != 0)
        //{
        //    Sit();
        //}
        //if (states[stateIndexDict["lay"]] != 0)
        //{
        //    Lay();
        //}
        //if (states[stateIndexDict["stand"]] != 0)
        //{
        //    Stand();
        //}
        //if (states[stateIndexDict["consume"]] != 0)
        //{
        //    Consume();
        //}
        //if (states[stateIndexDict["sleep"]] != 0)
        //{
        //    Sleep();
        //}
        //if (states[stateIndexDict["rest"]] != 0)
        //{
        //    Rest();
        //}
        //if (states[stateIndexDict["look"]] != 0)
        //{
        //    Look();
        //}
        //if (states[stateIndexDict["use hands"]] != 0)
        //{
        //    UseHand();
        //}

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

        actionList.Add(Crouch);
        actionList.Add(Sit);
        actionList.Add(Lay);
        actionList.Add(Stand);
        actionList.Add(Rotate);
        actionList.Add(TakeSteps);
        actionList.Add(Consume);
        actionList.Add(Sleep);
        actionList.Add(Rest);
        actionList.Add(Look);
        actionList.Add(UseHand);
    }

    public abstract void Crouch();
    public abstract void Sit();
    public abstract void Lay();
    public abstract void Stand();
    public abstract void Rotate();
    public abstract void TakeSteps();
    public abstract void Consume();
    public abstract void Sleep();
    public abstract void Rest();
    public abstract void Look();
    public abstract void UseHand();
    public abstract void Collapse();

    public abstract void Reset();
}