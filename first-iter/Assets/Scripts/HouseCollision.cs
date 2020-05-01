using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseCollision : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    { 

        if (collision.collider.tag == "Obstacle")
        {
            Debug.Log("We hit an " + collision.collider.tag);
        }
    }
}
