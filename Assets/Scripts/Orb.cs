using UnityEngine;
using System.Collections;

public class Orb : MonoBehaviour {
    [SerializeField]
    private int _damage;
    [SerializeField]
    private float _followSpeed;

    private Player _player;
	
    public int GetDamage() {
        return _damage;
    }

    void Start() {
        GameObject playerObj = GameObject.Find("Player");
        if (playerObj != null) {
            _player = playerObj.GetComponent<Player>();
        }
        if (_player == null) {
            Debug.Log("Cannot find 'Player' script");
        }
    }

    // Update is called once per frame
    void FixedUpdate () {
        if (!GameManager.gm.GetPause())
            transform.position = Vector3.MoveTowards(transform.position, _player.gameObject.transform.position, _followSpeed); // follow 
    }

    void OnCollisionEnter(Collision col) {
        if (col.gameObject.layer == LayerMask.NameToLayer("Player")) {
            col.gameObject.GetComponent<Player>().ReduceLife(_damage);
        }
        else if (col.gameObject.layer == LayerMask.NameToLayer("Bullet")) {
            Destroy(col.gameObject);
        }
        Destroy(gameObject);
    }

    void OnBecomeInvisible() {
        Destroy(gameObject);
    }
}
