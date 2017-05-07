using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeController : MonoBehaviour {
    public MyObjectType myType;

    public void Explode ()
    {
        MyLog(myType.ToString());
        if (myType == MyObjectType.hovercraft)
        {
            this.gameObject.GetComponentInParent<ShipController>().Die();
        }
        else if (myType == MyObjectType.bomb)
        {

        }
        else if (myType == MyObjectType.door)
        {
            this.gameObject.GetComponent<DoorController>().Explode();
        }
    }

    void MyLog(string msg)
    {
        Debug.Log(string.Format("ExplodeController-{0}", msg));
    }
}
