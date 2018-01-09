using UnityEngine;
using System.Collections;

public class Destroy : MonoBehaviour {
        
    public void DestroyObject() {
        Animator anim = GetComponent<Animator>();
        if (anim != null)
            anim.enabled = false;
        Destroy(gameObject);
    }
}
