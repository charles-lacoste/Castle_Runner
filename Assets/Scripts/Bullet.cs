using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {
    void OnCollisionEnter(Collision col) {
        if (col.gameObject.layer == LayerMask.NameToLayer("Charger")) {
            col.gameObject.GetComponent<Charger>().DeductLife();
        }
        else if (col.gameObject.layer == LayerMask.NameToLayer("Archer")) {
            col.gameObject.GetComponent<Archer>().DeductLife();
        }
        else if (col.gameObject.layer == LayerMask.NameToLayer("Mage")) {
            col.gameObject.GetComponent<Mage>().DeductLife();
        }
        else if (col.gameObject.layer == LayerMask.NameToLayer("Necromancer")) {
            col.gameObject.GetComponent<Necromancer>().DeductLife();
        }
        else if (col.gameObject.layer == LayerMask.NameToLayer("Warlock")) {
            col.gameObject.GetComponent<Warlock>().DeductLife();
        }
        else if (col.gameObject.layer == LayerMask.NameToLayer("Skeleton")) {
            col.gameObject.GetComponent<Skeleton>().DeductLife();
        }
        else if (col.gameObject.layer == LayerMask.NameToLayer("Boss")) {
            col.gameObject.SendMessage("DeductLife", null, SendMessageOptions.DontRequireReceiver);
        }
        else if (col.gameObject.layer == LayerMask.NameToLayer("Player")) {
            col.gameObject.GetComponent<Player>().ReduceLife(1);
        }
        Destroy(gameObject);
    }
}
