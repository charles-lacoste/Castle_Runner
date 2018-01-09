using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public static GameManager gm;

    [SerializeField]
    private Texture2D cursor;
    [SerializeField]
    private GameObject _loadingOverlay, _pauseMenu, _deathOverlay, _winOverlay, _loreOverlay;
    [SerializeField]
    Sprite _closedGate, _openGate;
    [SerializeField]
    private GameObject[] _itemList, _weapons, _roomsTemp;
    [SerializeField]
    private AudioClip _bgm, _bossBgm;
    [SerializeField]
    Camera _camera;

    private IDictionary<int, GameObject> _rooms;
    private GameObject _minimap;
    private int _currentLevel;
    private bool _pause, _minimapState, _isEnabled;
    public static PauseGame pg;

    void Start() {
        Cursor.SetCursor(cursor, new Vector2(cursor.width / 2, cursor.height / 2), CursorMode.Auto);
        _currentLevel = 1;
        LoadNextLevel();
        //AudioManager.instance.PlayBGM(_bgm);
    }

    void Awake() {
        if (gm == null)
            gm = this;
        DontDestroyOnLoad(_pauseMenu);
        DontDestroyOnLoad(_deathOverlay);
        DontDestroyOnLoad(_winOverlay);
    }

    void Update() {
        // Enable pause menu
        if (Input.GetKeyDown(KeyCode.Escape) && !_isEnabled) {
            Pause();
        }
        // disable pause menu
        else if (Input.GetKeyDown(KeyCode.Escape) && _isEnabled) {
            Unpause();
        }
    }

    public float GetMultiplier() {
        if (_currentLevel == 3 || _currentLevel == 4) { 
            return 1.25f;
        }
        return 1.0f;
    }

    public IEnumerator GameOver(bool b) {
        yield return new WaitForSeconds(1.5f);
        Time.timeScale = 0;
        TogglePause();
        _isEnabled = true;
        if (b) {
            _deathOverlay.SetActive(true);
        }
        else {
            _winOverlay.SetActive(true);
        }
    }

    public Sprite GetClosedGate() {
        return _closedGate;
    }

    public Sprite GetOpenedGate() {
        return _openGate;
    }

    public void LoadNextLevel() {
        Player p = FindObjectOfType<Player>();
        p.Loading(true);
        if(_currentLevel == 2) {

        }
        StartCoroutine(LoadOverlay(++_currentLevel, p));
        if (_currentLevel > 2) {
            SceneManager.LoadScene(_currentLevel);
        }
    }

    public void LoadRooms(GameObject[] newRooms) {
        _rooms = new Dictionary<int, GameObject>();
        int i = 1;
        foreach (GameObject room in newRooms) {
            _rooms.Add(i, room);
            ++i;
        }
    }

    IEnumerator LoadOverlay(int i, Player p) {
        if(_currentLevel == 2) {
            _loreOverlay.SetActive(true);
            yield return new WaitForSeconds(13.0f);
            _loreOverlay.SetActive(false);
        }
        _loadingOverlay.SetActive(true);
        Text levelText = GameObject.Find("LevelText").GetComponent<Text>();
        levelText.text = "Loading floor " + (i - 1);
        Camera.main.GetComponent<SmoothCamera>().SetSmoothness(4.0f);
        yield return new WaitForSeconds(1.0f); //Give system time to destroy all the previous floor objects.
        FindObjectOfType<Player>().GetComponent<Rigidbody>().velocity = Vector3.zero;
        FindObjectOfType<Player>().transform.position = GameObject.FindGameObjectWithTag("SpawnPoint").transform.position;
        yield return new WaitForSeconds(1.0f);
        _loadingOverlay.SetActive(false);
        Camera.main.GetComponent<SmoothCamera>().SetSmoothness(1.0f);
        p.Loading(false);
    }

    public void RemoveEnemyFromRoom(int roomNb) {
        GameObject room;
        _rooms.TryGetValue(roomNb, out room);
        if (room != null)
            room.GetComponent<RoomManager>().RemoveEnemy();
    }

    public void LockRoom(int roomNb, bool isBoss) {
        GameObject room;
        _rooms.TryGetValue(roomNb, out room);
        if (room != null) {
            if (isBoss) {
                GameObject.Find("Music").GetComponent<AudioSource>().Stop();
                if (_currentLevel == 4)
                    FindObjectOfType<DeathBoss>().gameObject.SetActive(false);
            }
            room.gameObject.GetComponent<RoomManager>().LockDoors(isBoss);
        }
    }

    public Camera GetCamera() {
        return _camera;
    }

    public GameObject GetDrop(int drop) {
        return _itemList[drop];
    }

    public GameObject[] GetWeapons() {
        return _weapons;
    }

    public void TogglePause() {
        _pause = !_pause;
    }

    public bool GetPause() {
        return _pause;
    }

    public void Pause() {
        Time.timeScale = 0;
        TogglePause();
        _pauseMenu.SetActive(true);
        _isEnabled = true;
    }

    public void Unpause() {
        Time.timeScale = 1;
        TogglePause();
        _pauseMenu.SetActive(false);
        _isEnabled = false;
    }

    public void SetMinimapCamera(GameObject c) {
        _minimap = c;
    }

    public void ToggleMinimap() {
        _minimap.gameObject.SetActive(_minimapState = !_minimapState);
    }

    public void OnClickPlay() {
        Unpause();
    }

    public void OnClickBackToMenu() {
        DestroyObject(_pauseMenu);
        DestroyObject(_deathOverlay);
        DestroyObject(_winOverlay);
        foreach (DontDestroyOnLoad o in FindObjectsOfType<DontDestroyOnLoad>()) {
            Destroy(o.gameObject);
        }
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void OnClickExit() {
        Application.Quit();
    }
}