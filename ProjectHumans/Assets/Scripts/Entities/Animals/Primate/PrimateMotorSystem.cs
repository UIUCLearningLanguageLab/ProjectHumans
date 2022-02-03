//using System;
//using System.Collections;
//using System.Collections.Generic;
//using MathNet.Numerics.LinearAlgebra;
//using UnityEngine;
//using System.Linq;
//public class PrimateMotorSystem : MotorSystem {
//    Transform abdomenTrans;
//    PrimateBody primateBody;
//    Vector3 goalPos;
//    bool firstRight = true; // change this at some point like handedness
//    private float xMin = -1f, xMax = 1f;
//    private float timeValue = 0.0f;

//    public PrimateMotorSystem(Animal animal) : base(animal) {
//        Debug.Log("An ape was born!");
//        abdomenTrans = thisBody.abdomen.transform;
//        primateBody = (PrimateBody)thisBody;
//        StandUp();
//    }
    
//    public override void Crouch() {
//        if (stateDict["crouch"] == -1f) {
//            CrouchDown(); }
//        else if (stateDict["crouch"] == 1f) {
//            SitUp();
//        } else { Rest(); }
//        LegCheck();
//    }

//    public override void Sit() {
//        if (stateDict["sit"] == -1f) {
//            SitDown();
//        } else { SitUp(); }
//        LegCheck();
//    }

//    public override void Lay() {
//        if (stateDict["lay"] == -1f) {
//            LayDown();
//        } else if (stateDict["lay"] == 1f) {
//            SitUp();
//        } else { Rest(); }
//        LegCheck();
//    }

//    public override void Stand() {
//        if (stateDict["stand"] == 1f) {
//            StandUp();
//        } else if (stateDict["stand"] == -1f) {
//            SitDown();
//        } else { Rest(); }
//        LegCheck();
//    }
//    public override void Rotate() {
//        if(thisBody.GetState("standing") == 1f) {
//            float degree = stateDict["rotate"] * 0.5f;
//            float rotatingSpeed = degree * thisAnimal.GetPhenotype().GetTrait("max_rotation");

//            thisBody.globalPos.Rotate(0, rotatingSpeed, 0, Space.World);
//        } else {
//            float degree = stateDict["rotate"] * 0.25f;
//            float rotatingSpeed = degree * thisAnimal.GetPhenotype().GetTrait("max_rotation");
//            abdomenTrans.Rotate(0, rotatingSpeed, 0, Space.Self);
//        }
//    }
//    public override void TakeSteps() {
//        UnlockFeet();
//        if (primateBody.CheckSitting()) {
//            StandUp();
//        } else {
//            float direction = stateDict["take steps"];
//            float stepProportion = direction * thisAnimal.GetPhenotype().GetTrait("max_step") * 0.8f;

//            ConfigurableJoint legR = thisBody.GetSkeleton("Femur_R").GetComponent<ConfigurableJoint>();
//            ConfigurableJoint legL = thisBody.GetSkeleton("Femur_L").GetComponent<ConfigurableJoint>();

//            float crouchAdjustR = legR.targetRotation.x;
//            float crouchAdjustL = legL.targetRotation.x;

//            float xQR = Mathf.Lerp(xMin, xMax, timeValue) + crouchAdjustR/90;
//            float xQL = Mathf.Lerp(xMax, xMin, timeValue) + crouchAdjustL/90;
//            timeValue += 0.5f * Time.deltaTime;

//            if (timeValue > 1.0f) {
//                timeValue = 0.0f;
//                firstRight = !firstRight;
//            }

//            if (firstRight) {
//                // Right foot first
//                legR.targetRotation = new Quaternion(xQR, 0, 0, 1);
//                legL.targetRotation = new Quaternion(xQL, 0, 0, 1);

//                if (xQL > -0.25f) {
//                thisBody.DisableKinematic("Foot_L");
//                } else {
//                thisBody.EnsureKinematic("Foot_L");
//                }

