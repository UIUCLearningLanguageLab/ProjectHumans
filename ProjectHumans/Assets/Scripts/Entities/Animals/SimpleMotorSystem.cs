//using System;
//using System.Collections.Generic;
//using MathNet.Numerics.LinearAlgebra;
//using UnityEngine;

//public class SimpleMotorSystem : MotorSystem {
//    public SimpleMotorSystem(Animal passed) : base(passed) {}

//    void InitStates(List<string> passedList) {
//        states = Vector<float>.Build.Dense(passedList.Count);
//        stateLabelList = passedList;
//        stateIndexDict = new Dictionary<string, int>();
//        stateDict = new Dictionary<string, float>();

//        if (passedList != null){
//            for (int i = 0; i < passedList.Count; i++) {
//                states[i] = 0f;
//                stateIndexDict[passedList[i]] = i;
//                stateDict[passedList[i]] = 0f;
//            }
//        } else { Debug.Log("No actions passed to this animal"); }
//    }

//    void InitActionDict() {
//        actionList = new List<Action>();

//        actionList.Add(Crouch);
//        actionList.Add(Sit);
//        actionList.Add(Lay);
//        actionList.Add(Stand);
//        actionList.Add(Rotate);
//        actionList.Add(TakeSteps);
//        actionList.Add(Consume);
//        actionList.Add(Sleep);
//        actionList.Add(Rest);
//        actionList.Add(Look);
//        actionList.Add(UseHand);
//    }

//    public override void Crouch() {
//        Debug.Log("Called here.");
//    }
//    public override void Sit() {
//        Debug.Log("Called here.");
//    }
//    public override void Lay() {
//        Debug.Log("Called here.");
//    }
//    public override void Stand() {
//        Debug.Log("Called here.");
//    }
//    public override void Rotate() {
//        Debug.Log("Called here.");
//    }
//    public override void TakeSteps() {
//        Debug.Log("Called here.");
//    }
//    public override void Consume() {
//        Debug.Log("Called here.");
//    }
//    public override void Sleep() {
//        Debug.Log("Called here.");
//    }
//    public override void Rest() {
//        Debug.Log("Called here.");
//    }
//    public override void Look() {
//        Debug.Log("Called here.");
//    }
//    public override void UseHand() {
//        Debug.Log("Called here.");
//    }
//    public override void Collapse()
//    {
//        Debug.Log("Called here.");
//    }

//    public override void Reset()
//    {
//        Debug.Log("Called here.");
//    }
//}