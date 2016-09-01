using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{

    public AudioClip mainMenuMusic;
    public AudioClip gamePlayMusic;
    public AudioClip pokeSound;
    public AudioClip pickStoneSound;

    private AudioSource cameraAudioSource;

    void Awake()
    {
        cameraAudioSource = ( AudioSource )Camera.main.GetComponent<AudioSource>();
    }


    public void playPokeSound( Vector3 position )
    {
        AudioSource.PlayClipAtPoint( pokeSound, position, 2f );
    }

    public void playPickStoneSound( Vector3 position )
    {
        AudioSource.PlayClipAtPoint( pickStoneSound, position, 2f );
    }

    public void setMainMenuMusic()
    {
        if ( cameraAudioSource.isPlaying )
        {
            cameraAudioSource.Stop();
        }
        cameraAudioSource.clip = mainMenuMusic;
        cameraAudioSource.Play();
    }

    public void setGamePlayMusic()
    {
        if ( cameraAudioSource.isPlaying )
        {
            cameraAudioSource.Stop();
        }
        cameraAudioSource.clip = gamePlayMusic;
        cameraAudioSource.Play();
    }
}
