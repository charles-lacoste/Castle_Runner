using UnityEngine;
using System.Collections;

public class Chest : MonoBehaviour {
    private Player _playerScript;
    [SerializeField]
    private Sprite _openChest;
    private bool open = false;

    void Start() {
        _playerScript = FindObjectOfType<Player>();
    }

    public bool IsOpen() {
        return open;
    }

    public void OpenChest() {
        GetComponent<SpriteRenderer>().sprite = _openChest;
        int dropChance = Random.Range(0, 100);
        open = true;
        _playerScript.UpdateInteractionText("");
        Vector3 spawnPoint = new Vector3(transform.position.x, transform.position.y - 0.8f, 0.0f);
        Weapon _currWeap = _playerScript.GetWeapon();
        GameObject[] _weapons = GameManager.gm.GetWeapons();
        if(dropChance >= 0 && dropChance <= 9) { //rpg
            Instantiate(_weapons[_weapons.Length - 1], spawnPoint, Quaternion.identity);
        }
        else if (dropChance >= 10 && dropChance <= 34) { //new weapon
            int weapon = Random.Range(0, _weapons.Length - 1);
            while (_weapons[weapon].GetComponent<Weapon>().GetType() == _currWeap.GetType() || weapon % 2.0f != 0) {
                weapon = Random.Range(0, _weapons.Length - 1);
            }
            Instantiate(_weapons[weapon], spawnPoint, Quaternion.identity);
        }
        else if (dropChance >= 35 && dropChance <= 50 && _currWeap.IsUpgradeable()) { //upgrade for current weapon
            for (int i = 0; i < _weapons.Length - 1; i++) {
                if (_weapons[i].GetComponent<Weapon>().GetType() == _currWeap.GetType()) {
                    Instantiate(_weapons[i + 1], spawnPoint, Quaternion.identity);
                    break;
                }
            }
        }
        else { //medkit
            Instantiate(GameManager.gm.GetDrop(2), spawnPoint, Quaternion.identity);
        }
    }

    void OnCollisionEnter(Collision col) {
        if (col.gameObject.layer == LayerMask.NameToLayer("Player") && !open) {
            _playerScript.CollidingWithChest(true, gameObject);
        }
    }

    void OnCollisionStay(Collision col) {
        if (col.gameObject.layer == LayerMask.NameToLayer("Player") && !open) {
            _playerScript.UpdateInteractionText("Press 'F' to open chest");
        }
    }

    void OnCollisionExit(Collision col) {
        if(col.gameObject.layer == LayerMask.NameToLayer("Player")) {
            _playerScript.UpdateInteractionText("");
            _playerScript.CollidingWithChest(false);
        }
    }

    void OnTriggerExit(Collider col) {
        GetComponent<BoxCollider>().isTrigger = false;
    }
}
