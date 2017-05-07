using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour {

    public GameObject powerUp;
    public float value;
    public float respownTime; //in sec
    public PowerUpType powerUpType;

    private float time;

	// Use this for initialization
	void Start () {

        time = Time.time;
    }
	
	// Update is called once per frame
	void Update () {

        //if power up is not active
		if(powerUp.activeSelf == false)
        {
            //cheak if respown time expired
            if ((Time.time - time) >= respownTime)
            {
                powerUp.SetActive(true);
            }
        }
	}
    /*
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("test");
        //if this power up is active
        if (powerUp.activeSelf == true) {
            //if detect collision with hovercraft
            if (other.transform.tag == GameRepository.hovercraftTag)
            {
                //run get power function for hovercraft to collide
                other.gameObject.GetComponent<ShipStatus>().GetPowerUp(powerUpType, value);
                //deactive power up
                powerUp.SetActive(false);
                //get current time
                time = Time.time;
            }
        }
    }
    */
    private void OnTriggerStay(Collider other)
    {
        //Debug.Log("test");
        //if this power up is active
        if (powerUp.activeSelf == true)
        {
            //if detect collision with hovercraft
            if (other.transform.tag == GameRepository.hovercraftTag)
            {
                //run get power function for hovercraft to collide
                other.gameObject.transform.parent.gameObject.GetComponent<ShipStatus>().GetPowerUp(powerUpType, value);
                //deactive power up
                powerUp.SetActive(false);
                //get current time
                time = Time.time;
            }
        }
    }

}
