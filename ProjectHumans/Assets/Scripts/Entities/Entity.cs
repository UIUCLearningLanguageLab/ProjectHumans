using System;
using UnityEngine;

abstract public class Entity {

    public string displayName;
    public string GetDisplayName() { return displayName; }
    public void SetDisplayName(string named) { displayName = named; }

    protected string species;
    public string GetSpecies() { return species;}

    protected string name;
    public string GetName() { return name; }
    public void SetName(String passed) { name = passed; }

    protected string index;
    public string GetIndex() { return index; }
    public void SetIndex(string passed) { index = passed; }

    protected int age;
    public int GetAge(){ return this.age; }
    public void IncreaseAge(int amount){ this.age += amount; }

    protected Genome genome;
    public Genome GetGenome() { return genome; }

    protected Phenotype phenotype;
    public Phenotype GetPhenotype() { return phenotype; }

    protected bool isAnimal;
    public bool CheckAnimal() { return isAnimal; }

    protected string thoughtStream = "";
    public string GetStream() { return thoughtStream; }
    public void AddThought(string thought) {
        //Debug.Log("adding a thought to " + name);
        string toUpdate = DateTime.Now.ToString("HH:mm:ss");
        thoughtStream += (thought + "\t" + toUpdate + "\n");
    }
    
    // Body variables
    protected GameObject gameObject;
    protected Body body;
    protected string networkName = "Unresponsive";

    public GameObject GetGameObject() { return gameObject; }
    public void SetGameObject(GameObject passed) { this.gameObject = passed; }
    public Body GetBody() { return body; }
    public virtual string GetNetworkName(){ return networkName; }

    // Sexual reproduction constructor
    public Entity(string objType, string id, Genome motherGenome, Genome fatherGenome, Vector3 spawn, bool isAnimal) {
        species = objType;
        index = id;
        name = (species + " " + index.ToString());
        displayName = name;
        this.isAnimal = isAnimal;
        
        genome = new Genome(motherGenome, fatherGenome);
        phenotype = new Phenotype(this);
    }

    // Asexual reproduction constructor
    public Entity(string objType, string id, Genome motherGenome, Vector3 spawn, bool isAnimal) {
        species = objType;
        index = id;
        name = (species + " " + index.ToString());
        displayName = name;
        this.isAnimal = isAnimal;

        genome = new Genome(motherGenome);
        phenotype = new Phenotype(this);
    }

    public virtual void UpdateEntity() {
        Debug.Log("No update defined for this entity");
    }

    public virtual void InitBody() {
        Debug.Log("This entity is not of this realm");
    }
    
}