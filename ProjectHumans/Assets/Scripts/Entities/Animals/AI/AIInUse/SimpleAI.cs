using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;

public class SimpleAI: AI {
    MotorSystem thisMotor;
    static Vector3 blankPos = new Vector3(0, 0, 0);
    Transform transform;
    List <GameObject> inSight = new List<GameObject>();
    Vector3 randomPos = blankPos;
    Matrix <float> decidedActions;
    List<Action> goalList;
    int goalIndex;
    public float thirst = 10;
    bool facingTarget;
    public float rotatedAngle = 0.0f;
    Vector3 randomPosition = Vector3.negativeInfinity;

    public SimpleAI(Animal animal, Body body, DriveSystem drives, MotorSystem motor, SensorySystem senses, Phenotype traits):
    base(body, drives, motor, senses, traits) {
        thisMotor = motor;
        thisAnimal = animal;
        //InitGoalDict();
    }

    public override Matrix < float > ChooseAction() {
        transform = thisAnimal.GetGameObject().transform;
        decidedActions = Matrix < float > .Build.Dense(actionStates.Count(), 1);
        UpdateFOV(transform, 45, 10);
        //DecreaseThirst();
        return decidedActions;
    }

    public void ChooseGoal() {
        goalIndex = driveStateLabelList.Count;
    }
    void DecreaseThirst()
    {
        if (inSight.Count > 0)
        {
            rotatedAngle = 0;
            ReachAndGrab();
        }
        else
        {
            if (rotatedAngle <= 360)
            {
                decidedActions[4,0] = 1.0f;
                rotatedAngle += 10 * Time.deltaTime;
            }
            else
            {
                Explore();
            }
        }
    }
    void ReachAndGrab()
    {
        foreach (GameObject x in inSight)
        {

            if (IsReachable(x) || thisMotor.reached)
            {

                if (thisMotor.isCrouching)
                {
                    if (thisMotor.leftHand.childCount > 1)
                    {
                        thisMotor.Stand();
                        if (thirst > 9)
                        {
                            decidedActions[10, 0] = -1;
                            decidedActions[13, 0] = -0.5f;
                            decidedActions[14, 0] = 0.6f;
                            decidedActions[15, 0] = -0.3f;
                            decidedActions[6, 0] = -1;
                            thirst = 0;
                        }
                    }
                    else
                    {
                        decidedActions[10, 0] = -1;
                        decidedActions[13, 0] = -1f;
                        decidedActions[14, 0] = 0f;
                        decidedActions[15, 0] = -0.5f;
                    }
                }
                else
                {
                    decidedActions[0,0] = 1;
                }
            }
            else
            {
                if (x.CompareTag("Water"))
                {
                    FacePosition(x.transform.position);
                    if (facingTarget)
                    {
                        decidedActions[5, 0] = 1;
                    }
                }
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
                decidedActions[5, 0] = 1.0f;
            }
        }

    }
    public void Sleep()
    {
        decidedActions[2,0] = 1;
        decidedActions[7,0] = 1;
    }
    public void FacePosition(Vector3 targetPos)
    {
        if (!IsFacing(targetPos))
        {
            if (GetRelativePosition(targetPos) == -1)
            {
                decidedActions[4,0] = -1.0f;
            }
            else
            {
                decidedActions[4,0] = 1.0f;
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
        if (distance < 1f)
        {
            return true;
        }
        return false;
    }
    public Vector3 GenerateRandomPos()
    {
        float randomX = UnityEngine.Random.Range(-10, 10);
        float randomZ = UnityEngine.Random.Range(-10, 10);
        return new Vector3(transform.position.x + randomX, 0.75f, transform.position.z + randomZ);
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