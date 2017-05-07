using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//enum for bomb fsm state
public enum BombState
{
    isNull = -1,
    armed = 0,
    exploding = 1,
    exploded = 2,
    paused = 3,
    unPausing = 4
}