//                if (xQR > -0.25f) {
//                thisBody.DisableKinematic("Foot_R");
//                } else {
//                thisBody.EnsureKinematic("Foot_R");
//                }
//            } else {
//                // Left foot first
//                legL.targetRotation = new Quaternion(xQR, 0, 0, 1);
//                legR.targetRotation = new Quaternion(xQL, 0, 0, 1);

//                if (xQR > -0.25f) {
//                thisBody.DisableKinematic("Foot_L");
//                } else {
//                thisBody.EnsureKinematic("Foot_L");
//                }

//                if (xQL > -0.25f) {
//                thisBody.DisableKinematic("Foot_R");
//                } else {
//                thisBody.EnsureKinematic("Foot_R");
//                }
//            }
            
//            float diff = (float) Math.Sqrt((xQR * xQR) + (xQL * xQL));
//            // Super important to move parts of the body, not the whole gameObject. 
//            abdomenTrans.Translate(abdomenTrans.forward * stepProportion * Time.deltaTime * diff);
            
//            LegCheck();
//        }
//    }

//    public override void UseHand() {
//        GameObject holder = GetActiveHand();

//        if (stateDict["hand action"] == 1f) { 
//            PickUp();
//        } else { DropArm(); }
//    }
//    public override void Consume() {
//        thisAnimal.AddThought("Tried to eat something");
//        GameObject holder = GetActiveHand();

//        if (ArmTo(thisBody.head.transform.position)) {
//            thisBody.EatObject(holder.name);
//            DropArm();
//        }
//    }
//    public override void Sleep() {
//        if (stateDict["consume"] == 1f) {
//            FallAsleep();
//        } else { WakeUp(); }
//    }

//    public override void Rest() {
//        thisAnimal.AddThought("Resting");
//        thisBody.RestAdjust();
//    }

//    public override void Look() {
//        if (stateDict["look"] == 1f) {
//            LookAt();
//        } else { LookForward(); }
//    }

//    public override void Collapse()
//    {
//        throw new NotImplementedException();
//    }

//    public override void Reset()
//    {
//        throw new NotImplementedException();
//    }
//    // BEGIN HELPER FUNCTIONS
//    void SitDown() {
//        if (((PrimateBody)thisBody).CheckCrouchingTop()) {
//            thisBody.SlerpRotateTo("Abdomen", Quaternion.identity, 0.5f);
//            Vector3 toSend = abdomenTrans.position;
//            double sitHeight = thisBody.GetHeight() / 2.25;
//            if (toSend.y > sitHeight) {
//                BendLegs(.5f, 0.15f);
//                abdomenTrans.Translate(Vector3.up * (Time.deltaTime * -1));
//            } else {
//                thisBody.EnsureKinematic("Abdomen");
//                thisBody.FreeRotation("Abdomen");
//                thisAnimal.AddThought("I think I'm sitting");
//                thisBody.SetState("sitting", 1f);
//            }
//        } else { CrouchDown(); }
//    }
//    void SitUp() {
//        thisAnimal.AddThought("Tried to lift abdomen");
//        thisBody.EnsureKinematic("Abdomen");
//        thisBody.SlerpRotateTo("Abdomen", Quaternion.identity, 0.5f);
//    }

//    void LayDown() {
//        thisAnimal.AddThought("Tried to lay down");
//        if (thisAnimal.GetBodyState("sitting")) {
//            Collapse();
//            //thisBody.RotateJointTo("Femur_R", new Quaternion(-1, 0, 0, 1));
//            //thisBody.RotateJointTo("Femur_L", new Quaternion(-1, 0, 0, 1));
//        } else { SitDown(); }
//    }
    
//    void StandUp() {
//        Vector3 toSend = abdomenTrans.localPosition;
//        if(abdomenTrans.rotation.x > 15f || abdomenTrans.rotation.x < -15f) {
//            SitUp();
//        } else {
//            if (Math.Pow(toSend.y - 0.2f, 2) > 0.05 ) {
//                if (toSend.y < 0.2f) {
//                    abdomenTrans.Translate(Vector3.up * (Time.deltaTime) * .5f);
//                } else {
//                    abdomenTrans.Translate(Vector3.up * (Time.deltaTime) * -.5f);
//                } 
//            } else {
//               thisAnimal.AddThought("I think I'm standing");
//               thisBody.SetState("standing", 1f);
//            }
//        }
//    }
    
