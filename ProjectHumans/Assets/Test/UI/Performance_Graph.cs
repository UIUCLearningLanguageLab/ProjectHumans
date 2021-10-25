using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CodeMonkey.Utils;
using TMPro;

public class Performance_Graph : MonoBehaviour
{
    private static Performance_Graph instance;
    Entity selectedEntity;
    AI selectedAI;
    [SerializeField] UIController uicontroller;
    [SerializeField] World world;
    [SerializeField] private Sprite dotSprite;

    private Transform templates;
    private RectTransform lableTemplateX;
    private RectTransform lableTemplateY;
    private RectTransform dashTemplateX;
    private RectTransform dashTemplateY;

    private List<GameObject> redGameObjectList;
    private List<GameObject> greebGameObjectList;
    private List<GameObject> blueGameObjectList;
    private List<GameObject> bodyGameObjectList;
    private List<GameObject> driveGameObjectList;
    private List<GameObject> actionGameObjectList;
    private List<GameObject> allGameObjectList;

    private RectTransform redPerformance;
    private RectTransform greenPerformance;
    private RectTransform bluePerformance;
    private RectTransform bodyPerformance;
    private RectTransform drivePerformance;
    private RectTransform actionPerformance;
    private RectTransform allPerformance;

    private List<IGraphVisualObject> redGraphVisualObjectList;
    private List<IGraphVisualObject> greenGraphVisualObjectList;
    private List<IGraphVisualObject> blueGraphVisualObjectList;
    private List<IGraphVisualObject> bodyGraphVisualObjectList;
    private List<IGraphVisualObject> driveGraphVisualObjectList;
    private List<IGraphVisualObject> actionGraphVisualObjectList;
    private List<IGraphVisualObject> allGraphVisualObjectList;

    private IGraphVisual redLineGraphVisual;
    private IGraphVisual greenLineGraphVisual;
    private IGraphVisual blueLineGraphVisual;
    private IGraphVisual bodyLineGraphVisual;
    private IGraphVisual driveLineGraphVisual;
    private IGraphVisual actionLineGraphVisual;
    private IGraphVisual allLineGraphVisual;

    private GameObject tooltipGameObject;
    private List<RectTransform> yLabelList;
    private float xSize;
    private bool startYScaleAtZero;
    private InputField startXUI;
    private InputField endXUI;
    //private IGraphVisual lineGraphVisual;

