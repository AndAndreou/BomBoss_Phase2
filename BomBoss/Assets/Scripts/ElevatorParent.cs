using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorParent : MonoBehaviour {

    private Animator animator;

    // Use this for initialization
    void Start () {
        animator = this.GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    public void ResetAnimationVar()
    {
        StartCoroutine(ResetWithDelay());
    }
    IEnumerator ResetWithDelay()
    {
        yield return new WaitForSeconds(2);
        animator.SetBool("GoUp", false);
    }
}
