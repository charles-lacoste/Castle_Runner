using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {
    [SerializeField]
    protected GameObject _hitAnimation;
    [SerializeField]
    protected RoomManager _roomManager;

    protected int _scoreValue;
    protected float _life;
    protected Rigidbody _rb;
    protected Player _player;
    protected bool _dead = false;
    protected Animator _anim;
    protected SpriteRenderer _sr;

    protected virtual void DropItem() {
        int dropChance = Random.Range(0, 100);
        if (dropChance >= 41 && dropChance <= 70) { //heart
            Instantiate(GameManager.gm.GetDrop(0), transform.position, Quaternion.identity);
        } else if (dropChance >= 71 && dropChance <= 100) { //ammo
            Instantiate(GameManager.gm.GetDrop(1), transform.position, Quaternion.identity);
        }
    }

    public virtual void DeductLife() {
        _life -= _player.GetWeaponDamage();
        Instantiate(_hitAnimation, transform.position, Quaternion.identity);
        if (_life <= 0 && !_dead) {
            _dead = true;
            //add explosions
            _roomManager.RemoveEnemy();
            _player.GetComponent<Player>().AddScore(_scoreValue);
            DropItem();
            Destroy(gameObject);
        }
    }

    protected void UpdateAnimator(bool b) {
        Vector2 diff = _player.gameObject.transform.position - transform.position;
        float sign = (_player.transform.position.y < transform.position.y) ? -1.0f : 1.0f;
        float angle = Vector2.Angle(Vector2.right, diff) * sign;
        if (angle > 90 || angle < -90) {
            _sr.flipX = true;
        } else if (angle < 90 && angle > -90) {
            _sr.flipX = false;
        }
        _anim.SetBool("Attacking", b);
    }
}
