using System;
using System.Collections;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using UnityEngine;
using System.Linq;
public class SimplePrimateMotorSystem : MotorSystem {
   
    Transform abdomenTrans;
    Transform bodyTrans;
    Transform leftEye;
    Transform rightEye;
    PrimateBody primateBody;
    Vector3 goalPos;
    int footUpdates = 0;
    bool firstRight = true; // change this at some point like handedness
    private float xMin = -1f, xMax = 1f;
    private float timeValue = 0.0f;
    ConfigurableJoint rightFemur;
    ConfigurableJoint leftFemur;
    ConfigurableJoint rightTibia;
    ConfigurableJoint leftTibia;
    ConfigurableJoint rightHumerus;
    ConfigurableJoint leftHumerus;
    ConfigurableJoint rightRadius;
    ConfigurableJoint leftRadius;
    
    public SimplePrimateMotorSystem(Animal animal) : base(animal) {
        Debug.Log("A simple ape was born!");
        bodyTrans = thisAnimal.GetGameObject().transform;
        abdomenTrans = thisBody.GetSkeleton("Abdomen").transform;
        leftEye = thisBody.head.transform.GetChild(0).GetChild(1);
        rightEye = thisBody.head.transform.GetChild(0).GetChild(0);
        primateBody = (PrimateBody)thisBody;
        rightFemur = thisBody.GetSkeleton("Femur_R").GetComponent<ConfigurableJoint>();
        leftFemur = thisBody.GetSkeleton("Femur_L").GetComponent<ConfigurableJoint>();
        rightTibia = thisBody.GetSkeleton("Tibia_R").GetComponent<ConfigurableJoint>();
        leftTibia = thisBody.GetSkeleton("Tibia_L").GetComponent<ConfigurableJoint>();
        rightHumerus = thisBody.GetSkeleton("Humerus_R").GetComponent<ConfigurableJoint>();
        leftHumerus = thisBody.GetSkeleton("Humerus_L").GetComponent<ConfigurableJoint>();
        rightRadius = thisBody.GetSkeleton("Radius_R").GetComponent<ConfigurableJoint>();
        leftRadius = thisBody.GetSkeleton("Radius_L").GetComponent<ConfigurableJoint>();
        rightHand = thisBody.GetSkeleton("Hand_R").transform;
        leftHand = thisBody.GetSkeleton("Hand_L").transform;
    }
    public override void Consume() {
        CheckActionLegality();
        Debug.Log("consuming");
        if(stateDict["consume"] == 1)
        {
            rightRadius.targetRotation = new Quaternion(3, 0, 0, 1);
            skeletonInUse.Add(rightRadius.name);
        }
        else if(stateDict["consume"] == -1)
        {
            leftRadius.targetRotation = new Quaternion(-3, 0, 0, 1);
            skeletonInUse.Add(leftRadius.name);
        }
    }
    public override void TakeSteps()
    {
        CheckActionLegality();
        //Debug.Log("taking steps");
        float stepRange = states[stateIndexDict["take steps"]] * thisAnimal.GetPhenotype().GetTrait("max_step");
        bodyTrans.Translate(bodyTrans.forward * stepRange * Time.deltaTime, Space.World);
        skeletonInUse.Add(rightFemur.name);
        skeletonInUse.Add(leftFemur.name);
        skeletonInUse.Add(rightTibia.name);
        skeletonInUse.Add(leftTibia.name);
    }
    public override void Rotate()
    {
        CheckActionLegality();
        Debug.Log("rotatinig");
        float degree = states[stateIndexDict["rotate"]];
        float rotatingSpeed = degree * thisAnimal.GetPhenotype().GetTrait("max_rotation");
        bodyTrans.Rotate(0, rotatingSpeed * Time.deltaTime, 0, Space.Self);
    }
    public override void Crouch() {
        CheckActionLegality();
        Debug.Log("crouching");
        leftTibia.targetRotation = new Quaternion(-1, 0, 0, 1);
        skeletonInUse.Add(leftTibia.name);
        rightTibia.targetRotation = new Quaternion(-1, 0, 0, 1);
        skeletonInUse.Add(rightTibia.name);
        leftFemur.targetRotation = new Quaternion(0.5f, 0, 0, 1);
        skeletonInUse.Add(leftFemur.name);
        rightFemur.targetRotation = new Quaternion(0.5f, 0, 0, 1);
        skeletonInUse.Add(rightFemur.name);

        if (abdomenTrans.localPosition.y > -1.1)
        {
            abdomenTrans.Translate(-abdomenTrans.up * 2 * Time.deltaTime, Space.Self);
        }
        else
        {
            if (abdomenTrans.localRotation.x < 0.5)
            {
                abdomenTrans.Rotate(30 * Time.deltaTime, 0, 0);
            }
            else
            {
                isCrouching = true;
            }
        }
    }
    public override void Stand()
    {
        CheckActionLegality();
        Debug.Log("standing");
        if (abdomenTrans.localRotation.x > 0)
        {
            abdomenTrans.Rotate(-30 * Time.deltaTime, 0, 0);
        }
        leftTibia.targetRotation = new Quaternion(0, 0, 0, 1);
        skeletonInUse.Add(leftTibia.name);
        rightTibia.targetRotation = new Quaternion(0, 0, 0, 1);
        skeletonInUse.Add(rightTibia.name);
        leftFemur.targetRotation = new Quaternion(0, 0, 0, 1);
        skeletonInUse.Add(leftFemur.name);
        rightFemur.targetRotation = new Quaternion(0, 0, 0, 1);
        skeletonInUse.Add(rightFemur.name);
        if (abdomenTrans.localPosition.y < -0.2)
        {
            abdomenTrans.Translate(Vector3.up * 2 * Time.deltaTime, Space.Self);
        }
    }
    public override void UseHand()
    {
        CheckActionLegality();
        Debug.Log("using hand");
        float reach_x = states[stateIndexDict["RP x"]];
        float reach_y = states[stateIndexDict["RP y"]];
        float reach_z = states[stateIndexDict["RP z"]];
        LayerMask layermask = ~(1 << 8 | 1 << 9);
        if (states[stateIndexDict["use hands"]] == 1)
        {
            rightHumerus.targetRotation = new Quaternion(reach_x, reach_y, reach_z, 1);
            skeletonInUse.Add(rightHumerus.name);
            int maxColliders = 10;
            Collider[] hitColliders = new Collider[maxColliders];
            int numColliders = 0;//Physics.OverlapSphereNonAlloc(rightHand.position, 0.5f, hitColliders, layermask);
            if (numColliders > 0)
            {
                hitColliders[0].transform.GetComponent<Rigidbody>().isKinematic = true;
                hitColliders[0].transform.GetComponent<CapsuleCollider>().isTrigger = true;
                hitColliders[0].transform.parent = rightHand;
                rightHand.GetChild(0).localPosition = new Vector3(0, 0, 0);
                reached = true;
            }
        }
        if (states[stateIndexDict["use hands"]] == -1)
        {
            leftHumerus.targetRotation = new Quaternion(reach_x, reach_y, reach_z, 1);
            skeletonInUse.Add(leftHumerus.name);
            int maxColliders = 10;
            Collider[] hitColliders = new Collider[maxColliders];
            int numColliders = Physics.OverlapSphereNonAlloc(leftHand.position, 0.5f, hitColliders, layermask);
            if (numColliders > 0)
            {
                hitColliders[0].transform.GetComponent<Rigidbody>().isKinematic = true;
                hitColliders[0].transform.GetComponent<CapsuleCollider>().isTrigger = true;
                hitColliders[0].transform.parent = leftHand;
                leftHand.GetChild(1).localPosition = new Vector3(0, 0, 0);
                leftHand.GetChild(1).localRotation = Quaternion.Euler(283.640747f, 84.107254f, 138.337387f);
                reached = true;
            }
        }
    }
    public override void Lay() {
        CheckActionLegality();
        Debug.Log("laying");
        skeletonInUse.Add(rightFemur.name);
        skeletonInUse.Add(leftFemur.name);
        skeletonInUse.Add(rightTibia.name);
        skeletonInUse.Add(leftTibia.name);
        //abdomenTrans.localPosition = new Vector3(0, -0.84f, 0);
        if (abdomenTrans.localRotation.x >= -0.7)
        {
            abdomenTrans.Rotate(-30 * Time.deltaTime, 0, 0);
        }

        if (abdomenTrans.localPosition.y > -2.5)
        {
            abdomenTrans.Translate(thisAnimal.GetGameObject().transform.right * 1 * Time.deltaTime, Space.Self);
        }
    }
    public override void Look() {
        CheckActionLegality();
        Debug.Log("Looking");
    }
    public override void Rest() {
        Debug.Log("resting");
        Sit();
    }
    public override void Sit() {
        CheckActionLegality();
        Debug.Log("sitting");
        leftFemur.targetRotation = new Quaternion(1.0f, 0, 0, 1);
        skeletonInUse.Add(leftFemur.name);
        rightFemur.targetRotation = new Quaternion(1.0f, 0, 0, 1);
        skeletonInUse.Add(rightFemur.name);
        if (abdomenTrans.localPosition.y > -2.0)
        {
            abdomenTrans.Translate(-Vector3.up * 1 * Time.deltaTime, Space.Self);
        }
    }
    public override void Sleep() {
        CheckActionLegality();
        Debug.Log("sleeping");
        leftEye.localScale = new Vector3(0.4f, 0.0f, 0.4f);
        rightEye.localScale = new Vector3(0.4f, 0.0f, 0.4f);
    }
    public override void Collapse()
    {
        Debug.Log("collapsing");
        abdomenTrans.GetComponent<Rigidbody>().isKinematic = false;
    }
    public override void Reset()
    {
        Debug.Log("resetting");
        abdomenTrans.GetComponent<Rigidbody>().isKinematic = true;
        abdomenTrans.localPosition = new Vector3(0, -0.1f, 0);
        abdomenTrans.localRotation = new Quaternion(0, 0, 0, 1);
        foreach (GameObject x in thisAnimal.GetBody().GetSkeletonDict().Values)
        {
            if (x.GetComponent<ConfigurableJoint>() != null)
            {
                x.GetComponent<ConfigurableJoint>().targetRotation = new Quaternion(0,0,0,1);
            }
        }
    }
}