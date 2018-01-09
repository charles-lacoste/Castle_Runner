using UnityEngine;
using System.Collections;

public class DoorTrigger : MonoBehaviour {
    [SerializeField]
    int _room;
    [SerializeField]
    bool _isBossRoom;
    	
    void OnTriggerEnter(Collider col) {
        if (col.gameObject.layer == LayerMask.NameToLayer("Player")) {
            GameManager.gm.LockRoom(_room, _isBossRoom);
            Destroy(gameObject);
        }
    }
}