    private List<float> valueList;
    private IGraphVisual graphVisual;

    
    private int startX;
    private int endX;
    private Func<int, string> getAxisLabelX;
    private Func<float, string> getAxisLabelY;
    private void Awake()
    {
        instance = this;
        templates = GameObject.Find("Templates").GetComponent<RectTransform>();
        lableTemplateX = templates.Find("LableTemplateX").GetComponent<RectTransform>();
        lableTemplateY = templates.Find("LableTemplateY").GetComponent<RectTransform>();
        dashTemplateX = templates.Find("DashTemplateX").GetComponent<RectTransform>();
        dashTemplateY = templates.Find("DashTemplateY").GetComponent<RectTransform>();
        tooltipGameObject = templates.Find("Tooltip").gameObject;

        redPerformance = GameObject.Find("RedPerformance").GetComponent<RectTransform>();
        greenPerformance = GameObject.Find("GreenPerformance").GetComponent<RectTransform>();
        bluePerformance = GameObject.Find("BluePerformance").GetComponent<RectTransform>();
        bodyPerformance = GameObject.Find("BodyPerformance").GetComponent<RectTransform>();
        drivePerformance = GameObject.Find("DrivePerformance").GetComponent<RectTransform>();
        actionPerformance = GameObject.Find("ActionPerformance").GetComponent<RectTransform>();
        allPerformance = GameObject.Find("AllPerformance").GetComponent<RectTransform>();

        redGraphVisualObjectList = new List<IGraphVisualObject>();
        greenGraphVisualObjectList = new List<IGraphVisualObject>();
        blueGraphVisualObjectList = new List<IGraphVisualObject>();
        bodyGraphVisualObjectList = new List<IGraphVisualObject>();
        driveGraphVisualObjectList = new List<IGraphVisualObject>();
        actionGraphVisualObjectList = new List<IGraphVisualObject>();
        allGraphVisualObjectList = new List<IGraphVisualObject>();

        redGameObjectList = new List<GameObject>();
        greebGameObjectList = new List<GameObject>();
        blueGameObjectList = new List<GameObject>();
        bodyGameObjectList = new List<GameObject>();
        driveGameObjectList = new List<GameObject>();
        actionGameObjectList = new List<GameObject>();
        allGameObjectList = new List<GameObject>();
        yLabelList = new List<RectTransform>();

        startYScaleAtZero = true;
        valueList = new List<float>() {1,2,3,4,5 };
        HideTooltip();

        redLineGraphVisual = new LineGraphVisual(redPerformance, dotSprite, Color.red, new Color(1, 1, 1, .5f));
        greenLineGraphVisual = new LineGraphVisual(greenPerformance, dotSprite, Color.green, new Color(1, 1, 1, .5f));
        blueLineGraphVisual = new LineGraphVisual(bluePerformance, dotSprite, Color.cyan, new Color(1, 1, 1, .5f));
        bodyLineGraphVisual = new LineGraphVisual(bodyPerformance, dotSprite, Color.white, new Color(1, 1, 1, .5f));
        driveLineGraphVisual = new LineGraphVisual(drivePerformance, dotSprite, Color.white, new Color(1, 1, 1, .5f));
        actionLineGraphVisual = new LineGraphVisual(actionPerformance, dotSprite, Color.white, new Color(1, 1, 1, .5f));
        allLineGraphVisual = new LineGraphVisual(allPerformance, dotSprite, Color.white, new Color(1, 1, 1, .5f));

        //ShowGraph(valueList, redLineGraphVisual, redPerformance, greenGraphVisualObjectList, redGameObjectList, 0, valueList.Count);
        //ShowGraph(valueList, greenLineGraphVisual, greenPerformance, redGraphVisualObjectList, greebGameObjectList, 0, valueList.Count);
        //ShowGraph(valueList, blueLineGraphVisual, bluePerformance, blueGraphVisualObjectList, blueGameObjectList, 0, valueList.Count);
        //ShowGraph(valueList, bodyLineGraphVisual, bodyPerformance, bodyGraphVisualObjectList, bodyGameObjectList, 0, valueList.Count);
        //ShowGraph(valueList, driveLineGraphVisual, drivePerformance, driveGraphVisualObjectList, driveGameObjectList , 0, valueList.Count);
        //ShowGraph(valueList, actionLineGraphVisual, actionPerformance, actionGraphVisualObjectList, actionGameObjectList, 0, valueList.Count);
        //ShowGraph(valueList, allLineGraphVisual, allPerformance, allGraphVisualObjectList, allGameObjectList, 0, valueList.Count);

    }
    private void Update()
    {
        selectedEntity = uicontroller.selectedEntity;
        if (world.updateCounter != 0 && world.updateCounter % 100 == 0 && transform.localScale.y == 1)
        {
            selectedAI = ((Animal)selectedEntity).GetAI();
            Dictionary<string, List<float>> valueDict = ((NeuralAI)selectedAI).errorDicts;
            List<float> allErrorList = ((NeuralAI)selectedAI).allErrorList;
            ShowGraph(valueDict["outputVisionRedErrors"], redLineGraphVisual, redPerformance, greenGraphVisualObjectList, redGameObjectList, 0, valueDict["outputVisionRedErrors"].Count);
            ShowGraph(valueDict["outputVisionGreenErrors"], greenLineGraphVisual, greenPerformance, redGraphVisualObjectList, greebGameObjectList, 0, valueDict["outputVisionGreenErrors"].Count);
            ShowGraph(valueDict["outputVisionBlueErrors"], blueLineGraphVisual, bluePerformance, blueGraphVisualObjectList, blueGameObjectList, 0, valueDict["outputVisionBlueErrors"].Count);
            ShowGraph(valueDict["outputBodyErrors"], bodyLineGraphVisual, bodyPerformance, bodyGraphVisualObjectList, bodyGameObjectList, 0, valueDict["outputBodyErrors"].Count);
            ShowGraph(valueDict["outputDriveErrors"], driveLineGraphVisual, drivePerformance, driveGraphVisualObjectList, driveGameObjectList, 0, valueDict["outputDriveErrors"].Count);
            ShowGraph(valueDict["outputActionErrors"], actionLineGraphVisual, actionPerformance, actionGraphVisualObjectList, actionGameObjectList, 0, valueDict["outputActionErrors"].Count);
            ShowGraph(allErrorList, allLineGraphVisual, allPerformance, allGraphVisualObjectList, allGameObjectList, 0, allErrorList.Count);

        }
    }
    public static void ShowTooltip_Static(string tooltipText, Vector2 anchoredPosition)
    {
        instance.ShowTooltip(tooltipText, anchoredPosition);
    }
    private void ShowTooltip(string tooltipText, Vector2 anchoredPosition)
    {
        tooltipGameObject.SetActive(true);
        tooltipGameObject.GetComponent<RectTransform>().anchoredPosition = anchoredPosition;
        TextMeshProUGUI tooltipUIText = tooltipGameObject.transform.Find("Text").GetComponent<TextMeshProUGUI>();
        tooltipUIText.text = tooltipText;

        float textPaddingSize = 4f;
        Vector2 backgroundSize = new Vector2(
            tooltipUIText.preferredWidth + textPaddingSize * 2f,
            tooltipUIText.preferredHeight + textPaddingSize * 2f);

        tooltipGameObject.transform.Find("Background").GetComponent<RectTransform>().sizeDelta = backgroundSize;
        tooltipGameObject.transform.SetAsLastSibling();
    }
    public static void HideTooltip_Static()
    {
        instance.HideTooltip();
    }
    private void HideTooltip()
    {
        tooltipGameObject.SetActive(false);
    }
    private void ShowGraph(List<float> valueList, IGraphVisual graphVisual, RectTransform performancePanel, List<IGraphVisualObject> graphVisualObjectList, List<GameObject> gameObjectList, int startX, int endX, Func<int, string> getAxisLabelX = null, Func<float, string> getAxisLabelY = null)
    {
        this.graphVisual = graphVisual;
        this.startX = startX;
        this.endX = endX;
        this.getAxisLabelX = getAxisLabelX;
        this.getAxisLabelY = getAxisLabelY;

        if (getAxisLabelX == null)
        {
            getAxisLabelX = delegate (int _i) { return _i.ToString(); };
        }
        if (getAxisLabelY == null)
        {
            getAxisLabelY = delegate (float _f) {return Mathf.RoundToInt(_f).ToString(); };
        }
        if(endX <= 0)
        {
            endX = valueList.Count;
        }
        foreach (GameObject gameObject in gameObjectList)
        {
            Destroy(gameObject);
        }
        gameObjectList.Clear();
        yLabelList.Clear();
        foreach (IGraphVisualObject graphVisualObject in graphVisualObjectList)
        {
            graphVisualObject.CleanUp();
        }
        graphVisualObjectList.Clear();
        graphVisual.CleanUp();

        float graphWidth = redPerformance.sizeDelta.x;
        float graphHeight = redPerformance.sizeDelta.y;
        float yMax = valueList[0];
        float yMin = valueList[0];

        for(int i = Mathf.Max(startX, 0); i < endX; i++)
        {
            float value = valueList[i];
            if(value > yMax)
            {
                yMax = value;
            }
            if(value < yMin)
            {
                yMin = value;
            }
        }
        float yDifference = yMax - yMin;
        if(yDifference <= 0)
        {
            yDifference = 5f;
        }
        yMax = yMax + (yDifference * 0.2f);
        yMin = yMin - (yDifference * 0.2f);

        if (startYScaleAtZero)
        {
            yMin = 0f; // Start the graph at zero
        }
        xSize = graphWidth / ((endX - startX) + 1);
        int index = 0;
        int gap = 0;
        if (ScaleXLabel(valueList) != 0)
        {
            gap = valueList.Count / ScaleXLabel(valueList);
        }
        for (int i = Mathf.Max(startX, 0); i < endX; i++)
        {
            float xPosition = xSize + index * xSize;
            float yPosition = ((valueList[i] - yMin) / (yMax-yMin)) * graphHeight;

            string tooltipText = valueList[i].ToString();
            IGraphVisualObject graphVisualObject = graphVisual.CreateGraphVisualObject(new Vector2(xPosition, yPosition), xSize, tooltipText);
            
            graphVisualObjectList.Add(graphVisualObject);
            
            if(i % gap == 0)
            {
                RectTransform labelX = Instantiate(lableTemplateX);
                labelX.SetParent(performancePanel, false);
                labelX.gameObject.SetActive(true);
                labelX.anchoredPosition = new Vector2(xPosition, -20f);
                labelX.GetComponent<TextMeshProUGUI>().text = getAxisLabelX(i);
                gameObjectList.Add(labelX.gameObject);
            }
            

            //RectTransform dashX = Instantiate(dashTemplateY);
            //dashX.SetParent(performance, false);
            //dashX.gameObject.SetActive(true);
            //dashX.anchoredPosition = new Vector2(xPosition, -4f);
            //gameObjectList.Add(dashX.gameObject);
            index++;
        }
        int separatorCount = 5;
        for (int i = 0; i <= separatorCount; i++)
        {
            RectTransform labelY = Instantiate(lableTemplateY);
            labelY.SetParent(performancePanel, false);
            labelY.gameObject.SetActive(true);
            float normalizedValue = i * 1f / separatorCount;
            labelY.anchoredPosition = new Vector2(-20f, normalizedValue * graphHeight);
            labelY.GetComponent<TextMeshProUGUI>().text = getAxisLabelY(yMin + normalizedValue * (yMax - yMin));
            yLabelList.Add(labelY);
            gameObjectList.Add(labelY.gameObject);

            //    //RectTransform dashY = Instantiate(dashTemplateX);
            //    //dashY.SetParent(performance, false);
            //    //dashY.gameObject.SetActive(true);
            //    //dashY.anchoredPosition = new Vector2(-4f, normalizedValue * graphHeight);
            //    //gameObjectList.Add(dashY.gameObject);
            }
        }
    private int ScaleXLabel(List<float> valueList)
    {
        if (valueList.Count <= 10)
        {
            return valueList.Count;
        }
        else
        {
            for (int j = 10; j > 0; j--)
            {
                if (valueList.Count % j == 0)
                {
                    return j;
                }
                else
                {
                    if ((valueList.Count - 1) % j == 0)
                    {
                        return j;
                    }
                }
            }
        }
        return 0;
    }
    //private void UpdateValue(int index, float value)
    //{
    //    float yMinBefore, yMaxBefore;
    //    CalculateYScale(out yMinBefore, out yMaxBefore);

