using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour { 
    // Function used when a button is clicked on the main menu.
    public void OnClickPlay() {
        SceneManager.LoadScene(2);
    }
    public void OnClickHowTo() {
        SceneManager.LoadScene(1);
    }
    public void OnClickBackToMenu() {
        SceneManager.LoadScene(0);
    }
    public void OnClickExit() {
        Application.Quit();
    }
}
