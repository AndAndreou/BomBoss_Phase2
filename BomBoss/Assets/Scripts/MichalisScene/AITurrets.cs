using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AITurrets : MonoBehaviour {

    // Rigidbody rigidbody;
    public int gunDamage = 1;                                           // Set the number of hitpoints that this gun will take away from shot objects with a health script
    public float fireRate = 0.25f;                                      // Number in seconds which controls how often the player can fire
    public float weaponRange = 10f;                                     // Distance in Unity units over which the player can fire
    public float hitForce = 100f;                                       // Amount of force which will be added to objects with a rigidbody shot by the player
    public Transform gunEnd;											// Holds a reference to the gun end object, marking the muzzle location of the gun

    public GameObject gunBase;
    public GameObject gunCylinder;
    public float rotateSpeed = 20.0f;

    private WaitForSeconds shotDuration = new WaitForSeconds(0.07f);    // WaitForSeconds object used by our ShotEffect coroutine, determines time laser line will remain visible
    private AudioSource gunAudio;                                       // Reference to the audio source which will play our shooting sound effect
    private LineRenderer laserLine;                                     // Reference to the LineRenderer component which will display our laserline
    private float nextFire;                                             // Float to store the time the player will be allowed to fire again, after firing

    float distanceToHovercraft = 0f;
    float scanRadius = 10f;

    // Use this for initialization
    void Start () {
        //rigidbody = GetComponent<Rigidbody>();
        // Get and store a reference to our LineRenderer component
        laserLine = GetComponent<LineRenderer>();

        // Get and store a reference to our AudioSource component
        gunAudio = GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update () {

        RaycastHit hit;

        Vector3 p1 = transform.position;
       
        // Set the start position for our visual effect for our laser to the position of gunEnd
        laserLine.SetPosition(0, gunEnd.position);


        //p1 = origin of cat,  scanRadius = the radius of the sphere, transform.forward = scan direction
        if (Physics.SphereCast(p1, scanRadius, transform.forward, out hit, weaponRange))
        {
            distanceToHovercraft = hit.distance;

            if(hit.transform.tag == GameRepository.hovercraftTag && Time.time > nextFire)
            {
                // target.position = hit.transform.position;
                Vector3 relativePosition = hit.transform.position - transform.position;

                gunBase.transform.rotation = Quaternion.LookRotation(relativePosition);
                gunCylinder.transform.rotation = Quaternion.LookRotation(relativePosition);

                //check where the enemy player is and rotate base accordingly
                //Vector3 currentRotation = gunBase.transform.localRotation.eulerAngles;
                //currentRotation.y = Mathf.Clamp(currentRotation.y, -30f, 30f);


                //if (gunBase.transform.position.z > hit.transform.position.z) {                 
                //        Debug.Log("Rotate clockwise to enemy");                   
                //   gunBase.transform.Rotate(Vector3.up * rotateSpeed, Space.Self);


                //    //Mathf.Clamp(gunBase.transform.rotation.y, -30, 30);                   
                //    gunBase.transform.Rotate(Vector3.up * rotateSpeed, Space.Self);                     

                //}

                //else if (gunBase.transform.position.z < hit.transform.position.z) {
                //       Debug.Log("Rotate anticlockwise to enemy");                     
                //        gunBase.transform.Rotate(Vector3.up * -rotateSpeed, Space.Self);                 

                //    //gunBase.transform.localRotation = Quaternion.Euler(currentRotation * -rotateSpeed);
                //}

                // Update the time when our turret can fire next   
                nextFire = Time.time + fireRate;

                // Start our ShotEffect coroutine to turn our laser line on and off                                           
                StartCoroutine(ShotEffect());

                // Set the end position for our laser line 
                laserLine.SetPosition(1, hit.point);

                ShipStatus shipStatus = hit.collider.GetComponentInParent<ShipStatus>();
             
                if (shipStatus != null)
                {
                    Debug.Log("shipStatus != null");
                    shipStatus.applyDamage(gunDamage, gameObject);
                }

                // Check if the object we hit has a rigidbody attached
                if (hit.rigidbody != null)
                {
                    // Add force to the rigidbody we hit, in the direction from which it was hit
                    hit.rigidbody.AddForce(-hit.normal * hitForce);
                }


                Debug.Log("Found a hovercraft!");
            }
        }
        else
        {
            laserLine.SetPosition(1, transform.forward * weaponRange);
        }
	}

    private IEnumerator ShotEffect()
    {
        // Play the shooting sound effect
        gunAudio.Play();

        // Turn on our line renderer
        laserLine.enabled = true;

        //Wait for .07 seconds
        yield return shotDuration;

        // Deactivate our line renderer after waiting
        laserLine.enabled = false;
    }


}
