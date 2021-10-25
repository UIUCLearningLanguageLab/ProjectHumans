using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Random;
using MathNet.Numerics.Distributions;

public class TextureTest : MonoBehaviour
{
    Matrix<float> m = Matrix<float>.Build.Random(100, 100, new ContinuousUniform(-1.0, 1.0));
    private void Start()
    {
        foreach (float x in m.Enumerate()) 
        {
            if(x > 1)
            {
                Debug.Log(x);
            }
            if(x < -1)
            {
                Debug.Log(x);
            }
        }
    }
}
