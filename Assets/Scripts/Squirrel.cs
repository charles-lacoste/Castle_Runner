using UnityEngine;
using System.Collections;

public class Squirrel : MonoBehaviour {

    private Player _player;
    private bool _up, _down, _left, _right, _colliding;
    private Animator _anim;
    private float _followSpeed;
    void Start() {
        _anim = GetComponent<Animator>();
        _followSpeed = 0.06f;
        _player = FindObjectOfType<Player>();
    }

    void Update() {
        if (!GameManager.gm.GetPause() && !_colliding) {
            UpdateAnimator();
        }
    }

    void FixedUpdate() {
        if (!GameManager.gm.GetPause() && !_colliding) {
            transform.position = Vector3.MoveTowards(transform.position, _player.transform.position, _followSpeed);
        }
    }

    void UpdateAnimator() {
        _up = false;
        _down = false;
        _left = false;
        _right = false;

        Vector2 diff = _player.gameObject.transform.position - transform.position;
        float sign = (_player.transform.position.y < transform.position.y) ? -1.0f : 1.0f;
        float angle = Vector2.Angle(Vector2.right, diff) * sign;

        if (angle > 135 || angle < -135) {
            _left = true;
        } else if (angle > 45 && angle < 135) {
            _up = true;
        } else if (angle < 45 && angle > -45) {
            _right = true;
        } else {
            _down = true;
        }

        _anim.SetBool("Up", _up);
        _anim.SetBool("Down", _down);
        _anim.SetBool("Left", _left);
        _anim.SetBool("Right", _right);
    }

    IEnumerator Delay() {
        yield return new WaitForSeconds(0.6f);
        _colliding = false;
    }

    void OnCollisionEnter(Collision col) {
        if (col.gameObject.layer == LayerMask.NameToLayer("Bullet")) {
            Destroy(gameObject);
        } else if (col.gameObject.layer == LayerMask.NameToLayer("Player")) {
            _colliding = true;
            _player.ReduceLife(1);
        }
    }

    void OnCollisionStay(Collision col) {
        if (col.gameObject.layer == LayerMask.NameToLayer("Player")) {
            _colliding = true;
        }
    }
     void OnCollisionExit(Collision col) {
        if (col.gameObject.layer == LayerMask.NameToLayer("Player")) {
            _colliding = false;
            StartCoroutine(Delay());
        }
    }

    void DeductLife() {
        Destroy(gameObject);
    }
}
