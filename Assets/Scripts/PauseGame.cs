using UnityEngine;
using System.Collections;

public class PauseGame : MonoBehaviour {

    public GameObject pauseMenu;
    private bool isEnabled = false;
    public static PauseGame pg;

    void Awake() {
        //DontDestroyOnLoad(pauseMenu);
        //if (pg == null)
        //    pg = this;
    }

    void Update() {
        // Enable pause menu
        if (Input.GetKeyDown(KeyCode.Escape) && !isEnabled) {
            Pause();
        }

        // disable pause menu
        else if (Input.GetKeyDown(KeyCode.Escape) && isEnabled) {
            Unpause();
        }
    }

    public void Pause() {
        Time.timeScale = 0;
        GameManager.gm.TogglePause();
        pauseMenu.SetActive(true);
        isEnabled = true;
    }

    public void Unpause() {
        Time.timeScale = 1;
        GameManager.gm.TogglePause();
        pauseMenu.SetActive(false);
        isEnabled = false;
    }

}