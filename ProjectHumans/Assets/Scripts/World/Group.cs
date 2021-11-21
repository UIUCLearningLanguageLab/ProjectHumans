using System;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.Distributions;

public class Group {
    protected string name;
    World world;
    public string GetName() { return name; }

    protected static List<Entity> memberList = new List<Entity>();
    protected static Dictionary<string, Entity> memberDict = new Dictionary<string, Entity>();
    public static List<Entity> GetEntityList() { return memberList; }
    public Group(Population population, int numEntities, float groupX, float groupZ, float stdevEntityX, float stdevEntityZ, World World) {
        name = population.NameGroup();
        this.world = World;
        SpawnMembers(population, numEntities, groupX, groupZ, stdevEntityX, stdevEntityX);
    }
    public Group(Population population, int numEntities, float groupX, float groupZ, float stdevEntityX, float stdevEntityZ)
    {
        name = population.NameGroup();
        SpawnMembersAfter(population, numEntities, groupX, groupZ, stdevEntityX, stdevEntityX);
    }

    public void SpawnMembers(Population population, int numEntities, float groupX, float groupZ,float stdevEntityX, float stdevEntityZ) {

        for (int i = 0; i < numEntities; i++) {

            float entityX = (float)new Normal(groupX, stdevEntityX).Sample();
            float entityZ = (float)new Normal(groupZ, stdevEntityZ).Sample();
            string index = population.popIndex.ToString() + " " + i.ToString();
            Entity newMember = population.AddEntity(new Vector3(entityX, 0, entityZ), index);
            world.SaveEntity(newMember);
        }
    }
    public void SpawnMembersAfter(Population population, int numEntities, float groupX, float groupZ, float stdevEntityX, float stdevEntityZ)
    {

        for (int i = 0; i < numEntities; i++)
        {

            float entityX = (float)new Normal(groupX, stdevEntityX).Sample();
            float entityZ = (float)new Normal(groupZ, stdevEntityZ).Sample();
            
            Entity newMember = population.AddEntity(new Vector3(entityX, 0, entityZ), i.ToString());
            if(newMember.GetGameObject() != null)
            {
                World world = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<World>();
                world.SaveEntity(newMember);
            }
        }
    }
}