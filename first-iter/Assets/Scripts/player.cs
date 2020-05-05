using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour
{
    private Animator anim;

    [Header("Player Parameters")]
    public float fSpeed;
    public float fRotation;

    [Header("Player Landing effects")]
    public ParticleSystem jumpDust;
    public AudioSource jumpSfx;

    [Header("Physics coefficients")]
    public float jumpForceFactor;
    public int playerDropForceFactor;
    public float playerMaxStayForceFactor;
    public float playerHouseDistanceRangeLow;
    public float playerHouseDistanceRangeHigh;
    public int normalisePlusFactor;
    public int magnifiedPower;

    [Header("Game Over")]
    public GameObject gameOverScreen;

    void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetBool("isJumping", true);
    }

    // Update is called once per frame
    void Update()
    {
        // TODO move the config somewhere else
        // Movement multiply factors
        int forwardFactor = 8;
        int backFactor = 3;
        int sideFactor = 5;
        int jumpFactor = 15;

        // anim.Play("run");

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
        if (Input.GetKey(KeyCode.Space))
        {
            transform.Translate(Vector3.up * Time.deltaTime * jumpFactor, Space.World);
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
                ContactPoint firstContactPoint = collision.contacts[0];
                collision.collider.attachedRigidbody.AddForceAtPosition(Vector3.down * jumpForceFactor, firstContactPoint.point);
                jumpDust.Play();
                jumpSfx.Play();
                Debug.Log("landed!");

                //Horizontal force, same code from buoyancy
                Vector3 normalisePlayerPos = new Vector3(transform.position.x + 3.8f, transform.position.y, transform.position.z + 4.9f);
                Vector3 normaliseHousePos = new Vector3(collision.transform.position.x - 0.34f, collision.transform.position.y, collision.transform.position.z + 1.23f);
                float distance = Vector3.Distance(normaliseHousePos, normalisePlayerPos);  // Range is around (6, 12)
                float normalisedBaseDiff = (distance - playerHouseDistanceRangeLow + normalisePlusFactor);
                float normalisedForceFactor = Mathf.Min(Mathf.Pow(normalisedBaseDiff, magnifiedPower), playerMaxStayForceFactor);
                // TODO improve the direction of the force
                Vector3 forceVector = (normalisePlayerPos - normaliseHousePos) / 100;  // Force from player to house
                forceVector = new Vector3(forceVector.x, 0f, forceVector.z);
                collision.collider.attachedRigidbody.AddRelativeForce(forceVector * normalisedForceFactor);
            }
        }
    }

    private void OnTriggerEnter(Collider triggerCollider)
    {
        Debug.Log("Player has collided with " + triggerCollider.tag);
        
        if (triggerCollider.tag == "Flood" && transform.position.y < 0)
        {
            Debug.Log("Game Over");
            GameOver();
        }
    }

    private void OnTriggerStay(Collider triggerCollider) 
    {
        Debug.Log(transform.position.y);
        if (triggerCollider.tag == "Flood" && transform.position.y < 0)
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
