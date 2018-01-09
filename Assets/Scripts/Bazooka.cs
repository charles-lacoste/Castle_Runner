using UnityEngine;
using System.Collections;

public class Bazooka : Weapon {
    private float _explosionRadius;

    void Awake() {
        _ammoCrateAmount = 0;
        _ammo = 3;
        _maxMagSize = _mag = 1;
        _reloadTime = 2.0f;
        _fireRate = 0.0f;
        _bulletSpeed = 15.0f;
        _damage = 10;
        _explosionRadius = 2.0f;
        _player = GameObject.FindObjectOfType<Player>();
        _reloadBar = _player.GetComponentInChildren<Canvas>().GetComponentInChildren<UnityEngine.UI.Image>();
        _sr = GetComponentInChildren<SpriteRenderer>();
        _weaponPositionOnPlayer = new Vector3(0.013f, -0.163f, 0);
    }

    void Update() {
        if (_equipped) {
            UpdateReloadBar(_reloadTime);
            Vector3 mousePos = Input.mousePosition;
            Vector3 objectPos = Camera.main.WorldToScreenPoint(transform.position);
            mousePos.x = mousePos.x - objectPos.x;
            mousePos.y = mousePos.y - objectPos.y;
            float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;
            if (angle > 45 && angle < 135) { //Up
                _sr.sortingOrder = 1;
            } else {
                _sr.sortingOrder = 3;
            }
            if (angle > 90 || angle < -90) {
                _sr.sprite = _sprites[0];
                _sr.flipX = true;
                _sr.flipY = true;
            } else {
                _sr.sprite = _sprites[1];
                _sr.flipX = false;
                _sr.flipY = false;
            }
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }
    }

    public override void Fire() {
        if (_mag > 0 && !_reloading) {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = transform.position.z;
            GameObject bullet = Instantiate(_bullet, _shootingPoint.position, Quaternion.identity) as GameObject;
            float angle = Mathf.Atan2((mousePos - bullet.transform.position).y, (mousePos - bullet.transform.position).x) * Mathf.Rad2Deg;
            bullet.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            bullet.GetComponent<Rocket>().SetExplosionRadius(_explosionRadius);
            bullet.GetComponent<Rigidbody>().velocity = (mousePos - transform.position).normalized * _bulletSpeed;
            _mag--;
            StartCoroutine(Reload());
        }
    }
}
