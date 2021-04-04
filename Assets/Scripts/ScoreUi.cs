using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreUi : MonoBehaviour {

    [SerializeField]
    private TextMeshProUGUI score;
    [SerializeField]
    private GameController gameController;

    // Start is called before the first frame update
    void Start() {
        gameController.onScoreChange += handleScoreChanged;
        score.SetText("0");
    }

    void handleScoreChanged(object sender, EventArgs args) {
        score.SetText(gameController.score.ToString());
    }
}
