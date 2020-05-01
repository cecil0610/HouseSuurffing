﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour
{

    private Animator anim;
        
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

        if (Input.GetKey(KeyCode.W)) {
            transform.Translate(Vector3.forward * Time.deltaTime * forwardFactor, Space.World);
        }
        if (Input.GetKey(KeyCode.S)) {
            transform.Translate(Vector3.back * Time.deltaTime * backFactor, Space.World);
        }
        if (Input.GetKey(KeyCode.A)) {
            transform.Translate(Vector3.left * Time.deltaTime * sideFactor, Space.World);
            // transform.eulerAngles.y += -rotspd * Time.deltaTime * 7;  // TODO rotation not working
        }
        if (Input.GetKey(KeyCode.D)) {
            transform.Translate(Vector3.right * Time.deltaTime * sideFactor, Space.World);
            // transform.eulerAngles.y += rotspd * Time.deltaTime * 7;  // TODO rotation not working
        }

        //Animator Controller setting parameter isRunning
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            anim.SetBool("isRunning", true);
        }
        else {
            anim.SetBool("isRunning", false);
        }
    }
}
