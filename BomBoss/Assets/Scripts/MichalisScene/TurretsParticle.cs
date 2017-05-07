using EazyTools.SoundManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretsParticle : MonoBehaviour {

    public GameObject shot;
    public Transform shotSpawn;

    // Rigidbody rigidbody;
    public int gunDamage = 1;                                           // Set the number of hitpoints that this gun will take away from shot objects with a health script
    public float fireRate = 0.25f;                                      // Number in seconds which controls how often the player can fire
    public float weaponRange = 10f;                                     // Distance in Unity units over which the player can fire
    public float hitForce = 100f;                                       // Amount of force which will be added to objects with a rigidbody shot by the player
    public Transform gunEnd;											// Holds a reference to the gun end object, marking the muzzle location of the gun

    public GameObject gunBase;
    public GameObject gunCylinder;
    public float rotateSpeed = 20.0f;
    public LayerMask layerMaskSphereCast;

    private WaitForSeconds shotDuration = new WaitForSeconds(0.07f);    // WaitForSeconds object used by our ShotEffect coroutine, determines time laser line will remain visible
    public AudioClip gunAudio;                                       // Reference to the audio source which will play our shooting sound effect
    private float volume;
    private LineRenderer laserLine;                                     // Reference to the LineRenderer component which will display our laserline
    private float nextFire;                                             // Float to store the time the player will be allowed to fire again, after firing

    float distanceToHovercraft = 0f;
    float scanRadius = 10f;

    // To tell which team this player belongs
     public Team myTeam;
    Team shipTeam;


    TurretShot turretShot;
    // Use this for initialization
    void Start () {
        // Get and store a reference to our LineRenderer component
        laserLine = GetComponent<LineRenderer>();

        // Get and store a reference to our AudioSource component
        //gunAudio = GetComponent<AudioSource>();

        volume = PlayerPrefs.GetFloat("Volume", 1f);
    }
	
	// Update is called once per frame
	void Update () {
        RaycastHit hit;

        Vector3 p1 = transform.position;
        
        //p1 = origin of cast,  scanRadius = the radius of the sphere, transform.forward = scan direction
        if (Physics.SphereCast( p1, scanRadius, transform.forward, out hit, weaponRange, layerMaskSphereCast))
        {
            shipTeam = hit.rigidbody.GetComponent<HoverControl>().myTeam;
            if (myTeam != shipTeam)
            {
                //distanceToHovercraft = hit.distance;
                Vector3 aimAT = (hit.transform.position - shotSpawn.transform.position).normalized;

                if (hit.transform.tag == GameRepository.hovercraftTag && Time.time > nextFire)
                {

                    // Update the time when our turret can fire next   
                    nextFire = Time.time + fireRate;

                    GameObject go = Instantiate(shot, shotSpawn.position, shotSpawn.rotation); //spawn a missile

                    SoundManager.PlaySound(gunAudio, volume, false,this.transform);

                    go.GetComponent<TurretShot>().parentTransform = this.transform;
                    go.GetComponent<TurretShot>().aimAtTarget = aimAT;

                    //ShipStatus shipStatus = hit.collider.GetComponentInParent<ShipStatus>();

                    //if (shipStatus != null)
                    //{
                    //    Debug.Log("shipStatus != null");
                    //    shipStatus.applyDamage(gunDamage, gameObject);
                    //}


                    Debug.Log("Found a hovercraft! at:" + hit.transform.position);
                    Debug.Log("Aim at:" + aimAT);
                }
            }
            else if(myTeam == shipTeam)
            {
                Debug.Log("Found an ally hovercraft! at:" + hit.transform.position);
            }

            
        }
       

    }

   
}