    //    valueList[index] = value;

    //    float graphWidth = redPerformance.sizeDelta.x;
    //    float graphHeight = redPerformance.sizeDelta.y;

    //    float yMin, yMax;
    //    CalculateYScale(out yMin, out yMax);

    //    bool yScaleChanged = yMinBefore != yMin || yMaxBefore != yMax;

    //    if (!yScaleChanged)
    //    {
    //        // Y Scale did not change, update only this value
    //        float xPosition = xSize + index * xSize;
    //        float yPosition = ((value - yMin) / (yMax - yMin)) * graphHeight;
    //        // Add data point visual
    //        string tooltipText = value.ToString();
    //        graphVisualObjectList[index].SetGraphVisualObjectInfo(new Vector2(xPosition, yPosition), xSize, tooltipText);
    //    }
    //    else
    //    {
    //        // Y scale changed, update whole graph and y axis labels
    //        // Cycle through all visible data points
    //        int xIndex = 0;
    //        for (int i = Mathf.Max(startX, 0); i < endX; i++)
    //        {
    //            float xPosition = xSize + xIndex * xSize;
    //            float yPosition = ((valueList[i] - yMin) / (yMax - yMin)) * graphHeight;

    //            // Add data point visual
    //            string tooltipText = valueList[i].ToString();
    //            graphVisualObjectList[xIndex].SetGraphVisualObjectInfo(new Vector2(xPosition, yPosition), xSize, tooltipText);

