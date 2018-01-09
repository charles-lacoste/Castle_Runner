using UnityEngine;
using System.Collections;

public class Charger : Enemy {
    [SerializeField]
    private float followRange, chargeRange, followSpeed, chargeSpeed;
    private float chargeTimer;
    private bool charged, recharge, charging, colliding;

    void Start() {
        _life = 1 * GameManager.gm.GetMultiplier();
        _scoreValue = 25;
        charged = recharge = charging = colliding = false;
        _rb = gameObject.GetComponent<Rigidbody>();
        _player = FindObjectOfType<Player>();
        _anim = GetComponent<Animator>();
        _sr = GetComponent<SpriteRenderer>();
    }

    IEnumerator delay() {
        yield return new WaitForSeconds(0.6f);
        colliding = false;
    }

    IEnumerator charge() {
        charged = true;
        _rb.velocity = Vector3.zero;
        //UpdateAnimator(true);
        yield return new WaitForSeconds(0.70f);
        _rb.velocity = Vector3.zero; // Needed if it gets hit while waiting or else it wont charge
        Vector3 direction = (_player.transform.position - transform.position).normalized;
        _rb.velocity = direction * chargeSpeed;
        yield return new WaitForSeconds(0.75f);
        _rb.velocity = Vector3.zero;
        UpdateAnimator(false);
        recharge = true;
        charging = false;
        chargeTimer = 3.5f;
    }

    // Update is called once per frame
    void Update() {
        if (!GameManager.gm.GetPause()) {
            if (recharge) {
                chargeTimer -= Time.deltaTime;
                if (chargeTimer < 0.0f) {
                    recharge = false;
                    charged = false;
                }
            }
            if (Vector3.Distance(transform.position, _player.transform.position) < chargeRange) { // in charge range
                if (!charged) {
                    charging = true;
                    UpdateAnimator(true);
                }
            }
        }
    }

    void FixedUpdate() {
        if (!colliding && ((Vector3.Distance(transform.position, _player.transform.position) < followRange
                && Vector3.Distance(transform.position, _player.transform.position) > chargeRange && !charging)
                || Vector3.Distance(transform.position, _player.transform.position) < chargeRange && chargeTimer > 0.0f && !charging)) { // in follow range
            transform.position = Vector3.MoveTowards(transform.position, _player.transform.position, followSpeed); // follow 
        }
    }



    void OnCollisionEnter(Collision col) {
        if (col.gameObject.layer == LayerMask.NameToLayer("Player")) {
            if (charging) {
                _player.transform.GetComponent<Player>().ReduceLife(2);
            } else {
                _player.transform.GetComponent<Player>().ReduceLife(1);
            }
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
}