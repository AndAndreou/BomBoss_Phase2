using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour {

    public int kills;
    public int deaths;
    public int score;

	// Use this for initialization
	void Start () {
        kills = 0;
        deaths = 0;
        score = 0;
	}

    public void incrementKills()
    {
        kills++;
    }

    public void incrementDeaths()
    {
        deaths++;
    }

    public void incrementScore()
    {
        score++;
    }

}
