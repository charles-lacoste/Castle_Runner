using UnityEngine;
using System.Collections;

public class BossOrb : MonoBehaviour {

    private int _damage = 3;
	
	// Update is called once per frame
	void Update () {
        if (!GameManager.gm.GetPause())
            transform.Translate(Vector3.up * 3.0f * Time.deltaTime);
	}
    
    void OnCollisionEnter(Collision col) {
        if(col.gameObject.layer == LayerMask.NameToLayer("Player")) {
            col.gameObject.GetComponent<Player>().ReduceLife(_damage);
        }
        Destroy(gameObject);
    }
}
