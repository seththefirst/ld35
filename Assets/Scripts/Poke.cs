using UnityEngine;
using System.Collections;

public class Poke : MonoBehaviour
{
    private Transform _thisTransform;

    void Awake()
    {
        _thisTransform = transform;
    }

    public void endPokeAnim()
    {
        _thisTransform.gameObject.SetActive( false );
    }
	
}
