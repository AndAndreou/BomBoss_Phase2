using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUps : MonoBehaviour {

    ShipStatus shipStatus;
    
    public int healAmount;


	// Use this for initialization
	void Start () {
        shipStatus = GetComponent<ShipStatus>();

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider other)
    {
        if ((gameObject.tag == "HealthPowerUp") && (other.tag == "Player"))
        {        
            MyLog(string.Format("Health pack collided with: {0}", other.tag));
        }

        if ((gameObject.tag == "ShieldPowerUp") && (other.tag == "Player"))
        {
            shipStatus.hasShield = true;
            MyLog(string.Format("Shield pack collided with: {0}", other.tag));
        }

        if ((gameObject.tag == "BoostPowerUp") && (other.tag == "Player"))
        {
            shipStatus.hasBoost = true;
            MyLog(string.Format("Boost pack collided with: {0}", other.tag));
        }

        if ((gameObject.tag == "JumpPowerUp") && (other.tag == "Player"))
        {
            shipStatus.hasJump = true;
            MyLog(string.Format("Jump pack collided with: {0}", other.tag));
        }

    }



    void MyLog(string msg)
    {
        Debug.Log(string.Format("PowerUp-{0}", msg));
    }








}
