using UnityEngine;
using System.Collections;

public class RoomManager : MonoBehaviour {
    int _nbOfEnemiesLeft;
    bool _unlocked, _cleared;
    [SerializeField]
    GameObject[] _doors, _enemies;
    [SerializeField]
    GameObject _chest, _minimapObject;
    Player player;

    // Use this for initialization
    void Start() {
        player = GameObject.FindObjectOfType<Player>();
        _nbOfEnemiesLeft = _enemies.Length;
        _unlocked = true;
        foreach (GameObject e in _enemies) {
            e.SetActive(false);
        }
        if (_chest != null)
            _chest.SetActive(false);
    }

    void Update() {
        //Debug.Log(Physics.CheckSphere(player.gameObject.transform.position, 1.5f));
    }
    public void RemoveEnemy() {
        _nbOfEnemiesLeft--;
        if (_nbOfEnemiesLeft == 0) {
            _cleared = true;
            UnlockDoors();
            if (_chest != null) {
                //check if player is in chest collider bounds
                if (Physics.CheckSphere(_chest.transform.position, 0.2f)) {
                    _chest.GetComponent<BoxCollider>().isTrigger = true;
                }
                _chest.SetActive(true);
            }
        }
    }

    public bool IsCleared() {
        return _cleared;
    }

    public void LockDoors(bool isBoss) {
        if (!_cleared && _unlocked) {
            GameManager.gm.GetCamera().GetComponent<SmoothCamera>().Lock(transform.position);
            if (isBoss)
                GameManager.gm.GetCamera().orthographicSize = 10.5f;
            foreach (GameObject door in _doors) {
                if (door.layer == LayerMask.NameToLayer("Gate")) {
                    door.GetComponent<BoxCollider>().enabled = true;
                    door.GetComponent<SpriteRenderer>().sprite = GameManager.gm.GetClosedGate();
                } else {
                    door.GetComponent<Door>().Close();
                }
            }
            foreach (GameObject e in _enemies) {
                e.SetActive(true);
            }
            _unlocked = false;
        }
    }

    public void UnlockDoors() {
        foreach (GameObject door in _doors) {
            if (door.layer == LayerMask.NameToLayer("Gate")) {
                door.GetComponent<BoxCollider>().enabled = false;
                door.GetComponent<SpriteRenderer>().sprite = GameManager.gm.GetOpenedGate();
            } else {
                door.GetComponent<Door>().Open();
            }
        }
        _unlocked = true;
        _minimapObject.layer = LayerMask.NameToLayer("Minimap");
        GameManager.gm.GetCamera().GetComponent<SmoothCamera>().Unlock();
        GameManager.gm.GetCamera().orthographicSize = 6.5f;
    }
}
