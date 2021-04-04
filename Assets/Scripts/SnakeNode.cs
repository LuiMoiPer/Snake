using System;

public class SnakeNode {

    public event EventHandler onGrowth;

    public Coord position {
        get {
            return _position;
        }
    }
    public Coord oldPosition {
        get {
            return _oldPosition;
        }
    }
    public SnakeNode before{
        get {
            return _before;
        }
    }
    public SnakeNode after{
        get {
            return _after;
        }
    }
    public bool isHead{
        get {
            return _isHead;
        }
    }
    public bool isTail{
        get {
            return _isTail;
        }
    }
    public int foodAmount{
        get {
            return _foodAmount;
        }
    }

    private Coord _position;
    private Coord _oldPosition;
    private SnakeNode _before;
    private SnakeNode _after;
    private bool _isHead;
    private bool _isTail;
    private int _foodAmount;

    public SnakeNode(Coord position, SnakeNode before, SnakeNode after, int foodAmount) {
        this._position = position;
        this._before = before;
        this._after = after;
        this._foodAmount = foodAmount;
        this._oldPosition = position;
        updateIsHead();
        updateIsTail();
    }

    public void moveTo(Coord position) {
        _oldPosition = this.position;
        this._position = position;
        if (this.isTail == false) {
            this.after.moveTo(_oldPosition);
        }
        grow();
    }

    public void setBefore(SnakeNode node) {
        this._before = node;
        updateIsHead();
    }

    public void setAfter(SnakeNode node) {
        this._after = node;
        updateIsTail();
    }

    public void setFood(int foodAmount) {
        this._foodAmount = foodAmount;
    }

    private void grow() {
        if (this.isTail == false && foodAmount > 0) {
            after.setFood(foodAmount);
        }
        else if (this.isTail && foodAmount > 0) {
            // make new node
            this._after = new SnakeNode(_oldPosition, this, null, foodAmount - 1);
            this._isTail = false;

            onGrowth?.Invoke(this, EventArgs.Empty);
        }
        this._foodAmount = 0;
    }

    private void updateIsHead() {
        if (before == null) {
            this._isHead = true;
        }
        else {
            this._isHead = false;
        }
    }

    private void updateIsTail() {
        if (after == null) {
            this._isTail = true;
        }
        else {
            this._isTail = false;
        }
    }
}