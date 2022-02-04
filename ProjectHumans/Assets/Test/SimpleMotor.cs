using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMotor : MonoBehaviour
{
    float maxStepRange = 0.002f;
    float maxRotateRange = 180;
    [SerializeField] BodyTest body;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.C))
        {
            Crouch();
            //Crouch();
            //LayDown();
            //SitDown();
        }
        if (Input.GetKey(KeyCode.V))
        {
            //StandFromSit();
            PickUp(0, -1);
            //DropDown(-1);
            //SitUp();
            //StandFromSit();
        }
        if (Input.GetKey(KeyCode.S))
        {
            //StandFromSit();
            //DropDown(-1);
            //SitUp();
            StandFromSit();
        }

    }
    public void PickUp (int index, int hand)
    {
        if(hand == 1)
        {
            if (body.bodyStateDict["RHHolding"] == -1)
            {
                body.bpDict["armR"].targetRotation = Quaternion.Euler(90, 20, 10);
                //ConfigurableJointExtensions.SetTargetRotationLocal(body.bpDict["armR"], Quaternion.Euler(0, -150, 0), body.bpDict["armR"].transform.localRotation);
                LayerMask layermask = ~(1 << 9 | 1 << 8);
                int maxCollider = 20;
                Collider[] hitColliders = new Collider[maxCollider];
                int numColliders = Physics.OverlapSphereNonAlloc(body.bpDict["armR"].transform.position, 0.8f, hitColliders, layermask);
                if(hitColliders[index]!= null)
                {
                    hitColliders[index].transform.GetComponent<Rigidbody>().isKinematic = true;
                    hitColliders[index].transform.GetComponent<CapsuleCollider>().isTrigger = true;
                    hitColliders[index].transform.parent = body.bpDict["handR"].transform;
                    body.bpDict["handR"].transform.GetChild(0).localPosition = new Vector3(0.108f, -0.064f, 0);
                    body.bpDict["handR"].transform.GetChild(0).localRotation = Quaternion.Euler(90 ,0 ,0);
                }
            }
        }
        if(hand == -1)
        {
            if(body.bodyStateDict["LHHolding"] == -1)
            {
                body.bpDict["armL"].targetRotation = Quaternion.Euler(90, -20, 10);
                LayerMask layermask = ~(1 << 9 | 1 << 8);
                int maxCollider = 20;
                Collider[] hitColliders = new Collider[maxCollider];
                int numColliders = Physics.OverlapSphereNonAlloc(body.bpDict["armL"].transform.position, 0.8f, hitColliders, layermask);
                if (hitColliders[index] != null)
                {
                    hitColliders[index].transform.GetComponent<Rigidbody>().isKinematic = true;
                    hitColliders[index].transform.GetComponent<CapsuleCollider>().isTrigger = true;
                    hitColliders[index].transform.parent = body.bpDict["handL"].transform;
                    body.bpDict["handL"].transform.GetChild(0).localPosition = new Vector3(0, -0.12f, 0);
                }

            }
        }
    }
    public void DropDown(int hand)
    {
        if (hand == 1)
        {
            if (body.bodyStateDict["RHHolding"] == 1)
            {
                body.bpDict["handR"].transform.GetChild(0).GetComponent<Rigidbody>().isKinematic = true;
                body.bpDict["handR"].transform.GetChild(0).GetComponent<CapsuleCollider>().isTrigger = true;
                body.bpDict["handR"].transform.GetChild(0).parent = null;
            }
        }
        if (hand == -1)
        {
            if (body.bodyStateDict["LHHolding"] == 1)
            {
                body.bpDict["handL"].transform.GetChild(0).GetComponent<Rigidbody>().isKinematic = true;
                body.bpDict["handL"].transform.GetChild(0).GetComponent<CapsuleCollider>().isTrigger = true;
                body.bpDict["handL"].transform.GetChild(0).parent = null;
            }
        }
    }
    public void SetDefaultArmJoint()
    {
        body.bpDict["armL"].targetRotation = Quaternion.Euler(0, -80, 0);
        body.bpDict["armR"].targetRotation = Quaternion.Euler(0, 80, 0);
    }
    public void Crouch()
    {
        float angle = body.body.transform.localEulerAngles.x;
        angle = (angle > 180) ? angle - 360 : angle;
        if (angle < 45)
        {
            body.body.transform.Rotate(Vector3.right, 0.5f, Space.Self);
        }
            if (body.body.transform.position.y > 0.7)
            {
                body.body.transform.Translate(Vector3.up * -0.01f, Space.World);
            }

        body.bpDict["thighL"].targetRotation = Quaternion.Euler(200, 0, 0);
        body.bpDict["thighR"].targetRotation = Quaternion.Euler(200, 0, 0);
        body.bpDict["shinL"].targetRotation = Quaternion.Euler(-200, 0, 0);
        body.bpDict["shinR"].targetRotation = Quaternion.Euler(-200, 0, 0);
        SetDefaultArmJoint();
    }
    public void LayDown()
    {
        float angle = 0;
        if(transform.eulerAngles.x >= 0 && transform.eulerAngles.x <= 1)
        {
            angle = -0.5f;
        }

        if (transform.eulerAngles.x > 275)
        {
            angle = -0.5f;
        }
        transform.Rotate(transform.right, angle, Space.Self);
        body.bpDict["thighL"].targetRotation = Quaternion.Euler(0, 0, 0);
        body.bpDict["thighR"].targetRotation = Quaternion.Euler(0, 0, 0);
        body.bpDict["shinL"].targetRotation = Quaternion.Euler(0, 0, 0);
        body.bpDict["shinR"].targetRotation = Quaternion.Euler(0, 0, 0);
    }
    public void SitUp()
    {
        float angle = 0;
        if (transform.eulerAngles.x < 359f)
        {
            angle = 0.5f;
        }
        else
        {
            if (body.bodyStateDict["Sitting"] != 1)
            {
                body.body.transform.Translate(-Vector3.up * 3f * Time.deltaTime, Space.Self);
            }
        }
        transform.Rotate(transform.right, angle, Space.Self);
        body.bpDict["thighL"].targetRotation = Quaternion.Euler(-120, 0, 0);
        body.bpDict["thighR"].targetRotation = Quaternion.Euler(-120, 0, 0);
        body.bpDict["shinL"].targetRotation = Quaternion.Euler(40, 0, 0);
        body.bpDict["shinR"].targetRotation = Quaternion.Euler(40, 0, 0);
    }
    public void SitDown()
    {
        if (body.bodyStateDict["Sitting"] != 1)
        {
            body.body.transform.Translate(-Vector3.up * 0.5f * Time.deltaTime, Space.Self);
        }
        body.bpDict["thighL"].targetRotation = Quaternion.Euler(-120, 0, 0);
        body.bpDict["thighR"].targetRotation = Quaternion.Euler(-120, 0, 0);
        body.bpDict["shinL"].targetRotation = Quaternion.Euler(40, 0, 0);
        body.bpDict["shinR"].targetRotation = Quaternion.Euler(40, 0, 0);
    }
    public void StandFromSit()
    {
        float angle = body.body.transform.localEulerAngles.x;
        angle = (angle > 180) ? angle - 360 : angle;
        if (angle > 0)
        {
            body.body.transform.Rotate(Vector3.right, -0.5f, Space.Self);
        }
        if (body.body.localPosition.y <= body.defaultBodyPosY + 0.01f)
        {
            body.body.Translate(Vector3.up * 0.5f * Time.deltaTime, Space.Self);
        }
        else
        {
            body.bpDict["thighL"].targetRotation = Quaternion.Euler(0, 0, 0);
            body.bpDict["thighR"].targetRotation = Quaternion.Euler(0, 0, 0);
            body.bpDict["shinL"].targetRotation = Quaternion.Euler(0, 0, 0);
            body.bpDict["shinR"].targetRotation = Quaternion.Euler(0, 0, 0);
        }
    }
    void Rotate()
    {
        transform.Rotate(Vector3.up, 0.5f * maxRotateRange, Space.Self);
    }
    void takeSteps()
    {
        float stepRange = 0.5f * maxStepRange;
        transform.Translate(transform.forward * stepRange, Space.Self);
        body.bpDict["armL"].targetRotation = Quaternion.Euler(Oscilate(-45f, 45f, stepRange * 2000), -50f, 0);
        body.bpDict["armR"].targetRotation = Quaternion.Euler(Oscilate(45f, -45f, stepRange * 2000), 50f, 0);
        body.bpDict["thighL"].targetRotation = Quaternion.Euler(Oscilate(30f, -30f, stepRange * 2000), 0, 0);
        body.bpDict["thighR"].targetRotation = Quaternion.Euler(Oscilate(-30f, 30f, stepRange * 2000), 0, 0);
    }
    float Oscilate(float startValue, float EndValue, float speed)
    {
        float oscilateRange = (EndValue - startValue) / 2;
        float oscilateOffset = oscilateRange + startValue;
        return oscilateOffset + Mathf.Sin(Time.time * speed) * oscilateRange;
    }
}
