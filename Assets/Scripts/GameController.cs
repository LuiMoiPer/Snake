using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

    public event EventHandler onScoreChange;
    public int score {
        get {
            return _score;
        }
    }

    private static readonly float MAX_TIME_BETWEEN_MOVES = 1f;
    private static readonly float MIN_TIME_BETWEEN_MOVES = 0.25f;
    private static Vector3 offset = new Vector3(-0.5f, 0f, -0.5f);
    private static float borderSize = 0.25f;

    [SerializeField]
    private Transitions transitions;
    [SerializeField]
    private GameObject tilePrefab;
    [SerializeField]
    private GameObject gapFillPrefab;
    [SerializeField]
    private GameObject snakePrefab;
    [SerializeField]
    private GameObject pelletPrefab;
    [SerializeField]
    private InputManager inputManager;

    private Grid grid;
    private Snake snakeModel;
    private SnakeGo snakeGo;
    private float timeToNextMove;
    private float timeBetweenMoves;
    private bool runGame = false;
    private int pelletSize = 1;
    private List<GameObject> pelletVisuals = new List<GameObject>();
    private String[] toneNames = {"1", "2", "3", "4", "5", "6", "7"};
    private System.Random random = new System.Random();
    private AudioManager audioManager;
    private int audioCounter = 0;
    private int _score = 0;
    
    // Start is called before the first frame update
    void Start() {
        PlayerPrefs.DeleteAll();

        timeBetweenMoves = MAX_TIME_BETWEEN_MOVES;

        grid = new Grid(Grid.DEFAULT_WIDTH, Grid.DEFAULT_HEIGHT);
        grid.onPelletConsumed += handlePelletConsumed;
        grid.onSnakeCollision += handleSnakeCollision;

        snakeModel = new Snake(new Coord(grid.width / 2, grid.height / 2), 3, Coord.left);
        snakeModel.onGrow += handleSnakeGrow;
        grid.setSnake(snakeModel);
        
        timeToNextMove = timeBetweenMoves;
        makeGridVisual();
        makeSnakeVisual();
        makePelletVisual(grid.addPelletInEmptyLocation());

        audioManager = GameObject.Find("AudioManager")
            .GetComponent(typeof(AudioManager)) as AudioManager;
        transitions = GameObject.Find("Transitions")
            .GetComponent(typeof(Transitions)) as Transitions;
        transitions.onScreenRevealed += handleFadeIn;
        transitions.onScreenCovered += handleFadeOut;
    }

    // Update is called once per frame
    void Update() {
        if (timeToNextMove <= 0f && runGame) {
            moveSnake();
            grid.checkCollisions();
            timeToNextMove = timeBetweenMoves;
        }

        snakeModel.direction = inputManager.direction;
        timeToNextMove -= Time.deltaTime;
        snakeGo.setTimeToNextMove(timeToNextMove);
    }

    private void makeGridVisual() {
        for (int i = 0; i < grid.width; i++) {
            for (int j = 0; j < grid.height; j++) {
                // make tile
                GameObject tile = GameObject.Instantiate(
                    tilePrefab,
                    new Vector3(i, 0, j),
                    Quaternion.identity,
                    gameObject.transform
                );
                tile.name = i + "," + j;
            }
        }

        // make gap filler
        GameObject gapFiller = GameObject.Instantiate(
            gapFillPrefab,
            new Vector3(0, 0, 0),
            Quaternion.identity,
            gameObject.transform
        );
        gapFiller.transform.localScale = new Vector3(
            grid.width + borderSize, 
            0.9f,
            grid.height + borderSize
        );
        Vector3 center = new Vector3(grid.width / 2f, 0f, grid.height / 2f);
        gapFiller.transform.position = center + offset;
    }

    private void makeSnakeVisual() {
        GameObject snakeVisual = GameObject.Instantiate(snakePrefab, new Vector3(0, 0, 0), Quaternion.identity);
        snakeGo = snakeVisual.GetComponent(typeof(SnakeGo)) as SnakeGo;
        snakeGo.setSnake(snakeModel);
        snakeGo.setTimeBetweenMoves(timeBetweenMoves);
    }

    private GameObject makePelletVisual(Pellet pellet) {
        GameObject pelletVisual = GameObject.Instantiate(
            pelletPrefab,
            new Vector3(0, 0, 0),
            Quaternion.identity,
            gameObject.transform
        );
        PelletGo pelletGo = pelletVisual.GetComponent(typeof(PelletGo)) as PelletGo;
        pelletGo.setPellet(pellet);
        return pelletVisual;
    }

    private void moveSnake(){
        snakeModel.move();
        playRandomTone();
    }

    private void handleSnakeCollision(object sender, EventArgs args) {
        runGame = false;
        PlayerPrefs.SetInt("Score", score);
        audioManager.Play("15");
        transitions.Wipe();
    }

    private void handleSnakeGrow(object sender, EventArgs args) {
        timeBetweenMoves = Mathf.Lerp(MAX_TIME_BETWEEN_MOVES, MIN_TIME_BETWEEN_MOVES, grid.percentCovered);
        snakeGo.setTimeBetweenMoves(timeBetweenMoves);
    }

    private void handlePelletConsumed(object sender, EventArgs args) {
        grid.setPelletSize(pelletSize);
        makePelletVisual(grid.addPelletInEmptyLocation());
        audioManager.Play("" + random.Next(10,15));
        _score++;
        onScoreChange?.Invoke(this, EventArgs.Empty);
    }

    private void handleFadeIn(object sender, EventArgs empty) {
        runGame = true;
    }

    private void handleFadeOut(object sender, EventArgs empty) {
        SceneManager.LoadScene(2);
    }

    private void playRandomTone() {
        audioManager.Play(toneNames[random.Next(toneNames.Length)]);
    }

    private void playAudio() {
        audioManager.Play("" + random.Next(0,5));
        if (audioCounter % 2 == 0) {
            audioManager.Play("" + random.Next(5,10));
        }
        audioCounter++;
    }
}
