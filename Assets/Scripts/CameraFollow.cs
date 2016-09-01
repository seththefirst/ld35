using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    public float smoothTime = .3f;

    private Transform _thisTransform;
    private Transform _player;
    private Vector3 _velocity = Vector3.zero;

    // Use this for initialization
    void Start()
    {
        _thisTransform = transform;
        _player = ( FindObjectOfType( typeof( PlayerControl ) ) as PlayerControl ).transform;
    }
	
    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 goalPos = _player.position;
        goalPos.y = _thisTransform.position.y;
        _thisTransform.position = Vector3.SmoothDamp( _thisTransform.position, goalPos, ref _velocity, smoothTime );
    }


}
