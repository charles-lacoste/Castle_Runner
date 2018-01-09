using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Necromancer : Enemy {
    [SerializeField]
    GameObject skeleton;
    [SerializeField]
    GameObject _cast;
    List<GameObject> _army;

    private float summonTimer;
    private bool summoning, _finalBoss;
    private int armyCount = 0;
    // Use this for initialization
    void Start() {
        _life = 7 * GameManager.gm.GetMultiplier();
        _scoreValue = 400;
        _player = FindObjectOfType<Player>();
        _army = new List<GameObject>();
        _anim = GetComponent<Animator>();
        _sr = GetComponent<SpriteRenderer>();
    }

    public void SetFinalBoss(bool b) {
        _finalBoss = b;
    }

    IEnumerator Summoning() {
        if (_army.Count < 16) {
            summoning = true;
            _cast.SetActive(true);
            yield return new WaitForSeconds(1.5f);
            UpdateAnimator(true);
            summonTimer = 4.0f;
            yield return new WaitForSeconds(1.0f);
            UpdateAnimator(false);
            _cast.SetActive(false);
            summoning = false;
        }
    }

    void Summon() {
        GameObject s1 = Instantiate(skeleton, transform.position + new Vector3(1, 0, 0), Quaternion.identity) as GameObject;
        s1.transform.SetParent(this.transform);
        _army.Add(s1);
        GameObject s2 = Instantiate(skeleton, transform.position + new Vector3(0, -1, 0), Quaternion.identity) as GameObject;
        s2.transform.SetParent(this.transform);
        _army.Add(s2);
    }

    // Update is called once per frame
    void Update() {
        if (!GameManager.gm.GetPause()) {
            if (!summoning && summonTimer < 0.0f) {
                StartCoroutine(Summoning());
            }
            else {
                summonTimer -= Time.deltaTime;
            }
        }
    }

    public void RemoveSkeleton(GameObject s) {
        _army.Remove(s);
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
            }
            else {
                _roomManager.RemoveEnemy();
                DropItem();
            }
            _dead = true;
            foreach (GameObject s in _army) {
                Destroy(s);
            }
            _player.GetComponent<Player>().AddScore(_scoreValue);
            Destroy(gameObject);
        }
    }
}
