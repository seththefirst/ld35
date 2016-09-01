using UnityEngine;
using System.Collections;

public class FishControl : MonoBehaviour
{
    public enum FishState
    {
        Idle,
        MovingToIdle,
        Sleeping,
        PickingPlace,
        Hiding,
        Shifted,
        ReturningToPickStone
    }

    public FishState fishState;
    public float minDistanceToShift = .1f;
    public float minDistanceToPick = .5f;
    public float minDistanceToFlick = 5f;
    public float timeBetweenFlick = 4f;
    public float timeBetweenCheckDistance = 1f;
    public float sleepTime;

    private float acumFlick = 3f;
    private float acumCheckDistance = 6f;
    private float acumSleepTime;
    private bool _flickTexture;
    private bool _sleeping;

    private Transform _thisTransform;
    private GameManager _gameManager;
    private NavMeshAgent _navMeshAgent;
    private SpriteRenderer _artSpriteRender;
    private Vector3 _spotPos;
    private Transform _artTransform;
    private Sprite _fishToShift;
    private Sprite _baseSprite;
    private ParticleSystem _bubbles;
    private Transform _sleepTransform;

    void Awake()
    {
        _gameManager = ( GameManager )FindObjectOfType( typeof( GameManager ) );
        _thisTransform = transform;
        _navMeshAgent = _thisTransform.GetComponent<NavMeshAgent>();
        _artTransform = _thisTransform.FindChild( "art" ) as Transform;
        _artSpriteRender = ( SpriteRenderer )_artTransform.GetComponent<SpriteRenderer>();
        _bubbles = ( ParticleSystem )_thisTransform.FindChild( "bubbles" ).GetComponent<ParticleSystem>();
        _baseSprite = _artSpriteRender.sprite;
        _fishToShift = _baseSprite;
    }

    void Start()
    {
        _navMeshAgent.updateRotation = false;
        _gameManager.OnNewRound += onNewRound;
        _gameManager.registerFish( this );
        restart();
        changeState( FishState.Idle );

    }

    void Update()
    {
        updateSpriteDirection();

        switch ( fishState )
        {
            case FishState.MovingToIdle:
                if ( Vector3.Distance( _thisTransform.position, _spotPos ) <= minDistanceToShift )
                {
                    changeState( FishState.Idle );
                }
                break;

            case FishState.Shifted:
                checkDistanceToPlayer();
                break;

            case FishState.Sleeping:
                acumSleepTime += Time.deltaTime;
                if ( acumSleepTime >= sleepTime )
                {
                    sleep( false );
                    changeState( FishState.ReturningToPickStone );
                }
                break;

            case FishState.PickingPlace:
                findSpotToHide();
                changeState( FishState.Hiding );
                break;

            case FishState.Hiding:
                if ( Vector3.Distance( _thisTransform.position, _spotPos ) <= minDistanceToShift )
                {
                    shift();
                }
                break;

            case FishState.ReturningToPickStone:
                if ( Vector3.Distance( _thisTransform.position, _spotPos ) <= minDistanceToPick )
                {
                    pickStone();
                }
                break;

            default:
                break;
        }
    }


    void changeState( FishState newState )
    {
        switch ( newState )
        {
            case FishState.MovingToIdle:
                shiftBack();
                _spotPos = _gameManager.CavesManager.getRandomIdleSpot();
                moveToSpot();
                fishState = newState;
                break;

            case FishState.Idle:
                shiftBack();
                _navMeshAgent.Stop();
                fishState = newState;
                break;

            case FishState.Hiding:
                if ( _fishToShift != null )
                {
                    moveToSpot();
                    fishState = newState;
                }
                break;

            case FishState.Sleeping:
                if ( !_sleeping )
                { 
                    sleep( true );
                }
                fishState = newState;
                break;

            case FishState.ReturningToPickStone:
                shiftBack();
                _spotPos = _gameManager.pickStone.transform.position;
                moveToSpot();
                fishState = newState;
                break;

            default:
                fishState = newState;
                break;
        }
    }

