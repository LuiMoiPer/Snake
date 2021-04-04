using System;
using UnityEngine;

public class Transitions : MonoBehaviour {
    // Start is called before the first frame update
    public event EventHandler onScreenCovered;
    public event EventHandler onScreenRevealed;

    private static float WIPE_TIME = 2f;
    private static bool setup = false;
    
    [SerializeField]
    private RectTransform wiper;
    [SerializeField]
    private AnimationCurve ease;
    private Vector2 start;
    private Vector2 end;
    private bool wiping = false;
    private bool entering = true;
    private float timeSinceStart;
    private float lerpPercent;
    
    void Awake() {
            DontDestroyOnLoad(gameObject);
            start = wiper.anchoredPosition;
            end = start * -1;
    }

    // Update is called once per frame
    void Update() {
        if (wiping) {
            updateState();
            moveWiper();
        }
    }

    public void Wipe() {
        Debug.Log("Wiping");
        wiping = true;
        entering = true;
        timeSinceStart = 0f;
        wiper.anchoredPosition = start;
    }

    private void updateState() {
        timeSinceStart += Time.deltaTime;
        lerpPercent = timeSinceStart / WIPE_TIME;
        // Finished transitioning to black
        if (lerpPercent > 0.5f && entering) {
            onScreenCovered?.Invoke(this, EventArgs.Empty);
            entering = false;            
        }
        else if (lerpPercent > 1f) {
            onScreenRevealed?.Invoke(this, EventArgs.Empty);
            wiping = false;
        }
    }

    private void moveWiper() {
        wiper.anchoredPosition = Vector2.Lerp(
            start,
            end,
            ease.Evaluate(lerpPercent)
        );
    }
}
