using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;

public class SensorySystem {

    public Animal thisAnimal;

    public Camera visualInputCamera;
    Matrix<float> visualInputArray;

    int visualResolution;

    public SensorySystem(Animal animal) {
        
        this.thisAnimal = animal;
        visualInputCamera = thisAnimal.GetGameObject().GetComponentInChildren<Camera>();
        
        visualResolution = (int)this.thisAnimal.GetPhenotype().GetTraitDict()["visual_resolution"];
        
        InitVisualInput();
        

    }

    public void InitVisualInput() {
        

        if (visualInputCamera != null) {
            if (visualInputCamera.targetTexture == null) {
                visualInputCamera.targetTexture = new RenderTexture(visualResolution, visualResolution, 24);
                /* 24 is the depth buffer, or depth texture, is actually just a render texture that contains values of how far objects in the scene are from the camera.*/
            } else {
                
                visualResolution = visualInputCamera.targetTexture.width;
            }
        }
        visualInputArray = Matrix<float>.Build.Dense(3, visualResolution * visualResolution);
        UpdateVisualInput();
    }

    public Matrix<float> GetVisualInput() {
        if (this.thisAnimal.GetAge() % this.thisAnimal.GetPhenotype().GetTraitDict()["visual_refresh_rate"] == 0)
        {
            UpdateVisualInput();
            //string outputString = visualInputArray.GetLength(0).ToString() + "," + visualInputArray.GetLength(1).ToString();
        }
        return visualInputArray;
    }

    public void UpdateVisualInput() {
        if (this.thisAnimal.visualInputCamera.gameObject.activeInHierarchy) {
            
            Texture2D visualInputTexture = new Texture2D(visualResolution, visualResolution, TextureFormat.RGB24, false);
            
            this.thisAnimal.visualInputCamera.Render();
            RenderTexture.active = this.thisAnimal.visualInputCamera.targetTexture;
            visualInputTexture.ReadPixels(new Rect(0, 0, visualResolution, visualResolution), 0, 0);
            Color[] colorArray = visualInputTexture.GetPixels();
            
            int resolutionSquared = visualResolution*visualResolution;
            
            for (int i=0; i<resolutionSquared; i++){
                visualInputArray[0,i] = (colorArray[i].r * 2) - 1;
                visualInputArray[1,i] = (colorArray[i].g * 2) - 1;
                visualInputArray[2,i] = (colorArray[i].b * 2) - 1;
            }
            //SaveVisualImage(visualInputTexture);

        } else {
            Debug.Log("Camera is off");
        }
    }
    
    void SaveVisualImage(Texture2D visualInputTexture){
        byte[] bytes = visualInputTexture.EncodeToPNG();
        string fileName = GetImageName();
        System.IO.File.WriteAllBytes(fileName, bytes);
    }

    string GetImageName() {
        return string.Format("{0}/VisualInputs/visualInput_{1}x{2}_{3}.png",
        Application.dataPath,
        visualResolution,
        visualResolution,
        System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
    }

    public Camera GetInternalCam(){
        return visualInputCamera;
    }

}