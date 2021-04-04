using System;
using UnityEngine;

public class PelletGo : MonoBehaviour {

    private static float ROTATION_SPEED = 30f;
    private static float DESTRUCTION_DELAY = 5f;
    private static Vector3 reuseVector = new Vector3(0f, 0f, 0f);

    [SerializeField]
    private GameObject visual;
    private Pellet model;
    private Vector3 targetSize;
    private Vector3 rotationVector = Vector3.zero;

    void Start() {
        visual.transform.localScale = Vector3.zero;
        rotationVector = UnityEngine.Random.onUnitSphere * ROTATION_SPEED;
        targetSize = Vector3.one;
    }

    void Update() {
        updateSize();
        updateRotation();
    }
    
    public void setPellet(Pellet model) {
        this.model = model;
        model.onConsumed += handleConsume;
        rotationVector = UnityEngine.Random.onUnitSphere * ROTATION_SPEED;
        visual.transform.localScale = Vector3.zero;
        updatePosition();
    }

    public void handleConsume(object sender, EventArgs args) {
        model.onConsumed -= handleConsume;
        targetSize = Vector3.zero;
        Destroy(gameObject, DESTRUCTION_DELAY);
    }

    private void updatePosition() {
        reuseVector.x = model.position.x;
        reuseVector.z = model.position.y;
        gameObject.transform.position = reuseVector;
    }

    private void updateSize() {
        visual.transform.localScale = Vector3.Lerp(visual.transform.localScale, targetSize, 0.1f);
    }

    private void updateRotation() {
        visual.transform.Rotate(rotationVector * Time.deltaTime);
    }
}