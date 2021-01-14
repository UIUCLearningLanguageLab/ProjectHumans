using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class HumanMotorSystem : MotorSystem
{
    private Human thisHuman;
    float max_rotation_speed = 1;
    float maxStepDistance = 2;

    public float rotateAngle = 360;

    public float velocity = 0;

    public HumanMotorSystem(Human human) : base(human) {
        this.thisHuman = human;
        actionStateLabelList = new List<string>
        {
            "sitting down", 
            "sitting up", 
            "laying down",
            "standing up", 
            "rotating", 
            "taking steps",
            "picking up", 
            "setting down",
            "eating", 
            "drinking",
            "waking up",
            "falling asleep"
        };

        actionArgumentLabelList = new List<string>
        {        
            "movement velocity",
            "step rate",            
            "rotation angle",               
            "rotation velocity",               
            "hand",
            "hand target x",
            "hand target y",
            "hand target z"
        };
    }

    public override void InitActionRuleDicts(){

        bodyStateRequirementDict["sitting down"].Add("standing");
        bodyStateObstructorDict["sitting down"].Add("sleeping");
        
        bodyStateRequirementDict["sitting up"].Add("laying");
        bodyStateObstructorDict["sitting up"].Add("sleeping");

        bodyStateRequirementDict["laying down"].Add("standing");
        bodyStateObstructorDict["laying down"].Add("sleeping");
        
        bodyStateRequirementDict["standing up"].Add("sitting");
        bodyStateObstructorDict["standing up"].Add("sleeping");

        bodyStateRequirementDict["rotating"].Add("standing");
        bodyStateObstructorDict["rotating"].Add("sleeping");

        bodyStateRequirementDict["taking steps"].Add("standing");
        bodyStateObstructorDict["taking steps"].Add("sleeping");

        bodyStateRequirementDict["picking up"].Add("standing");
        bodyStateObstructorDict["picking up"].Add("sleeping");

        bodyStateRequirementDict["setting down"].Add("standing");
        bodyStateObstructorDict["setting down"].Add("sleeping");

        bodyStateRequirementDict["eating"].Add("standing");
        bodyStateObstructorDict["eating"].Add("sleeping");

        bodyStateRequirementDict["drinking"].Add("standing");
        bodyStateObstructorDict["drinking"].Add("sleeping");

        bodyStateRequirementDict["waking up"].Add("laying");
        bodyStateRequirementDict["waking up"].Add("sleeping");

        bodyStateRequirementDict["falling asleep"].Add("laying");
        bodyStateObstructorDict["falling asleep"].Add("sleeping");
    }

    public override void UpdateActionStates(){
        this.actionStateArray = new bool[this.numActionStates];

        if (this.thisHuman.animator.GetCurrentAnimatorStateInfo(0).IsName("Rotate")) {
             this.actionStateArray[actionStateIndexDict["rotating"]] = true;
        }
        else if (this.thisHuman.animator.GetCurrentAnimatorStateInfo(0).IsName("Sit down")) {
            this.actionStateArray[actionStateIndexDict["sitting down"]] = true;
        }
        else if (this.thisHuman.animator.GetCurrentAnimatorStateInfo(0).IsName("Sit up")) {
            this.actionStateArray[actionStateIndexDict["sitting up"]] = true;
        }
        else if (this.thisHuman.animator.GetCurrentAnimatorStateInfo(0).IsName("Lay down")) {
             this.actionStateArray[actionStateIndexDict["laying down"]] = true;
        }
        else if (this.thisHuman.animator.GetCurrentAnimatorStateInfo(0).IsName("Taking steps")) {
            this.actionStateArray[actionStateIndexDict["taking steps"]] = true;
        }
        else if (this.thisHuman.animator.GetCurrentAnimatorStateInfo(0).IsName("Drink")) {
            this.actionStateArray[actionStateIndexDict["drinking"]] = true;
        }
        else if (this.thisHuman.animator.GetCurrentAnimatorStateInfo(0).IsName("Eat")) {
            if (this.thisHuman.animator.GetFloat("EatL/R") == 0) {
                this.actionStateArray[actionStateIndexDict["eating"]] = true;
            }
            if (this.thisHuman.animator.GetFloat("EatL/R") == 1) {
                this.actionStateArray[actionStateIndexDict["eating"]] = true;
            }
        }
        else if (this.thisHuman.animator.GetCurrentAnimatorStateInfo(0).IsName("PickUp")) {
            if (this.thisHuman.animator.GetFloat("PickupL/R") == 0) {
                this.actionStateArray[actionStateIndexDict["picking up"]] = true;
            }
            if (this.thisHuman.animator.GetFloat("PickupL/R") == 1) {
                this.actionStateArray[actionStateIndexDict["picking up"]] = true;
            }
        }
        else if (this.thisHuman.animator.GetCurrentAnimatorStateInfo(0).IsName("SetDown")) {
            if (this.thisHuman.animator.GetFloat("SetdownL/R") == 0) {
                 this.actionStateArray[actionStateIndexDict["setting down"]] = true;
            }
            if (this.thisHuman.animator.GetFloat("SetdownL/R") == 1) {
                 this.actionStateArray[actionStateIndexDict["setting down"]] = true;
            }
        }
    }
    

    public override bool CheckActionLegality(string action){
        bool legal = false;

        List<string> requiredBodyStateList = bodyStateRequirementDict[action];
        List<string> obstructorBodyStateList = bodyStateObstructorDict[action];

        int numRequirements = requiredBodyStateList.Count;
        int numObstructors = obstructorBodyStateList.Count;

        bool[] bodyStateRequirementArray = new bool[numRequirements];
        bool[] bodyStateObstructorArray = new bool[numObstructors];
        bool status;
        int index;

        for (int i = 0; i < numRequirements; i++){
            index = this.thisHuman.GetBody().GetBodyStateIndex(requiredBodyStateList[i]);
            status = this.thisHuman.GetBody().GetBodyState(index);
            bodyStateRequirementArray[i] = status;
        }

        for (int i = 0; i < numObstructors; i++){
            index = this.thisHuman.GetBody().GetBodyStateIndex(obstructorBodyStateList[i]);
            status = this.thisHuman.GetBody().GetBodyState(index);
            bodyStateObstructorArray[i] = status;
        }

        if (bodyStateRequirementArray.All(x => x)){
            legal = true;
        }
        if (bodyStateObstructorArray.Any(x => x)){
            legal = false;
        }
        return legal;
    }

    public override void TakeAction(Animal.ActionChoiceStruct actionChoiceStruct){
        bool doingNothing = !this.actionStateArray.Any(x => x);
        if (doingNothing) {
            if (actionChoiceStruct.actionChoiceArray[actionStateIndexDict["sitting down"]]){
                SitDown();
            }
            else if (actionChoiceStruct.actionChoiceArray[actionStateIndexDict["sitting up"]]){
                SitUp();
            }
            else if (actionChoiceStruct.actionChoiceArray[actionStateIndexDict["standing up"]]){
                StandUp();
            }
            else if (actionChoiceStruct.actionChoiceArray[actionStateIndexDict["laying down"]]){
                LayDown();
            }
            else if (actionChoiceStruct.actionChoiceArray[actionStateIndexDict["falling asleep"]]){
                Sleep();
            }
            else if (actionChoiceStruct.actionChoiceArray[actionStateIndexDict["waking up"]]){
                WakeUp();
            }
            else if (actionChoiceStruct.actionChoiceArray[actionStateIndexDict["eating"]]){
                Eat(actionChoiceStruct.actionArgumentArray[actionArgumentIndexDict["hand"]]);
            }
            else if (actionChoiceStruct.actionChoiceArray[actionStateIndexDict["drinking"]]){
                Drink();
            }
            else if (actionChoiceStruct.actionChoiceArray[actionStateIndexDict["taking steps"]]){
                TakeSteps(actionChoiceStruct.actionArgumentArray[actionArgumentIndexDict["step rate"]]);
            }
            else if (actionChoiceStruct.actionChoiceArray[actionStateIndexDict["rotating"]]){
                Rotate(actionChoiceStruct.actionArgumentArray[actionArgumentIndexDict["rotation angle"]]);
            }
            else if (actionChoiceStruct.actionChoiceArray[actionStateIndexDict["picking up"]]){
                PickUp(actionChoiceStruct.actionArgumentArray[actionArgumentIndexDict["hand"]]);
            }
            else if (actionChoiceStruct.actionChoiceArray[actionStateIndexDict["setting down"]]){
                SetDown(actionChoiceStruct.actionArgumentArray[actionArgumentIndexDict["hand"]]);
            }
                
        }
        else{
            if (actionStateArray[actionStateIndexDict["taking steps"]] == true){
                if (actionChoiceStruct.actionChoiceArray[actionStateIndexDict["taking steps"]]){
                    TakeSteps(actionChoiceStruct.actionArgumentArray[actionArgumentIndexDict["step rate"]]);
                }
            }

            else if (actionStateArray[actionStateIndexDict["rotating"]] == true){
                if (actionChoiceStruct.actionChoiceArray[actionStateIndexDict["rotating"]]){
                    Rotate(actionChoiceStruct.actionArgumentArray[actionArgumentIndexDict["rotation angle"]]);
                }
            }

            else if (actionStateArray[actionStateIndexDict["picking up"]] == true){
                if (actionChoiceStruct.actionChoiceArray[actionStateIndexDict["picking up"]]){
                    PickUp(actionChoiceStruct.actionArgumentArray[actionArgumentIndexDict["hand"]]);
                }
            }

            else if (actionStateArray[actionStateIndexDict["setting down"]] == true){
                if (actionChoiceStruct.actionChoiceArray[actionStateIndexDict["setting down"]]){
                   SetDown(actionChoiceStruct.actionArgumentArray[actionArgumentIndexDict["hand"]]);
                }
            }
        }
    }
    public override void EndAction(Animal.ActionChoiceStruct actionChoiceStruct) {
        if (!actionChoiceStruct.actionChoiceArray[actionStateIndexDict["taking steps"]]){
            this.thisHuman.animator.SetBool("takingSteps", false);
            this.actionStateArray = new bool[this.numActionStates];;
        }
        if(!actionChoiceStruct.actionChoiceArray[actionStateIndexDict["rotating"]]) {
            this.thisHuman.animator.SetBool("rotate", false);
            this.actionStateArray = new bool[this.numActionStates];
        }
    }

    public void TakeSteps(float stepProportion) {
        // call check to see if legal
        if (CheckActionLegality("taking steps")){
            velocity = (stepProportion * maxStepDistance);
            if (stepProportion != 0) {
                var step = stepProportion * maxStepDistance;
                thisHuman.gameObject.transform.Translate(Vector3.forward *step*Time.deltaTime);
                this.thisHuman.animator.SetBool("takingSteps", true);
                this.thisHuman.animator.SetFloat("speed", 0);// need to change according to the moving speed of human
                this.actionStateArray = new bool[this.numActionStates];
                this.actionStateArray[this.actionStateIndexDict["taking steps"]] = true;
            }
            else {
                this.thisHuman.animator.SetBool("takingSteps", false);
                this.actionStateArray = new bool[this.numActionStates];
            }
        }
    }

    public void Rotate(float rotatingSpeed){
        if(CheckActionLegality("rotating")) {
            if (rotatingSpeed != 0) {
                this.thisHuman.gameObject.transform.Rotate(0,rotatingSpeed * Time.deltaTime,0);
                this.thisHuman.animator.SetBool("rotate", true);
            }
            else {
                this.thisHuman.animator.SetBool("rotate", false);
            }
            
        }
    }

    public void Drink() {
        bool doingNothing = !this.actionStateArray.Any(x => x);
        //same
        if (CheckActionLegality("drinking")) {

            this.thisHuman.animator.SetTrigger("drink");
            // if you can drink while drinking, this cant be called here. should be once per completed drink
            // and it should be a call to a function in driveSystem, telling it what was drank
            this.thisHuman.GetDriveSystem().SetDriveState(this.thisHuman.GetDriveSystem().GetDriveStateIndex("thirst"), 0);
            this.actionStateArray = new bool[this.numActionStates];
            this.actionStateArray[this.actionStateIndexDict["drinking"]] = true;
        }
    }
    
    public void SitDown(){
        bool doingNothing = !this.actionStateArray.Any(x => x);
        //same
        if (CheckActionLegality("sitting down")) {

            this.thisHuman.animator.SetTrigger("sitDown");

            this.actionStateArray = new bool[this.numActionStates];
            this.actionStateArray[this.actionStateIndexDict["sitting down"]] = true;

            this.thisHuman.GetBody().SetBodyState(this.thisHuman.GetBody().GetBodyStateIndex("standing"), false);
            this.thisHuman.GetBody().SetBodyState(this.thisHuman.GetBody().GetBodyStateIndex("sitting"), true);                     
        }
    }

    public void SitUp(){
        bool doingNothing = !this.actionStateArray.Any(x => x);
        //same
        if (CheckActionLegality("sitting up")) {

            this.thisHuman.animator.SetTrigger("sitUp");

            this.actionStateArray = new bool[this.numActionStates];
            this.actionStateArray[this.actionStateIndexDict["sitting up"]] = true;

            this.thisHuman.GetBody().SetBodyState(this.thisHuman.GetBody().GetBodyStateIndex("laying"), false);
            this.thisHuman.GetBody().SetBodyState(this.thisHuman.GetBody().GetBodyStateIndex("sitting"), true);          
        }
    }
        
    public void StandUp(){
        bool doingNothing = !this.actionStateArray.Any(x => x);
        //same
        if (CheckActionLegality("standing up")) {

            this.thisHuman.animator.SetTrigger("standUp");

            this.actionStateArray = new bool[this.numActionStates];
            this.actionStateArray[this.actionStateIndexDict["standing up"]] = true;

            this.thisHuman.GetBody().SetBodyState(this.thisHuman.GetBody().GetBodyStateIndex("sitting"), false);
            this.thisHuman.GetBody().SetBodyState(this.thisHuman.GetBody().GetBodyStateIndex("standing"), true);  
        }
       
    }

    public void LayDown(){
        bool doingNothing = !this.actionStateArray.Any(x => x);
        //same
        if (CheckActionLegality("laying down")) {

            this.thisHuman.animator.SetTrigger("layDown");

            this.actionStateArray = new bool[this.numActionStates];
            this.actionStateArray[this.actionStateIndexDict["laying down"]] = true;

            this.thisHuman.GetBody().SetBodyState(this.thisHuman.GetBody().GetBodyStateIndex("sitting"), false);
            this.thisHuman.GetBody().SetBodyState(this.thisHuman.GetBody().GetBodyStateIndex("laying"), true);  
            
        }
    }

    public void Sleep(){
        bool doingNothing = !this.actionStateArray.Any(x => x);
        Debug.Log("sleep");
        //same
        if (CheckActionLegality("falling asleep")) {

            ((HumanBody)this.thisHuman.GetBody()).leftEye.localScale = new Vector3(1, 0.09f, 2);
            ((HumanBody)this.thisHuman.GetBody()).rightEye.localScale = new Vector3(1, 0.09f, 2);

            this.actionStateArray = new bool[this.numActionStates];
            this.actionStateArray[this.actionStateIndexDict["falling asleep"]] = true;
            this.thisHuman.GetBody().SetBodyState(this.thisHuman.GetBody().GetBodyStateIndex("sleeping"), true);  
        
        }
    }
    
    public void WakeUp(){
        bool doingNothing = !this.actionStateArray.Any(x => x);
        // same
        if (CheckActionLegality("waking up"))  {
            ((HumanBody)this.thisHuman.GetBody()).leftEye.localScale = new Vector3(1, 1, 1);
            ((HumanBody)this.thisHuman.GetBody()).rightEye.localScale = new Vector3(1, 1, 1);
                  
            this.actionStateArray = new bool[this.numActionStates];
            this.actionStateArray[this.actionStateIndexDict["waking up"]] = true;
            this.thisHuman.GetBody().SetBodyState(this.thisHuman.GetBody().GetBodyStateIndex("sleeping"), false);  

        }
    }
    
    public void PickUp(float hand){
        Collider[] pickableObj = new Collider[5];
        bool doingNothing = !this.actionStateArray.Any(x => x);

        if (CheckActionLegality("picking up"))  {
            
            int pickUpHand = -1;
            int numObj = -1;
            if ((hand == 0) && (!this.thisHuman.GetBody().GetBodyState(this.thisHuman.GetBody().GetBodyStateIndex("holding with left hand")))) {
                pickUpHand = 0;
                numObj = Physics.OverlapSphereNonAlloc(((HumanBody)this.thisHuman.GetBody()).leftHand.position,0.2f,pickableObj);
            }
            else if ((hand == 1) && (!this.thisHuman.GetBody().GetBodyState(this.thisHuman.GetBody().GetBodyStateIndex("holding with right hand")))){
                pickUpHand = 1;
                numObj = Physics.OverlapSphereNonAlloc(((HumanBody)this.thisHuman.GetBody()).rightHand.position,0.2f,pickableObj);
            }

            if (pickUpHand != -1){
                this.thisHuman.animator.SetTrigger("pickUp");
                this.thisHuman.animator.SetFloat("left/rightHand",pickUpHand);
                
                for(int i = 0; i < numObj; i++) {
                    if (!pickableObj[i].CompareTag("Human") && !pickableObj[i].CompareTag("ground")) {
                        pickableObj[i].GetComponent<Rigidbody>().isKinematic = true;
                        pickableObj[i].GetComponent<Rigidbody>().useGravity = false;
                        if (pickUpHand == 0){
                            pickableObj[i].transform.parent = ((HumanBody)this.thisHuman.GetBody()).leftHand.transform;
                            this.thisHuman.animator.ResetTrigger("pickUp");
                            pickableObj[i].transform.localPosition = new Vector3(0.1311f, 0.0341f, 0.073f);

                            this.actionStateArray = new bool[this.numActionStates];
                            this.actionStateArray[this.actionStateIndexDict["picking up"]] = true;
                            this.thisHuman.GetBody().SetBodyState(this.thisHuman.GetBody().GetBodyStateIndex("holding with left hand"), true);  
                        }
                        else{
                            pickableObj[i].transform.parent = ((HumanBody)this.thisHuman.GetBody()).rightHand.transform;
                            this.thisHuman.animator.ResetTrigger("pickUp");
                            pickableObj[i].transform.localPosition = new Vector3(-0.1593f, -0.026f, -0.0665f);

                            this.actionStateArray = new bool[this.numActionStates];
                            this.actionStateArray[this.actionStateIndexDict["picking up"]] = true;
                            this.thisHuman.GetBody().SetBodyState(this.thisHuman.GetBody().GetBodyStateIndex("holding with right hand"), true);
                        }
                    }
                }
            }
            else{
                Debug.Log("can't pick up because hand is full");
            }
            
        }
        else{
            Debug.Log("can't pick up because another action is happening");
        }
        
    }

    // change the rest of the actions so that they are using the new nervous system data structures
    // check for other places, like simpleAI, that use the old way of keeping track
    // then we need to arrange everything in MotorSystem vs. HumanMotorSystem, so it is like Nervous System

    public void SetDown(float hand){
        bool doingNothing = !this.actionStateArray.Any(x => x);
        if (CheckActionLegality("setting down"))  {
            if ((hand == 0) 
                && (this.thisHuman.GetBody().GetBodyState(this.thisHuman.GetBody().GetBodyStateIndex("holding with left hand")))) 
            {
                //this.thisHuman.animator.ResetTrigger("pick");
                this.thisHuman.animator.SetTrigger("setDown");
                this.thisHuman.animator.SetFloat("left/rightHand",0);
                if (((HumanBody)this.thisHuman.GetBody()).leftHand.GetChild(0).transform.position.y < 0.1) {
                    ((HumanBody)this.thisHuman.GetBody()).leftHand.GetChild(0).GetComponent<Rigidbody>().isKinematic = false;
                    ((HumanBody)this.thisHuman.GetBody()).leftHand.GetChild(0).transform.parent = null;
                    if (((HumanBody)this.thisHuman.GetBody()).leftHand.childCount == 0) {
                        this.thisHuman.animator.ResetTrigger("set");
                        this.actionStateArray = new bool[this.numActionStates];
                        this.actionStateArray[this.actionStateIndexDict["setting down"]] = true;
                        this.thisHuman.GetBody().SetBodyState(this.thisHuman.GetBody().GetBodyStateIndex("holding with left hand"), false);  
                    }
                }
            }
                
            else if ((hand == 1) 
                && (this.thisHuman.GetBody().GetBodyState(this.thisHuman.GetBody().GetBodyStateIndex("holding with right hand")))) 
            {

                //this.thisHuman.animator.ResetTrigger("pick");
                this.thisHuman.animator.SetTrigger("set");
                this.thisHuman.animator.SetFloat("left/rightHand",1);
                if (((HumanBody)this.thisHuman.GetBody()).rightHand.GetChild(0).transform.position.y < 0.1) {
                    ((HumanBody)this.thisHuman.GetBody()).rightHand.GetChild(0).GetComponent<Rigidbody>().isKinematic = false;
                    ((HumanBody)this.thisHuman.GetBody()).rightHand.GetChild(0).transform.parent = null;
                    if (((HumanBody)this.thisHuman.GetBody()).rightHand.childCount == 0) 
                    {
                        this.thisHuman.animator.ResetTrigger("set");
                        this.actionStateArray = new bool[this.numActionStates];
                        this.actionStateArray[this.actionStateIndexDict["setting down"]] = true;
                        this.thisHuman.GetBody().SetBodyState(this.thisHuman.GetBody().GetBodyStateIndex("holding with right hand"), false);  
                    }
                }
            }

            else {
                Debug.Log("Can't set down because doing something or nothing in the specified hand");
            }
        }
        else {
            Debug.Log("Can't set down because not standing");
        }

    }

    public void Eat(float hand) {
        Debug.Log((this.thisHuman.GetBody().GetBodyState(this.thisHuman.GetBody().GetBodyStateIndex("holding with left hand"))));
        bool success = false;
        bool doingNothing = !this.actionStateArray.Any(x => x);
        if (CheckActionLegality("eating")){
            if ((hand == 0) 
                && (this.thisHuman.GetBody().GetBodyState(this.thisHuman.GetBody().GetBodyStateIndex("holding with left hand"))))
            {
                success = true;
            }

            else if ((hand == 1)
                && (this.thisHuman.GetBody().GetBodyState(this.thisHuman.GetBody().GetBodyStateIndex("holding with right hand"))))
            {
                success = true;
            }
            else {
                Debug.Log("can't eat");
            }
            
        }

        if (success){
            // this needs to be fixed the same way as thirst.
            // should probably be a call to drive system with what was eaten, and the code for how drive system should be updated should be located there
            
            this.thisHuman.animator.SetTrigger("eat");
            this.thisHuman.animator.SetFloat("left/rightHand", hand);
            this.actionStateArray = new bool[this.numActionStates];
            if (hand == 0){
                UnityEngine.Object.Destroy(((HumanBody)this.thisHuman.GetBody()).leftHand.GetChild(0).gameObject, 8);                 
                this.actionStateArray[this.actionStateIndexDict["eating"]] = true;
                this.thisHuman.GetBody().SetBodyState(this.thisHuman.GetBody().GetBodyStateIndex("holding with left hand"), false);  
            }
            else{
                UnityEngine.Object.Destroy(((HumanBody)this.thisHuman.GetBody()).rightHand.GetChild(0).gameObject, 8);
                this.actionStateArray[this.actionStateIndexDict["eating"]] = true;
                this.thisHuman.GetBody().SetBodyState(this.thisHuman.GetBody().GetBodyStateIndex("holding with right hand"), false);  
            }

        }

    }
}



