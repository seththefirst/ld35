using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerControl : MonoBehaviour
{
    public float moveSpeed;

    private Transform _thisTransform;
    private SpriteRenderer _spriteRender;
    private Rigidbody _rigidBody;
    private GameManager _gameManager;

    private Vector3 _newMovementDirection;
    private Vector3 _oldDirection;
    private ParticleSystem _bubbles;
    private string _fishName;
    private Text _textMsg;

    public bool freeze;

    void Awake()
    {
        _thisTransform = transform;
        _rigidBody = _thisTransform.GetComponent<Rigidbody>();
        _spriteRender = _thisTransform.GetComponent<SpriteRenderer>();
        _gameManager = ( GameManager )FindObjectOfType( typeof( GameManager ) );
        _bubbles = ( ParticleSystem )_thisTransform.FindChild( "bubbles" ).GetComponent<ParticleSystem>();
    }

    void Start()
    {
        Transform tmpCanvas = ( Transform )_thisTransform.FindChild( "Canvas" ).transform;
        _textMsg = ( UnityEngine.UI.Text )tmpCanvas.FindChild( "text" ).GetComponent<UnityEngine.UI.Text>();
        _textMsg.enabled = false;
        freeze = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if ( !freeze )
        {
            _gameManager.setPlayerTransform( _thisTransform );
            _newMovementDirection = new Vector3( Input.GetAxis( "Horizontal" ), 0.0f, Input.GetAxis( "Vertical" ) );
            _rigidBody.velocity = _newMovementDirection * moveSpeed;

            _rigidBody.position = new Vector3( _rigidBody.position.x,
                                               -3.56f,
                                               _rigidBody.position.z );
        }

    }

    void Update()
    {
        float direction = _rigidBody.velocity.x;
        string hitTag;

        if ( direction != 0 )
        {
            _spriteRender.flipX = direction < 0 ? false : true;
            _oldDirection = _rigidBody.velocity;

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

        if ( !freeze )
        {
            if ( Input.GetKeyDown( KeyCode.Space ) )
            {
                RaycastHit hit;
                if ( Physics.Raycast( _thisTransform.position, _oldDirection, out hit, .6f ) )
                {
                    hitTag = hit.transform.tag;
                    if ( hitTag == "Fish" || hitTag == "FakeFish" || hitTag == "PickStone" )
                    { 
                        _gameManager.AudioManager.playPokeSound( hit.transform.position );

                        if ( hitTag == "Fish" )
                        {
                            _gameManager.PoolManager.getNewPoolObject( _gameManager.pokeAnimation,
                                                                       hit.transform.position );
                            hit.transform.GetComponent<FishControl>().poke();
                            _fishName = hit.transform.name;
                        }
                        else if ( hitTag == "PickStone" )
                        { 
                            _gameManager.PoolManager.getNewPoolObject( _gameManager.pokeAnimation,
                                                                       hit.transform.position );
                            _gameManager.pickStone.hit( _thisTransform.name, _fishName );
                            _fishName = "";

                        }
                        else if ( hitTag == "FakeFish" )
                        {
                            _gameManager.PoolManager.getNewPoolObject( _gameManager.pokeAnimation,
                                                                       hit.transform.position );
                            hit.transform.GetComponent<FakeFish>().poke();
                        }
                    }

//                #if UNITY_EDITOR
//                Debug.Log( hit.transform.name + " Tag " + hitTag );
//                Debug.DrawRay( _thisTransform.position, _oldDirection, Color.red );
//                #endif

                }
            }
        }

        if ( Input.GetKeyDown( KeyCode.Escape ) )
        {
            _gameManager.goSleep();
        }
    }

    public void setFreeze( bool status )
    {
        freeze = status;
    }

    public void updateTextString( string text )
    {
        _textMsg.text = text;
    }

    public void showTextMsg( bool status )
    {
        _textMsg.enabled = status;
    }

}
