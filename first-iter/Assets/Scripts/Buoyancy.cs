using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Buoyancy : MonoBehaviour
{
    //  ▀▄▀▄▀▄ Editor Variables ▄▀▄▀▄▀
    [Header("Physics coefficients")]
    public float buoyantForce;  // Increase value to make object more buoyant
    public float buoyantForceMax;
    public float depthPower;  // "value 0 mean no additional Buoyant Force underwater, 1 mean Double buoyant Force underwater (underwater pressure)"), Range(0f, 1f) 
    public float offsetY;  // Center of Mass on Y axis?
    public int playerDropForceFactor;
    public float playerMaxStayForceFactor;
    public float playerHouseDistanceRangeLow;
    public float playerHouseDistanceRangeHigh;
    public int normalisePlusFactor;
    public int magnifiedPower;

    public Vector3 maxVelocity;

    string waterVolumeTag = "Flood";
    string playerTag = "Player";
    string barrelTag = "Barrel";

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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(waterVolumeTag)) 
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
        //Initial strong force when player drops on to the house
        if (other.collider.CompareTag(playerTag))
        {
            rb.AddForce(Vector3.down * playerDropForceFactor);
        }
        
        // Collect the barrel
        if (other.collider.CompareTag(barrelTag)) 
        {
            // TODO compute bounds of water before moving the barrel
            // TODO exclude the bounds nearby the player
            GameObject waterBody = GameObject.FindWithTag(waterVolumeTag);
            float generationBounds = 20f;
            other.transform.position = new Vector3(Random.Range(-generationBounds, generationBounds), 0.5f, Random.Range(-generationBounds, generationBounds));
            // Destroy(other.collider.gameObject);
            // Gain bonus on barrel collection
            if (buoyantForce <= buoyantForceMax) {
                buoyantForce += 1;
            }
            Debug.Log("Barrel collected >^_^<-----------------------");
        }
    }

    private void OnCollisionStay(Collision other) {
        // Apply force based on the relative position of the player on top of the house
        if (other.collider.CompareTag(playerTag))
        {

            Vector3 normalisePlayerPos = new Vector3(other.transform.position.x - 0.34f, other.transform.position.y, other.transform.position.z + 1.23f);
            Vector3 normaliseHousePos = new Vector3(transform.position.x + 3.8f, transform.position.y, transform.position.z + 4.9f);
            float distance = Vector3.Distance(normaliseHousePos, normalisePlayerPos);  // Range is around (6, 12)

            //COMMENTED OUT
            float normalisedBaseDiff = (distance - playerHouseDistanceRangeLow + normalisePlusFactor);
            float normalisedForceFactor = Mathf.Min(Mathf.Pow(normalisedBaseDiff, magnifiedPower), playerMaxStayForceFactor);
            ContactPoint contact = other.contacts[0];
            // TODO improve the direction of the force
            Vector3 forceVector = (normalisePlayerPos - normaliseHousePos) / 100;  // Force from player to house
            forceVector = new Vector3(forceVector.x, 0f, forceVector.z);
            //rb.AddForceAtPosition(forceVector * normalisedForceFactor, contact.point);
            rb.AddRelativeForce(forceVector * normalisedForceFactor);

            // Debug.Log(normalisedForceFactor);

            //Debug.Log("Velocity = " + rb.velocity);

            //Debug.Log("player.z = " + other.transform.position.z);
            //Debug.Log("block.z = " + transform.position.z);

            // TODO remove debug code
            //Debug.Log("normalise Base Diff = " + normalisedBaseDiff);
            //Debug.Log("normalise Forced Factor = " + normalisedForceFactor);
        }
    }

}
