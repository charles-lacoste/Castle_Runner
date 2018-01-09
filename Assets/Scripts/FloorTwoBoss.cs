using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FloorTwoBoss : Enemy {
    [SerializeField]
    Image _healthBar;
    [SerializeField]
    GameObject _attackBar, _orb;
    [SerializeField]
    Transform _shootingPoint;
    
    private Image _attackBarImage;
    private Text _attackText;
    private BoxCollider _bc;
    private float _castTime, _chargeCastTime, _chargeSpeed, _barrageCastTime, _barrageChannelTime, _timeBetweenShots, _arrowSpeed;
    private int _barragePenalty, _nbOfArrowsInBarrage, _maxLife;
    private bool _attacking, _casting, _channeling;

    void Start() {
        _barragePenalty = 1;
        _life = _maxLife = 250;
        _scoreValue = 3500;
        _chargeCastTime = 1.5f;
        _chargeSpeed = 11.5f;
        _barrageCastTime = 2.0f;
        _barrageChannelTime = 6.0f;
        _arrowSpeed = 10.0f;
        _timeBetweenShots = 1.0f;
        _player = FindObjectOfType<Player>();
        _rb = GetComponent<Rigidbody>();
        _attackBarImage = _attackBar.GetComponentInChildren<Image>();
        _attackText = _attackBarImage.GetComponentInChildren<Text>();
        _anim = GetComponent<Animator>();
        _anim.speed = 2.0f;
        _bc = GetComponentInChildren<BoxCollider>();
    }

    void Update() {
        if (!GameManager.gm.GetPause()) {
            if (!_attacking)
                StartCoroutine(Attacks());
            if (_casting) {
                _attackBarImage.fillAmount -= 1.0f / _chargeCastTime * Time.deltaTime;
            } else if (_channeling) {
                _attackBarImage.fillAmount += 1.0f / _barrageChannelTime * Time.deltaTime;
            }
        }
    }

    IEnumerator Attacks() {
        // Charge
        _anim.speed = 2.0f;
        _attacking = true;
        _casting = true;
        _attackBar.SetActive(true);
        _castTime = _chargeCastTime;
        _attackBarImage.fillAmount = 1;
        _attackText.text = "Casting Charge";
        _anim.SetBool("Charging", false);
        _anim.SetBool("Casting", false);
        _anim.SetBool("FinishingCast", false);
        _anim.SetBool("StartCharge", true);
        _anim.SetBool("FinishCharge", false);
        yield return new WaitForSeconds(1.5f);
        _casting = false;
        _attackBar.SetActive(false);
        _rb.velocity = Vector3.zero;
        Vector3 direction = (_player.transform.position - transform.position).normalized;
        _anim.SetBool("Charging", true);
        _anim.SetBool("Casting", false);
        _anim.SetBool("FinishingCast", false);
        _anim.SetBool("StartCharge", false);
        _anim.SetBool("FinishCharge", false);
        _anim.speed = 2.0f;
        _bc.enabled = true;
        _rb.velocity = direction * _chargeSpeed;
        yield return new WaitForSeconds(1.5f);
        _bc.enabled = false;
        _anim.SetBool("Charging", false);
        _anim.SetBool("Casting", false);
        _anim.SetBool("FinishingCast", false);
        _anim.SetBool("StartCharge", false);
        _anim.SetBool("FinishCharge", true);
        _rb.velocity = Vector3.zero;
        yield return new WaitForSeconds(1.0f);
        // Barrage
        _casting = true;
        _attackBar.SetActive(true);
        _castTime = _barrageCastTime;
        _attackBarImage.fillAmount = 1;
        _attackText.text = "Casting Barrage";
        yield return new WaitForSeconds(0.5f);
        _anim.SetBool("Charging", false);
        _anim.SetBool("Casting", true);
        _anim.SetBool("FinishingCast", false);
        _anim.SetBool("StartCharge", false);
        _anim.SetBool("FinishCharge", false);
        yield return new WaitForSeconds(1.5f);
        _casting = false;
        _channeling = true;
        _attackBar.SetActive(true);
        _attackBarImage.fillAmount = 0;
        _attackText.text = "Channeling Barrage";
        _barragePenalty = 2;
        StartCoroutine(Multishot());
    }

    IEnumerator Multishot() {
        Vector3 position = _player.transform.position;
        float originAngle = Mathf.Atan2((position - _shootingPoint.position).y, (position - _shootingPoint.position).x) * Mathf.Rad2Deg;
        for (int i = 0; i < _barrageChannelTime / _timeBetweenShots; ++i) {
            if(i == 3) {
                position = _player.transform.position;
                originAngle = Mathf.Atan2((position - _shootingPoint.position).y, (position - _shootingPoint.position).x) * Mathf.Rad2Deg;
            }
            Quaternion newQuat = Quaternion.AngleAxis(originAngle, Vector3.forward);
            GameObject arrow = Instantiate(_orb, _shootingPoint.position, Quaternion.identity) as GameObject;
            arrow.transform.rotation = newQuat;
            arrow.GetComponent<Rigidbody>().velocity = (position - arrow.transform.position).normalized * _arrowSpeed;

            newQuat *= Quaternion.Euler(_shootingPoint.eulerAngles.x, _shootingPoint.eulerAngles.y, 5f); 
            GameObject arrow1 = Instantiate(_orb, _shootingPoint.position, newQuat) as GameObject;
            Vector3 v = position - arrow1.transform.position;
            v = Quaternion.Euler(0f, 0f, 5f) * v; 
            arrow1.GetComponent<Rigidbody>().velocity = v.normalized * _arrowSpeed;

            newQuat *= Quaternion.Euler(_shootingPoint.eulerAngles.x, _shootingPoint.eulerAngles.y, -10f);
            GameObject arrow2 = Instantiate(_orb, _shootingPoint.position, newQuat) as GameObject;
            v = position - arrow2.transform.position;
            v = Quaternion.Euler(0f, 0f, -5f) * v;
            arrow2.GetComponent<Rigidbody>().velocity = v.normalized * _arrowSpeed;

            newQuat *= Quaternion.Euler(_shootingPoint.eulerAngles.x, _shootingPoint.eulerAngles.y, 15f);
            GameObject arrow3 = Instantiate(_orb, _shootingPoint.position, newQuat) as GameObject;
            v = position - arrow3.transform.position;
            v = Quaternion.Euler(0f, 0f, 10f) * v;
            arrow3.GetComponent<Rigidbody>().velocity = v.normalized * _arrowSpeed;

            newQuat *= Quaternion.Euler(_shootingPoint.eulerAngles.x, _shootingPoint.eulerAngles.y, -20f);
            GameObject arrow4 = Instantiate(_orb, _shootingPoint.position, newQuat) as GameObject;
            v = position - arrow4.transform.position;
            v = Quaternion.Euler(0f, 0f, -10f) * v;
            arrow4.GetComponent<Rigidbody>().velocity = v.normalized * _arrowSpeed;

            newQuat *= Quaternion.Euler(_shootingPoint.eulerAngles.x, _shootingPoint.eulerAngles.y, 25f);
            GameObject arrow5 = Instantiate(_orb, _shootingPoint.position, newQuat) as GameObject;
            v = position - arrow5.transform.position;
            v = Quaternion.Euler(0f, 0f, 15f) * v;
            arrow5.GetComponent<Rigidbody>().velocity = v.normalized * _arrowSpeed;

            newQuat *= Quaternion.Euler(_shootingPoint.eulerAngles.x, _shootingPoint.eulerAngles.y, -30f);
            GameObject arrow6 = Instantiate(_orb, _shootingPoint.position, newQuat) as GameObject;
            v = position - arrow6.transform.position;
            v = Quaternion.Euler(0f, 0f, -15f) * v;
            arrow6.GetComponent<Rigidbody>().velocity = v.normalized * _arrowSpeed;

            yield return new WaitForSeconds(_timeBetweenShots);
        }
        _anim.SetBool("Charging", false);
        _anim.SetBool("Casting", false);
        _anim.SetBool("FinishingCast", true);
        _anim.SetBool("StartCharge", false);
        _anim.SetBool("FinishCharge", false);
        yield return new WaitForSeconds(1.0f);
        _barragePenalty = 1;
        _channeling = false;
        _attacking = false;

    }

    public override void DeductLife() {
        _life -= _player.GetWeaponDamage() * _barragePenalty;
        _healthBar.fillAmount -= (float)(_player.GetWeaponDamage() * _barragePenalty) / _maxLife;
        Instantiate(_hitAnimation, transform.position, Quaternion.identity);
        if (_life <= 0) {
            _roomManager.RemoveEnemy();
            _player.GetComponent<Player>().AddScore(_scoreValue);
            GameObject.Find("Music").GetComponent<AudioSource>().Play();
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision col) {
        if (col.gameObject.layer != LayerMask.NameToLayer("Bullet")) {
            if (col.gameObject.layer == LayerMask.NameToLayer("Player")) {
                _player.transform.GetComponent<Player>().ReduceLife(4);
            }
            _rb.velocity = Vector3.zero;
        }
    }
}
