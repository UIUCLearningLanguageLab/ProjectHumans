using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyTest : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform body;
    public float defaultBodyPosY;
    Transform eyeL;
    Transform eyeR;
    ConfigurableJoint armR;
    ConfigurableJoint foreArmR;
    ConfigurableJoint handR;
    ConfigurableJoint thighR;
    ConfigurableJoint shinR;
    ConfigurableJoint footR;
    ConfigurableJoint armL;
    ConfigurableJoint foreArmL;
    ConfigurableJoint handL;
    ConfigurableJoint thighL;
    ConfigurableJoint shinL;
    ConfigurableJoint footL;
    ConfigurableJoint head;
    float footDisToGround;
    float bodyHeight;
    float bodyWidth;
    public Vector3 bodyPosWhileSitting;
    public Dictionary<string, ConfigurableJoint> bpDict;
    public Dictionary<string, float> bodyStateDict = new Dictionary<string, float>() { { "LHHolding", 0 }, { "RHHolding", 0 }, { "Standing", 0 }, { "Sitting", 0 }, { "Laying", 0 }, { "Sleeping", 0 } };
    void Start()
    {
        body = transform.Find("body");
        defaultBodyPosY = body.localPosition.y;
        armR = transform.Find("armR").GetComponent<ConfigurableJoint>();
        foreArmR = transform.Find("foreArmR").GetComponent<ConfigurableJoint>();
        handR = transform.Find("handR").GetComponent<ConfigurableJoint>();
        thighR = transform.Find("thighR").GetComponent<ConfigurableJoint>();
        shinR = transform.Find("shinR").GetComponent<ConfigurableJoint>();
        footR = transform.Find("footR").GetComponent<ConfigurableJoint>();
        armL = transform.Find("armL").GetComponent<ConfigurableJoint>();
        foreArmL = transform.Find("foreArmL").GetComponent<ConfigurableJoint>();
        handL = transform.Find("handL").GetComponent<ConfigurableJoint>();
        thighL = transform.Find("thighL").GetComponent<ConfigurableJoint>();
        shinL = transform.Find("shinL").GetComponent<ConfigurableJoint>();
        footL = transform.Find("footL").GetComponent<ConfigurableJoint>();
        head = transform.Find("head").GetComponent<ConfigurableJoint>();
        eyeL = head.transform.Find("eyeL");
        eyeR = head.transform.Find("eyeR");
        footDisToGround = footL.GetComponent<Collider>().bounds.center.y;
        bodyHeight = body.GetComponent<Collider>().bounds.size.y;
        bodyWidth = body.GetComponent<Collider>().bounds.size.z;
        bpDict = new Dictionary<string, ConfigurableJoint>() { { "armR", armR }, { "foreArmR", foreArmR }, { "handR", handR }, { "thighR", thighR }, { "shinR", shinR },
                    { "footR", footR },{ "armL", armL },{ "foreArmL", foreArmL },{ "handL", handL },{ "thighL", thighL },{ "shinL", shinL },{ "footL", footL },{ "head", head }};

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        GroundCheck();
        SitCheck();
        LayCheck();
    }

    void CheckHandsHolding()
    {
        if(handR.transform.childCount > 0)
        {
            bodyStateDict["RHHolding"] = 1;
        }
        else
        {
            bodyStateDict["RHHolding"] = -1;
        }
        if(handL.transform.childCount > 0)
        {
            bodyStateDict["LHHolding"] = 1;
        }
        else
        {
            bodyStateDict["LHHolding"] = -1;
        }
    }
    void GroundCheck()
    {
        RaycastHit hit;
        if (Physics.Raycast(footL.transform.position, Vector3.down, out hit, footDisToGround + 0.1f))
        {
            if (hit.transform.tag == "Ground")
            {

                bodyStateDict["Standing"] = 1;
            }
        }
        else if(Physics.Raycast(footR.transform.position, Vector3.down, out hit, footDisToGround + 0.1f))
        {
            if (hit.transform.tag == "Ground")
            {
                bodyStateDict["Standing"] = 1;
            }
        }
        else
        {
            bodyStateDict["Standing"] = -1;
        }
    }

    void SitCheck()
    {
        if(Physics.Raycast(body.position, -body.up, bodyHeight/2 + 0.1f))
        {
            bodyStateDict["Sitting"] = 1;
            bodyPosWhileSitting = body.transform.localPosition;
        }
        else
        {
            bodyStateDict["Sitting"] = -1;
        }
    }
    void LayCheck()
    {
        if (Physics.Raycast(body.position, -body.forward, bodyWidth / 2 + 0.1f))
        {
            bodyStateDict["Laying"] = 1;
        }
        else
        {
            bodyStateDict["Laying"] = -1;
        }
    }
    void SleepCheck()
    {
        if(eyeL.localScale.y < 0.2f && eyeR.localScale.y < 0.2f)
        {
            bodyStateDict["Sleeping"] = 1;
        }
        else
        {
            bodyStateDict["Sleeping"] = -1;
        }
    }
}
