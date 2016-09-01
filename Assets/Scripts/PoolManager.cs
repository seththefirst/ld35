using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PoolManager : MonoBehaviour
{
    private Dictionary<string, List<Transform>> pool;
    private Transform poolTransform;
    private Transform poolHolder;

    void Awake()
    {
        poolTransform = new GameObject( "PoolManager" ).transform;
        poolTransform.SetParent( transform );
    }

    void Start()
    {
        pool = new Dictionary<string, List<Transform>>();
    }
	
    // Update is called once per frame
    void Update()
    {
	
    }

    public Transform getNewPoolObject( Transform poolObject, Vector3 position )
    {
        int returnPoolObjIndex;
        Transform returnPoolObj;

        if ( pool.ContainsKey( poolObject.name ) )
        {
            returnPoolObjIndex = pool[ poolObject.name ].FindIndex( i => !i.gameObject.activeSelf );
            if ( returnPoolObjIndex >= 0 )
            {
                returnPoolObj = pool[ poolObject.name ][ returnPoolObjIndex ];
                returnPoolObj.position = position;
            }
            else
            {
                returnPoolObj = createNewPoolObject( poolObject, position );
            }
        }
        else
        { 
            pool[ poolObject.name ] = new List<Transform>();
            poolHolder = new GameObject( "Pool : " + poolObject.name ).transform;
            poolHolder.SetParent( poolTransform );
            returnPoolObj = createNewPoolObject( poolObject, position );
        }

        returnPoolObj.gameObject.SetActive( true );
        return returnPoolObj;
    }

    Transform createNewPoolObject( Transform poolObject, Vector3 position )
    {
        Transform returnPoolObj = Instantiate( poolObject, position, poolObject.rotation ) as Transform;
        returnPoolObj.SetParent( poolTransform.FindChild( "Pool : " + poolObject.name ) );
        returnPoolObj.name = poolObject.name;
        pool[ returnPoolObj.name ].Add( returnPoolObj );
        return returnPoolObj;
    }

}
