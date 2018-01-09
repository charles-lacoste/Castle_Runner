using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour {
    // Function used when a button is clicked on the main menu.
    public void OnClickPlay() {
        PauseGame.pg.Unpause();
    }

    public void OnClickBackToMenu() {
        DontDestroyOnLoad[] ok = FindObjectsOfType<DontDestroyOnLoad>();
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
