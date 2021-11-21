using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics;
using UnityEngine.UI;
using TMPro;

public class NetworkVisualization : MonoBehaviour
{
    Camera FPSCamera;
    Entity selectedEntity;
    public bool switchEntity;
    bool resetted;
    [SerializeField]
    RawImage displayImage;
    [SerializeField]
    Transform name_Value_Pair;
    [SerializeField]
    UIController uicontroller;
    int[] resolution = new int[2];
    AI selectedAI;
    List<string> driveLabelList = new List<string>();
    List<string> bodyLabelList = new List<string>();
    List<string> actionLabelList = new List<string>();
    Matrix<float> m = Matrix<float>.Build.Dense(1, 5, -1);
    [SerializeField]
    Transform driveInputContent
     ,bodyInputContent
     ,actionInputContent
     ,redVisualInputContent
     ,greenVisualInputContent
     ,blueVisualInputContent
     ,hiddenContent
     ,driveOutputContent
     ,bodyOutputContent
     ,actionOutputContent
     ,redVisualOutputContent
     ,greenVisualOutputContent
     ,blueVisualOutputContent;

    List<RawImage> driveInputImages = new List<RawImage>();
    List<RawImage> bodyInputImages = new List<RawImage>();
    List<RawImage> actionInputImages = new List<RawImage>();
    List<RawImage> driveOutputImages = new List<RawImage>();
    List<RawImage> bodyOutputImages = new List<RawImage>();
    List<RawImage> actionOutputImages = new List<RawImage>();
    RawImage redInputImage;
    RawImage greenInputImage;
    RawImage blueInputImage;
    RawImage hiddenImage;
    RawImage redOutputImage;
    RawImage greenOutputImage;
    RawImage blueOutputImage;

