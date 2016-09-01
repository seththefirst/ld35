using UnityEngine;
using System.Collections;

[System.Serializable]
public class Spot
{
    public Transform spot;
    public bool ocuppied;

    public Spot( Transform spot )
    {
        this.spot = spot;
        this.ocuppied = false;
    }

}
