using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering;

public class TestCodeOnly : MonoBehaviour
{
    Matrix<float> m1 = Matrix<float>.Build.Dense(2, 2, 1);
    Matrix<float> m2 = Matrix<float>.Build.Dense(2, 2, 2);
    Vector<float> v1 = Vector<float>.Build.Dense(2, 2);
    private void Awake()
    {
        Debug.Log(m1.Transpose().Append(m2.Transpose()).Transpose());
    }
}
