using UnityEngine;
using System.Collections;

public class Mage : Enemy {
    [SerializeField]
    GameObject _orb, _cast;
    private bool _casting, _finalBoss;

    void Start() {
        _life = 6 * GameManager.gm.GetMultiplier();
        _scoreValue = 200;
        _player = FindObjectOfType<Player>();
        _anim = GetComponent<Animator>();
        _sr = GetComponent<SpriteRenderer>();
    }

    void Update() {
        if (!GameManager.gm.GetPause() && !_casting) {
            StartCoroutine(Cast());
        }
    }

    public void SetFinalBoss(bool b) {
        _finalBoss = b;
    }

    IEnumerator Cast() {
        _casting = true;
        _cast.SetActive(true);
        yield return new WaitForSeconds(0.8f);
        //UpdateAnimator(true);
        yield return new WaitForSeconds(0.7f);
        UpdateAnimator(true);
        //Instantiate(_orb, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(1.0f);
        _cast.SetActive(false);
        UpdateAnimator(false);
        yield return new WaitForSeconds(1.0f);
        _casting = false;
    }

    void CastOrb() {
        Instantiate(_orb, transform.position, Quaternion.identity);
    }

    protected override void DropItem() {
        int dropChance = Random.Range(0, 100);
        if (dropChance >= 0 && dropChance <= 5) { //upgrade for current weapon
            Weapon _currWeap = _player.GetWeapon();
            GameObject[] _weapons = GameManager.gm.GetWeapons();
            if (_currWeap.IsUpgradeable()) {
                for (int i = 0; i < _weapons.Length; ++i) {
                    if (_weapons[i].GetComponent<Weapon>().GetType() == _currWeap.GetType()) {
                        Instantiate(_weapons[i + 1], transform.position, Quaternion.identity);
                        break;
                    }
                }
            } else {
                Instantiate(_weapons[_weapons.Length - 1], transform.position, Quaternion.identity);
            }
        } else if (dropChance >= 41 && dropChance <= 70) { //heart
            Instantiate(GameManager.gm.GetDrop(0), transform.position, Quaternion.identity);
        } else if (dropChance >= 71 && dropChance <= 100) { //ammo
            Instantiate(GameManager.gm.GetDrop(1), transform.position, Quaternion.identity);
        }
    }

    public override void DeductLife() {
        _life -= _player.GetWeaponDamage();
        Instantiate(_hitAnimation, transform.position, Quaternion.identity);
        if (_life <= 0 && !_dead) {
            if (_finalBoss) {
                GetComponentInParent<FinalBoss>().RemoveAd();
            } else {
                _roomManager.RemoveEnemy();
                DropItem();
            }
            _dead = true;
            _player.GetComponent<Player>().AddScore(_scoreValue);
            Destroy(gameObject);
        }
    }

}