    void updateSpriteDirection()
    {
        float direction = _navMeshAgent.velocity.x;

        if ( direction != 0 )
        {
            _artSpriteRender.flipX = direction < 0 ? false : true;

            if ( !_bubbles.isPlaying )
            { 
                _bubbles.Play();
            }
            _bubbles.startSpeed = direction < 0 ? 2 : -2;
            _bubbles.transform.localPosition = new Vector3( ( .2f * ( direction < 0 ? 1 : -1f ) ),
                                                            0,
                                                            0 );
        }
        else
        {
            if ( _bubbles.isPlaying )
            { 
                _bubbles.Stop();
            }
        }

    }

    void findSpotToHide()
    {
        _gameManager.CavesManager.getRandomSpot( ref _fishToShift, ref _spotPos );
    }

    void moveToSpot()
    {
        _navMeshAgent.Resume();
        _navMeshAgent.SetDestination( _spotPos );
    }

    void shift()
    {
        _artSpriteRender.sprite = _fishToShift;
        _artTransform.localEulerAngles = new Vector3( 90, 0, 0 );
        _navMeshAgent.Stop();
        changeState( FishState.Shifted );
    }

    void shiftBack()
    {
        if ( _artSpriteRender.sprite != _baseSprite )
        { 
            _artSpriteRender.sprite = _baseSprite;
        }
    }


    void onNewRound()
    {
        shiftBack();
        changeState( FishState.PickingPlace );
    }

    void pickStone()
    {
        _gameManager.PoolManager.getNewPoolObject( _gameManager.pokeAnimation,
                                                   _gameManager.pickStone.transform.position );
        _gameManager.pickStone.hit( _thisTransform.tag, _thisTransform.name );
        changeState( FishState.MovingToIdle );
    }

    public void poke()
    {
        
        if ( fishState.Equals( FishState.Hiding ) || fishState.Equals( FishState.Shifted ) )
        { 
            _gameManager.pickStone.setOpenForPick( _thisTransform.name );
            changeState( FishState.ReturningToPickStone );
        }
        else if ( fishState.Equals( FishState.ReturningToPickStone ) )
        {
            changeState( FishState.Sleeping );
        }
    }

    void checkDistanceToPlayer()
    {
        acumCheckDistance += Time.deltaTime;
        acumFlick += Time.deltaTime;
        if ( acumCheckDistance >= timeBetweenCheckDistance )
        {
            acumCheckDistance = 0;
            if ( Vector3.Distance( _gameManager.playerTransform.position, _thisTransform.position ) <= minDistanceToFlick )
            {
                if ( acumFlick >= timeBetweenFlick )
                { 
                    acumFlick = 0;
                    if ( !_flickTexture )
                    { 
                        StartCoroutine( flick() );
                    }
                }
            }
        }
    }

    void sleep( bool status )
    {
        if ( status )
        { 
            if ( !_sleeping )
            {
                _sleeping = true;
                _navMeshAgent.Stop();
                acumSleepTime = 0;
                Vector3 animPos = _thisTransform.position;
                animPos.z += .5f;
                _sleepTransform = _gameManager.PoolManager.getNewPoolObject( _gameManager.sleepAnimation, animPos );
            }
        }
        else
        {
            _sleeping = false;
            _navMeshAgent.Resume();
            _sleepTransform.gameObject.SetActive( false );
        }
    }

    public void restart()
    {       
        _flickTexture = false;
        _sleeping = false;

        _navMeshAgent.speed = Random.Range( _gameManager.minFishSpeed, _gameManager.maxFishSpeed );
        sleepTime = Random.Range( _gameManager.minSleepTime, _gameManager.maxSleepTime );
        _navMeshAgent.Stop();

        acumFlick = ( timeBetweenFlick * 2 );
        acumCheckDistance = ( timeBetweenCheckDistance * 2 );
        acumSleepTime = ( sleepTime * 2 );
        changeState( FishState.MovingToIdle );
    }

    IEnumerator flick()
    {
        _flickTexture = true;
        _artSpriteRender.sprite = _baseSprite;
        yield return new WaitForSeconds( 0.2f );
        _artSpriteRender.sprite = _fishToShift;
        _flickTexture = false;
        yield return new WaitForSeconds( 0.2f );
    }

    #if UNITY_EDITOR
    void OnDrawGizmos()
    {
        UnityEditor.Handles.color = Color.black;
        UnityEditor.Handles.Label( transform.position, fishState.ToString() );
    }
    #endif
}
