using UnityEngine;
using System.Collections;

public class SmoothCamera : MonoBehaviour {
    [SerializeField]
    private Transform _playerPosition;
    [SerializeField]
    private float _smoothness;
    private bool _lockedToRoom;
    private Vector3 _roomPosition;

    void Start() {

    }

    public void SetSmoothness(float s) {
        _smoothness = s;
    }

    // Update is called once per frame
    void FixedUpdate() {
        if (!GameManager.gm.GetPause()) {
            if (!_lockedToRoom)
                transform.position = Vector3.Lerp(transform.position, new Vector3(_playerPosition.position.x, _playerPosition.position.y, -10), _smoothness * Time.deltaTime);
            else
                transform.position = Vector3.Lerp(transform.position, _roomPosition, _smoothness * 2.0f * Time.deltaTime);
        }

    }

    public void Lock(Vector3 t) {
        _lockedToRoom = true;
        _roomPosition = t;
        _roomPosition.z = -10.0f;
    }

    public void Unlock() {
        _lockedToRoom = false;
    }
}
