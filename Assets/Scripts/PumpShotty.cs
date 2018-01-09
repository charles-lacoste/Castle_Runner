using UnityEngine;
using System.Collections;

public class PumpShotty : Weapon {

	// Use this for initialization
	void Awake () {
        _ammoCrateAmount = 8;
        _ammo = 24;
        _maxMagSize = _mag = 2;
        _reloadTime = 1.25f;
        _fireRate = 1.10f;
        _bulletSpeed = 10.0f;
        _damage = 1;
        _upgradeable = true;
        _weaponSound = GetComponent<AudioSource>();
        _player = GameObject.FindObjectOfType<Player>();
        _reloadBar = _player.GetComponentInChildren<Canvas>().GetComponentInChildren<UnityEngine.UI.Image>();
        _sr = GetComponentInChildren<SpriteRenderer>();
        _weaponPositionOnPlayer = new Vector3(0.013f, -0.163f, 0);
    }
	
	// Update is called once per frame
	void Update () {
        if (_equipped) {
            UpdateReloadBar(_reloadTime);
            Vector3 mousePos = Input.mousePosition;
            Vector3 objectPos = Camera.main.WorldToScreenPoint(_player.gameObject.transform.position);
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
            float originAngle = Mathf.Atan2((mousePos - bullet.transform.position).y, (mousePos - bullet.transform.position).x) * Mathf.Rad2Deg;

            Quaternion newQuat = Quaternion.AngleAxis(originAngle, Vector3.forward);
            bullet.transform.rotation = newQuat;
            bullet.GetComponent<Rigidbody>().velocity = (mousePos - transform.position).normalized * _bulletSpeed;
            
            newQuat *= Quaternion.Euler(_shootingPoint.eulerAngles.x, _shootingPoint.eulerAngles.y, 5f); //rotation
            GameObject bullet1 = Instantiate(_bullet, _shootingPoint.position, newQuat) as GameObject;
            Vector3 v = mousePos - bullet1.transform.position;
            v = Quaternion.Euler(0f, 0f, 5f) * v; //spread
            bullet1.GetComponent<Rigidbody>().velocity = v.normalized * _bulletSpeed;

            newQuat *= Quaternion.Euler(_shootingPoint.eulerAngles.x, _shootingPoint.eulerAngles.y, -10f);
            GameObject bullet2 = Instantiate(_bullet, _shootingPoint.position, newQuat) as GameObject;
            v = mousePos - bullet2.transform.position;
            v = Quaternion.Euler(0f, 0f, -5f) * v;
            bullet2.GetComponent<Rigidbody>().velocity = v.normalized * _bulletSpeed;

            newQuat *= Quaternion.Euler(_shootingPoint.eulerAngles.x, _shootingPoint.eulerAngles.y, 15f);
            GameObject bullet3 = Instantiate(_bullet, _shootingPoint.position, newQuat) as GameObject;
            v = mousePos - bullet3.transform.position;
            v = Quaternion.Euler(0f, 0f, 10f) * v;
            bullet3.GetComponent<Rigidbody>().velocity = v.normalized * _bulletSpeed;

            newQuat *= Quaternion.Euler(_shootingPoint.eulerAngles.x, _shootingPoint.eulerAngles.y, -20f);
            GameObject bullet4 = Instantiate(_bullet, _shootingPoint.position, newQuat) as GameObject;
            v = mousePos - bullet4.transform.position;
            v = Quaternion.Euler(0f, 0f, -10f) * v;
            bullet4.GetComponent<Rigidbody>().velocity = v.normalized * _bulletSpeed;

            _weaponSound.Play();
            _mag--;
            if (_mag == 0) { //automatic reload
                StartCoroutine(Reload());
            }
        }
    }
}
