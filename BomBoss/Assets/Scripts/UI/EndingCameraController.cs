using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingCameraController : MonoBehaviour {

    public int speed = 10;
    public ParticleSystem[] exhaustBoostParticles;

    private void Start()
    {
        foreach (ParticleSystem particleSystem in exhaustBoostParticles)
        {
            particleSystem.Play();
        }
    }

    // Update is called once per frame
    void Update () {
        transform.Rotate(new Vector3(0, 1, 0) * Time.deltaTime * speed);
    }
}
