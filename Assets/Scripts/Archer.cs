using UnityEngine;
using System.Collections;

public class Archer : Enemy {
    [SerializeField]
    GameObject _arrow;
    [SerializeField]
    float _arrowSpeed;
    float shotTimer;

    // Use this for initialization
    void Start() {
        _life = 2 * GameManager.gm.GetMultiplier();
        _scoreValue = 50;
        _player = FindObjectOfType<Player>();
        _anim = GetComponent<Animator>();
        _sr = GetComponent<SpriteRenderer>();
    }

    void Shoot() {
        GameObject arrow = Instantiate(_arrow, transform.position, Quaternion.identity) as GameObject;
        float angle = Mathf.Atan2((_player.transform.position - arrow.transform.position).y, (_player.transform.position - arrow.transform.position).x) * Mathf.Rad2Deg;
        arrow.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        arrow.GetComponent<Rigidbody>().velocity = _arrowSpeed * (_player.transform.position - arrow.transform.position).normalized;
        shotTimer = 2.0f;
        UpdateAnimator(false);
    }

    // Update is called once per frame
    void Update() {
        if (!GameManager.gm.GetPause()) {
            if (shotTimer < 0.0f) {
                UpdateAnimator(true);
            }
            shotTimer -= Time.deltaTime;
        }
    }
}
