using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundaryCheck : MonoBehaviour
{
    public float fXBoundary = 40f;
    public float fZBoundary = 50f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x > fXBoundary)
        {
            transform.position = new Vector3(fXBoundary, transform.position.y, transform.position.z);
        }
        else if(transform.position.x < -fXBoundary)
        {
            transform.position = new Vector3(-fXBoundary, transform.position.y, transform.position.z);
        }

        if(transform.position.z > fZBoundary)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, fZBoundary);
        }
        else if (transform.position.z < -fZBoundary)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, -fZBoundary);
        }
    }
}
