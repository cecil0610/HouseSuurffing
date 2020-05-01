using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Buoyancy : MonoBehaviour
{
    //  ▀▄▀▄▀▄ Editor Variables ▄▀▄▀▄▀

    float buoyantForce = 8f;  // Increase value to make object more buoyant
    float depthPower = 1f;  // "value 0 mean no additional Buoyant Force underwater, 1 mean Double buoyant Force underwater (underwater pressure)"), Range(0f, 1f) 
    float offsetY = 0f;  // Center of Mass on Y axis?
    string waterVolumeTag = "Flood";

    //  ▀▄▀▄▀▄ Private Variables ▄▀▄▀▄▀

    private Rigidbody rb;
    private Collider coll;
    private WaterBody waterBody;
    private float yBound;
    private bool isWaterBodySet;
    private int waterCount;

    //  ▀▄▀▄▀▄ Core Functions ▄▀▄▀▄▀

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (waterCount == 0)
        {
            waterBody = null;
            isWaterBodySet = false;
        }
        
    }

    //  ▀▄▀▄▀▄ Shared Functions ▄▀▄▀▄▀

    //Set and Get for Under water Buoyancy (depth pressure)(0 to 1 range).
    public void SetDepthPower(in float value)
    {
        if (value >= 0f && value <= 1f) depthPower = value;
    }

    public float GetDepthPower() => depthPower;

    //if this object fully submerged into water, returns true.
    public bool IsUnderWater() => isWaterBodySet && yBound > coll.bounds.max.y;

    //if this object floating on surface of water, returns true.
    public bool IsFloating() => isWaterBodySet && !(yBound > coll.bounds.max.y);

    //  ▀▄▀▄▀▄ Trigger Functions ▄▀▄▀▄▀

    private void OnTriggerEnter(Collider water)
    {
        if (water.CompareTag(waterVolumeTag)) 
        {
            waterCount++;
        }
    }

    private void OnTriggerStay(Collider water)
    {
        //if this object inside Water, it object start floating,
        if (water.CompareTag(waterVolumeTag))
        {
            if (transform.position.x < water.bounds.max.x
            && transform.position.z < water.bounds.max.z
            && transform.position.x > water.bounds.min.x
            && transform.position.z > water.bounds.min.z)
            {
                if (waterBody != null && !ReferenceEquals(waterBody.gameObject, water.gameObject))
                {
                    waterBody = null;
                    isWaterBodySet = false;
                }

                if (!isWaterBodySet)
                {
                    waterBody = water.GetComponent<WaterBody>();
                    if (waterBody != null) isWaterBodySet = true;
                }
                else
                {
                    float objectYValue = coll.bounds.center.y + offsetY;
                    yBound = waterBody.GetYBound();
                    if (objectYValue < yBound)
                    {
                        float buoyantForceMass = buoyantForce * rb.mass;
                        float underWaterBuoyantForce = Mathf.Clamp01((yBound - objectYValue) * depthPower);
                        float buoyency = buoyantForceMass + (buoyantForceMass * underWaterBuoyantForce);
                        rb.AddForce(0f, buoyency, 0f);
                    }
                }
            }
        }
    }

    private void OnTriggerExit(Collider water)
    {
        if (water.CompareTag(waterVolumeTag)) 
        {
            waterCount--;
        }
    }

}
