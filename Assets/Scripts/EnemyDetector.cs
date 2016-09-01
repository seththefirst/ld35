using UnityEngine;
using System.Collections;

public class EnemyDetector : MonoBehaviour
{
    private Transform _thisTransform;
    private FishControl _enemyControl;

    void Awake()
    {
        _thisTransform = transform;
        _enemyControl = _thisTransform.parent.GetComponent<FishControl>();
    }

    // Use this for initialization
    void Start()
    {
	
    }
	
    // Update is called once per frame
    void Update()
    {
	
    }

    void OnTriggerEnter2D( Collider2D other )
    {
        if ( other.CompareTag( "Player" ) )
        {
            RaycastHit2D hit = Physics2D.Raycast( _enemyControl.transform.position, other.transform.position );
            if ( hit.collider != null && hit.collider.CompareTag( "Player" ) )
            {
//                _enemyControl.playerSeen( hit.transform.position );
            }
        }
    }

    void OnTriggerExit2D( Collider2D other )
    {
        if ( other.CompareTag( "Player" ) )
        {
//            _enemyControl.playerOutOfView();
        }
    }
}