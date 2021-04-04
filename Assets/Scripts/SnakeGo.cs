using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeGo : MonoBehaviour {
    private static readonly float MIN_NODE_SCALE = 0.1f;
    private static readonly float FLOOR_OFFSET = 0.6f;
    private static readonly float EASE_AMOUNT = 0.6f;

    private float timeBetweenMoves = 10f;
    private float timeToNextMove = 10f;
    private float lerpPercent = 0f;

    [SerializeField]
    private AnimationCurve easeCurve;

    private static Vector3 reuseVector = new Vector3(0f, 0f, 0f);

    [SerializeField]
    private GameObject nodePrefab;
    [SerializeField]
    private GameObject headPrefab;
    [SerializeField]
    private GameObject segmentPrefab;
    private Snake snake;
    private GameObject headVisual = null;
    private List<GameObject> segmentVisuals = new List<GameObject>();
    private List<GameObject> nodeVisuals = new List<GameObject>();
    private Vector3 targetHeadLocation;
    private Vector3 lookTarget = Vector3.zero;
    private List<Vector3> targetNodeLocations = new List<Vector3>();
    private List<Coord> oldNodeLocations = new List<Coord>();


    public void setSnake(Snake snake) {
        this.snake = snake;
        initialize();
    }

    public void setTimeBetweenMoves(float time) {
        this.timeBetweenMoves = time;
        lerpPercent = easeCurve.Evaluate(1 - timeToNextMove / timeBetweenMoves);
    }

    public void setTimeToNextMove(float time) {
        this.timeToNextMove = time;
        lerpPercent = easeCurve.Evaluate(1- timeToNextMove / timeBetweenMoves);
    }

    void Update() {
        if(snake != null) {
            updateVisuals();
        }
    }

    void OnDrawGizmos() {
        Vector3 nextDirection = new Vector3(
            (snake.headPosition + snake.direction).x,
            FLOOR_OFFSET,
            (snake.headPosition + snake.direction).y
        );
        Gizmos.DrawWireSphere(nextDirection, 0.5f);
        Gizmos.DrawLine(targetNodeLocations[0], nextDirection);
        Gizmos.DrawWireSphere(targetNodeLocations[0], 0.5f);
    }

    private void updateVisuals() {
        addMissingVisuals();
        updateTargetPositions(snake.getNodePositions());
        oldNodeLocations = snake.getOldNodePositions();
        updateHead();
        updateBody();
    }

    private void initialize() {
        makeHead();
        updateVisuals();
    }

    private void addMissingVisuals() {
        List<Coord> nodePositions = snake.getNodePositions();
        if (nodePositions.Count - 1 > segmentVisuals.Count) {
            addSegmentVisuals(nodePositions);
        }
        if (nodePositions.Count > nodeVisuals.Count) {
            addNodeVisuals(nodePositions);
        }
    }

    private void addNodeVisuals(List<Coord> nodePositions) {
        int difference = nodePositions.Count - nodeVisuals.Count;
        for (int i = nodeVisuals.Count; i < nodePositions.Count; i++) {
            Vector3 position = new Vector3(nodePositions[i].x, FLOOR_OFFSET, nodePositions[i].y);
            GameObject nodeVisual = GameObject.Instantiate(
                nodePrefab, 
                position,
                Quaternion.identity, 
                gameObject.transform
            );
            nodeVisuals.Add(nodeVisual);
            targetNodeLocations.Add(position);
        }
        updateScale();
    }

    private void updateTargetPositions(List<Coord> nodePositions) {
        for (int i = 0; i < nodePositions.Count; i++) {
            Vector3 position = new Vector3(nodePositions[i].x, FLOOR_OFFSET, nodePositions[i].y);
            targetNodeLocations[i] = position;
        }
    }

    private void moveTowardsTarget() {
        for (int i = 0; i < nodeVisuals.Count; i++) {
            nodeVisuals[i].transform.position = Vector3.Lerp(
                nodeVisuals[i].transform.position,
                targetNodeLocations[i], 
                EASE_AMOUNT
            );
        }
    }

    private void updateHead() {
        Vector3 position = new Vector3(oldNodeLocations[0].x, FLOOR_OFFSET, oldNodeLocations[0].y);
        headVisual.transform.position = Vector3.Lerp(
            position,
            targetNodeLocations[0],
            lerpPercent
        );
        
        Vector3 nextDirection = new Vector3(
            (snake.headPosition + snake.direction).x,
            FLOOR_OFFSET,
            (snake.headPosition + snake.direction).y
        );
        lookTarget = Vector3.Lerp(lookTarget, nextDirection, 0.5f);
        headVisual.transform.LookAt(lookTarget);
    }

    private void updateBody() {
        smoothMoveNode();
        updateSegments();
    }

    private void updateScale() {
        // figure out how much scale decreases per node
        float deltaScale = (1.0f - MIN_NODE_SCALE) / nodeVisuals.Count;
        
        // scale the nodes
        for (int i = 0; i < nodeVisuals.Count; i++) {
            float currScale = 1.0f - (deltaScale * i);
            Vector3 vecScale = new Vector3(currScale, currScale, currScale);
            nodeVisuals[i].transform.localScale = vecScale;
        }

        // scale the segments
        for (int i = 0; i < segmentVisuals.Count; i++) {
            float currScale = 1.0f - deltaScale * (i + 1);
            Vector3 vecScale = new Vector3(currScale, currScale, segmentVisuals[i].transform.localScale.z);
            segmentVisuals[i].transform.localScale = vecScale;
        }
    }

    private void updateSegments() {
        for (int i = 0; i < segmentVisuals.Count; i++) {
            // move to node position
            segmentVisuals[i].transform.position = nodeVisuals[i].transform.position;
            // look at next node
            segmentVisuals[i].transform.LookAt(nodeVisuals[i + 1].transform.position);
            // scale segment to as long as the distance to the next node
            Vector3 vecScale = new Vector3(
                segmentVisuals[i].transform.localScale.x,
                segmentVisuals[i].transform.localScale.y,
                Vector3.Distance(nodeVisuals[i].transform.position, nodeVisuals[i + 1].transform.position)
            );
            segmentVisuals[i].transform.localScale = vecScale;
        }
    }

    private void makeHead() {
        Vector3 position = new Vector3(snake.headPosition.x, FLOOR_OFFSET, snake.headPosition.y);
        targetHeadLocation = position;
        headVisual = GameObject.Instantiate(
            headPrefab,
            position,
            Quaternion.identity,
            gameObject.transform
        );
    }

    private void addSegmentVisuals(List<Coord> nodePositions) {
        int segmentsNeeded = nodePositions.Count - segmentVisuals.Count - 1;
        for (int i = segmentVisuals.Count; i < nodePositions.Count - 1; i++) {
            GameObject bodyVisual = GameObject.Instantiate(
                segmentPrefab,
                nodeVisuals[i].transform.position,
                Quaternion.identity,
                gameObject.transform
            );

            segmentVisuals.Add(bodyVisual);
        }
        updateScale();
    }

    private void smoothMoveNode() {
        List<Coord> oldPositions = snake.getOldNodePositions();
        for (int i = 0; i < nodeVisuals.Count; i++) {
            Vector3 position = new Vector3(oldPositions[i].x, FLOOR_OFFSET, oldPositions[i].y);
            nodeVisuals[i].transform.position = Vector3.Lerp(
                position,
                targetNodeLocations[i],
                lerpPercent
            );
        }
    }
}
