using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;

public class SimpleAI: AI {
    static Vector3 blankPos = new Vector3(0, 0, 0);
    Transform transform;
    List <GameObject> inSight = new List<GameObject>();
    Vector3 randomPos = blankPos;
    Matrix <float> decidedActions;
    public float thirst = 10;
    bool facingTarget;
    public float rotatedAngle = 0.0f;
    Vector3 randomPosition = Vector3.negativeInfinity;
    bool grabed;
    string currentGoal = "decrease thirst";

    public SimpleAI(Animal animal, Body body, DriveSystem drives, MotorSystem motor, SensorySystem senses, Phenotype traits):
    base(body, drives, motor, senses, traits) {
        thisAnimal = animal;
        //InitGoalDict();
    }

    public override Matrix < float > ChooseAction() {
        transform = thisAnimal.GetGameObject().transform;
        decidedActions = Matrix < float > .Build.Dense(actionStates.Count(), 1);
        UpdateFOV(transform, 45, 10);
        DecreaseThirst();
        return decidedActions;
    }

    public void ChooseGoal() {
        
    }
    void DecreaseThirst()
    {
        if(thisAnimal.GetBody().GetStateDict()["RHHolding"] == 1)
        {
            if(thisAnimal.GetBody().GetLimbDict()["handR"].transform.childCount > 0)
            {
                if (thisAnimal.GetBody().GetLimbDict()["handR"].transform.GetChild(0).CompareTag("Water"))
                {
                    decidedActions[4, 0] = 1;
                    decidedActions[11, 0] = 1;
                }
            }
        }
        else if (thisAnimal.GetBody().GetStateDict()["LHHolding"] == 1)
        {
            if (thisAnimal.GetBody().GetLimbDict()["handL"].transform.GetChild(0).CompareTag("Water"))
            {
                decidedActions[4, 0] = 1;
                decidedActions[11, 0] = -1;
            }
        }
        else
        {
            List<GameObject> targetObjs = GetTargetObjs();
            if (targetObjs.Count > 0)
            {
                rotatedAngle = 0.0f;
                ReachAndGrab(GetClosestObj(targetObjs));
            }
            else
            {
                if (rotatedAngle <= 360)
                {
                    decidedActions[1, 0] = 0.01f;
                    rotatedAngle += 0.01f * 100;
                }
                else
                {
                    Explore();
                }
            }
        }
    }
    void ReachAndGrab(GameObject obj)
    {
        if (IsReachable(obj))
        {
            decidedActions[5, 0] = 1.0f;
            float[] handIndex = UseHand(obj);
            
            if(handIndex[0] > 0)
            {
                decidedActions[10, 0] = 1;
            }
            if(handIndex[0] < 0)
            {
                decidedActions[10, 0] = -1;
            }
            decidedActions[12, 0] = Mathf.Abs(handIndex[1]);
        }
        else
        {
            FacePosition(obj.transform.position);
            if (IsFacing(obj.transform.position))
            {
                decidedActions[0, 0] = 0.8f;
            }
        }
    }
    void Explore()
    {
        if (randomPosition.Equals(Vector3.negativeInfinity))
        {
            randomPosition = GenerateRandomPos();
        }
        if (Vector3.Distance(transform.position, randomPosition) < 1)
        {
            randomPosition = Vector3.negativeInfinity;
        }
        else
        {
            FacePosition(randomPosition);
            if (IsFacing(randomPosition))
            {
                decidedActions[0, 0] = 0.8f;
            }
        }

    }
    float[] UseHand(GameObject obj)
    {
        float[] handInfo = new float[] { 0, 0 };
        if(thisAnimal.GetBody().GetStateDict()["RHHolding"] != 1)
        {
            handInfo[0] = 1;
            LayerMask layermask = ~(1 << 9 | 1 << 8);
            int maxCollider = 20;
            Collider[] hitColliders = new Collider[maxCollider];
            Physics.OverlapSphereNonAlloc(thisAnimal.GetBody().bpDict["armR"].transform.position, 0.9f, hitColliders, layermask);
            handInfo[1] = Array.IndexOf(hitColliders, obj) / 10;
        }
        else if (thisAnimal.GetBody().GetStateDict()["LHHolding"] != 1)
        {
            handInfo[0] = -1;
            LayerMask layermask = ~(1 << 9 | 1 << 8);
            int maxCollider = 20;
            Collider[] hitColliders = new Collider[maxCollider];
            Physics.OverlapSphereNonAlloc(thisAnimal.GetBody().bpDict["armL"].transform.position, 0.9f, hitColliders, layermask);
            handInfo[1] = Array.IndexOf(hitColliders, obj) / 10;
        }
        return handInfo;
    }
    public void Sleep()
    {
        decidedActions[2,0] = 1;
        decidedActions[7,0] = 1;
    }

