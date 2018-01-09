using UnityEngine;
using System.Collections;

public class ItemRespawn : MonoBehaviour {
    [SerializeField]
    private bool _enableRespawn;

    private BoxCollider _bc;
    private SpriteRenderer _sr;

    void Start() {
        _bc = GetComponent<BoxCollider>();
        _sr = GetComponent<SpriteRenderer>();
    }

    IEnumerator SpawnItem() {
        yield return new WaitForSeconds(30.0f);
        _bc.enabled = true;
        _sr.enabled = true;
    }

    public void DespawnItem() {
        if (_enableRespawn) {
            _bc.enabled = false;
            _sr.enabled = false;
            StartCoroutine(SpawnItem());
        } else {
            Destroy(gameObject);
        }
    }
}
