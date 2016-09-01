using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cave : MonoBehaviour
{
    public List<SpawnedFish> fakeFishs;
    public List<Spot> spots;

    private Transform _thisTransform;
    private GameManager _gameManager;
    private Transform _fishsHolder;


    [System.Serializable]
    public struct SpawnedFish
    {
        public Transform fish;
        public int spotIndex;

        public SpawnedFish( Transform fish, int spotIndex )
        {
            this.fish = fish;
            this.spotIndex = spotIndex;
        }
    }

    void Awake()
    {
        _gameManager = ( GameManager )FindObjectOfType( typeof( GameManager ) );
        _thisTransform = transform;
        _fishsHolder = _thisTransform.FindChild( "Objects" );
        setupSpots();
        _gameManager.CavesManager.registerCave( this );

    }

    // Use this for initialization
    void Start()
    {
        _gameManager.OnNewRound += resetSpots;
    }
	
    // Update is called once per frame
    void Update()
    {
	
    }

    public void setupSpots()
    {
        Transform spotsTransform = _thisTransform.FindChild( "Spots" );
        spots = new List<Spot>();
        for ( int ispot = 0; ispot < spotsTransform.childCount; ispot++ )
        {
            spots.Add( new Spot( spotsTransform.GetChild( ispot ) ) );
        }
    }

    public void spawnFishs()
    {
        int spotIndex;
        int amountOfFishsToPlace = Random.Range( 3, spots.Count - 2 );
        int fishIndex;
        Transform newFish;
        List<int> alreadyPlaced = new List<int>();

        fakeFishs = new List<SpawnedFish>();

        for ( int iplaceFish = 0; iplaceFish < amountOfFishsToPlace; iplaceFish++ )
        {
            spotIndex = Random.Range( 0, spots.Count );
            while ( alreadyPlaced.Contains( spotIndex ) )
            {
                spotIndex = Random.Range( 0, spots.Count );
            }
            alreadyPlaced.Add( spotIndex );
            fishIndex = Random.Range( 0, _gameManager.CavesManager.fishs.Count );
            newFish = Instantiate( _gameManager.CavesManager.fishs[ fishIndex ],
                                   spots[ spotIndex ].spot.position,
                                   Quaternion.identity ) as Transform;
            newFish.SetParent( _fishsHolder );
            newFish.localEulerAngles = new Vector3( 0, 0, 0 );
            newFish.GetComponent<SpriteRenderer>().flipX = Random.Range( 0f, 1f ) > .5f ? true : false;
            newFish.gameObject.AddComponent<FakeFish>();
            fakeFishs.Add( new SpawnedFish( newFish, spotIndex ) );
        }
    }

    void resetSpots()
    {
        foreach ( Spot spot in spots )
        {
            spot.ocuppied = false;
        }
    }

    public bool hasFreeSpots()
    {
        return spots.Exists( i => !i.ocuppied );
    }


}
