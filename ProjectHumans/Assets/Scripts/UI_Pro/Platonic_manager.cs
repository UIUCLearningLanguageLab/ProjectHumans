using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MathNet.Numerics.LinearAlgebra;
using System.Linq;

public class Platonic_manager : MonoBehaviour
{
    [SerializeField] World world;
    [SerializeField] Sprite dotSprite;
    [SerializeField] Camera platonicCamera;
    [SerializeField] Light mainLight;
    [SerializeField] UIController uiController;
    [SerializeField] GameObject semanticGameObject;
    [SerializeField] Transform reference;
    [SerializeField] TMP_InputField x, y, z;
    public Dictionary<GameObject, Matrix<float>> instanceKnowledgeDict;
    List<Transform> referenceList = new List<Transform>();
    List<GameObject> dotList = new List<GameObject>();
    private void Awake()
    {
        CreateAxis("zAxis", 90);
        CreateAxis("yAxis", 0);
        CreateAxis("xAxis", -135);
    }
    public void ClearGraph()
    {
        if (dotList.Count > 0)
        {
            foreach (GameObject x in dotList)
            {
                Destroy(x);
            }
            dotList.Clear();
        }
    }
    public void DrawGraph()
    {
        foreach (Transform transform in referenceList)
        {
            Destroy(transform.gameObject);
        }
        referenceList.Clear();

        SetInputDefaultValue(x, 1);
        SetInputDefaultValue(y, 2);
        SetInputDefaultValue(z, 3);

        world.PauseGame();
        List<Entity> entityList = world.entityList;
        Entity selectedEntity = uiController.selectedEntity;
        AI selectedAI = ((Animal)selectedEntity).GetAI();
        Matrix<float> visualHiddenLayer = ((NeuralAI)selectedAI).FeedForwardPlatonicImages();
        Matrix<float> driveHiddenLayer = ((NeuralAI)selectedAI).FeedForwardDriveStates();
        Matrix<float> semanticKnowledge = visualHiddenLayer.Transpose().Append(driveHiddenLayer.Transpose()).Transpose().Svd().U;
        var query = entityList.GroupBy(
        entity => entity.GetSpecies(),
        (species, entity) => new
        {
            Key = species,
            entity = entity
        });
        foreach (var result in query)
        {
            Color color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
            Transform colorReference = Instantiate(reference.GetChild(0),reference);
            referenceList.Add(colorReference);
            colorReference.GetChild(0).GetComponent<TextMeshProUGUI>().text = result.Key;
            colorReference.GetChild(1).GetComponent<RawImage>().color = color;
            colorReference.gameObject.SetActive(true);
            foreach(Entity entity in result.entity)
            {
                int index = entityList.IndexOf(entity);
                CreateDot(semanticKnowledge[index, int.Parse(x.text)], semanticKnowledge[index, int.Parse(y.text)], semanticKnowledge[index, int.Parse(z.text)], color);
            }
        }
        int driveIndex = 0;
        foreach(Vector<float> rows in semanticKnowledge.EnumerateRows(entityList.Count - 1, semanticKnowledge.RowCount - entityList.Count))
        {
            Color color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
            Transform colorReference = Instantiate(reference.GetChild(0), reference);
            referenceList.Add(colorReference);
            colorReference.GetChild(0).GetComponent<TextMeshProUGUI>().text = ((Animal)selectedEntity).GetDriveSystem().GetStateLabels()[driveIndex];
            colorReference.GetChild(1).GetComponent<RawImage>().color = color;
            colorReference.gameObject.SetActive(true);
            CreateDot(driveHiddenLayer[driveIndex, int.Parse(x.text)], driveHiddenLayer[driveIndex, int.Parse(y.text)], driveHiddenLayer[driveIndex, int.Parse(z.text)], color);
            driveIndex++;
        }
    }
    void SetInputDefaultValue(TMP_InputField inputField, int value)
    {
        if(string.IsNullOrEmpty(inputField.text))
        {
            inputField.text = value.ToString();
        }
    }
    public void GetSemanticKnowledge()
    {
        Entity selectedEntity = uiController.selectedEntity;
        int visualResolution = ((Animal)selectedEntity).GetSensorySystem().visualResolution;
        int resolutionSquared = visualResolution * visualResolution;
        instanceKnowledgeDict = new Dictionary<GameObject, Matrix<float>>();
        platonicCamera.clearFlags = CameraClearFlags.SolidColor;
        platonicCamera.targetTexture = new RenderTexture(32, 32, 24);

        foreach (Entity entity in world.entityList)
        {
            GameObject gameObject = entity.GetGameObject();
            int defaultLayer = gameObject.layer;
            gameObject.layer = 10;
            SetChildLayers(gameObject.transform, 10);
            Collider objCollider = null;
            if (gameObject.GetComponent<Collider>())
            {
                objCollider = gameObject.GetComponent<Collider>();
            }
            else
            {
                objCollider = gameObject.GetComponentInChildren<Collider>();
            }
            
            platonicCamera.transform.SetParent(gameObject.transform);
            if (entity.GetGameObject().tag == "Ground")
            {
                platonicCamera.transform.localPosition = new Vector3(0, 850, 0);
                platonicCamera.transform.localEulerAngles = new Vector3(90, 0, 0);
                platonicCamera.orthographicSize = 500;
            }
            else
            {
                platonicCamera.transform.localPosition = new Vector3(0, objCollider.bounds.size.y / 2, (objCollider.bounds.size.z / 2) + 1f);
                platonicCamera.transform.localEulerAngles = new Vector3(0, 180, 0);
                platonicCamera.orthographicSize = objCollider.bounds.size.y + 0.1f;
            }
            mainLight.transform.eulerAngles = new Vector3(45, platonicCamera.transform.eulerAngles.y, 0);

            Texture2D image = new Texture2D(32, 32, TextureFormat.RGB24, false);
            platonicCamera.Render();
            RenderTexture.active = platonicCamera.targetTexture;
            image.ReadPixels(new Rect(0, 0, 32, 32), 0, 0);
            Color[] colorArray = image.GetPixels();

            Matrix<float> platonicInputArray = Matrix<float>.Build.Dense(3, visualResolution * visualResolution);
            for (int i = 0; i < resolutionSquared; i++)
            {
                platonicInputArray[0, i] = (colorArray[i].r * 2) - 1;
                platonicInputArray[1, i] = (colorArray[i].g * 2) - 1;
                platonicInputArray[2, i] = (colorArray[i].b * 2) - 1;
            }
            instanceKnowledgeDict.Add(gameObject, platonicInputArray);
            SaveVisualImage(image, gameObject.name);

            gameObject.layer = defaultLayer;
            SetChildLayers(gameObject.transform, defaultLayer);
        }
        //GetSkyImage(visualResolution);
    }
    void GetSkyImage(int visualResolution)
    {
        platonicCamera.clearFlags = CameraClearFlags.Skybox;
        Texture2D image = new Texture2D(32, 32, TextureFormat.RGB24, false);
        platonicCamera.Render();
        RenderTexture.active = platonicCamera.targetTexture;
        image.ReadPixels(new Rect(0, 0, 32, 32), 0, 0);
        Color[] colorArray = image.GetPixels();

        Matrix<float> platonicInputArray = Matrix<float>.Build.Dense(3, visualResolution * visualResolution);
        for (int i = 0; i < visualResolution * visualResolution; i++)
        {
            platonicInputArray[0, i] = (colorArray[i].r * 2) - 1;
            platonicInputArray[1, i] = (colorArray[i].g * 2) - 1;
            platonicInputArray[2, i] = (colorArray[i].b * 2) - 1;
        }
        instanceKnowledgeDict.Add(gameObject, platonicInputArray);
        SaveVisualImage(image, gameObject.name);
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
    private void CreateAxis(string name, float angle)
    {
        Vector2 origin = new Vector2(0.5f, 0.4f);
        GameObject axisObj = new GameObject(name, typeof(Image));
        axisObj.transform.SetParent(semanticGameObject.transform, false);
        RectTransform rectTransform = axisObj.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.4f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.4f);
        rectTransform.sizeDelta = new Vector2(550, 3f);
        Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
        rectTransform.anchoredPosition = origin + dir * 550 * .5f;
        rectTransform.localEulerAngles = new Vector3(0, 0, angle);
    }
    private GameObject CreateDot(float x, float y, float z, Color color)
    {
        GameObject dotObj = new GameObject("dot", typeof(Image));
        dotObj.transform.SetParent(semanticGameObject.transform, false);
        dotObj.GetComponent<Image>().sprite = dotSprite;
        dotObj.GetComponent<Image>().color = color;
        RectTransform rectTransform = dotObj.GetComponent<RectTransform>();
        float x2D = x * 100 - y * 100 * Mathf.Cos(Mathf.Deg2Rad * 45);
        float y2D = z * 100 - y * 100 * Mathf.Sin(Mathf.Deg2Rad * 45);
        rectTransform.anchoredPosition = new Vector2(x2D, y2D);
        rectTransform.sizeDelta = new Vector2(22, 22);
        rectTransform.anchorMin = new Vector2(0.5f, 0.4f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.4f);
        dotList.Add(dotObj);
        return dotObj;
    }
}

