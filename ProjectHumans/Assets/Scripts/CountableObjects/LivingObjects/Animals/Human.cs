﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class Human : Animal
{
    public HumanSimpleAI humanSimpleAI;
    public AI activeAI;
    public string activeAILabel = "HumanSimpleAI";

    /// <summary>
    /// Human constructor
    /// </summary>
    public Human(int index, Nullable<Vector3> position, Genome motherGenome, Genome fatherGenome): 
            base("Human", index, position, motherGenome, fatherGenome) 
    {
        SetObjectType("Human");
        
        // All of these are getting passed empty lists right now, need to read in state arrays
        SetBody(new HumanBody(this));
        GetBody().InitStates(this.GetBody().GetStateLabels());
        GetBody().UpdateBodyStates();
        visualInputCamera = this.gameObject.GetComponentInChildren<Camera>();
        
        SetDriveSystem(new HumanDriveSystem(this));
        GetDriveSystem().InitStates(this.GetDriveSystem().GetStateLabels());
        GetDriveSystem().SetState("health", 1.0f);

        SetMotorSystem(new HumanMotorSystem(this));
        GetMotorSystem().InitStates(this.GetMotorSystem().GetStateLabels());
        GetMotorSystem().InitActionArguments(this.GetMotorSystem().GetArgLabels());

        SetSensorySystem(new HumanSensorySystem(this));


        if (activeAILabel == "blankAI") {
            activeAI = new AI(GetBody(), GetDriveSystem(), GetMotorSystem(), GetPhenotype());
        } else {
            humanSimpleAI = new HumanSimpleAI(this, GetBody(), GetDriveSystem(), GetMotorSystem(), GetPhenotype());
            activeAI = humanSimpleAI;
        }
    }

    public override void UpdateAnimal(){
        float[ , ] visualInputMatrix = GetSensorySystem().GetVisualInput();
        activeAI.actionChoiceStruct = activeAI.ChooseAction(visualInputMatrix, GetPhenotype().GetTraitDict());

        GetMotorSystem().TakeAction(activeAI.actionChoiceStruct);
        IncreaseAge(1);
    }
}





    





    






    