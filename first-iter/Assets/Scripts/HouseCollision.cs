using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseCollision : MonoBehaviour
{
    private bool isInvinsible;
    public float invinsibilityDuration;

    private void Start()
    {
        isInvinsible = false;
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.collider.tag == "Obstacle" && !isInvinsible)
        {
            //Debug.Log("We hit an " + collision.collider.tag);

            isInvinsible = true;

            Invoke("NotInvinsible", invinsibilityDuration);
        }
    }

    private void NotInvinsible()
    {
        isInvinsible = false;

        Debug.Log("No longer invinsible");
    }

}
