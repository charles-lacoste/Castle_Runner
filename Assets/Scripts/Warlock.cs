using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Warlock : Enemy {
    [SerializeField]
    GameObject _cast, _attackBar;

    private Image _attackBarImage;
    private bool casting, _finalBoss;

    void Start() {
        _life = 3 * GameManager.gm.GetMultiplier();
        _scoreValue = 200;
        _player = FindObjectOfType<Player>();
        _anim = GetComponent<Animator>();
        _sr = GetComponent<SpriteRenderer>();
        _attackBarImage = _attackBar.GetComponentInChildren<Image>();
    }

    public void SetFinalBoss(bool b) {
        _finalBoss = b;
    }

    IEnumerator Casting() {
        casting = true;
        _cast.SetActive(true);
        _attackBar.SetActive(true);
        UpdateAnimator(true);
        _player.SetTargetted(true);
        yield return new WaitForSeconds(6.0f);
        _cast.SetActive(false);
        _attackBar.SetActive(false);
        UpdateAnimator(false);
        _player.SetTargetted(false);
        _player.ReduceLife(_player.GetComponent<Player>().GetLife()); //udedm7
    }

    void Update() {
        if (!GameManager.gm.GetPause()) {
            if (!casting) {
                StartCoroutine(Casting());
            } else {
                _attackBarImage.fillAmount -= 1.0f / 6.0f * Time.deltaTime;
            }
        }
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
            if (casting) {
                _player.SetTargetted(false);
            }
            GameObject.FindObjectOfType<Player>().AddScore(_scoreValue);
            Destroy(gameObject);
        }
    }
}
