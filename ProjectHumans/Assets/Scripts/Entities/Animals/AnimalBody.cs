using System;
using System.Collections;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using UnityEngine;

public class AnimalBody : Body {

    protected Animal thisAnimal;
    public GameObject abdomen;
    public GameObject head;
    public GameObject mouth;
    public List<string> LegList;

    public float xMax, yMax, zMax;

    protected Dictionary<string, GameObject> limbDict;
    public Dictionary<string, GameObject> GetLimbDict() { return limbDict; }

    protected Dictionary<string, GameObject> skeletonDict;
    public Dictionary<string, GameObject> GetSkeletonDict() { return skeletonDict; }
    public GameObject GetSkeleton(string name) { return skeletonDict[name]; }

    protected Dictionary<string, ConfigurableJoint> jointDict;
    public Dictionary<string, ConfigurableJoint> GetJointDict() { return jointDict; }

    
    protected Dictionary<string, GameObject> holdings;
    public GameObject GetHeld(string holder) { return holdings[holder]; }
    public Dictionary<string, GameObject> GetHoldings() {return holdings;}

    public AnimalBody(Animal animal, Vector3 position) : base((Entity) animal, position) {
        thisAnimal = animal;
        stateLabelList = new List<string> {
            "standing", 
            "sitting",
            "crouching",
            "sleeping",
            "laying",
            "alive"
        };
        InitStates(stateLabelList);
        InitBodyDicts();
        InitHolders();
        PlaceBody(position);
    }

    public virtual void InitHolders() {
        holdings = new Dictionary<string, GameObject>();
        xMax = thisAnimal.GetPhenotype().GetTrait("default_max_reach_x");
        yMax = thisAnimal.GetPhenotype().GetTrait("default_max_reach_y");
        zMax = thisAnimal.GetPhenotype().GetTrait("default_max_reach_z");
    }

    public void InitBodyDicts() {
        limbDict = new Dictionary <string, GameObject>();
        skeletonDict = new Dictionary <string, GameObject>();
        jointDict = new Dictionary<string, ConfigurableJoint>();

        foreach (Transform child in globalPos) {
            if(child.name == "Body") {
                globalPos = child;
            }
        }

        foreach (Transform child in globalPos) {
            limbDict.Add(child.name, child.gameObject);
            foreach(Transform grandChild in child) {
                if(grandChild.name != "Eyes") {
                    skeletonDict.Add(grandChild.name, grandChild.gameObject);
                }
                if (grandChild.TryGetComponent(out ConfigurableJoint configurable)) {
                    jointDict.Add(grandChild.name, configurable);
                }
                foreach(Transform greatChild in grandChild) {
                    skeletonDict.Add(greatChild.name, greatChild.gameObject);
                }
            }  
        }
        abdomen = skeletonDict["Abdomen"];
        head = skeletonDict["Head"];
        //mouth = skeletonDict["Mouth"];
    }