    //            xIndex++;
    //        }
    //        if (getAxisLabelY == null)
    //        {
    //            getAxisLabelY = delegate (float _f) { return Mathf.RoundToInt(_f).ToString(); };
    //        }
    //        for (int i = 0; i < yLabelList.Count; i++)
    //        {
    //            float normalizedValue = i * 1f / (yLabelList.Count -1);
    //            yLabelList[i].GetComponent<TextMeshProUGUI>().text = getAxisLabelY(yMin + (normalizedValue * (yMax - yMin)));
    //        }
    //    }
    //}

    private void CalculateYScale(out float yMin, out float yMax)
    {
        // Identify y Min and Max values
        yMax= valueList[0];
        yMin = valueList[0];

        for (int i = Mathf.Max(startX, 0); i < endX; i++)
        {
            float value = valueList[i];
            if (value > yMax)
            {
                yMax = value;
            }
            if (value < yMin)
            {
                yMin = value;
            }
        }

        float yDifference = yMax - yMin;
        if (yDifference <= 0)
        {
            yDifference = 5f;
        }
        yMax = yMax + (yDifference * 0.2f);
        yMin = yMin - (yDifference * 0.2f);

        if (startYScaleAtZero)
        {
            yMin = 0f; // Start the graph at zero
        }
    }

