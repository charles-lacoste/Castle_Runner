using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerDeathOverlay : MonoBehaviour {
    [SerializeField]
    Text _kills, _score;
    [SerializeField]
    Image _shade;

    private Color _c;

    void OnEnable() {
        _c = _shade.color;
        _kills.text = "Kills: " + FindObjectOfType<Player>().GetNumberOfKills();
        _score.text = "Score: " + FindObjectOfType<Player>().GetScore();
    }

    // Update is called once per frame
    void Update() {
        if (_c.a <= 0.75f) {
            _c.a += 0.01f;
            _shade.color = _c;
        }
    }
}
