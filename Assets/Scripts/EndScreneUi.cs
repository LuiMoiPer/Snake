using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class EndScreneUi : MonoBehaviour {
    [SerializeField]
    private TextMeshProUGUI finalScore;
    private Transitions transitions;
    // Start is called before the first frame update
    void Start() {
        finalScore.SetText(PlayerPrefs.GetInt("Score").ToString());
        transitions = GameObject.Find("Transitions")
            .GetComponent(typeof(Transitions)) as Transitions;
        transitions.onScreenCovered += handleFadeOut;
    }
    public void quit() {
        Application.Quit();
    }

    public void play() {
        transitions.Wipe();
    }

    private void handleFadeOut(object sender, EventArgs empty) {
        SceneManager.LoadScene(1);
    }
}
