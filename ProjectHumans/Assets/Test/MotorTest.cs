using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Random;
using MathNet.Numerics.Distributions;

public class MotorTest : MonoBehaviour
{
    [SerializeField] BodyTest body;
    private void Awake()
    {
    }

    private void FixedUpdate()
    {
        //TakeSteps(0f);
        //Look(new Quaternion(0, Oscilate(1, -1, 2), 0, 1));
        //Look(-1, -1, 0.5f);
        if (Input.GetKey(KeyCode.S))
        {
            SitDown();
        }
        if (Input.GetKey(KeyCode.W))
        {
            //StandFromSit();
            Lay();
        }
        //if(Input.GetKey(KeyCode.Q))
        //{
        //    SitUp();
        //}
        //if (Input.GetKey(KeyCode.C))
        //{
        //    Crouch(0.55f, 45);
        //}
        if (Input.GetKey(KeyCode.L))
        {
            Consume(-1);
        }
        if (Input.GetKey(KeyCode.R))
        {
            Consume(1);
        }
        //if (Input.GetKey(KeyCode.G))
        //{
        //    Grab(apple);
        //}

    }
    public void TakeSteps(float stepProportion)
    {
        float stepRange = stepProportion * 10;
        transform.Translate(transform.forward * stepRange * Time.deltaTime, Space.World);
        body.bpDict["armL"].targetRotation = Quaternion.Euler(Oscilate(-45f, 45f, 2), -50f, 0);
        body.bpDict["armR"].targetRotation = Quaternion.Euler(Oscilate(45f, -45f, 2), 50f, 0);
        body.bpDict["thighL"].targetRotation = Quaternion.Euler(Oscilate(30f, -30f, 2), 0, 0);
        body.bpDict["thighR"].targetRotation = Quaternion.Euler(Oscilate(-30f, 30f, 2), 0, 0);
    }
    public void Look(float x, float y, float z)
    {
        SetJointTargetRotation(body.bpDict["head"], x, y, z);
    }
    public void SitDown()
    {
        if (body.bodyStateDict["Sitting"] != 1)
        {
            body.body.transform.Translate(-Vector3.up * 0.5f * Time.deltaTime, Space.Self);
        }
        body.bpDict["thighL"].targetRotation = Quaternion.Euler(-120, 0, 0);
        body.bpDict["thighR"].targetRotation = Quaternion.Euler(-120, 0, 0);
        body.bpDict["shinL"].targetRotation = Quaternion.Euler(30, 0, 0);
        body.bpDict["shinR"].targetRotation = Quaternion.Euler(30, 0, 0);
    }
    public void StandFromSit()
    {
        if (body.body.localPosition.y <=  body.defaultBodyPosY + 0.01f)
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
    public void Lay()
    {
        Debug.Log(body.body.localEulerAngles.x);
        RotateMe(body.body, Vector3.left * 90, 10f);
        //else
        //{
        //    body.bpDict["thighL"].targetRotation = Quaternion.Euler(0, 0, 0);
        //    body.bpDict["thighR"].targetRotation = Quaternion.Euler(0, 0, 0);
        //    body.bpDict["shinL"].targetRotation = Quaternion.Euler(0, 0, 0);
        //    body.bpDict["shinR"].targetRotation = Quaternion.Euler(0, 0, 0);
        //}
        
    }
    //public void SitUp()
    //{
    //    if (bodyDict["Body"].localRotation.x < 0.0f)
    //    {
    //        bodyDict["Body"].Rotate(0.1f, 0, 0);
    //    }
    //    UseBodyPart("Thigh_L", new Quaternion(2, 0, 0, 1));
    //    UseBodyPart("Thigh_R", new Quaternion(2, 0, 0, 1));
    //    UseBodyPart("Calf_L", new Quaternion(-0.7f, 0, 0, 1));
    //    UseBodyPart("Calf_R", new Quaternion(-0.7f, 0, 0, 1));
    //}
    //public void Crouch(float height, float angle)
    //{
    //    if (bodyDict["Body"].localPosition.y > height)
    //    {
    //        bodyDict["Body"].Translate(-Vector3.up * 0.5f * Time.deltaTime, Space.Self);
    //    }
    //    else
    //    {
    //        if (bodyDict["Body"].localRotation.eulerAngles.x < angle)
    //        {
    //            bodyDict["Body"].Rotate(0.1f, 0, 0, Space.Self); ;
    //        }
    //    }
    //    float ratio = defaultBodyPosition.y - height;
    //    UseBodyPart("Thigh_L", new Quaternion(Mathf.Round(ratio * 3f * 10) / 10, 0, 0, 1));
    //    UseBodyPart("Thigh_R", new Quaternion(Mathf.Round(ratio * 3f * 10) / 10, 0, 0, 1));
    //    UseBodyPart("Calf_L", new Quaternion(Mathf.Round(ratio * -5f * 10) / 10, 0, 0, 1));
    //    UseBodyPart("Calf_R", new Quaternion(Mathf.Round(ratio * -5f * 10) / 10, 0, 0, 1));
    //}
    public void Consume(int hand)
    {
        if (hand == -1)
        {
            body.bpDict["foreArmL"].targetRotation = Quaternion.Euler(160, 0, 0);
            body.bpDict["armL"].targetRotation = Quaternion.Euler(45, -70, 0);
        }
        else
        {
            body.bpDict["foreArmR"].targetRotation = Quaternion.Euler(160, 0, 0);
            body.bpDict["armR"].targetRotation = Quaternion.Euler(40, 70, 0);
        }

    }
    //public void Grab(GameObject gameObject)
    //{
    //    //jointDict["Arm_R"].configuredInWorldSpace = true;
    //    Vector3 targetDirection = (gameObject.transform.position - bodyDict["Arm_R"].TransformPoint(jointDict["Arm_R"].anchor)).normalized;
    //    Vector3 armDirection = new Quaternion(0,0,0,1) * jointDict["Arm_R"].axis .normalized;
    //    Debug.DrawRay(bodyDict["Arm_R"].TransformPoint(jointDict["Arm_R"].anchor), targetDirection, Color.red);
    //    Debug.DrawRay(bodyDict["Arm_R"].TransformPoint(jointDict["Arm_R"].anchor), armDirection, Color.blue);
    //    Quaternion rotationDelta = Quaternion.FromToRotation(armDirection, bodyDict["Body"].InverseTransformDirection(targetDirection));
    //    Debug.Log(targetDirection);
    //    UseBodyPart("Arm_R", Quaternion.Inverse(rotationDelta));
    //    //jointDict["Arm_R"].SetTargetRotationLocal(Quaternion.Euler(rotationDelta.eulerAngles + bodyDict["Body"].localRotation.eulerAngles), new Quaternion(0, 0, 0, 1));
    //}
    float Oscilate(float startValue, float EndValue, float speed)
    {
        float oscilateRange = (EndValue - startValue) / 2;
        float oscilateOffset = oscilateRange + startValue;
        return oscilateOffset + Mathf.Sin(Time.time * speed) * oscilateRange;
    }
    public void SetJointTargetRotation(ConfigurableJoint joint, float x, float y, float z)
    {
        x = (x + 1f) * 0.5f;
        y = (y + 1f) * 0.5f;
        z = (z + 1f) * 0.5f;

        var xRot = Mathf.Lerp(joint.lowAngularXLimit.limit, joint.highAngularXLimit.limit, x);
        var yRot = Mathf.Lerp(-joint.angularYLimit.limit, joint.angularYLimit.limit, y);
        var zRot = Mathf.Lerp(-joint.angularZLimit.limit, joint.angularZLimit.limit, z);
        joint.targetRotation = Quaternion.Euler(xRot, yRot, zRot);
    }
    public void RotateMe(Transform transform, Vector3 byAngles, float inTime)
    {
        var fromAngle = transform.localRotation;
        var toAngle = Quaternion.Euler(transform.localEulerAngles + byAngles);
        for (var t = 0f; t < 1; t += Time.deltaTime / inTime)
        {
            transform.localRotation = Quaternion.Slerp(fromAngle, toAngle, t);
        }
        transform.localRotation = toAngle;
    }
}
