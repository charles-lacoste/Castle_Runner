using UnityEngine;
using System.Collections;

public class Rocket : MonoBehaviour {
    [SerializeField]
    GameObject _explosion;
    private float _explosionRadius;

    public void SetExplosionRadius(float r) {
        _explosionRadius = r;
    }

	void OnCollisionEnter(Collision col) {
        Vector3 hitPoint = col.contacts[0].point;
        Collider[] hitColliders = Physics.OverlapSphere(hitPoint, _explosionRadius);
        int i = 0;
        while(i < hitColliders.Length) {
            hitColliders[i].gameObject.SendMessage("DeductLife", null, SendMessageOptions.DontRequireReceiver);
            i++;
        }
        Instantiate(_explosion, hitPoint, Quaternion.identity);
        Destroy(gameObject);
    }
}
