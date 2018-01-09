using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour {

    [SerializeField]
    private Text _lifeText, _ammoText, _scoreText, _notification, _interaction;
    [SerializeField]
    private Weapon _weapon;
    [SerializeField]
    private Image _healthBar;
    [SerializeField]
    private bool invincible;
    [SerializeField]
    private float _fireRate;
    [SerializeField]
    private GameObject warlock_cast, _deathAnimation, _hitAnimation;

    private Rigidbody _rb;
    private Animator _anim;
    private bool _lookingRight, _lookingLeft, _lookingUp, _lookingDown, _idle;
    private int _score, _maxLife, _life, _notifCounts, _numberOfKills;
    private float _speed = 4.0f, _lastShot;
    private bool _pickingUpWeapon, _loading, _collidingWithWeapon, _collidingWithChest, _opening, _bazookaEquipped, _noWeaponEquipped;
    private AudioSource _footsteps;
    private GameObject _tempWeapon, _tempChest;
    private Weapon _bazooka;

    void Start() {
        _life = 10;
        _maxLife = _life;
        _score = 0;
        _notifCounts = 0;
        _noWeaponEquipped = true;
        //_fireRate = _weapon.GetFireRate();
        //_weapon.IsEquipped(true);
        UpdateLife();
        //UpdateAmmo();
        UpdateScore();
        _rb = GetComponent<Rigidbody>();
        _anim = GetComponent<Animator>();
        _footsteps = GetComponent<AudioSource>();
    }

    void Update() {
        if (!GameManager.gm.GetPause() && !_loading) {
            UpdateAnimator();
            if (Input.GetMouseButton(0) && _weapon != null && !_weapon.IsReloading())
                Fire();

            if (Input.GetKeyDown("r") && _weapon != null && !_weapon.IsReloading())
                StartCoroutine(_weapon.Reload());

            //Super hacks
            if (Input.GetKeyDown("=")) {
                invincible = !invincible;
            }

            if (Input.GetKeyDown("t") && _collidingWithWeapon && !_pickingUpWeapon && (_noWeaponEquipped || !_weapon.IsReloading())) {
                _noWeaponEquipped = false;
                _pickingUpWeapon = true;
                PickupWeapon(_tempWeapon.GetComponent<Weapon>());
                _tempWeapon = null;
            }

            if (Input.GetKeyDown(KeyCode.Alpha1) && _weapon != null && _bazooka != null && (!_weapon.IsReloading() && !_bazooka.IsReloading())) {
                if (_bazookaEquipped) {
                    _bazooka.gameObject.SetActive(false);
                    _weapon.gameObject.SetActive(true);
                    _fireRate = _weapon.GetFireRate();
                    _bazooka.IsEquipped(false);
                    _weapon.IsEquipped(true);
                    _bazookaEquipped = false;
                } else {
                    _bazooka.gameObject.SetActive(true);
                    _weapon.gameObject.SetActive(false);
                    _fireRate = _bazooka.GetFireRate();
                    _weapon.IsEquipped(false);
                    _bazooka.IsEquipped(true);
                    _bazookaEquipped = true;
                }
                UpdateAmmo();
            }

            if (Input.GetKeyDown("f") && !_opening && _collidingWithChest) {
                _opening = true;
                _tempChest.GetComponent<Chest>().OpenChest();
                _opening = false;
                _tempChest = null;
            }

            if (Input.GetKeyDown(KeyCode.Tab)) {
                GameManager.gm.ToggleMinimap();
            }
        }
    }

    void FixedUpdate() {
        Move();
    }

    void Move() {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        if (!Mathf.Approximately(vertical, 0.0f) || !Mathf.Approximately(horizontal, 0.0f)) {
            _rb.velocity = new Vector2(horizontal, vertical) * _speed;
            _idle = false;
            if (!_footsteps.isPlaying)
                _footsteps.Play();
        } else {
            _rb.velocity = Vector2.zero;
            _idle = true;
        }
    }

    void UpdateAnimator() {
        _lookingRight = false;
        _lookingLeft = false;
        _lookingDown = false;
        _lookingUp = false;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 diff = mousePos - transform.position;
        float sign = (mousePos.y < transform.position.y) ? -1.0f : 1.0f;
        float angle = Vector2.Angle(Vector2.right, diff) * sign;

        if (angle > 135 || angle < -135) {
            _lookingLeft = true;
        } else if (angle > 45 && angle < 135) {
            _lookingUp = true;
        } else if (angle < 45 && angle > -45) {
            _lookingRight = true;
        } else {
            _lookingDown = true;
        }

        _anim.SetBool("WalkingRight", !_idle ? _lookingRight : false);
        _anim.SetBool("WalkingLeft", !_idle ? _lookingLeft : false);
        _anim.SetBool("WalkingUp", !_idle ? _lookingUp : false);
        _anim.SetBool("WalkingDown", !_idle ? _lookingDown : false);
        _anim.SetBool("LookingRight", _lookingRight);
        _anim.SetBool("LookingLeft", _lookingLeft);
        _anim.SetBool("LookingUp", _lookingUp);
        _anim.SetBool("LookingDown", _lookingDown);
    }

    // If the character can move or shoot
    public void Loading(bool b) {
        _loading = b;
        _lastShot = 0;
    }

    public int GetLife() {
        return _life;
    }

    public void SetTargetted(bool b) {
        if (b) {
            warlock_cast.SetActive(true);
        } else {
            warlock_cast.SetActive(false);
        }
    }

    public void CollidingWithChest(bool b, GameObject chest) {
        _collidingWithChest = b;
        _tempChest = chest;
    }

    public void CollidingWithChest(bool b) {
        _collidingWithChest = b;
        _tempChest = null;
    }

    public void ReduceLife(int value) {
        if (!invincible && _life > 0) {
            _life -= value;
            _healthBar.fillAmount -= (float)value / _maxLife;
            UpdateLife();
            if (_life <= 0) {
                GetComponent<SpriteRenderer>().enabled = false;
                DestroyObject(_weapon.gameObject);
                if (_bazooka != null)
                    DestroyObject(_bazooka.gameObject);
                Instantiate(_deathAnimation, transform.position, Quaternion.identity);
                StartCoroutine(GameManager.gm.GameOver(true));
            } else {
                Instantiate(_hitAnimation, transform.position, Quaternion.identity);
            }
        }
    }

    public void AddLife(int value) {
        if (!invincible) {
            _life += value;
            _healthBar.fillAmount += (float)value / _maxLife;
            UpdateLife();
        }
    }

    public int GetNumberOfKills() {
        return _numberOfKills;
    }

    public void AddScore(int value) {
        _score += value;
        ++_numberOfKills;
        UpdateScore();
    }

    public int GetScore() {
        return _score;
    }

    public void UpdateInteractionText(string str) {
        _interaction.text = str;
    }

    void UpdateLife() {
        if(_life < 0) {
            _life = 0;
        }
        _lifeText.text = "Life: " + _life.ToString();
        if (_life > _maxLife) {
            _maxLife = _life;
        }
    }

    public void UpdateAmmo() {
        if (_bazookaEquipped) {
            if (_bazooka.GetAmmo() + _bazooka.GetMag() == 0) {
                Destroy(_bazooka.gameObject);
                _weapon.gameObject.SetActive(true);
                _weapon.IsEquipped(true);
                _fireRate = _weapon.GetFireRate();
                _bazookaEquipped = false;
                _bazooka = null;
                _ammoText.text = _weapon.GetMag() + "/" + _weapon.GetAmmo();
            } else
                _ammoText.text = _bazooka.GetMag() + "/" + _bazooka.GetAmmo();
        } else
            _ammoText.text = _weapon.GetMag() + "/" + _weapon.GetAmmo();
    }

    void UpdateScore() {
        _scoreText.text = "Score: " + _score.ToString();
    }

    public IEnumerator Notification(string notif, float time) {
        _notifCounts++;
        if (_notification.text != "") {
            _notification.text = _notification.text + "\n" + notif;
        } else
            _notification.text = notif;
        _notification.text = _notification.text.Trim();
        yield return new WaitForSeconds(time);
        if (_notifCounts > 1)
            _notification.text = _notification.text.Substring(notif.Length + 1);
        else
            _notification.text = _notification.text.Substring(notif.Length);
        _notifCounts--;
    }

    void Fire() {
        if (Time.timeSinceLevelLoad > _fireRate + _lastShot) {
            if (_bazookaEquipped)
                _bazooka.Fire();
            else
                _weapon.Fire();
            _lastShot = Time.timeSinceLevelLoad;
            UpdateAmmo();
        }
    }

    void PickupWeapon(Weapon wep) {
        if (_bazookaEquipped) {
            _weapon.gameObject.SetActive(true);
            DropWeapon(_weapon);
            wep.transform.parent = transform;
            wep.transform.localPosition = wep.GetWeaponPositionOnPlayer();
            _weapon = wep;
            if (_weapon.GetType() == typeof(GattlingGun))
                GetComponents<AudioSource>()[1].Play();
            _weapon.gameObject.SetActive(false);

        } else {
            if (wep.GetType() == typeof(Bazooka)) {
                _bazooka = wep;
                _bazookaEquipped = true;
                _weapon.IsEquipped(false);
                _weapon.gameObject.SetActive(false);
                _bazooka.IsEquipped(true);
                _bazooka.transform.parent = transform;
                _bazooka.transform.localPosition = wep.GetWeaponPositionOnPlayer();
            } else {
                if(_weapon != null)
                    DropWeapon(_weapon);
                wep.transform.parent = transform;
                wep.transform.localPosition = wep.GetWeaponPositionOnPlayer();
                _weapon = wep;
                _weapon.IsEquipped(true);
                if (_weapon.GetType() == typeof(GattlingGun))
                    GetComponents<AudioSource>()[1].Play();
            }
            _fireRate = _weapon.GetFireRate();
            UpdateAmmo();
        }
        _pickingUpWeapon = false;
    }

    void DropWeapon(Weapon wep) {
        wep.IsEquipped(false);
        wep.transform.parent = null;
    }

    public int GetWeaponDamage() {
        if (_bazookaEquipped) {
            return _bazooka.GetDamage();
        }
        return _weapon.GetDamage();
    }

    public Weapon GetWeapon() {
        return _weapon;
    }

    void OnCollisionEnter(Collision col) {
        if (col.gameObject.layer == LayerMask.NameToLayer("Ammo")) {
            col.gameObject.GetComponent<ItemRespawn>().DespawnItem();
            _weapon.AddAmmo(_weapon.GetAmmoCrateAmount());
            StartCoroutine(Notification("+" + _weapon.GetAmmoCrateAmount() + " Ammo", 1));
            UpdateAmmo();
        } else if (col.gameObject.layer == LayerMask.NameToLayer("Life")) {
            col.gameObject.GetComponent<ItemRespawn>().DespawnItem();
            _life++;
            _healthBar.fillAmount += 1.0f / _maxLife;
            UpdateLife();
            StartCoroutine(Notification("+1 Life", 1));
        } else if (col.gameObject.layer == LayerMask.NameToLayer("Medkit")) {
            Destroy(col.gameObject);
            _life += 3;
            _healthBar.fillAmount += 3.0f / _maxLife;
            UpdateLife();
            StartCoroutine(Notification("+3 Life", 1));
        }
    }

    void OnTriggerEnter(Collider col) {
        if (col.gameObject.layer == LayerMask.NameToLayer("Weapon") && col.gameObject.GetComponent<Weapon>().GetType() == typeof(Bazooka) && _bazooka != null) {
            _bazooka.AddAmmo(4);
            UpdateAmmo();
            StartCoroutine(Notification("+4 rockets", 1.5f));
            Destroy(col.gameObject);
        } else if (col.gameObject.layer == LayerMask.NameToLayer("Beam")) {
            ReduceLife(4);
        } else if (col.gameObject.layer == LayerMask.NameToLayer("Weapon")) {
            _interaction.text = "Press 'T' to pickup " + col.gameObject.GetComponent<Weapon>().GetType();
            _collidingWithWeapon = true;
            _tempWeapon = col.gameObject;
        }
    }

    void OnTriggerExit(Collider col) {
        if (col.gameObject.layer == LayerMask.NameToLayer("Weapon")) {
            _interaction.text = "";
            _collidingWithWeapon = false;
            _tempWeapon = null;
        }
    }
}