using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour {
    Animator _anim;
    BoxCollider _bc;
	void Start () {
        _anim = GetComponent<Animator>();
        _bc = GetComponent<BoxCollider>();
	}

    public void Open() {
        _anim.SetBool("Open", true);
        _anim.SetBool("Close", false);
    }

    public void Close() {
        _anim.SetBool("Open", false);
        _anim.SetBool("Close", true);
    }

    public void DisableCollider() {
        _bc.enabled = false;
    }

    public void EnableCollider() {
        _bc.enabled = true;
    }
}
