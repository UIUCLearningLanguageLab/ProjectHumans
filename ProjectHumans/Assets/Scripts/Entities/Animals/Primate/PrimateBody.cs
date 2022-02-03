using System;
using System.Collections;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using UnityEngine;

public class PrimateBody : AnimalBody {

    public PrimateBody(Animal animal, Vector3 position) : base(animal, position) {
        thisAnimal = animal;
    }

    public override void InitGameObject(Vector3 pos) {
        string filePath;
        thisAnimal = (Animal) thisEntity;
        filePath = "Prefabs/SimpleHuman" + thisAnimal.GetSex();

        GameObject loadedPrefab = Resources.Load(filePath, typeof(GameObject)) as GameObject;
        
        this.gameObject = (GameObject.Instantiate(loadedPrefab, new Vector3(0,0,0), Quaternion.identity) as GameObject);
        this.gameObject.name = thisEntity.GetName();

        rigidbody = GetGameObject().GetComponent<Rigidbody>();
        globalPos = this.gameObject.transform;
    }

    public override void UpdateBodyStates()
    {
        CheckGrounding();
        CheckSitting();
        CheckLaying();
        CheckHandsHolding();
    }
    public override void CheckGrounding()
    {
        RaycastHit hit;
        if (Physics.Raycast(limbDict["footL"].transform.position, Vector3.down, out hit, footDisToGround + 0.1f))
        {
            if (hit.transform.tag == "Ground")
            {

                stateDict["Standing"] = 1;
            }
        }
        else if (Physics.Raycast(limbDict["footR"].transform.position, Vector3.down, out hit, footDisToGround + 0.1f))
        {
            if (hit.transform.tag == "Ground")
            {
                stateDict["Standing"] = 1;
            }
        }
        else
        {
            stateDict["Standing"] = -1;
        }
    }
    public override void CheckSitting() {
        if (Physics.Raycast(abdomen.transform.position, -abdomen.transform.up, bodyHeight / 2 + 0.1f))
        {
            SetState("sitting", 1f);
            bodyPosWhileSitting = abdomen.transform.transform.localPosition;
        }
        else
        {
            SetState("sitting", -1f);
        }
    }

    public override void CheckCrouching() 
    { 
    }
    public override void CheckHandsHolding()
    {
        if (limbDict["handR"].transform.childCount > 0)
        {
            stateDict["RHHolding"] = 1;
        }
        else
        {
            stateDict["RHHolding"] = -1;
        }
        if (limbDict["handL"].transform.childCount > 0)
        {
            stateDict["LHHolding"] = 1;
        }
        else
        {
            stateDict["LHHolding"] = -1;
        }
    }

    public override void CheckLaying() 
    {
        if (Physics.Raycast(abdomen.transform.position, -abdomen.transform.forward, bodyWidth / 2 + 0.1f))
        {
            stateDict["Laying"] = 1;
        }
        else
        {
            stateDict["Laying"] = -1;
        }
    }
}