using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretShot : MonoBehaviour {

    public float shotSpeed;
    public float damageDealt;
    public float hitForce = 100000f;

    public GameObject mainProjectile;
    public ParticleSystem mainParticleSystem;

    public GameObject sparkParticle;

    [HideInInspector]
    public Transform parentTransform;

    public Vector3 aimAtTarget;

    // Use this for initialization
    void Start () {
        Vector3 fireAhead = this.transform.forward;
        GetComponent<Rigidbody>().velocity = aimAtTarget * shotSpeed;
        mainProjectile.SetActive(true);
    }
	
	// Update is called once per frame
	void Update () {
		
	}


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == GameRepository.hovercraftTag && collision.transform != parentTransform)
        {
            // If other hovercraft is hit
            Debug.Log("Shot hit a player");
            ShipStatus shipStatus = collision.gameObject.GetComponentInParent<ShipStatus>();
            if (shipStatus.shieldActivated)
            {
                shipStatus.currShieldHealth -= damageDealt;
                Debug.Log("Enemy had shield activated: " + shipStatus.currShieldHealth);
            }

            else if (!shipStatus.shieldActivated)
            {
                shipStatus.applyDamage(damageDealt, parentTransform.gameObject);
            }
        }

        if (collision.transform != parentTransform)
        {
            // Destroy the shot gameobject
            sparkParticle.SetActive(true);
            sparkParticle.GetComponent<ParticleSystem>().Play();
            this.GetComponent<MeshRenderer>().enabled = false;
            Destroy(this.GetComponent<Rigidbody>());
            Destroy(this.gameObject, 1f);
        }
    }
}
