//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//using System.Configuration;
//using System.Linq;
//using System.IO;
//using MathNet.Numerics.LinearAlgebra;
//using MathNet.Numerics.Distributions;
//using Random = UnityEngine.Random;

//public class OldWorld: MonoBehaviour {
//    [SerializeField]
//    public bool paused;
//    public GameObject mainCam;
    
//    public static int humanAIParam;
//    public static int maxEntities = 5000;
//    public static string biomeName;
//    public static string anthroBody;
//    System.Random rand = new System.Random();

//    /// This dict keeps track of the total number of each kind of object that has been created
//    // should all these things be static?
//    public static Dictionary < string, int > entityCountDict = new Dictionary < string, int > ();
//    public static Dictionary < string, int > startingCountsDict = new Dictionary < string, int > ();
//    public static Dictionary < string, Group > assortedGroupDict = new Dictionary < string, Group > ();
//    public static Dictionary < string, Population > populationDict = new Dictionary < string, Population > ();
//    public static Dictionary < string, float > worldConfigDict = new Dictionary < string, float > ();

//    /// This dict keeps track of data in world.config
//    public Dictionary < string, Dictionary < string, List < string >>> genomeInfoDict = new Dictionary < string, Dictionary < string, List < string >>> ();
//    public Dictionary < string, Dictionary < string, List < string >>> constantInfoDict = new Dictionary < string, Dictionary < string, List < string >>> ();

//    /// These lists keep track of entities needing an update each epoch
//    public static List < Entity > entityList = new List < Entity > ();
//    public static Dictionary < string, Entity > entityDict = new Dictionary < string, Entity > ();

//    /// <value>Setting initial world properties</value>
//    public static float worldSize;
//    public static float maxPosition;
//    public static float minPosition;

//    public static bool initWorld = false;

//    public Dictionary < string, AI > allAIDict;
//    public Dictionary < string, Body > allBodyDict;
//    public Dictionary < string, MotorSystem > allMotorDict;

//    private static bool updateCompleted = false;
//    public int updateCounter;

//    public static bool CheckUpdate() {
//    return updateCompleted;
//    }

//    void Start() {
//        mainCam = GameObject.Find("Main Camera");
//        CreateEntities();
//    }

//    void Update() {
        
//        if (initWorld) {
//            initWorld = false;
//            paused = true;
//            updateCounter = 0;

//            LoadWorldConfig();

//            worldSize = worldConfigDict["World_Size"];
//            maxPosition = worldSize / 2;
//            minPosition = -worldSize / 2;

//            CreateEntities();
            
//            MainUI.WakeUp();
            
//        }

//        //paused = MainUI.Check("isPaused");
//        if (!paused)
//        {
//            if (updateCompleted)
//            {
//                UpdateEntities();
//                updateCounter++;
                
//            }
//        }

//    }
//    void CreateEntities() {
//        foreach (KeyValuePair < string, Population > entry in populationDict) {
//        Population activePop = entry.Value;
            
//        string speciesType = entry.Key;
//        entityCountDict[speciesType] = 0;

//        if (startingCountsDict.ContainsKey(speciesType)) {
//        for (int i = 0; i < startingCountsDict[speciesType]; i++) {
//            if (i == 0) {
//            if (speciesType != "Human") {
//                SaveEntity(AddEntity(activePop, new Vector3(0f, 0f, 0f)), activePop);
//            } else {
//                            SaveEntity(AddEntity(activePop, null), activePop);
                            
//            }
//            } else {

//            SaveEntity(AddEntity(activePop, null), activePop);
//            }
//        }
//        } else {
//        SpawnGroups(activePop);
//        }
//    }
//    LogComment("All entities spawned");
//    updateCompleted = true;
//    }

//    public void SpawnGroups(Population population) {
//    Debug.Log("Trying to spawn clusters of " + population.name);
//    List < Vector3 > [] grid = InitGrid(population.numGroups, (int)(population.meanMembers * 2));
//    for (int i = 0; i < population.numGroups; i++) {
//        // Generate a normalized val for num members
//        Normal normalDist = new Normal(population.meanMembers, population.standardMembers);
//        double numMembers = normalDist.Sample();

//        int clusterIndex = (int) Random.Range(0, ((population.numGroups - 1) * (population.numGroups - 1)));
//        Group toAdd = new Group(population, (int) numMembers, population.spawnDensity, grid[clusterIndex][0]);
//        population.SaveGroup(toAdd);
//    }
//    }

//    public static void DestroyComponent(Component passed) {
//    Destroy(passed);
//    }

//    // Makes life easier for spawning without pop reference
//    public static Entity AddEntity(string species, Nullable < Vector3 > passedSpawn) {
        
