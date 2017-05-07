using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour {

    private Animator animator;
    private bool goUp;
	// Use this for initialization
	void Start () {
        animator = this.transform.parent.GetComponent<Animator>();
        goUp = false;
        animator.SetBool("GoUp", goUp);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == GameRepository.hovercraftTag)
        {
            Debug.Log("GoUp");
            goUp = true;
            animator.SetBool("GoUp", goUp);
        }
    }

    void MyLog(string msg)
    {
        Debug.Log(string.Format("Elevator-{0}", msg));
    }
}
