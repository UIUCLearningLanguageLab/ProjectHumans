using System;
using System.Collections;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using UnityEngine;

public class AnimalBody : Body {

    protected Animal thisAnimal;
    public float defaultBodyPosY;
    protected float footDisToGround;
    protected float bodyHeight;
    protected float bodyWidth;
    protected Transform eyeL;
    protected Transform eyeR;
    public Vector3 bodyPosWhileSitting;
    public GameObject abdomen;
    public GameObject head;
    protected Dictionary<string, GameObject> limbDict;
    public Dictionary<string, GameObject> GetLimbDict() { return limbDict; }

    public Dictionary<string, ConfigurableJoint> bpDict;
    public Dictionary<string, ConfigurableJoint> GetJointDict() { return bpDict; }

    public AnimalBody(Animal animal, Vector3 position) : base((Entity) animal, position) {
        thisAnimal = animal;
        stateLabelList = new List<string> {
            "standing", 
            "sitting",
            "crouching",
            "sleeping",
            "laying",
            "alive",
            "LHHolding",
            "RHHolding"
        };
        InitStates(stateLabelList);
        InitBodyDicts();
        PlaceBody(position);
        defaultBodyPosY = abdomen.transform.localPosition.y;
        eyeL = head.transform.Find("eyeL");
        eyeR = head.transform.Find("eyeR");
        footDisToGround = limbDict["footL"].GetComponent<Collider>().bounds.center.y;
        bodyHeight = abdomen.GetComponent<Collider>().bounds.size.y;
        bodyWidth = abdomen.GetComponent<Collider>().bounds.size.z;
    }

    public void InitBodyDicts() {
        limbDict = new Dictionary <string, GameObject>();
        bpDict = new Dictionary<string, ConfigurableJoint>();

        foreach (Transform child in globalPos) {
            if(child.name == "Body") {
                globalPos = child;
            }
        }

        foreach (Transform child in globalPos) {
            limbDict.Add(child.name, child.gameObject);
            if (child.TryGetComponent(out ConfigurableJoint configurable))
            {
                bpDict.Add(child.name, configurable);
                Debug.Log(child.name);
            }
        }
        abdomen = limbDict["body"];
        head = limbDict["head"];
    }

    public override void InitGameObject(Vector3 pos) {
        thisAnimal = (Animal) thisEntity;

        string bodyName = thisAnimal.GetSpecies() + thisAnimal.GetSex();
        string filePath = "Prefabs/" + bodyName + "Prefab";
        GameObject loadedPrefab = Resources.Load(filePath, typeof(GameObject)) as GameObject;
        
        this.gameObject = (GameObject.Instantiate(loadedPrefab, new Vector3(0,0.04f,0), Quaternion.identity) as GameObject);
        this.gameObject.name = thisEntity.GetName();

        rigidbody = GetGameObject().GetComponent<Rigidbody>();
        globalPos = this.gameObject.transform;
    }

    // Initializes state information but also calls standard height and holder info
    public void InitStates(List<string> passedList) {
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
        } else { Debug.Log("No body states passed to this animal"); }
    }


    public void PlaceBody(Vector3 position) {
        this.globalPos.position = position;
        this.gameObject.SetActive(true);
    }

    public void SetState(string label, float passed) {
        stateDict[label] = passed;
        int currentIndex = stateIndexDict[label];
        states[currentIndex] = passed;
    }

    public virtual void UpdateBodyStates() { Debug.Log("No update body states defined for this animal"); }

    public virtual void UpdateSkeletonStates() { Debug.Log("No update skeleton states defined for this animal"); }

    public virtual void CheckGrounding() { }

    public virtual void CheckSitting() { }

    public virtual void CheckCrouching() { }

    public virtual void CheckLaying() { }

    public virtual void CheckHandsHolding() { }

    public virtual void SleepAdjust() {
        
        float val = (thisAnimal.GetPhenotype().GetTrait("sleepiness_change") * 20);
        AdjustState("sleepiness", val);

        //Debug.Log("Snoozed a bit!");
    }

    public virtual void RestAdjust() {
        float val = (thisAnimal.GetPhenotype().GetTrait("fatigue_change") * 20);
        AdjustState("fatigue", val);

        //Debug.Log("Rested a bit!");
    }

    public void AdjustState(string label, float delta) {

        float val = thisAnimal.GetDriveSystem().GetState(label);
        val += delta;
        thisAnimal.GetDriveSystem().SetState(label, val);
    }
}


//public bool ConfirmRotation(string name, Vector3 target) {
//    if (skeletonDict.ContainsKey(name)) {
//        GameObject currentPart = skeletonDict[name];
//        Quaternion currentRotation = currentPart.transform.localRotation;

//        if (Math.Pow(currentRotation.x - target.x, 2) < 0.005 ) {
//            return true;
//        }
//    } 
//    return false;
//}


