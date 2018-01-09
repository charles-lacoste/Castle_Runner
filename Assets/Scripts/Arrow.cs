using UnityEngine;
using System.Collections;

public class Arrow : MonoBehaviour {
    [SerializeField]
    int _damage;

    public int GetDamage() {
        return _damage;
    }

    void OnCollisionEnter(Collision col) {
        if(col.gameObject.layer == LayerMask.NameToLayer("Player")) {
            col.gameObject.GetComponent<Player>().ReduceLife(_damage);
        }
        Destroy(gameObject);
    }

    void OnBecomeInvisible() {
        Destroy(gameObject);
    }
}