//    void PickUp() {
//        thisAnimal.AddThought("Tried to pick something up");
//        GameObject holder = GetActiveHand();

//        if (ArmToGoal()) {
//            GrabWithHolder(holder);
//            DropArm();
//        }
//    }
    
//    void SetDown() {
//        thisAnimal.AddThought("Tried to set something down");
//        GameObject holder = GetActiveHand();
//        if (thisBody.CheckSitting()) {
//            RemoveFromHolder(holder);
//        } else { KneelDown(); }
//    }
//    // Functional
//    void WakeUp() {
//        thisAnimal.AddThought("Waking up");
//        ToggleEyes(true);
//        thisBody.SetState("sleeping", -1f);
//    }

//    void FallAsleep() {
//        thisAnimal.AddThought("Sleeping");
//        ToggleEyes(false);

//        if (thisBody.CheckLaying()) {
//            thisBody.SetState("sleeping", 1f);
//            thisBody.SleepAdjust();
//        } else { LayDown(); }
//    }

//    // Functional (but hated)
//    void LookForward() {
//        Quaternion toSend = Quaternion.LookRotation(thisBody.globalPos.forward);
//        thisBody.RotateJointTo("Head", toSend);
//    }

//   // Need to look at active hand
//    void LookAt() {
//        GameObject holder = GetActiveHand();
//        Vector3 direction = (holder.transform.position - thisBody.head.transform.position).normalized;
//        Quaternion toSend = Quaternion.LookRotation(direction);
//        thisBody.RotateJointTo("Head", toSend);
//    }
    
//    bool ArmTo(Vector3 targetPos) {
//        string arm = GetActiveHand().name;
//        thisBody.EnsureKinematic(arm);
//        Transform armTrans = thisBody.GetSkeletonDict()[arm].transform;
//        armTrans.position = Vector3.Slerp(armTrans.position, targetPos, Time.deltaTime);
//        return (armTrans.position == targetPos);
//    }

//    // Functional
//    void BendKnees(float degree) {
//        Quaternion toSend = new Quaternion(degree, 0, 0, 0);
//        thisBody.RotateJointTo("Tibia_L", toSend);
//        thisBody.RotateJointTo("Tibia_R", toSend);
//    }
    
//    void BendLegs(float xDegree, float yDegree) {
//        Quaternion sendLeft = new Quaternion(xDegree, yDegree, 0, 1f);
//        Quaternion sendRight = new Quaternion(xDegree, yDegree, 0, 1f);
//        thisBody.RotateJointTo("Femur_L", sendLeft);
//        thisBody.RotateJointTo("Femur_R", sendRight);
//    }


//    bool ArmToGoal() {
//        Vector3 abAdj = primateBody.abAdjLeft;
//        string arm = GetActiveHand().name;
//        if (stateDict["active right"] == 1f) {
//            abAdj = primateBody.abAdjRight;
//        }

//        float xTrans = stateDict["RP x"] * thisBody.xMax;
//        float yTrans = stateDict["RP y"] * thisBody.yMax;
//        float zTrans = stateDict["RP z"] * thisBody.zMax;

//        Vector3 toAdd = new Vector3(.5f, -.5f, 1f);
//        // need to attach .abs to these
//        if (xTrans <= 1f && yTrans <= 1f && zTrans <= 1f) {
//            toAdd = new Vector3(xTrans, yTrans, zTrans);
//        } else {
//             thisAnimal.AddThought("Can't reach that far!");
//        }
        
//        Transform armTrans = GetActiveHand().transform;
//        Vector3 goalPos = abdomenTrans.position + abAdj + toAdd;

