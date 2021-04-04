using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour{

    [SerializeField]
    private Transitions transitions;

    void Start() {
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
        transitions.onScreenCovered -= handleFadeOut;
    }
}
