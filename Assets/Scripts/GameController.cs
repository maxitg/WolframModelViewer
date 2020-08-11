using UnityEngine;

[RequireComponent(typeof(GameUI))]
[RequireComponent(typeof(GameHID))]
[RequireComponent(typeof(GameCtrlHelper))]
[RequireComponent(typeof(GameInputReader))]
[RequireComponent(typeof(GraphController))]

public class GameController : MonoBehaviour 
{
    // Set Fixed Timestep to 10 fps for performance and smooth animation
    [SerializeField]
    private float timeStep = 0.1f;

    // Model action speed
    public float actionSpeed = 30.0f;

    private static GameHID gameHID;
    private static GraphController graphControl;
    private static GameInputReader gameInputReader;

    // smallest distance
    private float epsilon = 10.0f;

    void Awake()
    {
        // Base components
        gameHID = GetComponent<GameHID>();
        graphControl = GetComponent<GraphController>();
        gameInputReader = GetComponent<GameInputReader>();

        // Set Fixed Timestep 
        Time.fixedDeltaTime = timeStep;
    }

    void Start() 
    {
        gameInputReader.LoadWolframModel("graph", GameInputReader.SourceType.LOCAL_GRAPHML);
    }

    void Update()
    {
        // Rotate the model around its centroid.
        if (gameHID.OnRotation())
        {
			Vector3 centroid = graphControl.ModelCentroid();
            graphControl.Model.transform.RotateAround(centroid, Vector3.right, gameHID.RotationX() * actionSpeed);
			graphControl.Model.transform.RotateAround(centroid, Vector3.down, gameHID.RotationY() * actionSpeed);
        }
    }

    void LateUpdate()
    {
        // Zoom -- move the camera toward the model.
        if (gameHID.OnZoom())
        {           
            Vector3 centroid = graphControl.ModelCentroid();
            if (Vector3.Distance(gameHID.PlayerCamera().transform.localPosition, centroid) > epsilon) 
            {
                Vector3 diff = gameHID.PlayerCamera().transform.position + graphControl.Model.transform.position - centroid;
                graphControl.Model.transform.position = Vector3.MoveTowards(graphControl.Model.transform.position, diff, gameHID.ZoomScale() * actionSpeed);
                // gameHID.PlayerCamera().transform.position = Vector3.MoveTowards(gameHID.PlayerCamera().transform.position, centroid, gameHID.MovingY() * actionSpeed);
            } 
        }
    }

}
