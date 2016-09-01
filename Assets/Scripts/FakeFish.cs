using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FakeFish : MonoBehaviour
{
    private Transform _thisTransform;
    private Text _textMsg;

    private string [] _pokeMsgs = new string[] { "Dudeeee!!!", "Hey!!!", "Leave me Alone!!!", "WTF?", "Stop that!!!", "Avaaaaaaa!!!", "Avaaaaaa!!!" };

    void Awake()
    {
        _thisTransform = transform;
    }

    void Start()
    {
        BoxCollider coll = _thisTransform.gameObject.AddComponent<BoxCollider>();
        coll.size = new Vector3( .5f, .5f, .5f );
        _thisTransform.tag = "FakeFish";
        Transform tmpCanvas = ( Transform )_thisTransform.FindChild( "Canvas" ).transform;
        _textMsg = ( UnityEngine.UI.Text )tmpCanvas.FindChild( "text" ).GetComponent<UnityEngine.UI.Text>();
        _textMsg.enabled = false;
    }

    public void poke()
    {
        _textMsg.text = _pokeMsgs[ Random.Range( 0, _pokeMsgs.Length ) ];
        if ( !_textMsg.enabled )
        {
            StartCoroutine( displayPokeMsg() );
        }
    }

    IEnumerator displayPokeMsg()
    {
        _textMsg.enabled = true;
        yield return new WaitForSeconds( 0.5f );
        _textMsg.enabled = false;
    }
}
