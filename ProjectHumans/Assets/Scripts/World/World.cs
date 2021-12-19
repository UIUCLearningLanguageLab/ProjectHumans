using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.IO;
using TMPro;
using System.Linq;


public class World: MonoBehaviour {
    [SerializeField] public Transform settings;
    GameObject mainCam;
    TMP_Dropdown worldSelectionDD;
    TMP_Dropdown aISelectionDD;


    public static bool gameIsPaused;
    public static string aISelected;
    public static string worldSelected;
    public bool updateIsPaused;
    public Dictionary < string, float > worldConfigDict = new Dictionary < string, float > ();
    public Dictionary < string, Population > populationDict = new Dictionary < string, Population > ();
    public List<string> populationList = new List<string>();

    /// These list keep track of entities needing an update each epoch
    public List < Entity > entityList = new List < Entity > ();
    public Dictionary < string, Entity > entityDict = new Dictionary < string, Entity > ();

    /// <value>Setting initial world properties</value>
    public float worldSize;
    public float maxPosition;
    public float minPosition;

    public int updateCounter;

    void Awake() {
        PauseGame();
        mainCam = GameObject.Find("Main Camera");
        worldSelectionDD = settings.Find("WorldSelection").GetComponent<TMP_Dropdown>();
        aISelectionDD = settings.Find("AISelection").GetComponent<TMP_Dropdown>();
        string[] Worldfolder = Directory.GetDirectories(Application.streamingAssetsPath + "//Config/Worlds");
        foreach (string folder in Worldfolder)
        {
            worldSelectionDD.options.Add(new TMP_Dropdown.OptionData() { text = new DirectoryInfo(folder).Name });
        }
        settings.Find("Title").GetComponent<TextMeshProUGUI>().text = new DirectoryInfo(Worldfolder[0]).Name;
        //string[] AIfiles = Directory.GetFiles(Application.streamingAssetsPath + "//AI/AIInUse/", "*.cs");



        //foreach (string file in AIfiles)
        //{
        //    aISelectionDD.options.Add(new TMP_Dropdown.OptionData() { text = Path.GetFileNameWithoutExtension(file) });
        //}
        aISelectionDD.options.Add(new TMP_Dropdown.OptionData() { text = "SimpleAI" });
        aISelectionDD.options.Add(new TMP_Dropdown.OptionData() { text = "NeuralAI" });
        updateCounter = 0;
    }
    void Start()
    {
        StartCoroutine(GetOptions());
    }
    void Update()
    {
        if(!updateIsPaused)
        {
            UpdateEntities();
        }
    }
    IEnumerator GetOptions()
    {
        
        yield return new WaitUntil(() => gameIsPaused == false);
        LoadWorldConfig();
        worldSize = worldConfigDict["World_Size"];
        maxPosition = worldSize / 2;
        minPosition = -worldSize / 2;
        updateIsPaused = true;
    }
    public void BuildTheScene()
    {
        ResumeGame();
        settings.transform.localScale = new Vector3(0, 0, 0);
    }
    public void PauseGame()
    {
        Time.timeScale = 0f;
        gameIsPaused = true;
        Debug.Log("game is paused");
    }
    public void ResumeGame()
    {
        Time.timeScale = 1f;
        gameIsPaused = false;
        Debug.Log("game is resumed");
    }

    void UpdateEntities()
    {
        foreach (Entity entity in entityList.ToList())
        {
            entity.UpdateEntity();
        }
    }
    public void LoadWorldConfig() {
        worldSelected = worldSelectionDD.options[worldSelectionDD.value].text;
        aISelected = aISelectionDD.options[aISelectionDD.value].text;
        string line;
        string[] lineInfo;

        int groupCount = 0;
        string speciesName = "None";
        string populationNumber = "None";

        // WE NEED TO ADD ERROR CHECKING FOR SCREWED UP CONFIG FILES
        
        using(var reader = new StreamReader(Application.streamingAssetsPath + "/Config/Worlds/" + worldSelected + "/world.config")) {
            while ((line = reader.ReadLine()) != null) {
                
                lineInfo = line.Split(new [] {"="}, StringSplitOptions.None);
                string[] leftArray = lineInfo[0].Split(new [] {","}, StringSplitOptions.None);
                string[] rightArray = lineInfo[1].Split(new [] {","}, StringSplitOptions.None);

                if (leftArray[0] == "Constant"){
                    worldConfigDict.Add(leftArray[1], float.Parse(rightArray[0]));
                }
                else{
                    groupCount = Int32.Parse(rightArray[0]);
                    populationNumber = leftArray[2];
                    speciesName = leftArray[1];
                    int numGroups = Int32.Parse(rightArray[0]);
                    int meanGroupSize = Int32.Parse(rightArray[1]);
                    float stdevGroupSize = float.Parse(rightArray[2]);
                    int meanGroupX = Int32.Parse(rightArray[3]);
                    int meanGroupZ = Int32.Parse(rightArray[4]);
                    float stdevGroupX = float.Parse(rightArray[5]);
                    float stdevGroupZ = float.Parse(rightArray[6]);
                    float stdevEntityX = float.Parse(rightArray[7]);
                    float stdevEntityZ = float.Parse(rightArray[8]);
                    populationList.Add(speciesName);
                    var dupList = populationList.GroupBy(x => x).Where(g => g.Count() > 1).Select(y => new { Element = y.Key, Counter = y.Count() }).ToList();
                    int popNum = 0;
                    foreach(var x in dupList)
                    {
                        if(x.Element == speciesName)
                        {
                            popNum = x.Counter - 1;
                        }
                    }
                    Population newPopulation = new Population(this, popNum, leftArray[1], numGroups, meanGroupSize, stdevGroupSize, meanGroupX, meanGroupZ, stdevGroupX, stdevGroupZ, stdevEntityX, stdevEntityZ);
                }
            }
        }
    }
    public void SaveEntity(Entity newEnt)
    {
        entityList.Add(newEnt);
        entityDict[newEnt.GetName()] = newEnt;
    }
    public static void LogComment(string comment) {
        using(StreamWriter writetext = new StreamWriter("runtime.txt")) {
            //Debug.Log("Writing to log: " + comment);
            string toSend = DateTime.Now.ToString() + ":\t" + comment;
            writetext.WriteLine(toSend);
        }
    }
    public static Quaternion CreateRandomRotation()
    {
        var startRotation = Quaternion.Euler(0.0f, UnityEngine.Random.Range(180, -180), 0.0f);
        return startRotation;
    }
}