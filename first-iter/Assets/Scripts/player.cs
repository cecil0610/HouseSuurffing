using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour
{
    private Animator anim;
    public Rigidbody rb;

    [Header("Player Parameters")]
    public float fSpeed;
    public float fRotation;
    public int forwardFactor;
    public int backFactor;
    public int sideFactor;
    public int jumpForceFactor;
    public float jumpBuoyantForceDecrement;

    [Header("Physics coefficients")]
    public int playerDropForceFactor;
    public float playerMaxStayForceFactor;
    public float playerHouseDistanceRangeLow;
    public float playerHouseDistanceRangeHigh;
    public int normalisePlusFactor;
    public int magnifiedPower;
    public int forceVectorDampening;

    [Header("Player Landing effects")]
    public ParticleSystem jumpDust;
    public AudioSource jumpSfx;

    [Header("Game Over")]
    public GameObject gameOverScreen;

    void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetBool("isJumping", true);
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W)) 
        {
            transform.Translate(Vector3.forward * Time.deltaTime * forwardFactor, Space.World);
        }
        if (Input.GetKey(KeyCode.S)) 
        {
            transform.Translate(Vector3.back * Time.deltaTime * backFactor, Space.World);
        }
        if (Input.GetKey(KeyCode.A)) 
        {
            transform.Translate(Vector3.left * Time.deltaTime * sideFactor, Space.World);
            // transform.eulerAngles.y += -rotspd * Time.deltaTime * 7;  // TODO rotation not working
        }
        if (Input.GetKey(KeyCode.D)) 
        {
            transform.Translate(Vector3.right * Time.deltaTime * sideFactor, Space.World);
            // transform.eulerAngles.y += rotspd * Time.deltaTime * 7;  // TODO rotation not working
        }
        if (Input.GetKey(KeyCode.Space) && anim.GetBool("isJumping") == false)
        {
            rb.AddForce(Vector3.up * jumpForceFactor, ForceMode.Impulse);
            anim.SetBool("isJumping", true);
        }

        //Animator Controller setting parameter isRunning
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            anim.SetBool("isRunning", true);
        }
        else 
        {
            anim.SetBool("isRunning", false);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Player has collided with " + collision.collider.tag);
        foreach (ContactPoint contact in collision.contacts)
        {
            Debug.DrawRay(contact.point, contact.normal, Color.white);
        }

        if (collision.collider.CompareTag("House"))
        {
            if (anim.GetBool("isJumping") == true) {
                anim.SetBool("isJumping", false);
                jumpDust.Play();
                jumpSfx.Play();
                
                // Decrease buoyancy
                Buoyancy buoyancyController = collision.gameObject.GetComponent<Buoyancy>();
                buoyancyController.buoyantForce -= jumpBuoyantForceDecrement;
                
                //Horizontal force, same code from buoyancy
                Vector3 normalisePlayerPos = new Vector3(transform.position.x - 0.34f, transform.position.y, transform.position.z + 1.23f);
                Vector3 normaliseHousePos = new Vector3(collision.transform.position.x + 3.8f, collision.transform.position.y, collision.transform.position.z + 4.9f);
                float distance = Vector3.Distance(normaliseHousePos, normalisePlayerPos);  // Range is around (6, 12)
                float normalisedBaseDiff = (distance - playerHouseDistanceRangeLow + normalisePlusFactor);
                float normalisedForceFactor = Mathf.Min(Mathf.Pow(normalisedBaseDiff, magnifiedPower), playerMaxStayForceFactor);
                Vector3 forceVector = new Vector3(
                    normalisePlayerPos.x - normaliseHousePos.x, 
                    normalisePlayerPos.y - normaliseHousePos.y, 
                    normalisePlayerPos.z - normaliseHousePos.z
                );  // Force from player to house
                Debug.Log("Jump force vector" + forceVector);
                Vector3 horizontalForceVector = new Vector3(forceVector.x, 0f, forceVector.z);
                collision.collider.attachedRigidbody.AddForce(horizontalForceVector / forceVectorDampening * normalisedForceFactor, ForceMode.Impulse);
                Vector3 verticalForceVector = new Vector3(0f, forceVector.y, 0f);
                ContactPoint firstContactPoint = collision.contacts[0];
                collision.collider.attachedRigidbody.AddForceAtPosition(verticalForceVector * playerDropForceFactor, firstContactPoint.point);  // This will rotate the house
                
                
                
            }
        }
    }

    private void OnTriggerEnter(Collider triggerCollider)
    {
        Debug.Log("Player has collided with " + triggerCollider.tag);
        
        if (triggerCollider.tag == "Flood" && transform.position.y <= -2)
        {
            Debug.Log("Game Over");
            GameOver();
        }
    }

    private void OnTriggerStay(Collider triggerCollider) 
    {
        if (triggerCollider.tag == "Flood" && transform.position.y <= -2)
        {
            Debug.Log("Game Over");
            GameOver();
        }
    }

    private void GameOver()
    {
        gameOverScreen.SetActive(true);

        Time.timeScale = 0f;
    }
}