//        thisBody.EnsureKinematic(armTrans.name);
//        armTrans.position = Vector3.Slerp(armTrans.position, goalPos, Time.deltaTime);
//        return (armTrans.position == goalPos);
//    }
//    // Functional
//    void DropArm() {
//        string arm = GetActiveHand().name;
//        thisBody.DisableKinematic(arm);
//    }
    
//    void KneelDown() {
//        Vector3 toSend = abdomenTrans.position;
//        if (toSend.y > thisBody.GetHeight() / 2 + 0.5) {
//            BendLegs(.25f, 0f);
//            BendKnees(-.33f);
//            abdomenTrans.Translate(Vector3.up * (Time.deltaTime * -1));
//        } else { thisAnimal.AddThought("I think I'm kneeling"); }
//    }

//    void CrouchDown() {
//        thisAnimal.AddThought("Crouching down");
//        if (((PrimateBody)thisBody).CheckCrouchingBottom()) {
//            BendLegs(-1f, 0f);
//            BendLegs(.33f, 0f);
//            BendKnees(-.25f);
//            LockFeet();
//            abdomenTrans.Translate(Vector3.up * (Time.deltaTime * -0.75f));
//            UnlockFeet();
//            thisBody.SlerpRotateTo("Abdomen", new Quaternion(0.55f, 0, 0, 1), 0.5f);
//        } else { thisBody.SetState("crouching", 1f); }
//    }


//    void GrabWithHolder(GameObject holder) {
//        thisAnimal.AddThought("Tried to grab something");
//        Vector3 forCollider = holder.transform.position;
//        Collider[] hitColliders = Physics.OverlapSphere(forCollider, .025f);
//        Rigidbody toConnect = holder.GetComponent<Rigidbody>();

//        foreach (var hit in hitColliders) {
//            if (!thisBody.GetSkeletonDict().ContainsKey(hit.gameObject.name)) {
//                if (CheckMovableObject(hit.gameObject)) {
//                    FixedJoint newJoint = hit.gameObject.AddComponent<FixedJoint>() as FixedJoint;
//                    newJoint.connectedBody = toConnect;
//                } else { thisAnimal.AddThought("Object too big to lift!"); }
//            } else { thisAnimal.AddThought("Nothing to pick up!"); }
//        }
//    }

//    void RemoveFromHolder(GameObject holder) {
//        string holderName = holder.name;
//        if (thisBody.GetHeld(holderName) != null) { thisBody.RemoveObject(holderName); }
//    }

//    bool CheckMovableObject(GameObject toLift) {
//        float liftMass = toLift.GetComponent<Rigidbody>().mass;
//        return (liftMass < thisBody.rigidbody.mass);
//    }

//    GameObject GetActiveHand() {
//        if (stateDict["active right"] == 1f) {
//            return thisBody.GetSkeleton("Hand_R");
//        } else { return thisBody.GetSkeleton("Hand_L"); }
//    }

//    void ToggleEyes(bool open) {
//        Transform head = thisBody.GetSkeleton("Head").transform;
//        Transform face = null;
//        foreach (Transform part in head) {
//            if (part.name == "Face") {
//                face = part;
//                break;
//            }
//        }

//        foreach (Transform feature in face) {
//            if (feature.name == "Eye_R" || feature.name == "Eye_L") {
//                feature.gameObject.SetActive(open);
//            }
//        }
//    }

//    void LockFeet() {
//        thisBody.EnsureKinematic("Foot_R");
//        thisBody.EnsureKinematic("Foot_L");
//    }


//    void UnlockFeet() {
//        thisBody.DisableKinematic("Foot_R");
//        thisBody.DisableKinematic("Foot_L");
//    }

//    void LegCheck() {
//        foreach (string leg in thisBody.LegList) {
//            float rotCheck = thisBody.GetSkeleton(leg).transform.rotation.y;
//            if (rotCheck > 0.40f || rotCheck < -0.40f) {
//                Debug.Log("Fixing leg: " + leg);
//                thisBody.SlerpRotateTo(leg, Quaternion.identity, 1f);
//                thisBody.DisableKinematic(leg);
//            }
//        }
//    }
//}
