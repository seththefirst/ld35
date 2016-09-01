using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public enum UIType
    {
        GameStart,
        GameUI,
        GameOver
    }

    public UIType uiType;
    private Transform _thisTransform;
    private GameManager _gameManager;
    private Text _txtPlayerCounter;
    private Text _txtFishsCounter;
    private Text _txtTimer;
    private Transform _pnlFishs;
    private Text _restartMsg;
    private Transform _mainMenu;
    private Transform _instructions;

    private float timeToBlink = .3f;
    private float acumBlink;

    void Awake()
    {
        _gameManager = ( GameManager )FindObjectOfType( typeof( GameManager ) );
        _thisTransform = transform;

        if ( uiType.Equals( UIType.GameOver ) || uiType.Equals( UIType.GameUI ) )
        { 
            _txtFishsCounter = ( Text )_thisTransform.FindChild( "lblfishs" ).GetComponent<Text>();
            _txtPlayerCounter = ( Text )_thisTransform.FindChild( "lblplayer" ).GetComponent<Text>();
            _txtTimer = ( Text )_thisTransform.FindChild( "lbltimer" ).GetComponent<Text>();
            _pnlFishs = ( Transform )_thisTransform.FindChild( "pnlfishs" ).transform;
        }

        if ( uiType.Equals( UIType.GameOver ) || uiType.Equals( UIType.GameStart ) )
        {
            _restartMsg = ( Text )_thisTransform.FindChild( "lblrestart" ).GetComponent<Text>();
        }

        if ( uiType.Equals( UIType.GameStart ) )
        {
            _mainMenu = _thisTransform.FindChild( "MainScreen" );
            _instructions = _thisTransform.FindChild( "Instructions" );
        }

        if ( uiType.Equals( UIType.GameOver ) || uiType.Equals( UIType.GameUI ) )
        { 
            _gameManager.OnUpdateCounters += updateCounter;
            _gameManager.OnUpdateTimer += updateTimer;
        }

    }


    // Use this for initialization
    void Start()
    {
        acumBlink = 0;

        if ( uiType.Equals( UIType.GameStart ) )
        {
            _mainMenu.gameObject.SetActive( true );
            _instructions.gameObject.SetActive( false );
        }
    }
	
    // Update is called once per frame
    void Update()
    {
        if ( uiType.Equals( UIType.GameOver ) || uiType.Equals( UIType.GameStart ) )
        {
            acumBlink += Time.deltaTime;
            if ( acumBlink >= timeToBlink )
            {
                acumBlink = 0;
                _restartMsg.enabled = !_restartMsg.enabled;
            }

            if ( Input.GetKeyDown( KeyCode.Space ) )
            {
                if ( uiType.Equals( UIType.GameStart ) )
                {
                    if ( _mainMenu.gameObject.activeSelf )
                    { 
                        _mainMenu.gameObject.SetActive( false );
                        _instructions.gameObject.SetActive( true );
                    }
                    else
                    { 
                        _mainMenu.gameObject.SetActive( true );
                        _instructions.gameObject.SetActive( false );
                        _gameManager.restartGame();
                    }
                }
                else
                { 
                    _gameManager.restartGame();
                }
            }
        }
    }

    public void updateCounter()
    {
        _txtFishsCounter.text = _gameManager.fishCount.ToString();
        _txtPlayerCounter.text = _gameManager.playerCount.ToString();

        for ( int ifish = 0; ifish < _pnlFishs.childCount; ifish++ )
        {
            if ( ifish < _gameManager.amountOfFishsAlive )
            {
                if ( !_pnlFishs.GetChild( ifish ).gameObject.activeSelf )
                { 
                    _pnlFishs.GetChild( ifish ).gameObject.SetActive( true );
                }
            }
            else
            { 
                _pnlFishs.GetChild( ifish ).gameObject.SetActive( false );
            }
        }
    }

    public void updateTimer()
    {
        _txtTimer.text = Mathf.RoundToInt( _gameManager.acumLevelTimer ).ToString();
    }
}
