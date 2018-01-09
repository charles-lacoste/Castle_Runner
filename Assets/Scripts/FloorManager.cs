using UnityEngine;
using System.Collections;

public class FloorManager : MonoBehaviour {
    [SerializeField]
    GameObject[] _rooms;
    [SerializeField]
    GameObject _minimapCamera;
	
	void Awake () {
        GameManager.gm.LoadRooms(_rooms);
        GameManager.gm.SetMinimapCamera(_minimapCamera);
	}

    public GameObject[] GetRooms() {
        return _rooms;
    }
}
