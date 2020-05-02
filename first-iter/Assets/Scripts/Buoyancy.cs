using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Buoyancy : MonoBehaviour
{
    //  ▀▄▀▄▀▄ Editor Variables ▄▀▄▀▄▀
    [Header("Physics coefficients")]
    public float buoyantForce;  // Increase value to make object more buoyant
    public float depthPower;  // "value 0 mean no additional Buoyant Force underwater, 1 mean Double buoyant Force underwater (underwater pressure)"), Range(0f, 1f) 
    public float offsetY;  // Center of Mass on Y axis?
    public int playerDropForceFactor;
    public float playerMaxStayForceFactor;
    public int playerHouseDistanceRangeLow;
    public int playerHouseDistanceRangeHigh;
    public int normalisePlusFactor;
    public int magnifiedPower;

    string waterVolumeTag = "Flood";
    string playerTag = "Player";

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

    private void OnTriggerStay(Collider other)
    {
        // If this object inside Water, it object start floating,
        if (other.CompareTag(waterVolumeTag))
        {
            if (transform.position.x < other.bounds.max.x
            && transform.position.z < other.bounds.max.z
            && transform.position.x > other.bounds.min.x
            && transform.position.z > other.bounds.min.z)
            {
                if (waterBody != null && !ReferenceEquals(waterBody.gameObject, other.gameObject))
                {
                    waterBody = null;
                    isWaterBodySet = false;
                }

                if (!isWaterBodySet)
                {
                    waterBody = other.GetComponent<WaterBody>();
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

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(waterVolumeTag)) 
        {
            waterCount--;
        }

        // TODO potential lose condition here, if the player isn't contacting with the house anymore
    }

    private void OnCollisionEnter(Collision other) {
        // TODO move these constants elsewhere
        
        //Initial strong force when player drops on to the house
        if (other.collider.CompareTag(playerTag))
        {
            rb.AddForce(Vector3.down * playerDropForceFactor);
        }
    }

    private void OnCollisionStay(Collision other) {
        // Apply force based on the relative position of the player on top of the house
        if (other.collider.CompareTag(playerTag))
        {
            float distance = Vector3.Distance(transform.position, other.transform.position);  // Range is around (6, 12)
            float normalisedBaseDiff = (distance - playerHouseDistanceRangeLow + normalisePlusFactor);
            float normalisedForceFactor = Mathf.Min(Mathf.Pow(normalisedBaseDiff, magnifiedPower), playerMaxStayForceFactor);
            ContactPoint contact = other.contacts[0];
            // TODO improve the direction of the force
            Vector3 forceVector = (other.transform.position - transform.position) / 100;  // Force from player to house
            rb.AddForceAtPosition(forceVector * normalisedForceFactor, contact.point);

            // TODO remove debug code
            Debug.Log(normalisedBaseDiff);
            Debug.Log(normalisedForceFactor);
        }
    }

}
