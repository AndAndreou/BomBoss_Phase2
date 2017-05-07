using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionController : MonoBehaviour {

    private ParticleSystem myPS;
    private ParticleSystem[] childrenPS;
    //public float duration = 3f;

    void Start()
    {
        myPS = gameObject.GetComponent<ParticleSystem>();
        childrenPS = GetComponentsInChildren<ParticleSystem>();
        //SetIsLooping(false);
        //SetDuration(duration);
    } 

    //public void SetIsLooping(bool value)
    //{
    //    // Self
    //    this.GetComponent<ParticleSystem>().loop = value;
    //    // Children
    //    foreach (ParticleSystem ps in particleSystems)
    //    {
    //        ps.loop = value;
    //    }
    //}

    //public void SetDuration(float duration)
    //{
    //    // Self
    //    this.GetComponent<ParticleSystem>().duration = duration;
    //    // Children
    //    foreach (ParticleSystem ps in particleSystems)
    //    {
    //        ps.duration = duration;
    //    }
    //}

    public void PlayEffect()
    {
        // Self
        myPS.Play();
        // Children
        foreach (ParticleSystem ps in childrenPS)
        {
            ps.Play();
        }
    }

    public void PauseEffect()
    {
        // Self
        myPS.Pause();
        // Children
        foreach (ParticleSystem ps in childrenPS)
        {
            ps.Pause();
        }
    }

    public void StopEffect()
    {
        // Self
        myPS.Stop();
        // Children
        foreach (ParticleSystem ps in childrenPS)
        {
            ps.Stop();
        }
    }
}
