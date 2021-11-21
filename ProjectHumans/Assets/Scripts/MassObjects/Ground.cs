using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : Entity
{
    public Ground(string objectType, string index, Genome motherGenome, Vector3 spawn)
    : base(objectType, index, motherGenome, spawn, false)
    {
        body = new Body(this, spawn);
    }

    public override void UpdateEntity()
    {
        if (this.GetSpecies() == "Crabapple")
        {
            this.gameObject.GetComponent<MeshRenderer>().material.color = new Color(1f, 0.75f, .25f, 1f);
        }
    }
}