//    if (populationDict.ContainsKey(species)) {
//        Population toPass = populationDict[species];
//        return AddEntity(toPass, passedSpawn);
//    } else {
//        LogComment("Error for " + species + ": Can't spawn something that doesn't exist!");
//    }
//    return null;
//    }

//    // For initial entity spawning
//    public static Entity AddEntity(Population population, Nullable < Vector3 > passedSpawn) {

//        int val = IndexEntity(population.name);
//    Vector3 spawn;
//    Genome motherGenome = new Genome();
//    motherGenome.InheritGenome(population.baseGenome, true);

//    if (passedSpawn.HasValue) {
//        spawn = (Vector3) passedSpawn;
//    } 
//    else {
//        spawn = CreateRandomPosition();
//    }

//    if (population.entityType == "item") {
//        return new Item(population.name, val, motherGenome, spawn);
//    } 
//    else {
//            Genome fatherGenome = new Genome();
//        fatherGenome.InheritGenome(population.baseGenome, true);

//        if (population.entityType == "plant") {
//        return new Plant(population.name, val, motherGenome, fatherGenome, spawn);
//        } else {
//                return new Animal(population.name, val, motherGenome, fatherGenome, spawn);
//            }
//    }
//    }

//    // For sexual reproduction
//    public static Entity AddEntity(Population population, Vector3 passedSpawn, Genome mother, Genome father, Vector3 spawn) {
//    int val = IndexEntity(population.name);
//    if (population.entityType == "item") {
//        return new Item(population.name, val, mother, spawn);
//    } else {
//        if (population.entityType == "plant") {
//        return new Plant(population.name, val, mother, father, spawn);
//        } else {
//        return new Animal(population.name, val, mother, father, spawn);
//        }
//    }
//    }

//    public static void SaveEntity(Entity newEnt, Population pop) {
//    pop.SaveEntity(newEnt);
//    entityList.Add(newEnt);
//    entityDict[newEnt.GetName()] = newEnt;
//    entityCountDict[pop.name]++;
//    }

//    public static void RemoveEntity(string name) {
//    for (int i = 0; i < entityList.Count; i++) {
//        if (entityList[i].GetName() == name) {
//        Destroy(entityList[i].GetGameObject());
//        entityList.RemoveAt(i);
//        }
//    }
//    }

//    public static Entity GetEntity(string name) {
//    return entityDict[name];
//    }

//    public static int IndexEntity(string species) {
//    int val = 0;
//    if (entityCountDict.ContainsKey(species)) {
//        val = (entityCountDict[species]);
//    } else {
//        entityCountDict.Add(species, val);
//    }
//    return val;
//    }

//    public void UpdateEntities() {
//    updateCompleted = false;
//    foreach(KeyValuePair < string, Population > entry in populationDict) {
//        Population activePop = entry.Value;
//        int modCheck = activePop.GetUpdateRate();
//        if (updateCounter % modCheck == 0) {
//        LogComment("Updating population: " + activePop.name);
//        activePop.UpdatePopulation();
//        }
//    }
//    updateCompleted = true;
//    }

//    public static void InitNumSpecies(string species, int passed) {
//    LogComment("Update species spawn " + species + " to: " + passed);
//    if (startingCountsDict.ContainsKey(species)) {
//        startingCountsDict[species] = passed;
//    } else {
//        startingCountsDict.Add(species, passed);
//    }
//    }

//    public static void SetBiome(string type) {
//    biomeName = type;
//    }

//    public static void SetHumanBodies(string type) {
//    anthroBody = type;
//    }

//    public static void SetHumanAI(int param) {
//    humanAIParam = param;
//    }

//    public static AnimalBody GetAnimalBody(Animal passed, Vector3 spawn, string species) {
//    AnimalBody toReturn;
//    if(species == "Human") {
//        if (anthroBody == "SimpleHuman") {
//        toReturn = new PrimateBody(passed, spawn);
//        } else { toReturn = new PrimateBody(passed, spawn); }
//    } else { toReturn = new AnimalBody(passed, spawn); }
//    return toReturn;
//    }

//    public static MotorSystem GetMotor(Animal passed, string species) {
//    MotorSystem toReturn;

//    if(species == "Human") {
//        if (anthroBody == "SimpleHuman") {
//        toReturn = new SimplePrimateMotorSystem(passed);
//        } else { toReturn = new PrimateMotorSystem(passed); }
//    } else { toReturn = new SimpleMotorSystem(passed); }
//    return toReturn;
//    }
    
//    public void LoadWorldConfig() {
//    string worldName = "world.config";
//    worldName = biomeName + worldName;

