using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterBody : MonoBehaviour
{
    bool customSurfaceLevel = false;  // TODO update this surface level
    float surfaceLevel = 0f;  // In Y axis
    private Collider coll;

    // Start is called before the first frame update
    void Start()
    {
        coll = GetComponent<Collider>();
    }

    public float GetYBound()
    {
        if (!customSurfaceLevel) 
        {
            surfaceLevel = coll.bounds.max.y;
        }
        return surfaceLevel;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
