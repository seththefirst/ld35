using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CavesManager : MonoBehaviour
{
    public List<Transform> fishs;
    public List<Cave> caves;

    public Cave pickStoneCave;
    public Cave idleCave;

    // Use this for initialization
    void Awake()
    {
        caves = new List<Cave>();
    }
	
    // Update is called once per frame
    void Update()
    {
	
    }

    public void registerCave( Cave cave )
    {
        if ( cave.transform.name == "PickStoneCave" )
        {
            pickStoneCave = cave;
        }
        else if ( cave.transform.name == "IdleCave" )
        {
            idleCave = cave;
        }
        else
        { 
            caves.Add( cave );
            cave.spawnFishs();
        }
    }

    public Cave getRandomCave()
    {
        return caves[ Random.Range( 0, caves.Count ) ];
    }

    public void getRandomSpot( ref Sprite fishToShift, ref Vector3 spotPos )
    {
        Cave caveToHide = getRandomCave();
        int randomSpotIndex;
        while ( !caveToHide.hasFreeSpots() )
        {
            caveToHide = getRandomCave();
        }
        randomSpotIndex = Random.Range( 0, caveToHide.spots.Count );
        while ( caveToHide.fakeFishs.Exists( i => i.spotIndex == randomSpotIndex ) || caveToHide.spots[ randomSpotIndex ].ocuppied )
        {
            randomSpotIndex = Random.Range( 0, caveToHide.spots.Count );
        }
        caveToHide.spots[ randomSpotIndex ].ocuppied = true;

        fishToShift = caveToHide.fakeFishs[ Random.Range( 0, caveToHide.fakeFishs.Count ) ].fish.GetComponent<SpriteRenderer>().sprite;
        spotPos = caveToHide.spots[ randomSpotIndex ].spot.position;
    }

    public Vector3 getRandomIdleSpot()
    {
        int randomSpotIndex;
        randomSpotIndex = Random.Range( 0, idleCave.spots.Count );
        while ( idleCave.fakeFishs.Exists( i => i.spotIndex == randomSpotIndex ) || idleCave.spots[ randomSpotIndex ].ocuppied )
        {
            randomSpotIndex = Random.Range( 0, idleCave.spots.Count );
        }
        idleCave.spots[ randomSpotIndex ].ocuppied = true;

        return idleCave.spots[ randomSpotIndex ].spot.position;
    }

    public Sprite getRandomObject( Cave cave )
    {
        return cave.fakeFishs[ Random.Range( 0, cave.fakeFishs.Count ) ].fish.GetComponent<SpriteRenderer>().sprite;
    }

}
