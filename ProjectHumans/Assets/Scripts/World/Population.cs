using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Configuration;
using System.Linq;
using System.IO;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.Random;
using Random=UnityEngine.Random;

public class Population {
    string SpeciesName;
    int numGroups, meanGroupSize;
    float stdevGroupSize, stdevGroupX, stdevGroupZ, stdevEntityX, stdevEntityZ, meanGroupX, meanGroupZ;
    World world;
    public int popIndex;
    public Genome baseGenome;
    public string entityType;
    public bool isFirst = false;

    protected List<Entity> entityList = new List<Entity>();
    protected Dictionary<string, Entity> entityDict = new Dictionary<string, Entity>();
    protected Dictionary<string, Group> groupDict = new Dictionary<string, Group>();

    public  List<Entity> GetEntityList() { return entityList; }
    public List<string> GetEntityNames() { return new List<string>(entityDict.Keys); }

    public Population(World World, int popIndex, string passedSpeciesName, int numGroups, int meanGroupSize, float stdevGroupSize,
        float meanGroupX, float meanGroupZ,  float stdevGroupX, float stedvGroupZ, 
        float stdevEntityX, float stdevEntityZ) {
        baseGenome = new Genome();
        
        this.world = World;
        this.popIndex = popIndex;
        this.SpeciesName = passedSpeciesName;
        this.numGroups = numGroups;
        this.meanGroupSize = meanGroupSize;
        this.meanGroupX = meanGroupX;
        this.meanGroupZ = meanGroupZ;
        this.stdevGroupSize = stdevGroupSize;
        this.stdevGroupX = stdevGroupX;
        this.stdevGroupZ = stedvGroupZ;
        this.stdevEntityX = stdevEntityX;
        this.stdevEntityZ = stdevEntityZ;
        ImportPopConfig();
        SpawnGroups();
    }
    public Population(string passedSpeciesName, int numGroups, int meanGroupSize, float stdevGroupSize,
        float meanGroupX, float meanGroupZ, float stdevGroupX, float stedvGroupZ,
        float stdevEntityX, float stdevEntityZ)
    {
        baseGenome = new Genome();

        this.SpeciesName = passedSpeciesName;
        this.numGroups = numGroups;
        this.meanGroupSize = meanGroupSize;
        this.meanGroupX = meanGroupX;
        this.meanGroupZ = meanGroupZ;
        this.stdevGroupSize = stdevGroupSize;
        this.stdevGroupX = stdevGroupX;
        this.stdevGroupZ = stedvGroupZ;
        this.stdevEntityX = stdevEntityX;
        this.stdevEntityZ = stdevEntityZ;
        ImportPopConfig();
        SpawnGroupsAfter();
    }

    public void UpdatePopulation() {
        World.LogComment("Starting updating population: " + SpeciesName);
        foreach(Entity individual in entityList) {
            individual.UpdateEntity();
        }
        World.LogComment("Population update completed.");
    }
    public Entity AddEntity( Nullable<Vector3> passedSpawn, string index)
    {
        Vector3 spawn;
        Genome motherGenome = new Genome();
        motherGenome.InheritGenome(baseGenome, true);

        if (passedSpawn.HasValue)
        {
            spawn = (Vector3)passedSpawn;
        }
        else
        {
            spawn = CreateRandomPosition();
        }

        if (entityType == "item")
        {
            return new Item(SpeciesName, index, motherGenome, spawn);
        }
        else
        {
            Genome fatherGenome = new Genome();
            fatherGenome.InheritGenome(baseGenome, true);

            if (entityType == "plant")
            {
                return new Plant(SpeciesName, index, motherGenome, fatherGenome, spawn);
            }
            else
            {
                return new Animal(SpeciesName, index, motherGenome, fatherGenome, spawn);
            }
        }
    }
 

    public void ImportPopConfig()
    {
        string line;
        string[] lineInfo;
        string filename = SpeciesName + ".config";
        string[] worldSelectedFiles = Directory.GetFiles(Path.Combine(Application.streamingAssetsPath, "Config/Worlds") + "/" + World.worldSelected, "*.config");
        StreamReader reader = null;
        if(worldSelectedFiles.Contains(filename))
        {
            reader = new StreamReader(@"Assets/Scripts/Config/Worlds/" + World.worldSelected + filename);
        }
        else
        {
            reader = new StreamReader(@"Assets/Scripts/Config/Worlds/Default/" + filename);
        }
        using (reader)
        {
            while ((line = reader.ReadLine()) != null)
            {
                lineInfo = line.Split(new[] { "=" }, StringSplitOptions.None);
                string[] leftArray = lineInfo[0].Split(new[] { "." }, StringSplitOptions.None);
                string[] rightArray = lineInfo[1].Split(new[] { "," }, StringSplitOptions.None);

                if (leftArray[0] == "gene")
                {
                    baseGenome.AddGeneToGenome(leftArray[1], rightArray);
                }
                else if (leftArray[0] == "constant")
                {
                    baseGenome.AddConstantToGenome(leftArray[1], rightArray);
                }
                else if (leftArray[0] == "quality")
                {
                    baseGenome.AddQualToGenome(leftArray[1], rightArray);
                }
                else if (leftArray[0] == "object_type")
                {
                    entityType = rightArray[0];
                    //Debug.Log("Saving object type for " + name + " as " + entityType);
                }
            }
        }
    }

    public void SaveGroup(Group passed) {
        groupDict.Add(passed.GetName(), passed);
    }

    public string NameGroup() {
        string toName = SpeciesName + " Group " + groupDict.Count();
        return toName;
    }
    public void SpawnGroups()
    {
        //Debug.Log("Trying to spawn clusters of " + SpeciesName);
        for (int i = 0; i < numGroups; i++)
        {
            int groupSize = (int)new Normal(meanGroupSize, stdevGroupSize).Sample();
            float groupX = (float)new Normal(meanGroupX, stdevGroupX).Sample();
            float groupZ = (float)new Normal(meanGroupZ, stdevGroupZ).Sample();
            //int clusterIndex = (int)Random.Range(0, ((numGroups - 1) * (numGroups - 1)));
            Group toAdd = new Group(this, groupSize, groupX, groupZ, stdevEntityX, stdevEntityZ, world);
            SaveGroup(toAdd);
        }
    }
    public void SpawnGroupsAfter()
    {
        //Debug.Log("Trying to spawn clusters of " + SpeciesName);
        for (int i = 0; i < numGroups; i++)
        {
            int groupSize = (int)new Normal(meanGroupSize, stdevGroupSize).Sample();
            float groupX = (float)new Normal(meanGroupX, stdevGroupX).Sample();
            float groupZ = (float)new Normal(meanGroupZ, stdevGroupZ).Sample();
            //int clusterIndex = (int)Random.Range(0, ((numGroups - 1) * (numGroups - 1)));
            Group toAdd = new Group(this, groupSize, groupX, groupZ, stdevEntityX, stdevEntityZ);
            SaveGroup(toAdd);
        }
    }
    public Vector3 CreateRandomPosition()
    {
        float xRan = Random.Range(world.minPosition, world.maxPosition);
        float zRan = Random.Range(world.minPosition, world.maxPosition);
        Vector3 newStartPosition = new Vector3(xRan, 0.75f, zRan);

        return newStartPosition;
    }
}