    Matrix<float> visualInputMatrix;
    void Update()
    {
        selectedEntity = uicontroller.selectedEntity;
        if (switchEntity)
        {
            if (!resetted)
            {
                ResetAllContent();
            }
            else
            {
                BuildContent();
            }
        }
        if(transform.GetChild(0).localScale.y == 1 && !switchEntity)
        {
            resetted = false;
            Dictionary<string, Layer> networkLayerDict = ((NeuralAI)selectedAI).GetNetworkLayerDict();
            float[] inputRedColorArray = ConvertToColorArray(networkLayerDict["inputVisionRedLayer"].Output.Column(0));
            float[] inputGreenColorArray = ConvertToColorArray(networkLayerDict["inputVisionGreenLayer"].Output.Column(0));
            float[] inputBlueColorArray = ConvertToColorArray(networkLayerDict["inputVisionBlueLayer"].Output.Column(0));
            float[] hiddenColorArray = ConvertToColorArray(networkLayerDict["hiddenLayer"].Output.Column(0));
            DisplayColorMatrix(inputRedColorArray, redInputImage, resolution[0], resolution[1]);
            DisplayColorMatrix(inputGreenColorArray, greenInputImage, resolution[0], resolution[1]);
            DisplayColorMatrix(inputBlueColorArray, blueInputImage, resolution[0], resolution[1]);
            DisplayColorMatrix(hiddenColorArray, hiddenImage, 16, 8);
            Matrix<float> inputDriveColorMatrix = ConvertToColorMatrix(networkLayerDict["inputDriveLayer"].Output.Column(0));
            DisplaySingleColorBox(inputDriveColorMatrix, driveInputImages);
            Matrix<float> inputBodyColorMatrix = ConvertToColorMatrix(networkLayerDict["inputBodyLayer"].Output.Column(0));
            DisplaySingleColorBox(inputBodyColorMatrix, bodyInputImages);
            Matrix<float> inputActionColorMatrix = ConvertToColorMatrix(networkLayerDict["inputActionLayer"].Output.Column(0));
            DisplaySingleColorBox(inputActionColorMatrix, actionInputImages);

            float[] outputRedColorArray = ConvertToColorArray(networkLayerDict["outputVisionRedLayer"].Output.Column(0));
            float[] outputGreenColorArray = ConvertToColorArray(networkLayerDict["outputVisionRedLayer"].Output.Column(0));
            float[] outputBlueColorArray = ConvertToColorArray(networkLayerDict["outputVisionRedLayer"].Output.Column(0));
            DisplayColorMatrix(outputRedColorArray, redOutputImage, resolution[0], resolution[1]);
            DisplayColorMatrix(outputGreenColorArray, greenOutputImage, resolution[0], resolution[1]);
            DisplayColorMatrix(outputBlueColorArray, blueOutputImage, resolution[0], resolution[1]);

            Matrix<float> outputDriveColorMatrix = ConvertToColorMatrix(networkLayerDict["outputDriveLayer"].Output.Column(0));
            DisplaySingleColorBox(outputDriveColorMatrix, driveOutputImages);
            Matrix<float> outputBodyColorMatrix = ConvertToColorMatrix(networkLayerDict["outputBodyLayer"].Output.Column(0));
            DisplaySingleColorBox(outputBodyColorMatrix, bodyOutputImages);
            Matrix<float> outputActionColorMatrix = ConvertToColorMatrix(networkLayerDict["outputActionLayer"].Output.Column(0));
            DisplaySingleColorBox(outputActionColorMatrix, actionOutputImages);
            
        }
    }
    public void ResetAllContent() 
    {
        ResetContent(driveInputContent, driveInputImages);
        ResetContent(bodyInputContent, bodyInputImages);
        ResetContent(actionInputContent, actionInputImages);
        ResetContent(redVisualInputContent);
        ResetContent(greenVisualInputContent);
        ResetContent(blueVisualInputContent);
        ResetContent(hiddenContent);
        ResetContent(driveOutputContent, driveOutputImages);
        ResetContent(bodyOutputContent, bodyOutputImages);
        ResetContent(actionOutputContent, actionOutputImages);
        ResetContent(redVisualOutputContent);
        ResetContent(greenVisualOutputContent);
        ResetContent(blueVisualOutputContent);
        resetted = true;
    }
    void ResetContent(Transform passedContent, List<RawImage> passedImages)
    {
        passedImages.Clear();
        foreach(Transform child in passedContent)
        {
            Destroy(child.gameObject);
        }
    }
    void ResetContent(Transform passedContent)
    {
        foreach (Transform child in passedContent)
        {
            Destroy(child.gameObject);
        }
    }
    void BuildContent()
    {
        selectedAI = ((Animal)selectedEntity).GetAI();
        if (selectedEntity != null && selectedEntity.GetGameObject().tag == "Human")
        {
            GameObject selectedGO = selectedEntity.GetGameObject();
            FPSCamera = selectedGO.transform.GetComponentInChildren<Camera>();
            resolution[0] = FPSCamera.targetTexture.width;
            resolution[1] = FPSCamera.targetTexture.height;
            driveLabelList = ((Animal)selectedEntity).GetDriveSystem().GetStateLabels();
            bodyLabelList = ((Animal)selectedEntity).GetBody().GetStateLabels();
            actionLabelList = ((Animal)selectedEntity).GetMotorSystem().GetStateLabels();
            visualInputMatrix = Matrix<float>.Build.Dense(3, resolution[0] * resolution[1]);
            GenerateContentWithTitle(driveLabelList, driveInputContent, driveInputImages);
            GenerateContentWithTitle(bodyLabelList, bodyInputContent, bodyInputImages);
            GenerateContentWithTitle(actionLabelList, actionInputContent, actionInputImages);
            redInputImage = Instantiate(displayImage, redVisualInputContent);
            greenInputImage = Instantiate(displayImage, greenVisualInputContent);
            blueInputImage = Instantiate(displayImage, blueVisualInputContent);
            hiddenImage = Instantiate(displayImage, hiddenContent);
            GenerateContentWithTitle(driveLabelList, driveOutputContent, driveOutputImages);
            GenerateContentWithTitle(bodyLabelList, bodyOutputContent, bodyOutputImages);
            GenerateContentWithTitle(actionLabelList, actionOutputContent, actionOutputImages);
            redOutputImage = Instantiate(displayImage, redVisualOutputContent);
            greenOutputImage = Instantiate(displayImage, greenVisualOutputContent);
            blueOutputImage = Instantiate(displayImage, blueVisualOutputContent);
            switchEntity = false;
        }
    }
    void GenerateContentWithTitle(List<string> contentLableList, Transform content, List<RawImage> contentImages)
    {
        if (contentLableList.Count >= 15)
        {
            content.GetComponent<GridLayoutGroup>().cellSize = new Vector2(40, 30);
        }
        for (int i = 0; i < contentLableList.Count; i++)
        {
            Instantiate(name_Value_Pair, content);
            content.GetChild(i).GetChild(0).GetComponent<TMP_Text>().text = contentLableList[i];
            contentImages.Add(content.GetChild(i).GetChild(1).GetComponent<RawImage>());
        }
    }
    Matrix<float> ConvertToColorMatrix(Vector<float> passedV)
    {
        Matrix<float> colorMatrix = Matrix<float>.Build.Dense(passedV.Count, 3);
        foreach (System.Tuple<int, float> x in passedV.EnumerateIndexed())
        {
            float scaledValue = (x.Item2 + 1) / 2;
            if (scaledValue <= 0.5)
            {
                colorMatrix[x.Item1, 0] = 1;
                colorMatrix[x.Item1, 1] = scaledValue * 2;
                colorMatrix[x.Item1, 2] = scaledValue * 2;
            }
            else
            {
                colorMatrix[x.Item1, 0] = (scaledValue * -2) + 2;
                colorMatrix[x.Item1, 1] = 1;
                colorMatrix[x.Item1, 2] = (scaledValue * -2) + 2;
            }

        }
        return colorMatrix;
    }
    float[] ConvertToColorArray(Vector<float> passedV)
    {
        List<float> colorList = new List<float>();
        foreach (System.Tuple<int, float> x in passedV.EnumerateIndexed())
        {
            float scaledValue = (x.Item2 + 1) / 2;
            if (scaledValue <= 0.5)
            {
                colorList.Add(1);
                colorList.Add(scaledValue * 2);
                colorList.Add(scaledValue * 2);
                colorList.Add(1);
            }
            else
            {
                colorList.Add((scaledValue * -2) + 2);
                colorList.Add(1);
                colorList.Add((scaledValue * -2) + 2);
                colorList.Add(1);
            }

        }
        return colorList.ToArray();
    }
    void UpdateVisualInput(int width, int height)
    {

        Texture2D visualInputTexture = new Texture2D(width, height, TextureFormat.RGB24, false);

        FPSCamera.Render();
        RenderTexture.active = FPSCamera.targetTexture;
        visualInputTexture.ReadPixels(new Rect(0, 0, width, height), 0, 0);

        Color[] colorArray = visualInputTexture.GetPixels();

        int resolutionSquared = width * height;
        for (int i = 0; i < resolutionSquared; i++)
        {
            visualInputMatrix[0, i] = (colorArray[i].r * 2) - 1;
            visualInputMatrix[1, i] = (colorArray[i].g * 2) - 1;
            visualInputMatrix[2, i] = (colorArray[i].b * 2) - 1;
        }
    }
    void DisplayColorMatrix(float[] colorArray, RawImage image, int length, int width)
    {
        var tex = new Texture2D(length, width, TextureFormat.RGBAFloat, true);
        tex.SetPixelData(colorArray, 0, 0); // mip 0

        tex.filterMode = FilterMode.Point;
        tex.Apply(updateMipmaps: false);

        image.texture = tex;
    }
    void DisplaySingleColorBox(Matrix<float> colorMatrix, List<RawImage> contentImages)
    {
        foreach (System.Tuple<int, int, float> x in colorMatrix.EnumerateIndexed())
        {
            contentImages[x.Item1].color = new Color(colorMatrix[x.Item1, 0], colorMatrix[x.Item1, 1], colorMatrix[x.Item1, 2]);
        }
    }
}
