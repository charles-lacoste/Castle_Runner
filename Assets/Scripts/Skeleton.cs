using UnityEngine;
using System.Collections;

public class Skeleton : Enemy {
    [SerializeField]
    float followSpeed;
    private bool colliding = false;
    Necromancer parent;

    // Use this for initialization
    void Start() {
        _life = 1 * GameManager.gm.GetMultiplier();
        _scoreValue = 10;
        _rb = GetComponent<Rigidbody>();
        _player = FindObjectOfType<Player>();
    }

    // Update is called once per frame
    void FixedUpdate() {
        if (!GameManager.gm.GetPause() && !colliding) {
            transform.position = Vector3.MoveTowards(transform.position, _player.gameObject.transform.position, followSpeed); // follow 
        }
    }

    IEnumerator delay() {
        yield return new WaitForSeconds(0.6f);
        colliding = false;
    }

    void OnCollisionEnter(Collision col) {
        if (col.gameObject.layer == LayerMask.NameToLayer("Player")) {
            _player.GetComponent<Player>().ReduceLife(1);
        }
    }

    void OnCollisionStay(Collision col) {
        if (col.gameObject.layer == LayerMask.NameToLayer("Player")) {
            colliding = true;
            _rb.velocity = Vector3.zero;
        }
    }

    void OnCollisionExit(Collision col) {
        if (col.gameObject.layer == LayerMask.NameToLayer("Player")) {
            StartCoroutine(delay());
        }
    }

    public override void DeductLife() {
        _life -= _player.GetWeaponDamage();
        Instantiate(_hitAnimation, transform.position, Quaternion.identity);
        if (_life <= 0) {
            GetComponentInParent<Necromancer>().RemoveSkeleton(gameObject);
            GameObject.FindObjectOfType<Player>().AddScore(_scoreValue);
            Destroy(gameObject);
        }
    }
}

