using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FloorOneBoss : MonoBehaviour {
    [SerializeField]
    GameObject _gun;
    [SerializeField]
    GameObject _hitAnimation;
    [SerializeField]
    GameObject _shootingPoint;
    [SerializeField]
    Transform _target; //player
    [SerializeField]
    RoomManager _roomManager;
    [SerializeField]
    GameObject _bullet;
    [SerializeField]
    float _bulletSpeed;
    [SerializeField]
    GameObject _orb; //aoe with orbs.
    [SerializeField]
    Image _healthBar;
    [SerializeField]
    GameObject _reloadBar;
    [SerializeField]
    Sprite[] _gunSprites;
    [SerializeField]
    Sprite[] _modelSprites;

    private int _reloadPenalty;
    private float _shotTimer;
    private int _magSize, _curMag, _scoreValue;
    private int _life, _maxLife;
    private bool _firstTime = true;
    private bool _reloading = false;
    private bool _aoe = false;
    private float _aoeTimer, _reloadTime;
    private bool _waiting = false;
    private Player _player;
    private SpriteRenderer[] _sr;

    void Start () {
        _reloadPenalty = 1;
        _life = _maxLife = 150;
        _magSize = _curMag = 40;
        _scoreValue = 3500;
        _reloadTime = 6.5f;
        _aoeTimer = 7.5f;
        _player = FindObjectOfType<Player>();
        _sr = GetComponentsInChildren<SpriteRenderer>(); // 0 = boss, 1 = gun
    }

    public void DeductLife() {
        _life -= _player.GetWeaponDamage() * _reloadPenalty;
        _healthBar.fillAmount -= (float)(_player.GetWeaponDamage() * _reloadPenalty) / _maxLife;
        Instantiate(_hitAnimation, transform.position, Quaternion.identity);
        if (_life <= 0) {
            _roomManager.RemoveEnemy();
            DropItem();
            GameObject.FindObjectOfType<Player>().AddScore(_scoreValue);
            GameObject.Find("Music").GetComponent<AudioSource>().Play();
            Destroy(gameObject);
        }
    }

    void Shoot() {
        GameObject bullet = Instantiate(_bullet, _shootingPoint.transform.position, Quaternion.identity) as GameObject;
        float angle = Mathf.Atan2((_target.position - bullet.transform.position).y, (_target.position - bullet.transform.position).x) * Mathf.Rad2Deg;
        bullet.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        bullet.GetComponent<Rigidbody>().velocity = _bulletSpeed * (_target.position - bullet.transform.position).normalized;
        _shotTimer = 0.2f; //rate of fire. must be hard to beat the boss.
        _curMag--;
        if(_curMag == 0) {
            StartCoroutine(Reload());
        }
    }

    IEnumerator InitialWait() {
        _waiting = true;
        yield return new WaitForSeconds(2.0f);
        _waiting = false;
    }

    void DropItem() {
        int dropChance = Random.Range(0, 100);
        if (dropChance >= 0 && dropChance <= 10) { //upgrade for current weapon
            Weapon _currWeap = _player.GetWeapon();
            GameObject[] _weapons = GameManager.gm.GetWeapons();
            if (_currWeap.IsUpgradeable()) {
                for (int i = 0; i < _weapons.Length; ++i) {
                    if (_weapons[i].GetComponent<Weapon>().GetType() == _currWeap.GetType()) {
                        Instantiate(_weapons[i + 1], transform.position, Quaternion.identity);
                        break;
                    }
                }
            }
            else {
                Instantiate(_weapons[_weapons.Length - 1], transform.position, Quaternion.identity);
            }
        }
        else if (dropChance >= 41 && dropChance <= 70) { //heart
            Instantiate(GameManager.gm.GetDrop(0), transform.position, Quaternion.identity);
        }
        else if (dropChance >= 71 && dropChance <= 100) { //ammo
            Instantiate(GameManager.gm.GetDrop(1), transform.position, Quaternion.identity);
        }
    }

    IEnumerator AOEAttack() {
        _aoe = true;
        yield return new WaitForSeconds(2.0f);
        int offset = 0;
        for (int i = 0; i < 18; i++) {
            GameObject orb = Instantiate(_orb, transform.position, Quaternion.Euler(0, 0, 0 + offset)) as GameObject;
            offset += 20;
        }
        _aoeTimer = 10.0f;
        yield return new WaitForSeconds(5.0f);
        _aoe = false;
    }

    IEnumerator Reload() {
        _reloading = true;
        _reloadBar.SetActive(true);
        _reloadBar.GetComponentInChildren<Image>().fillAmount = 1;
        _reloadPenalty = 2;
        yield return new WaitForSeconds(_reloadTime);
        _curMag = _magSize;
        _reloadBar.SetActive(false);
        _reloading = false;
        _reloadPenalty = 1;
    }

    void UpdateModel() {
        Vector3 playerPos = Input.mousePosition;
        playerPos.x = _target.position.x - transform.position.x;
        playerPos.y = _target.position.y - transform.position.y;
        float angle = Mathf.Atan2(playerPos.y, playerPos.x) * Mathf.Rad2Deg;
        if (angle > 45 && angle < 135) { //Up
            _sr[1].sortingOrder = -1;
        }
        else {
            _sr[1].sortingOrder = 1;
        }
        if (angle > 90 || angle < -90) {
            _sr[1].sprite = _gunSprites[0];
            _sr[1].flipX = true;
            _sr[1].flipY = true;
        }
        else {
            _sr[1].sprite = _gunSprites[1];
            _sr[1].flipX = false;
            _sr[1].flipY = false;
        }
        _gun.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        if (angle > 135 || angle < -135) {
            _sr[0].sprite = _modelSprites[1];
        }
        else if (angle > 45 && angle < 135) {
            _sr[0].sprite = _modelSprites[2];
        }
        else if (angle < 45 && angle > -45) {
            _sr[0].sprite = _modelSprites[3];
        }
        else {
            _sr[0].sprite = _modelSprites[0];
        }
    }

    void Update() {
        //initial wait.
        if (!GameManager.gm.GetPause()) {
            UpdateModel();
            if (_firstTime) { //not very efficient, change this later.
                StartCoroutine(InitialWait());
                _firstTime = false;
            }
            if (_curMag == 0 && !_reloading && !_aoe) {
                StartCoroutine(Reload());
            }
            else if (_aoeTimer < 0.0f && !_reloading && !_aoe) {
                StartCoroutine(AOEAttack());
            }
            else if (_shotTimer < 0.0f && !_reloading && !_aoe && !_waiting) {
                Shoot();
            }
            _shotTimer -= Time.deltaTime;
            _aoeTimer -= Time.deltaTime;
            if (_reloading)
                _reloadBar.GetComponentInChildren<Image>().fillAmount -= 1.0f / _reloadTime * Time.deltaTime;
        }
    }
}
