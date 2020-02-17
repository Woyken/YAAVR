using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureScalingFix : MonoBehaviour
{
    void Awake()
    {

    }

    void Start()
    {
        var r = GetComponent<Renderer>();
        var mesh = GetComponent<MeshFilter>().mesh;
        if (r)
        {
            r.material.mainTextureScale = new Vector2(transform.lossyScale.y, transform.lossyScale.x);
        }
    }

    void Update()
    {

    }
}