    private interface IGraphVisual
    {
        IGraphVisualObject CreateGraphVisualObject(Vector2 graphPosition, float graphPositionWidth, string tooltipText);
        void CleanUp();
    }

    private interface IGraphVisualObject
    {
        void SetGraphVisualObjectInfo(Vector2 graphPosition, float graphPositionWidth, string tooltipText);
        void CleanUp();
    }

    private class LineGraphVisual : IGraphVisual
    {
        private RectTransform performance;
        private Sprite dotSprite;
        private LineGraphVisualObject lastLineGraphVisualObject;
        private Color dotColor;
        private Color dotConnectionColor;

        public LineGraphVisual(RectTransform performance, Sprite dotSprite, Color dotColor, Color dotConnectionColor)
        {
            this.performance = performance;
            this.dotSprite = dotSprite;
            this.dotColor = dotColor;
            this.dotConnectionColor = dotConnectionColor;
            lastLineGraphVisualObject = null;
        }
        public void CleanUp()
        {
            lastLineGraphVisualObject = null;
        }
        public IGraphVisualObject CreateGraphVisualObject(Vector2 graphPosition, float graphPositionWidth, string tooltipText)
        {
            GameObject dotGameObject = CreateDot(graphPosition);


            GameObject dotConnectionGameObject = null;
            if (lastLineGraphVisualObject != null)
            {
                dotConnectionGameObject = CreateDotConnection(lastLineGraphVisualObject.GetGraphPosition(), dotGameObject.GetComponent<RectTransform>().anchoredPosition);
            }

            LineGraphVisualObject lineGraphVisualObject = new LineGraphVisualObject(dotGameObject, dotConnectionGameObject, lastLineGraphVisualObject);
            lineGraphVisualObject.SetGraphVisualObjectInfo(graphPosition, graphPositionWidth, tooltipText);

            lastLineGraphVisualObject = lineGraphVisualObject;

            return lineGraphVisualObject;
        }
        private GameObject CreateDot(Vector2 anchoredPosition)
        {
            GameObject gameObject = new GameObject("Dot", typeof(Image));
            gameObject.transform.SetParent(performance, false);
            gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 200);
            gameObject.GetComponent<Image>().sprite = dotSprite;
            gameObject.GetComponent<Image>().color = dotColor;
            RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = anchoredPosition;
            rectTransform.sizeDelta = new Vector2(11, 11);
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(0, 0);

