using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//enum for round fsm state
public enum RoundState
{
    isNull = -1,
    starting = 0,
    running = 1,
    paused = 2,
    unPausing = 3,
    finished = 4
}
