using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MathNet.Numerics.Distributions;
using Random=UnityEngine.Random;

public class Plant : Entity {

    public Plant(string objectType, string index, Genome motherGenome, Genome fatherGenome, Vector3 spawn) 
    : base (objectType, index, motherGenome, fatherGenome, spawn, false) {
        body = new Body(this, spawn);
        SetTrunkSize(Random.Range(1.85f, 3f));
    }

    public override void UpdateEntity() {
        
        float growthRefreshRate = this.phenotype.GetTraitDict()["growth_refresh_rate"];
        float fruitRate = this.phenotype.GetTraitDict()["fruit_rate"];
        float reproductionAge = this.phenotype.GetTraitDict()["reproduction_age"];

        if (age % growthRefreshRate == 0) { Grow(); }
        if ((age >= reproductionAge) && (age % fruitRate == 0)) { CreateFruit(); }

        IncreaseAge(1);
    }

    public void Grow(){
        // tree starts at scale of .1 in height (y) and width (x,z)
        // it has a max_height and max_width stored in this.phenotype.traitDict["max_height"] and this.phenotype.traitDict["max_width"]
        // every time grow is called, it should grow by its growth rate parameter (a float between 0 and 1)
        //      towards its max height and width. So for example, if its scale vector3 starts at [.1, .1, .1]
        //      if its max_height is 5 and its max_width is 4, and its this.phenotype.traitDict["growth_rate"] = .01
        //      then its new height should be y += .01*(5-.1) = .149
        //      its new width should be x,z += .01*(4-.1) = .139
    }

    public void CreateFruit() {
        string fruitType = GetGenome().GetQualDict()["fruit_type"];
        
        if (fruitType != "None") {
            // Figure out where the apple should appear
            float distance = GetPhenotype().GetTraitDict()["fruit_drop_distance"];
        
            Vector3 fruitDisplacement = new Vector3(Random.Range(-distance,distance),0,Random.Range(-distance,distance));                           
            Vector3 fruitLocation = GetBody().globalPos.position + fruitDisplacement;
            Population population = new Population(fruitType, 1, 1, 0, fruitLocation.x, fruitLocation.z, 0, 0, 0, 0);
        }
    }

    public void SetTrunkSize(float y) {
        bool found = false;
        GameObject trunk = null;
        Transform toSearch = this.body.GetGameObject().transform;
        foreach (Transform child in toSearch) {
            if (child.gameObject.name == "Trunk") {
                found = true;
                trunk = child.gameObject;
            }
        }
        if (found) {
            trunk.transform.localScale = new Vector3(2,y,2);
        }
    }
}