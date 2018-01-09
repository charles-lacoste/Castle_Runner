using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public abstract class Weapon : MonoBehaviour {
    [SerializeField]
    protected GameObject _bullet;
    protected float _reloadTime, _fireRate, _bulletSpeed;
    protected AudioSource _weaponSound;
    protected Image _reloadBar;
    protected Player _player;    
    protected int _ammo, _mag, _maxMagSize, _damage, _ammoCrateAmount;
    protected bool _reloading, _equipped, _upgradeable;
    [SerializeField]
    protected Transform _shootingPoint, _spriteTransform;
    [SerializeField]
    protected Sprite[] _sprites;
    protected SpriteRenderer _sr;
    protected Vector3 _weaponPositionOnPlayer;
    
    public void AddAmmo(int ammo) {
        _ammo += ammo;
    }

    public Vector3 GetWeaponPositionOnPlayer() {
        return _weaponPositionOnPlayer;
    }

    public void IsEquipped(bool equipped) {
        _equipped = equipped;
    }

    public bool IsUpgradeable() {
        return _upgradeable;
    }

    public int GetAmmoCrateAmount() {
        return _ammoCrateAmount;
    }

    public virtual IEnumerator Reload() {
        if (_ammo > 0 && _mag != _maxMagSize) {
            _reloading = true;
            _reloadBar.fillAmount = 1;
            yield return new WaitForSeconds(_reloadTime);
            if (_ammo >= _maxMagSize) {
                _ammo -= _maxMagSize - _mag;
                _mag = _maxMagSize;
            } else {
                _mag = _ammo;
                _ammo = 0;
            }
            _player.UpdateAmmo();
            _reloading = false;
        }
    }

    public abstract void Fire();

    public int GetMag() {
        return _mag;
    }

    public int GetAmmo() {
        return _ammo;
    }

    public int GetMaxMagSize() {
        return _maxMagSize;
    }

    public float GetFireRate(){
        return _fireRate;
    }

    public void UpdateReloadBar(float reloadTime) {
        if (_reloading) {
            _reloadBar.fillAmount -= 1 / reloadTime * Time.deltaTime;
        }
    }

    public int GetDamage() {
        return _damage;
    }

    public bool IsReloading() {
        return _reloading;
    }
}
