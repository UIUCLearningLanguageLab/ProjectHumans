using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MathNet.Numerics.LinearAlgebra;
using TMPro;

public class Semantic_graph_test : MonoBehaviour
{
    [SerializeField] UIController uiController;
    [SerializeField] Sprite dotSprite;
    [SerializeField] TMP_Dropdown dropdown;
    //[SerializeField] TMP_InputField xInput, yInput, zInput;
    List<GameObject> dotList = new List<GameObject>();
    // Start is called before the first frame update
    private void Awake()
    {
        CreateAxis("zAxis", 90);
        CreateAxis("yAxis", 0);
        CreateAxis("xAxis", -135);
        
    }
    private void Update()
    {
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
        int index = dropdown.value;

        Entity selectedEntity = uiController.selectedEntity;
        AI selectedAI = ((Animal)selectedEntity).GetAI();
        Matrix<float> combinedHiddenLayer = ((NeuralAI)selectedAI).FeedForwardPlatonicImages();

        CreateDot(combinedHiddenLayer[index, 0], combinedHiddenLayer[index, 1], combinedHiddenLayer[index, 2]);
    }
    //public void drawGraph()
    //{
    //    CreateDot(float.Parse(xInput.text), float.Parse(yInput.text), float.Parse(zInput.text));
    //}
    private void CreateAxis(string name, float angle)
    {
        Vector2 origin = new Vector2(0.5f, 0.4f);
        GameObject axisObj = new GameObject(name, typeof(Image));
        axisObj.transform.SetParent(gameObject.transform, false);
        RectTransform rectTransform = axisObj.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.4f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.4f);
        rectTransform.sizeDelta = new Vector2(550, 3f);
        Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
        rectTransform.anchoredPosition = origin + dir * 550 * .5f;
        rectTransform.localEulerAngles = new Vector3(0, 0, angle);
    }
    private GameObject CreateDot(float x, float y, float z)
    {
        GameObject dotObj = new GameObject("dot", typeof(Image));
        dotObj.transform.SetParent(gameObject.transform, false);
        dotObj.GetComponent<Image>().sprite = dotSprite;
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
