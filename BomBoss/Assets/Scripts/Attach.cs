using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attach : MonoBehaviour {

    private bool enableTake;
    private GameManagerBomb gm;

    public HoverControl hoverControl; // To fetch myPlayer

    // Use this for initialization
    void Start () {
        gm = GameObject.FindWithTag(GameRepository.gameManagerTag).GetComponent<GameManagerBomb>();
        enableTake = false;
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButtonDown("Magnet" + hoverControl.myPlayer.ToString()))
        {
            enableTake = !enableTake;
        }
    }

    void LateUpdate()
    {
        if (gm.GetBombState() == BombState.exploding)
        {
            enableTake = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.transform.tag == GameRepository.bombTag)
        {
            if (enableTake)
            {
                // Retrieve the grandparent which is the hovercraft
                GameObject grandParent = gameObject.transform.parent.transform.parent.gameObject;
                other.GetComponent<BombHover>().Attach(this.transform.position, grandParent);

                //other.GetComponent<Rigidbody>().isKinematic = true;
                //Destroy(other.GetComponent<Rigidbody>());
                //other.transform.position = this.transform.position;
                //other.transform.SetParent(this.transform);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == GameRepository.bombTag)
        {
            if (enableTake)
            {
                other.GetComponent<BombHover>().Detach();

                //other.gameObject.AddComponent<Rigidbody>();
                //other.GetComponent<Rigidbody>().isKinematic = false;
                //other.transform.SetParent(null);
            }
        }
    }
}
