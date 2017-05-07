using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidController : MonoBehaviour
{
    void printCollided(Collider other)
    {
        MyLog(string.Format("Collided with: {0}-{1}", other.name, other.tag));
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == GameRepository.bombTag)
        {
            printCollided(other);
            BombController bomb = other.GetComponent<BombController>();
            if (bomb != null)
            {
                bomb.VoidCollided();
            }
        }

        if (other.tag == GameRepository.hovercraftTag)
        {
            printCollided(other);
            ShipController shipController = other.GetComponentInParent<ShipController>();
            if (shipController != null)
            {
                shipController.Die();
            }
        }
    }

    void MyLog(string msg)
    {
        Debug.Log(string.Format("Void-{0}", msg));
    }
}