using System;
using System.Collections.Generic;
public class Snake {

    public event EventHandler onMove;
    public event EventHandler onEat;
    public event EventHandler onGrow;

    public static readonly int DEFAULT_SIZE = 4;
    public static readonly Coord DEFAULT_DIRECTION = Coord.right;

    public Coord direction {
        get {
            return this._direction;
        }
        set {
            if (isBackwards(value) == false) {
                this._direction = value;
            }
        }
    }
    public Coord headPosition {
        get {
            return head.position;
        }
    }
    public Coord tailPosition {
        get {
            return tail.position;
        }
    }
    public int length {
        get {
            return _length();
        }
    }

    private bool isAlive;
    private Coord _direction;
    private SnakeNode head;
    private SnakeNode tail;

    public Snake(Coord headPosition, int foodAmount, Coord direction) {
        this.isAlive = true;
        this._direction = direction;
        this.head = new SnakeNode(headPosition, null, null, foodAmount);
        this.tail = head;
        // make sure growth event is handled
        tail.onGrowth += handleGrowth;
    }

    public void eat(Pellet pellet) {
        tail.setFood(pellet.size);
        pellet.consume();
        onEat?.Invoke(this, EventArgs.Empty);
    }

    public bool move(Coord direction) {
        if (isBackwards(direction) == false) {
            this._direction = direction;
        }
        
        move();
        return true;
    }

    public void move() {
        // go in the facing direction
        head.moveTo(head.position + _direction);
        // fire event
        onMove?.Invoke(this, EventArgs.Empty);
    }

    public List<Coord> getNodePositions() {
        List<Coord> positions = new List<Coord>();

        SnakeNode cursor = head;
        while (cursor != null) {
            positions.Add(cursor.position);
            cursor = cursor.after;
        }

        return positions;
    }

    public List<Coord> getOldNodePositions() {
        List<Coord> positions = new List<Coord>();

        SnakeNode cursor = head;
        while (cursor != null) {
            positions.Add(cursor.oldPosition);
            cursor = cursor.after;
        }

        return positions;
    }

    private void handleGrowth(object sender, EventArgs empty) {
        // move the event listener
        SnakeNode snakeNode = (SnakeNode) sender;
        snakeNode.onGrowth -= handleGrowth;
        // update tail
        tail = snakeNode.after;
        tail.onGrowth += handleGrowth;
        // fire event
        onGrow?.Invoke(this, EventArgs.Empty);
    }

    private int _length() {
        int length = 0;
        
        SnakeNode cursor = head;
        while (cursor != null) {
            length++;
            cursor = cursor.after;
        }

        return length;
    }

    private bool isBackwards(Coord direction) {
        if (head.after == null) {
            return false;
        }
        if (headPosition + direction == head.after.position) {
            return true;
        }
        return false;
    }
} 