    public override void InitGameObject(Vector3 pos) {
        thisAnimal = (Animal) thisEntity;

        string bodyName = thisAnimal.GetSpecies() + thisAnimal.GetSex();
        string filePath = "Prefabs/" + bodyName + "Prefab";
        GameObject loadedPrefab = Resources.Load(filePath, typeof(GameObject)) as GameObject;
        
        this.gameObject = (GameObject.Instantiate(loadedPrefab, new Vector3(0,0,0), Quaternion.identity) as GameObject);
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
                states[i] = 1f;
                stateIndexDict[passedList[i]] = i;
                stateDict[passedList[i]] = 1f;
            }
        } else { Debug.Log("No body states passed to this animal"); }
    }


    public void PlaceBody(Vector3 position) {
        this.globalPos.position = position;
        this.gameObject.SetActive(true);
    }

    public void SetState(string label, float passed) {
        //Debug.Log("Tried to pass " + passed + " to " + label);
        stateDict[label] = passed;
        int currentIndex = stateIndexDict[label];
        states[currentIndex] = passed;
    }

    public virtual void UpdateBodyStates() { Debug.Log("No update body states defined for this animal"); }

    public virtual void UpdateSkeletonStates() { Debug.Log("No update skeleton states defined for this animal"); }
    
    public virtual bool CheckSitting() { return false; }

    public virtual bool CheckCrouching() { return false; }

    public virtual bool CheckLaying() { return false; }

    public void RotateJointTo(string joint, Quaternion target) {
        if (this.jointDict.ContainsKey(joint)) {
            Quaternion initialRotation =  this.jointDict[joint].transform.localRotation;
            //this.jointDict[joint].SetTargetRotationLocal(target, initialRotation);
        }
    }

    public void RotateJointBy(string joint, Quaternion target) {
        if (this.jointDict.ContainsKey(joint)) {
            this.jointDict[joint].targetRotation = Quaternion.identity * (this.jointDict[joint].targetRotation * Quaternion.Inverse(target));
        }
    }

    public void SlerpTargetTo(string name, Vector3 target) {
        if (skeletonDict.ContainsKey(name)) {
            GameObject currentPart = skeletonDict[name];
            Transform partTrans = currentPart.transform;
            EnsureKinematic(name);
            partTrans.position = Vector3.Slerp(partTrans.position, target, Time.deltaTime);
        }
    }

    public void SlerpRotateTo(string name, Quaternion target, float rate) {
        if (skeletonDict.ContainsKey(name)) {
            GameObject currentPart = skeletonDict[name];
            Transform partTrans = currentPart.transform;
            EnsureKinematic(name);
            partTrans.rotation = Quaternion.Slerp(partTrans.rotation, target, Time.deltaTime * rate);
        }
    }

    public void TranslateSkeletonTo(string name, Vector3 goalPos) {
        //Debug.Log("Tried to translate " + name + " to " + goalPos);
        if (skeletonDict.ContainsKey(name)) {
            GameObject currentPart = skeletonDict[name];
            Vector3 currentPos = currentPart.transform.position;
            bool kinematicTemp = currentPart.GetComponent<Rigidbody>().isKinematic;

            if (Math.Pow(currentPos.y - goalPos.y, 2) < 0.005 ) { // something funky
                Debug.Log("Reached goal position");
                currentPart.GetComponent<Rigidbody>().isKinematic = kinematicTemp;
            } else {
                currentPart.GetComponent<Rigidbody>().useGravity = false;
                currentPart.GetComponent<Rigidbody>().isKinematic = true;
                currentPos = Vector3.MoveTowards(currentPos, goalPos, 10f * Time.deltaTime);
                //Debug.Log("Tried to translate " + name + " to " + goalPos);
                currentPart.GetComponent<Rigidbody>().isKinematic = kinematicTemp;
                currentPart.GetComponent<Rigidbody>().useGravity = true;
            }
        }
    }

    public bool ConfirmRotation(string name, Vector3 target) {
        if (skeletonDict.ContainsKey(name)) {
            GameObject currentPart = skeletonDict[name];
            Quaternion currentRotation = currentPart.transform.localRotation;

            if (Math.Pow(currentRotation.x - target.x, 2) < 0.005 ) {
                return true;
            }
        } 
        return false;
    }

    public void PrintSkelPos(string name) {
        if (skeletonDict.ContainsKey(name)) {
            GameObject currentPart = skeletonDict[name];
            Vector3 currentPos = currentPart.transform.position;

            Debug.Log("Current position of " + name + " is " + currentPos);
        }
    }

    public void AddHoldings(GameObject toAdd, string heldIndex) { 
        holdings[heldIndex] = toAdd;
    }

    public void DisableKinematic(string name) {
        if (name != null) {
            if (skeletonDict.ContainsKey(name)) {
                GameObject currentPart = skeletonDict[name];
                currentPart.GetComponent<Rigidbody>().isKinematic = false;
            }
        }
    }

    public void DisableGravity(string name) {
        if (name != null) {
            if (skeletonDict.ContainsKey(name)) {
                GameObject currentPart = skeletonDict[name];
                currentPart.GetComponent<Rigidbody>().useGravity = false;
            }
        }
    }

    public void EnsureKinematic(string name) {
        if (name != null) {
            if (skeletonDict.ContainsKey(name)) {
                GameObject currentPart = skeletonDict[name];
                currentPart.GetComponent<Rigidbody>().isKinematic = true;
            }
        }
    }

    public void LockRotation(string name) {
        if (name != null) {
            //Debug.Log(name);
            if (skeletonDict.ContainsKey(name)) {
                GameObject currentPart = skeletonDict[name];
                currentPart.GetComponent<Rigidbody>().freezeRotation = true;
            }
        }
    }

    public void FreeRotation(string name) {
        if (name != null) {
            if (skeletonDict.ContainsKey(name)) {
                GameObject currentPart = skeletonDict[name];
                currentPart.GetComponent<Rigidbody>().freezeRotation = false;
            }
        }
    }

    public void EnsureGravity(string name) {
        if (name != null) {
            if (skeletonDict.ContainsKey(name)) {
                GameObject currentPart = skeletonDict[name];
                currentPart.GetComponent<Rigidbody>().useGravity = true;
            }
        }
    }


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

    public virtual void EatObject(string holder) {
        GameObject toEat = holdings[holder];
        //Nutrition consumed = World.GetEntity(toEat.name).GetBody().freshStats;
        
        //health hunger stamina thirst sleep
        //AdjustState("health", consumed.healthMod);
        //AdjustState("hunger", consumed.hungerMod);
        //AdjustState("stamina", consumed.staminaMod);
        //AdjustState("thirst", consumed.thirstMod);
        //AdjustState("health", consumed.sleepMod);

        //World.RemoveEntity(toEat.name);
        holdings.Remove(holder);
    }

    public virtual void RemoveObject(string holder) {
        //World.DestroyComponent(holdings[holder].GetComponent<FixedJoint>());
        holdings.Remove(holder);
    }

    public Quaternion GetTargetRotation(string joint) {
        if (this.jointDict.ContainsKey(joint)) {
            return jointDict[joint].targetRotation;
        } else {
            return Quaternion.identity;
        }
    }

    public void Recolor(Color passedColor) {
        foreach(KeyValuePair<string, GameObject> skeleton in skeletonDict) {
            if (skeleton.Value.TryGetComponent(out MeshRenderer mesh)) {
                mesh.material.color = passedColor;
            }
        }
    }

    public void RecolorPart(Color passedColor, string part) {
        GameObject tobject = GetSkeleton(part);
        tobject.GetComponent<MeshRenderer>().material.color = passedColor;
    }
}