            Button_UI dotButtonUI = gameObject.AddComponent<Button_UI>();
            return gameObject;
        }

        private GameObject CreateDotConnection(Vector2 dotPositionA, Vector2 dotPositionB)
        {
            GameObject gameObject = new GameObject("dotConnection", typeof(Image));
            gameObject.transform.SetParent(performance, false);
            gameObject.GetComponent<Image>().color = dotConnectionColor;
            RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
            Vector2 dir = (dotPositionB - dotPositionA).normalized;
            float distance = Vector2.Distance(dotPositionA, dotPositionB);

            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(0, 0);
            rectTransform.sizeDelta = new Vector2(distance, 3f);
            rectTransform.anchoredPosition = dotPositionA + dir * distance * .5f;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            rectTransform.localEulerAngles = new Vector3(0, 0, angle);
            return gameObject;
        }

        public class LineGraphVisualObject : IGraphVisualObject
        {
            public event EventHandler OnChangedGraphVisualObjectInfo;

            private GameObject dotGameObject;
            private GameObject dotConnectionGameObject;
            private LineGraphVisualObject lastVisualObject;
            public LineGraphVisualObject(GameObject dotGameObject, GameObject dotConnectionGameObject, LineGraphVisualObject lastVisualObject)
            {
                this.dotGameObject = dotGameObject;
                this.dotConnectionGameObject = dotConnectionGameObject;
                this.lastVisualObject = lastVisualObject;

                if (lastVisualObject != null)
                {
                    lastVisualObject.OnChangedGraphVisualObjectInfo += LastVisualObject_OnChangedGraphVisualObjectInfo;
                }
            }
            private void LastVisualObject_OnChangedGraphVisualObjectInfo(object sender, EventArgs e)
            {
                UpdateDotConnection();
            }
            public void SetGraphVisualObjectInfo(Vector2 graphPosition, float graphPositionWidth, string tooltipText)
            {
                RectTransform rectTransform = dotGameObject.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = graphPosition;

                UpdateDotConnection();

                Button_UI dotButtonUI = dotGameObject.GetComponent<Button_UI>();

                // Show Tooltip on Mouse Over
                dotButtonUI.MouseOverOnceFunc = () => {
                    ShowTooltip_Static(tooltipText, graphPosition);
                };

                // Hide Tooltip on Mouse Out
                dotButtonUI.MouseOutOnceFunc = () => {
                    HideTooltip_Static();
                };

                if (OnChangedGraphVisualObjectInfo != null) OnChangedGraphVisualObjectInfo(this, EventArgs.Empty);
            }
            public void CleanUp()
            {
                Destroy(dotGameObject);
                Destroy(dotConnectionGameObject);
            }
            public Vector2 GetGraphPosition()
            {
                RectTransform rectTransform = dotGameObject.GetComponent<RectTransform>();
                return rectTransform.anchoredPosition;
            }

            private void UpdateDotConnection()
            {
                if (dotConnectionGameObject != null)
                {
                    RectTransform dotConnectionRectTransform = dotConnectionGameObject.GetComponent<RectTransform>();
                    Vector2 dir = (lastVisualObject.GetGraphPosition() - GetGraphPosition()).normalized;
                    float distance = Vector2.Distance(GetGraphPosition(), lastVisualObject.GetGraphPosition());
                    dotConnectionRectTransform.sizeDelta = new Vector2(distance, 3f);
                    dotConnectionRectTransform.anchoredPosition = GetGraphPosition() + dir * distance * .5f;
                    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                    dotConnectionRectTransform.localEulerAngles = new Vector3(0, 0, angle);
                }
            }
        }
    }
}
