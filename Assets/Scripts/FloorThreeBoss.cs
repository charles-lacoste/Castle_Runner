using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FloorThreeBoss : Enemy {
    [SerializeField]
    Image _healthBar;
    [SerializeField]
    GameObject _attackBar, _orb, _squirrel, _blinkCast, _warlockCast, _mageCast, _necroCast;

    private Image _attackBarImage;
    private Text _attackText;
    private int _maxLife, _warlockCastPenalty, _ability, _damgeTaken;
    private float _minX, _minY, _maxX, _maxY;
    private float _blinkCastTime, _warlockCastTime, _warlockCD, _mageCastTime, _necroCastTime, _castTime;
    private bool _casting, _castingWarlock, _interrupted;

    void Start() {
        _life = _maxLife = 300;
        _scoreValue = 3500;
        _blinkCastTime = 1.0f;
        _warlockCastTime = 8f;
        _warlockCastPenalty = 3;
        _warlockCD = 10.0f;
        _mageCastTime = 1.5f;
        _necroCastTime = 1.5f;
        _minX = 10.0f;
        _maxX = 26.0f;
        _minY = 42.0f;
        _maxY = 50.0f;
        _damgeTaken = 0;
        _player = FindObjectOfType<Player>();
        _attackBarImage = _attackBar.GetComponentInChildren<Image>();
        _attackText = _attackBarImage.GetComponentInChildren<Text>();
        _anim = GetComponent<Animator>();
        _anim.SetBool("Idle", false);
    }

    void Update() {
        if (!GameManager.gm.GetPause()) {
            if (!_casting && _warlockCD < 0.0f) {
                _warlockCD = 10.0f;
                StartCoroutine("Warlock");
            } else {
                if (!_casting) {
                    _casting = true;
                    _ability = Random.Range(0, 2);
                    if (_ability == 0) {
                        StartCoroutine(Mage());
                    } else {
                        StartCoroutine(Necromancer());
                    }
                }
                if (!_castingWarlock)
                    _warlockCD -= Time.deltaTime;
            }
            if (_casting) {
                _attackBarImage.fillAmount += 1.0f / _castTime * Time.deltaTime;
            }
            _anim.SetBool("Blink", _blinkCast.activeSelf);
            _anim.SetBool("Mage", _mageCast.activeSelf);
            _anim.SetBool("Necro", _necroCast.activeSelf);
            _anim.SetBool("Warlock", _warlockCast.activeSelf);
        }
    }

    IEnumerator Blink() {
        _blinkCast.SetActive(true);
        _castTime = _blinkCastTime;
        _attackBar.SetActive(true);
        _attackBarImage.fillAmount = 0;
        _attackText.text = "Casting Teleport";
        yield return new WaitForSeconds(_blinkCastTime);
        transform.position = new Vector3(Random.Range(_minX, _maxX), Random.Range(_minY, _maxY), 0.0f);
        _attackBar.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        _blinkCast.SetActive(false);
        _casting = false;
    }

    IEnumerator Warlock() {
        _casting = _castingWarlock = true;
        _warlockCastPenalty = 3;
        _damgeTaken = 0;
        _warlockCast.SetActive(true);
        _castTime = _warlockCastTime;
        _attackBar.SetActive(true);
        _attackBarImage.fillAmount = 0;
        _attackText.text = "Death comes";
        yield return new WaitForSeconds(_warlockCastTime);
        if (!_interrupted)
            _player.ReduceLife(_player.GetLife());
        else {
            _castingWarlock = false;
            _warlockCD = 10.0f;
            _attackBar.SetActive(false);
            _warlockCast.SetActive(false);
            _warlockCastPenalty = 1;
            StartCoroutine(Blink());
        }
    }

    IEnumerator Mage() {
        _casting = true;
        _mageCast.SetActive(true);
        _castTime = _mageCastTime;
        _attackBar.SetActive(true);
        _attackBarImage.fillAmount = 0;
        _attackText.text = "Casting Orb";
        yield return new WaitForSeconds(_mageCastTime);
        Instantiate(_orb, transform.position, Quaternion.identity);
        _attackBar.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        _mageCast.SetActive(false);
        StartCoroutine(Blink());
    }

    IEnumerator Necromancer() {
        _casting = true;
        _necroCast.SetActive(true);
        _castTime = _necroCastTime;
        _attackBar.SetActive(true);
        _attackBarImage.fillAmount = 0;
        _attackText.text = "Aw, Nuts!";
        yield return new WaitForSeconds(_necroCastTime);
        Squirrel s1 = Instantiate(_squirrel, new Vector3(transform.position.x + 1, transform.position.y - 1, 0.0f), Quaternion.identity) as Squirrel;
        Squirrel s2 = Instantiate(_squirrel, new Vector3(transform.position.x + 1, transform.position.y - 3, 0.0f), Quaternion.identity) as Squirrel;
        Squirrel s3 = Instantiate(_squirrel, new Vector3(transform.position.x, transform.position.y - 3, 0.0f), Quaternion.identity) as Squirrel;
        Squirrel s4 = Instantiate(_squirrel, new Vector3(transform.position.x - 1, transform.position.y - 1, 0.0f), Quaternion.identity) as Squirrel;
        Squirrel s5 = Instantiate(_squirrel, new Vector3(transform.position.x - 1, transform.position.y - 3, 0.0f), Quaternion.identity) as Squirrel;
        _attackBar.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        _necroCast.SetActive(false);
        StartCoroutine(Blink());
    }

    public override void DeductLife() {
        int damage = _player.GetWeaponDamage() * _warlockCastPenalty;
        _life -= damage;
        _healthBar.fillAmount -= (float)damage / _maxLife;
        _damgeTaken += damage;
        Instantiate(_hitAnimation, transform.position, Quaternion.identity);
        if (_castingWarlock && _damgeTaken >= 20) {
            _interrupted = true;
            _attackText.text = "Interrupted";
        }
        if (_life <= 0) {
            StopAllCoroutines();
            _roomManager.RemoveEnemy();
            _player.GetComponent<Player>().AddScore(_scoreValue);
            GameObject.Find("Music").GetComponent<AudioSource>().Play();
            Destroy(gameObject);
        }
    }
}
