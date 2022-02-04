using System;
using System.Collections;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using UnityEngine;
using System.Linq;
public class SimplePrimateMotorSystem : MotorSystem {
    PrimateBody body;
    Transform bodyTransform;
    public SimplePrimateMotorSystem(Animal animal) : base(animal) {
        body = (PrimateBody)(animal.GetBody());
        bodyTransform = body.GetGameObject().transform;
    }

    public override void TakeSteps()
    {
        float stepRange = states[stateIndexDict["take steps"]] * thisAnimal.GetPhenotype().GetTrait("max_step");
        bodyTransform.Translate(bodyTransform.forward * stepRange, Space.World);
        body.bpDict["armL"].targetRotation = Quaternion.Euler(Oscilate(-45f, 45f, stepRange * 1500), -50f, 0);
        body.bpDict["armR"].targetRotation = Quaternion.Euler(Oscilate(45f, -45f, stepRange * 1500), 50f, 0);
        body.bpDict["thighL"].targetRotation = Quaternion.Euler(Oscilate(15f, -15f, stepRange * 1500), 0, 0);
        body.bpDict["thighR"].targetRotation = Quaternion.Euler(Oscilate(-15f, 15f, stepRange * 1500), 0, 0);
    }

    public override void Rotate()
    {
        bodyTransform.Rotate(Vector3.up, states[stateIndexDict["rotate"]] * thisAnimal.GetPhenotype().GetTrait("max_rotation"), Space.Self);
    }
    public override void SitDown()
    {
        
        if (body.GetStateDict()["sitting"] != 1)
        {
            body.abdomen.transform.Translate(-Vector3.up * 0.5f * Time.deltaTime, Space.Self);
        }
        body.bpDict["thighL"].targetRotation = Quaternion.Euler(-120, 0, 0);
        body.bpDict["thighR"].targetRotation = Quaternion.Euler(-120, 0, 0);
        body.bpDict["shinL"].targetRotation = Quaternion.Euler(40, 0, 0);
        body.bpDict["shinR"].targetRotation = Quaternion.Euler(40, 0, 0);
    }
    public override void SitUp()
    {
        float angle = 0;
        if (bodyTransform.eulerAngles.x < 359f)
        {
            angle = 0.5f;
        }
        else
        {
            if (body.GetStateDict()["Sitting"] != 1)
            {
                body.abdomen.transform.Translate(-Vector3.up * 3f * Time.deltaTime, Space.Self);
            }
        }
        bodyTransform.Rotate(bodyTransform.right, angle, Space.Self);
        body.bpDict["thighL"].targetRotation = Quaternion.Euler(-120, 0, 0);
        body.bpDict["thighR"].targetRotation = Quaternion.Euler(-120, 0, 0);
        body.bpDict["shinL"].targetRotation = Quaternion.Euler(40, 0, 0);
        body.bpDict["shinR"].targetRotation = Quaternion.Euler(40, 0, 0);
    }
    public override void StandUp()
    {
        float angle = body.abdomen.transform.localEulerAngles.x;
        angle = (angle > 180) ? angle - 360 : angle;
        if (angle > 0)
        {
            body.abdomen.transform.Rotate(Vector3.right, -0.5f, Space.Self);
        }
        if (body.abdomen.transform.localPosition.y <= body.defaultBodyPosY + 0.01f)
        {
            body.abdomen.transform.Translate(Vector3.up * 0.5f * Time.deltaTime, Space.Self);
        }
        else
        {
            body.bpDict["thighL"].targetRotation = Quaternion.Euler(0, 0, 0);
            body.bpDict["thighR"].targetRotation = Quaternion.Euler(0, 0, 0);
            body.bpDict["shinL"].targetRotation = Quaternion.Euler(0, 0, 0);
            body.bpDict["shinR"].targetRotation = Quaternion.Euler(0, 0, 0);
            SetDefaultArmJoint();
        }
    }
    public override void Crouch()
    {
        float angle = body.abdomen.transform.localEulerAngles.x;
        angle = (angle > 180) ? angle - 360 : angle;
        if (angle < 45)
        {
            body.abdomen.transform.Rotate(Vector3.right, 0.5f, Space.Self);
        }
        if (body.abdomen.transform.position.y > 0.7)
        {
            body.abdomen.transform.Translate(Vector3.up * -0.005f, Space.World);
        }
        body.bpDict["thighL"].targetRotation = Quaternion.Euler(200, 0, 0);
        body.bpDict["thighR"].targetRotation = Quaternion.Euler(200, 0, 0);
        body.bpDict["shinL"].targetRotation = Quaternion.Euler(-200, 0, 0);
        body.bpDict["shinR"].targetRotation = Quaternion.Euler(-200, 0, 0);
        SetDefaultArmJoint();
    }
    public override void LayDown()
    {
        float angle = 0;
        if (bodyTransform.eulerAngles.x >= 0 && bodyTransform.eulerAngles.x <= 1)
        {
            angle = -0.5f;
        }

        if (bodyTransform.eulerAngles.x > 275)
        {
            angle = -0.5f;
        }
        bodyTransform.Rotate(bodyTransform.right, angle, Space.Self);
        body.bpDict["thighL"].targetRotation = Quaternion.Euler(0, 0, 0);
        body.bpDict["thighR"].targetRotation = Quaternion.Euler(0, 0, 0);
        body.bpDict["shinL"].targetRotation = Quaternion.Euler(0, 0, 0);
        body.bpDict["shinR"].targetRotation = Quaternion.Euler(0, 0, 0);
    }
    public override void PickUp()
    {
        int index = (int)states[stateIndexDict["index"]] * 10;

        if (states[stateIndexDict["pick up"]] == 1)
        {

            if (body.GetStateDict()["RHHolding"] == -1)
            {
                body.bpDict["armR"].targetRotation = Quaternion.Euler(90, 20, 10);
                LayerMask layermask = ~(1 << 9 | 1 << 8);
                int maxCollider = 20;
                Collider[] hitColliders = new Collider[maxCollider];
                int numColliders = Physics.OverlapSphereNonAlloc(body.bpDict["armR"].transform.position, 0.9f, hitColliders, layermask);
                
                if (hitColliders[index] != null)
                {
                    hitColliders[index].transform.GetComponent<Rigidbody>().isKinematic = true;
                    hitColliders[index].transform.GetComponent<Collider>().isTrigger = true;
                    hitColliders[index].transform.parent = body.bpDict["handR"].transform;
                    body.bpDict["handR"].transform.GetChild(0).localPosition = new Vector3(0.108f, -0.064f, 0.047f);
                    body.bpDict["handR"].transform.GetChild(0).localRotation = Quaternion.Euler(90, 0, 0);
                }
            }
        }
        if (states[stateIndexDict["pick up"]] == -1)
        {
            if (body.GetStateDict()["LHHolding"] == -1)
            {
                body.bpDict["armL"].targetRotation = Quaternion.Euler(90, -20, 10);
                LayerMask layermask = ~(1 << 9 | 1 << 8);
                int maxCollider = 20;
                Collider[] hitColliders = new Collider[maxCollider];
                int numColliders = Physics.OverlapSphereNonAlloc(body.bpDict["armL"].transform.position, 0.9f, hitColliders, layermask);
                if (hitColliders[index] != null)
                {
                    hitColliders[index].transform.GetComponent<Rigidbody>().isKinematic = true;
                    hitColliders[index].transform.GetComponent<Collider>().isTrigger = true;
                    hitColliders[index].transform.parent = body.bpDict["handL"].transform;
                    body.bpDict["handL"].transform.GetChild(0).localPosition = new Vector3(0.0f, -0.032f, -0.078f);
                    body.bpDict["handL"].transform.GetChild(0).localRotation = Quaternion.Euler(90, 0, 0);
                }

            }
        }
    }
    public override void Consume()
    {
        if(states[stateIndexDict["consume"]] == 1)
        {
            GameObject.Destroy(body.bpDict["handR"].transform.GetChild(0).gameObject, 3);
        }
        if (states[stateIndexDict["consume"]] == -1)
        {
            GameObject.Destroy(body.bpDict["handL"].transform.GetChild(0).gameObject, 3);
        }
    }
    public override void Look()
    {

    }
    public override void Rest()
    {

    }

    public override void Sleep()
    {

    }
    public override void Collapse()
    {

    }
    public override void Reset()
    {
    }
    float Oscilate(float startValue, float EndValue, float speed)
    {
        float oscilateRange = (EndValue - startValue) / 2;
        float oscilateOffset = oscilateRange + startValue;
        return oscilateOffset + Mathf.Sin(Time.time * speed) * oscilateRange;
    }
    public void SetDefaultArmJoint()
    {
        body.bpDict["armL"].targetRotation = Quaternion.Euler(0, -80, 0);
        body.bpDict["armR"].targetRotation = Quaternion.Euler(0, 80, 0);
    }
}