using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class FinalBoss : Enemy {
    [SerializeField]
    Image _healthBar;
    [SerializeField]
    GameObject _attackBar, leftBeam, rightBeam, necro, mage, warlock, leftSpawn, rightSpawn, orb, _leftEye, _rightEye, _middleEye;

    private Image _attackBarImage;
    private Text _attackText;
    private int _maxLife;
    private int _energizePenalty;
    private int ads;
    private float castTime, summonCastTime, orbCastTime, beamCastTime, energizeCastTime;
    private bool invulnerable, casting, weakPhaseOne, weakPhaseTwo;
    private float summonTimer, beamTimer;
    GameObject enemy1, enemy2;

    void Start() {
        ads = 0;
        summonTimer = 0;
        beamTimer = 7.0f;
        summonCastTime = 2.0f;
        orbCastTime = 1.5f;
        beamCastTime = 1.5f;
        energizeCastTime = 7.5f;
        _energizePenalty = 1;
        _life = _maxLife = 450;
        _scoreValue = 10000; //topkek
        _player = FindObjectOfType<Player>();
        _rb = GetComponent<Rigidbody>();
        _attackBarImage = _attackBar.GetComponentInChildren<Image>();
        _attackText = _attackBarImage.GetComponentInChildren<Text>();
        _anim = GetComponent<Animator>();
        _sr = GetComponent<SpriteRenderer>();
    }

    void Update() {
        if (!GameManager.gm.GetPause()) {
            if (summonTimer < 0.0f && ads == 0 && !casting) {
                _anim.SetBool("Summon", true);
                //StartCoroutine(SummonStuff()); //animator instead
            } else if (beamTimer < 0.0f && !casting) {
                _anim.SetBool("Beam", true);
                //StartCoroutine(LaserBeamStuff());
            }
            if (casting) {
                _attackBarImage.fillAmount -= 1.0f / castTime * Time.deltaTime;
            }
            beamTimer -= Time.deltaTime;
            summonTimer -= Time.deltaTime;
        }
    }

    IEnumerator SummonStuff() {
        //screen shake
        casting = true;
        _attackBar.SetActive(true);
        _attackBarImage.fillAmount = 1;
        _attackText.text = "Summoning.. stuff";
        castTime = summonCastTime;
        yield return new WaitForSeconds(summonCastTime);
        int enemy = Random.Range(0, 3);
        //0 = warlock
        //1 = necro
        //2 = mage
        switch (enemy) {
            case 0:
                enemy1 = Instantiate(warlock, leftSpawn.transform.position, Quaternion.identity) as GameObject;
                enemy2 = Instantiate(warlock, rightSpawn.transform.position, Quaternion.identity) as GameObject;
                enemy1.GetComponent<Warlock>().SetFinalBoss(true);
                enemy2.GetComponent<Warlock>().SetFinalBoss(true);

                break;
            case 1:
                enemy1 = Instantiate(necro, leftSpawn.transform.position, Quaternion.identity) as GameObject;
                enemy2 = Instantiate(necro, rightSpawn.transform.position, Quaternion.identity) as GameObject;
                enemy1.GetComponent<Necromancer>().SetFinalBoss(true);
                enemy2.GetComponent<Necromancer>().SetFinalBoss(true);

                break;
            case 2:
                enemy1 = Instantiate(mage, leftSpawn.transform.position, Quaternion.identity) as GameObject;
                enemy2 = Instantiate(mage, rightSpawn.transform.position, Quaternion.identity) as GameObject;
                enemy1.GetComponent<Mage>().SetFinalBoss(true);
                enemy2.GetComponent<Mage>().SetFinalBoss(true);

                break;
        }
        enemy1.transform.SetParent(this.transform);
        enemy2.transform.SetParent(this.transform);
        invulnerable = true;
        ads += 2;
        summonTimer = 5.0f;
        _attackBar.SetActive(false);
        _anim.SetBool("Summon", false);
        casting = false;
    }

    IEnumerator LaserBeamStuff() {
        casting = true;
        castTime = beamCastTime;
        _attackBar.SetActive(true);
        _attackBarImage.fillAmount = 1;
        _attackText.text = "Laser beam and.. stuff";
        yield return new WaitForSeconds(beamCastTime);
        _attackBar.SetActive(false);
        leftBeam.SetActive(true);
        rightBeam.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        _anim.SetBool("Beam", false);
        //StartCoroutine(SpawnOrbs());
        _anim.SetBool("Orb", true);
        //start orb coroutine
        //end of orb coroutine disable beams
    }

    IEnumerator SpawnOrbs() {
        castTime = orbCastTime;
        _attackBar.SetActive(true);
        _attackBarImage.fillAmount = 1;
        _attackText.text = "Orbs and.. stuff";
        yield return new WaitForSeconds(orbCastTime); //cast time
        _attackBar.SetActive(false);
        Instantiate(orb, _middleEye.transform.position, Quaternion.identity);
        Instantiate(orb, _leftEye.transform.position, Quaternion.identity);
        Instantiate(orb, _rightEye.transform.position, Quaternion.identity);
        yield return new WaitForSeconds(4.5f); //wait for orbs to reach player (if they make it)
        leftBeam.SetActive(false);
        rightBeam.SetActive(false);
        beamTimer = 5.0f;
        _anim.SetBool("Orb", false);
        casting = false;
    }

    IEnumerator Energize() {
        casting = true;
        _anim.SetBool("Summon", false);
        _anim.SetBool("Beam", false);
        _anim.SetBool("Orb", false);
        castTime = energizeCastTime;
        _energizePenalty = 3;
        if (leftBeam.activeSelf) {
            leftBeam.SetActive(false);
        }
        if (rightBeam.activeSelf) {
            rightBeam.SetActive(false);
        }
        _attackBar.SetActive(true);
        _attackBarImage.fillAmount = 1;
        _attackText.text = "Energizing.. and stuff";
        yield return new WaitForSeconds(energizeCastTime);
        _attackBar.SetActive(false);
        _energizePenalty = 1;
        casting = false;
    }

    public void RemoveAd() {
        ads--;
        if (ads == 0) {
            invulnerable = false;
        }
    }

    public override void DeductLife() {
        if (!invulnerable) {
            _life -= _player.GetWeaponDamage() * _energizePenalty;
            _healthBar.fillAmount -= (float)(_player.GetWeaponDamage() * _energizePenalty) / _maxLife;
            Instantiate(_hitAnimation, transform.position, Quaternion.identity);
            if (_life < 360 && !weakPhaseOne) {
                weakPhaseOne = true;
                StopAllCoroutines();
                StartCoroutine(Energize());
            }
            if (_life < 190 && !weakPhaseTwo) {
                weakPhaseTwo = true;
                StopAllCoroutines();
                StartCoroutine(Energize());
            }
            if (_life <= 0) {
                //add explosions
                _roomManager.RemoveEnemy();
                _player.GetComponent<Player>().AddScore(_scoreValue);
                _sr.enabled = false;
                StartCoroutine(GameManager.gm.GameOver(false));
            }
        }
    }
}
