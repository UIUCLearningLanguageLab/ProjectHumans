using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Platonic_manager_test : MonoBehaviour
{
    [SerializeField] Camera platonicCamera;
    [SerializeField] Light mainLight;
    public Dictionary<GameObject, Color[]> instanceImageDict;
    List<GameObject> allObjectsInScene = new List<GameObject>();
    private void Awake()
    {
        instanceImageDict = new Dictionary<GameObject, Color[]>();
        //Instantiate humans
        GameObject humanMale1 = Instantiate(Resources.Load<GameObject>("Prefabs/SimpleHumanMalePrefab"));
        humanMale1.name = "humanMale1";
        humanMale1.transform.position = new Vector3(1, 0.8f, 0);
        allObjectsInScene.Add(humanMale1);

        //Instantiate apples
        GameObject apple1 = Instantiate(Resources.Load<GameObject>("Prefabs/ApplePrefab"));
        apple1.name = "apple1";
        apple1.transform.position = new Vector3(5, 0.55f, 0);
        allObjectsInScene.Add(apple1);

        //Instantiate trees
        GameObject treeRound1 = Instantiate(Resources.Load<GameObject>("Prefabs/TreeRoundPrefab"));
        treeRound1.name = "treeRound1";
        treeRound1.transform.position = new Vector3(10, 0.6f, 0);
        allObjectsInScene.Add(treeRound1);

        //Instantiate flowers
        GameObject flower1 = Instantiate(Resources.Load<GameObject>("Prefabs/FlowerPrefab"));
        flower1.name = "flower1";
        flower1.transform.position = new Vector3(15, 0.64f, 0);
        allObjectsInScene.Add(flower1);

        platonicCamera.targetTexture = new RenderTexture(32, 32, 24);
        foreach (GameObject gameObject in allObjectsInScene)
        {
            int defaultLayer = gameObject.layer;
            gameObject.layer = 10;
            SetChildLayers(gameObject.transform, 10);

            Collider objCollider = gameObject.GetComponent<Collider>();
            platonicCamera.transform.SetParent(gameObject.transform);
            platonicCamera.transform.localPosition = new Vector3(0, objCollider.bounds.size.y / 2, (objCollider.bounds.size.z / 2) + 1f);
            platonicCamera.transform.localEulerAngles = new Vector3(0, 180, 0);
            platonicCamera.orthographicSize = objCollider.bounds.size.y + 0.1f;
            mainLight.transform.eulerAngles = new Vector3(45, platonicCamera.transform.eulerAngles.y, 0);

            Texture2D image = new Texture2D(32, 32, TextureFormat.RGB24, false);
            platonicCamera.Render();
            RenderTexture.active = platonicCamera.targetTexture;
            image.ReadPixels(new Rect(0, 0, 32, 32), 0, 0);
            Color[] colorArray = image.GetPixels();
            instanceImageDict.Add(gameObject, colorArray);
            //SaveVisualImage(image, gameObject.name);

            gameObject.layer = defaultLayer;
            SetChildLayers(gameObject.transform, defaultLayer);
        }
    }
    void SetChildLayers(Transform trans, int layer)
    {
        foreach (Transform t in trans)
        {

            t.gameObject.layer = layer;

            if (t.childCount != 0)
            {
                SetChildLayers(t, layer);
            }
        }
    }
    void SaveVisualImage(Texture2D visualInputTexture, string name)
    {
        byte[] bytes = visualInputTexture.EncodeToPNG();
        string fileName = GetImageName(name);
        System.IO.File.WriteAllBytes(fileName, bytes);
    }

    string GetImageName(string name)
    {
        return string.Format("{0}/PlatonicImages/platonicImage{1}x{2}_{3}_{4}.png",
        Application.dataPath,
        32,
        32,
        name,
        System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
    }
}
