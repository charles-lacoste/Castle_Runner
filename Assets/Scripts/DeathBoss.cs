using UnityEngine;
using System.Collections;

public class DeathBoss : MonoBehaviour {
    [SerializeField]
    float _followSpeed;
    [SerializeField]
    GameObject _deathRing;

    private Player _player;
    private bool _following, _death;
    private SpriteRenderer _sr;
    private CapsuleCollider _cc;
    private AudioSource _runAwayLittleGirl;
    // Use this for initialization
    void Start() {
        _player = FindObjectOfType<Player>();
        _cc = GetComponent<CapsuleCollider>();
        _sr = GetComponent<SpriteRenderer>();
        _sr.enabled = false;
        _cc.enabled = false;
        _deathRing.SetActive(false);
        _runAwayLittleGirl = GetComponent<AudioSource>();
    }

    IEnumerator Death() {
        _death = true;
        yield return new WaitForSeconds(7.5f);
        _sr.enabled = true;
        _deathRing.SetActive(true);
        _runAwayLittleGirl.Play();
        transform.position = new Vector3(Random.insideUnitCircle.x * 5.0f, Random.insideUnitCircle.y * 5.0f, 0.0f) + _player.gameObject.transform.position;
        _cc.enabled = false;
        yield return new WaitForSeconds(3.0f);
        _cc.enabled = true;
        _following = true;
        yield return new WaitForSeconds(15.0f);
        _following = false;
        _sr.enabled = false;
        _cc.enabled = false;
        _deathRing.SetActive(false);
        _death = false;
    }

    // Update is called once per frame
    void Update() {
        if (!GameManager.gm.GetPause()) {
            if (!_death) {
                StartCoroutine(Death());
            }
            if (_following) {
                UpdateAnimator();
            }
        }
    }

    void FixedUpdate() {
        if (_following)
            transform.position = Vector3.MoveTowards(transform.position, _player.gameObject.transform.position, _followSpeed);
    }

    void UpdateAnimator() {
        Vector2 diff = _player.gameObject.transform.position - transform.position;
        float sign = (_player.transform.position.y < transform.position.y) ? -1.0f : 1.0f;
        float angle = Vector2.Angle(Vector2.right, diff) * sign;
        if (angle > 90 || angle < -90) {
            _sr.flipX = false;
        } else if (angle < 90 && angle > -90) {
            _sr.flipX = true;
        }
    }

    void OnTriggerEnter(Collider col) {
        if (col.gameObject.layer == LayerMask.NameToLayer("Player")) {
            _player.ReduceLife(_player.GetLife());
        }
    }
}
