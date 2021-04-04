using System;

public class Pellet {

    public event EventHandler onConsumed;

    public Coord position {
        get {
            return _position;
        }
    }
    public int size {
        get {
            return _size;
        }
    }

    private Coord _position;
    private int _size;
    
    public Pellet(Coord position, int size) {
        this._position = position;
        this._size = size;
    }

    public void consume() {
        onConsumed?.Invoke(this, EventArgs.Empty);
    }
}