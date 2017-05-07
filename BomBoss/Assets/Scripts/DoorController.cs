using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour {

    public GameObject door;
    public GameObject lamp;

    public Team team;
    public GoalPosition goalPosition;
    //public GameManagerBomb gameManager;

    public void Explode()
    {
        MyLog("Explode");
        door.SetActive(false);
        lamp.SetActive(false);
    }

    void MyLog(string msg)
    {
        Debug.Log(string.Format("DoorController-{0}", msg));
    }

}