//    string line;
//    string[] lineInfo;

//    using(var reader = new StreamReader(@"Assets/Scripts/config/" + worldName)) {
//        while ((line = reader.ReadLine()) != null) {
            
//            lineInfo = line.Split(new [] {
//                "="
//            }, StringSplitOptions.None);
//            string[] leftArray = lineInfo[0].Split(new [] {
//                ","
//            }, StringSplitOptions.None);
//            string[] rightArray = lineInfo[1].Split(new [] {
//                ","
//            }, StringSplitOptions.None);
//            if (rightArray.Length < 5) {
//                if (leftArray[0] != "Constant") {
//                InitNumSpecies(leftArray[1], Int32.Parse(rightArray[0]));
//                Population temp = new Population(leftArray[1]);
//                SavePopulation(temp);
//                } else if (leftArray[0] == "Constant") {
//                worldConfigDict.Add(leftArray[1], float.Parse(rightArray[0]));
//                }

//            } else {
//                int groups = Int32.Parse(rightArray[0]);
//                float mean = float.Parse(rightArray[1]);
//                float std = float.Parse(rightArray[2]);
//                float density = float.Parse(rightArray[3]);
//                int refresh = Int32.Parse(rightArray[4]);
//                Population temp = new Population(leftArray[1], groups, mean, std, density, refresh);
//                SavePopulation(temp);
//            }
//            }
//        }
//    }

//    public static void SavePopulation(Population toSave) {
//    populationDict[toSave.name] = toSave;
//    }

//    public static Population GetPopulation(string species) {
//    if (populationDict.ContainsKey(species)) {
//        return populationDict[species];
//    }
//    return null;
//    }

//    public static List < string > GetPopulationNames() {
//    return new List < string > (populationDict.Keys);
//    }

//    public static string NameGroup() {
//    return ("Assorted Group " + assortedGroupDict.Count());
//    }

//    public static void SaveGroup(Group passed) {
//    assortedGroupDict.Add(passed.GetName(), passed);
//    }

//    public static Vector3 CreateRandomPosition() {
//    float xRan = Random.Range(World.minPosition, World.maxPosition);
//    float zRan = Random.Range(World.minPosition, World.maxPosition);
//    Vector3 newStartPosition = new Vector3(xRan, 0.75f, zRan);

//    return newStartPosition;
//    }

//    public static Quaternion CreateRandomRotation() {
//    var startRotation = Quaternion.Euler(0.0f, Random.Range(World.minPosition, World.maxPosition), 0.0f);
//    return startRotation;
//    }

//    public static void PrintStates(Dictionary < string, float > thisStateDict) {
//    foreach(KeyValuePair < string, float > entry in thisStateDict) {
//        LogComment(entry.Key + " " + entry.Value);
//    }
//    }

//    public List < Vector3 > [] InitGrid(int gridWidth, int numLocations) {
//    // Okay so we're going to divide the map into a grid
//    // Each of those 64 zones will have an array of 10 random locations within the zone

//    int gridDiv = gridWidth;
//    float bump = maxPosition / (gridDiv / 2);
//    int numLocs = numLocations;
//    List < Vector3 > [] grid = new List < Vector3 > [gridDiv * gridDiv];

//    for (int i = 0; i < grid.Length; i++) {
//        grid[i] = new List < Vector3 > ();
//    }

//    int index = 0;
//    for (int xN = 0; xN < gridDiv - 1; xN++) {
//        float minX = minPosition + (xN * bump);
//        float maxX = minPosition + ((xN + 1) * bump);

//        // Technically in unity this is the Z coord, but a 2D plane was being imagined for development
//        for (int yN = 0; yN < gridDiv - 1; yN++) {
//        float minY = minPosition + (yN * bump);
//        float maxY = minPosition + ((yN + 1) * bump);

//        for (int j = 0; j < numLocs; j++) {

//            float xRan = Random.Range(minX, maxX);
//            float zRan = Random.Range(minY, maxY);
//            Vector3 addPos = new Vector3(xRan, 0, zRan);

//            grid[index].Add(addPos);
//            //AddEntity("PinkCube", addPos);
//        }
//        index++;
//        }
//    }
//    return grid;
//    }

//    public static void DisplayError() {
//    LogComment("Oops! Can't do that now.");
//    }

//    public static void LogComment(string comment) {
//    using(StreamWriter writetext = new StreamWriter("runtime.txt")) {
//        //Debug.Log("Writing to log: " + comment);
//        string toSend = DateTime.Now.ToString() + ":\t" + comment;
//        writetext.WriteLine(toSend);
//    }
//    }
//}