using UnityEngine;
using System.Collections;

public class NextFloor : MonoBehaviour {

    void OnTriggerStay(Collider col) {
        if (col.gameObject.layer == LayerMask.NameToLayer("Player"))
            GameManager.gm.LoadNextLevel();
    }
}
