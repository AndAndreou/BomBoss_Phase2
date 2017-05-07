using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doors : MonoBehaviour {

    private Animator animator;
    private bool doorOpen;

    private int counter;
	// Use this for initialization
	void Start () {
        animator = this.GetComponentInParent<Animator>();
        doorOpen = false;
        counter = 0;

    }
	
	// Update is called once per frame
	void Update () {
		
	}
    private void LateUpdate()
    {
        if (counter > 0)
        {
            animator.SetBool("IsOpen", true);
        }
        else
        {
            animator.SetBool("IsOpen", false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == GameRepository.hovercraftTag)
        {
            counter++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == GameRepository.hovercraftTag)
        {
            counter--;
        }
    }
}
