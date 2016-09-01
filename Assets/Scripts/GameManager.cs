using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        Sleeping,
        Idle,
        Running,
        GameOver
    }

    public event System.Action OnNewRound;
    public event System.Action OnUpdateCounters;
    public event System.Action OnUpdateTimer;

    public int playerCount;
    public int fishCount;

    public float minFishSpeed = 1.5f;
    public float maxFishSpeed = 3f;
    public float minSleepTime = 1f;
    public float maxSleepTime = 2f;

    public int amountOfFishsToSpawn = 3;
    public int amountOfFishsAlive;

    public float playerFreezeTime = 10f;
    public float acumPlayerFreeze;

    public float levelTimer = 60f;
    public float acumLevelTimer = 0;

    public Transform pokeAnimation;
    public Transform sleepAnimation;

    public Canvas gameStartCanvas;
    public Canvas gameUICanvas;
    public Canvas gameOverCanvas;

    [HideInInspector]
    public Transform playerTransform;
    public PlayerControl playerControl;
    [HideInInspector]
    public PickStone pickStone;

    private CavesManager _cavesManager;
    private PoolManager _poolManager;
    private AudioManager _audioManager;

    private List<FishControl> _fishs;

    public GameState gameState;

    public CavesManager CavesManager
    {
        get
        {
            if ( _cavesManager == null )
            {
                _cavesManager = transform.GetComponent<CavesManager>();
            }
            return _cavesManager;
        }
    }

    public PoolManager PoolManager
    {
        get
        {
            if ( _poolManager == null )
            {
                _poolManager = transform.GetComponent<PoolManager>();
            }
            return _poolManager;
        }
    }

    public AudioManager AudioManager
    {
        get
        {
            if ( _audioManager == null )
            {
                _audioManager = transform.GetComponent<AudioManager>();
            }
            return _audioManager;
        }
    }

    void Awake()
    {
        _fishs = new List<FishControl>();
    }

    // Use this for initialization
    void Start()
    {
        goSleep();

        resetCounters();
    }
	
    // Update is called once per frame
    void Update()
    {
        if ( gameState.Equals( GameState.Running ) )
        { 
            if ( playerControl.freeze )
            {
                acumPlayerFreeze -= Time.deltaTime;
                playerControl.updateTextString( "New Game in ... " + Mathf.RoundToInt( acumPlayerFreeze ).ToString() );
                if ( Mathf.RoundToInt( acumPlayerFreeze ) == 0 )
                {
                    playerControl.setFreeze( false );
                    playerControl.showTextMsg( false );
                }
            }
            else
            { 
                acumLevelTimer -= Time.deltaTime;
                updateTimer();
            }

            if ( Mathf.RoundToInt( acumLevelTimer ) == 0 || amountOfFishsAlive == 0 )
            {
                gameOver();
            }
        }
    }

    void updateTimer()
    {
        if ( OnUpdateTimer != null )
        {
            OnUpdateTimer();
        }
    }

    void resetCounters()
    {
        playerCount = 0;
        fishCount = 0;
        amountOfFishsAlive = amountOfFishsToSpawn;
        acumLevelTimer = levelTimer;
        updateVisualCounters();
        updateTimer();
    }

    void updateVisualCounters()
    {
        if ( OnUpdateCounters != null )
        {
            OnUpdateCounters();
        }
    }

    void freezePlayer()
    {
        playerControl.setFreeze( true );
        playerControl.showTextMsg( true );
        acumPlayerFreeze = playerFreezeTime;
    }

    void gameOver()
    {
        gameUICanvas.gameObject.SetActive( false );
        gameOverCanvas.gameObject.SetActive( true );
        updateVisualCounters();
        updateTimer();
        playerControl.enabled = false;
        gameState = GameState.GameOver;
    }

    public void newRound()
    {
        if ( OnNewRound != null )
        {
            OnNewRound();
        }
        resetCounters();
        freezePlayer();
        gameState = GameState.Running;
    }

    public void updateFishCounter( int amount = 1 )
    {
        fishCount += amount;
        amountOfFishsAlive--;
        updateVisualCounters();
    }

    public void updatePlayerCounter( int amount = 1 )
    {
        playerCount += amount;
        amountOfFishsAlive--;
        updateVisualCounters();
    }

    public void setPickStone( PickStone pickStone )
    {
        this.pickStone = pickStone;
    }

    public void setPlayerTransform( Transform player )
    {
        this.playerTransform = player;
        this.playerControl = player.GetComponent<PlayerControl>();
    }

    public void restartGame()
    {
        gameStartCanvas.gameObject.SetActive( false );
        gameUICanvas.gameObject.SetActive( true );
        gameOverCanvas.gameObject.SetActive( false );
        pickStone.resetMsg();
        resetCounters();
        updateVisualCounters();
        resetFishs();
        playerControl.enabled = true;
        AudioManager.setGamePlayMusic();
        gameState = GameState.Idle;
    }

    public void goSleep()
    {
        gameStartCanvas.gameObject.SetActive( true );
        gameUICanvas.gameObject.SetActive( false );
        gameOverCanvas.gameObject.SetActive( false );
        if ( playerControl != null )
        { 
            playerControl.enabled = false;
        }
        AudioManager.setMainMenuMusic();
        gameState = GameState.Sleeping;
    }

    public void resetFishs()
    {
        foreach ( FishControl fishControl in _fishs )
        {
            fishControl.transform.position = pickStone.transform.position;
            fishControl.restart();
        }
    }

    public void registerFish( FishControl fishControl )
    {
        _fishs.Add( fishControl );
    }
}
