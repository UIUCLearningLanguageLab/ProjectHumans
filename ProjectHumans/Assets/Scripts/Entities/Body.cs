using System;
using System.Collections;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using UnityEngine;

public class Body {

    protected Entity thisEntity;
    protected float height;
    protected float heightScale;
    protected float currentMass;

    public Rigidbody rigidbody;
    protected GameObject gameObject;

    public Nutrition freshStats;
    public Nutrition staleStats;

    protected Vector<float> states;
    protected List<string> stateLabelList;
    protected Dictionary<string, int> stateIndexDict;
    protected Dictionary<string, float> stateDict;
    
    public Vector<float> GetStates() { return states; }
    public float GetState(string place) { return stateDict[place]; }
    public List<string> GetStateLabels() { return stateLabelList; }
    public Dictionary<string, int> GetStateIndices() { return stateIndexDict; }
    public Dictionary<string, float> GetStateDict() { return stateDict; }
    
    public GameObject GetGameObject() { return gameObject; }
    public void SetGameObject(GameObject toSet) { this.gameObject = toSet; }

    public Transform globalPos;
    public float displacement;

    public Body(Entity passed, Vector3 position) {
        thisEntity = passed;
        displacement = thisEntity.GetPhenotype().GetTraitDict()["displacement"];
        InitHeight();
        InitGameObject(position);
        passed.SetGameObject(this.gameObject);
        InitNutrition();
    }

    public virtual void InitGameObject(Vector3 pos) {
        string filePath = "Prefabs/" + thisEntity.GetSpecies();
        GameObject loadedPrefab = Resources.Load(filePath, typeof(GameObject)) as GameObject;
        this.gameObject = (GameObject.Instantiate(loadedPrefab, pos, Quaternion.identity) as GameObject);
        this.gameObject.name = thisEntity.GetName();

        rigidbody = GetGameObject().GetComponent<Rigidbody>();
        globalPos = this.gameObject.transform;
        this.VerticalBump(displacement);
        this.InitMass();
        this.gameObject.SetActive(true);
    }
    
    public void InitHeight() {
        heightScale = thisEntity.GetPhenotype().GetTraitDict()["size"]; 
        height = thisEntity.GetPhenotype().GetTraitDict()["height"] * heightScale; 
    }

    public void InitNutrition() {
        freshStats = new Nutrition();
        staleStats = new Nutrition();

        float temp = thisEntity.GetPhenotype().GetTrait("muscle_composition") * 100f;
        freshStats.healthMod = temp;
        staleStats.healthMod = temp * -0.5f;

        temp = thisEntity.GetPhenotype().GetTrait("sugar_composition") * 100f;
        freshStats.hungerMod = temp;
        staleStats.hungerMod = temp * 0.5f;

        temp = thisEntity.GetPhenotype().GetTrait("fat_composition") * 100f;
        freshStats.staminaMod = temp;
        staleStats.staminaMod = temp * 0.5f;

        temp = thisEntity.GetPhenotype().GetTrait("water_composition") * 100f;
        freshStats.thirstMod = temp;
        staleStats.thirstMod = temp * 0.5f;

        freshStats.sleepMod = 0.0f;
        staleStats.sleepMod = 0.0f;
    }

    public void InitMass() {
        //currentMass = thisEntity.GetPhenotype().GetTrait("mass"); 
        //rigidbody.mass = currentMass;
    }

    public float GetHeight() { return height; }

    public Vector3 GetXZPosition() {
        return new Vector3(globalPos.position.x, 0f, globalPos.position.z);
    }

    public void VerticalBump(float height) {
        globalPos.position += new Vector3(0, height, 0);
    }

    public void TranslateBodyTo(Vector3 goalPos) {
        Debug.Log("Tried to translate body to " + goalPos);

        globalPos.position = goalPos;
    }

    public void RotateBodyTo(Quaternion target) {
        Debug.Log("Tried to rotate body to " + target);

        globalPos.localRotation = target;  
    }

    public struct Nutrition {
        public float healthMod;
        public float hungerMod;
        public float staminaMod;
        public float sleepMod;
        public float thirstMod;
    }
}