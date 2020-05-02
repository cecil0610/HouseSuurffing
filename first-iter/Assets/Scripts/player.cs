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
    public float jumpForceFactor;

    [Header("Game Over")]
    public GameObject gameOverScreen;

    void Start()
    {
        anim = GetComponent<Animator>();
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
            }
        }
    }

    private void OnTriggerEnter(Collider triggerCollider)
    {
        Debug.Log("Player has collided with " + triggerCollider.tag);

        if (triggerCollider.tag == "Flood")
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
