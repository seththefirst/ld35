using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PickStone : MonoBehaviour
{
    private Transform _thisTransform;
    private GameManager _gameManager;
    private bool _openForPick;
    private List<string> _fishsForPick;
    private Text _textMsg;


    void Awake()
    {
        _gameManager = ( GameManager )FindObjectOfType( typeof( GameManager ) );
        _thisTransform = transform;
    }

    void Start()
    {
        Transform tmpCanvas = ( Transform )_thisTransform.FindChild( "Canvas" ).transform;
        _textMsg = ( UnityEngine.UI.Text )tmpCanvas.FindChild( "text" ).GetComponent<UnityEngine.UI.Text>();
        _textMsg.enabled = true;

        _gameManager.setPickStone( this );
        _fishsForPick = new List<string>();
        _openForPick = false;
    }

    void Update()
    {
	
    }

    public void hit( string hitTag, string fishName )
    {
        if ( _gameManager.gameState.Equals( GameManager.GameState.Idle ) )
        {
            _textMsg.enabled = false;
            _gameManager.AudioManager.playPickStoneSound( _thisTransform.position );
            _gameManager.newRound();

        }
        else if ( _gameManager.gameState.Equals( GameManager.GameState.Running ) )
        { 
            if ( _openForPick && _fishsForPick.Contains( fishName ) )
            { 
                _gameManager.AudioManager.playPickStoneSound( _thisTransform.position );
                switch ( hitTag )
                {
                    case "Player":
                        _gameManager.updatePlayerCounter();
                        break;

                    case "Fish":
                        _gameManager.updateFishCounter();
                        break;
                }
                _fishsForPick.Remove( fishName );
                _openForPick = false;
            }
        }
    }

    public void resetMsg()
    {
        _textMsg.enabled = true;
    }

    public void setOpenForPick( string fishName )
    {
        _openForPick = true;
        _fishsForPick.Add( fishName );
    }
}
