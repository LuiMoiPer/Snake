using UnityEngine;

public class InputManager : MonoBehaviour {

    public Coord direction { 
        get {
            return _direction;
        }
    }

    private Coord _direction;

    public InputManager() {
        this._direction = Coord.left;
    }

    void Update() {
        float xAxis = Input.GetAxisRaw("Horizontal");
        float yAxis = Input.GetAxisRaw("Vertical");

        if (xAxis < 0f) {
            _direction = Coord.left;
        }
        else if (xAxis > 0f) {
            _direction = Coord.right;
        }
        else if (yAxis > 0f) {
            _direction = Coord.up;
        }
        else if (yAxis < 0f) {
            _direction = Coord.down;
        }
    }
}