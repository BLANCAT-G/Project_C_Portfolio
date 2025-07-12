using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestShader : MonoBehaviour
{
    [Range(0.0f, 1.0f)]
    public float threshold;

    public Material mat;
    // Start is called before the first frame update
    void Start()
    {
        mat = GetComponent<SpriteRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        mat.SetFloat("_Threshold", threshold);
    }
}
