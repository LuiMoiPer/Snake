using System;
using System.Collections.Generic;

public class Grid {

    public static readonly int DEFAULT_WIDTH = 16;
    public static readonly int DEFAULT_HEIGHT = 10;
    public static Random random = new Random();

    public event EventHandler onPelletConsumed;
    public event EventHandler onSnakeCollision;

    public int width {
        get {
            return _width;
        }
    }
    public int height {
        get {
            return _height;
        }
    }
    public int numPellets {
        get {
            return pellets.Count;
        }
    }
    public float percentCovered {
        get {
            return (float) snake.length / (float) (_width * _height);
        }
    }

    private int _width;
    private int _height;
    private List<Pellet> pellets;
    private Snake snake;
    private bool[,] locationIsOccupied;
    private int pelletSize = 1;


    public Grid(int width, int height) {
        this._width = width;
        this._height = height;
        this.pellets = new List<Pellet>();
        this.locationIsOccupied = new bool[width, height];
    }

    private Pellet checkPelletCollision() {
        Coord snakeHead = snake.headPosition;

        for (int i = 0; i < pellets.Count; i++) {
            if (pellets[i].position == snakeHead) {
                return pellets[i];
            }
        }

        return null;
    }

    private bool snakeCollided() {
        // clear location is empty
        Array.Clear(locationIsOccupied, 0, locationIsOccupied.Length);
        
        bool collided = false;
        foreach (Coord nodePosition in snake.getNodePositions()) {
            // check if out of bounds
            if (isValidLocation(nodePosition) == false) {
                collided = true;
                break;
            }
            // check if already occupied
            if (locationIsOccupied[nodePosition.x, nodePosition.y]) {
                collided = true;
                break;
            }
            else {
                locationIsOccupied[nodePosition.x, nodePosition.y] = true;
            }
        }

        updateOccupiedByPellets();
        return collided;
    }

    private void updateOccupiedByPellets() {
        foreach (Pellet pellet in pellets) {
            locationIsOccupied[pellet.position.x, pellet.position.y] = true;
        }
    }

    public bool isValidLocation(Coord position) {
        if ((position.x >= 0 && position.x < width) && (position.y >= 0 && position.y < height)) {
            return true;
        }
        else {
            return false;
        }
    }

    public void checkCollisions() {
        if (snakeCollided()) {
            onSnakeCollision?.Invoke(this, EventArgs.Empty);
        }

        Pellet pellet = checkPelletCollision();
        if (pellet != null) {
            snake.eat(pellet);
            pellets.Remove(pellet);
            onPelletConsumed?.Invoke(this, EventArgs.Empty);
        }
    }

    public Pellet addPelletInEmptyLocation() {
        // get all empty locations
        List<Coord> emptyLocations = new List<Coord>();
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                if (locationIsOccupied[i, j] == false) {
                    emptyLocations.Add(new Coord(i, j));
                }
            }
        }

        if (emptyLocations.Count > 0) {
            Pellet pellet = new Pellet(emptyLocations[random.Next(emptyLocations.Count)], pelletSize);
            pellets.Add(pellet);
            return pellet;
        }
        return null;
    }

    public void setSnake(Snake snake) {
        this.snake = snake;
    }

    public void setPelletSize(int size) {
        pelletSize = size;
    }

    public List<Pellet> getPellets() {
        return pellets;
    }
}