    List<GameObject> GetTargetObjs()
    {
        List<GameObject> targetObjects = new List<GameObject>();
        if (inSight.Count > 0)
        {
            foreach (GameObject obj in inSight)
            {
                if (obj.CompareTag("Water"))
                {
                    targetObjects.Add(obj);
                }
            }
        }
        return targetObjects;
    }
    GameObject GetClosestObj(List<GameObject> objs)
    {
        GameObject closestObj = null;
        float minDist = Mathf.Infinity;
        foreach(GameObject obj in objs)
        {
            float dist = Vector3.Distance(obj.transform.position, transform.position);
            if(dist < minDist)
            {
                closestObj = obj;
                minDist = dist;
            }
        }
        return closestObj;
    }
    public void FacePosition(Vector3 targetPos)
    {
        if (!IsFacing(targetPos))
        {
            if (GetRelativePosition(targetPos) == -1)
            {
                decidedActions[1,0] = -0.005f;
            }
            else
            {
                decidedActions[1,0] = 0.005f;
            }
        }
    }
    public int GetRelativePosition(Vector3 targetPos)
    {
        Vector3 relativePosition = transform.InverseTransformPoint(targetPos);
        if (relativePosition.x < 0)
        {
            return -1;
        }
        else if (relativePosition.x > 0)
        {
            return 1;
        }
        return 0;
    }
    public bool IsFacing(Vector3 targetPos)
    {
        float angle = Vector3.Angle(transform.forward, targetPos - transform.position);
        if (angle <= 13f)
        {
            facingTarget = true;
            return true;
        }
        facingTarget = false;
        return false;
    }
    public bool IsReachable(GameObject target)
    {
        float distance = Vector3.Distance(transform.position, target.transform.position);
        if (distance < 0.7f)
        {
            return true;
        }
        return false;
    }
    public bool IsTaggedObjInsight(string tag)
    {
        if (inSight.Count > 0)
        {
            foreach (GameObject obj in inSight)
            {
                if (obj.tag == "water")
                {
                    return true;
                }
            }
        }
        return false;
    }
    public Vector3 GenerateRandomPos()
    {
        float randomX = UnityEngine.Random.Range(-10, 10);
        float randomZ = UnityEngine.Random.Range(-10, 10);
        return new Vector3(transform.position.x + randomX, 0.04f, transform.position.z + randomZ);
    }
    public void UpdateFOV(Transform checkingObject, float maxAngle, float maxRadius)
    {
        LayerMask layermask = ~(1 << 9 | 1 << 8);
        Collider[] overlaps = new Collider[60];
        int count = Physics.OverlapSphereNonAlloc(checkingObject.position, maxRadius, overlaps, layermask);
        inSight.Clear();
        for (int i = 0; i < count + 1; i++)
        {
            if (overlaps[i] != null)
            {
                Vector3 directionBetween = (overlaps[i].transform.position - checkingObject.position).normalized;
                directionBetween.y *= 0;
                float angle = Vector3.Angle(checkingObject.forward, directionBetween);
                if (angle <= maxAngle)
                {
                    inSight.Add(overlaps[i].gameObject);

                }
            }
        }
    }
}