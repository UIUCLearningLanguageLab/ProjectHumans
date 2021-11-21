
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Gene {
    
    public string label; //show
    public string geneType; //show
    public int geneSize;
    public BitArray geneSequence; //show
    public bool mutable; //show
    public float specifiedMean; 
    public float specifiedStdev;

    public float mutationRate = 0;//World.worldConfigDict["Mutation_Rate"];

    //I am not sure if this is working, need test later;
    public Gene ShallowCopy() {
        return (Gene) this.MemberwiseClone();
    }

    public void SexualReproduction(Gene fatherGene){
        // for each element in the gene sequence
        for (int i = 0; i < geneSize; i++){
            // randomly either keep the mother value, or switch to the father value
            if(Random.Range(0, 2) == 0 ){
                geneSequence[i] = fatherGene.geneSequence[i];
            }
            // draw a random float, if < mutation rate, flip the element to its opposite
            // Random random = new Random();
            float random = Random.value; // Returns a random number between 0.0 [inclusive] and 1.0 [inclusive]
            if (this.mutable){
                if (random < mutationRate){
                    geneSequence[i] = !geneSequence[i];
                }
            }
        }
    }

    public void SetImportedGeneInfo(string label, string[] geneInfo){
        if (geneInfo.Length != 5){
            string outputString = "ERROR: Gene " + label + "wrong number of arguments in config file";
            Debug.Log(outputString);
        } else {
            this.label = label;
            geneType = geneInfo[0];
            geneSize = int.Parse(geneInfo[1]);

            if (geneInfo[2] == "immutable") {
                mutable = true;
            } else { mutable = false; }

            specifiedMean = float.Parse(geneInfo[3]);
            specifiedStdev = float.Parse(geneInfo[4]);
            geneSequence = new BitArray(this.geneSize);
        }
    }

    public void GenerateGeneSequence(){
        for (int i = 0; i < this.geneSize; i++){
            int newValue = Random.Range(0, 2);
            if (newValue == 0) {
                geneSequence[i] = false;
            } else { geneSequence[i] = true; }
        }
    }
}
