using System;
using UnityEngine;
#if UNITY_IOS
using UnityEngine.iOS;
#endif

public class GameHID : MonoBehaviour
{
    private float inverseScreenHalfWidth = 2.0f / Screen.width;
    private float inverseScreenHalfHeight = 2.0f / Screen.height;
    private float inverseHalfScreenDiagonal = (float) (2.0 / Math.Sqrt(Screen.width * Screen.width + Screen.height * Screen.height));

    private static GraphController graphControl;
    private static GameUI gameUI;
    private static GameCtrlHelper gameCtrlHelper;

    // Mouse interface
    private bool playerDraggingNode = false;

    public bool PlayerDraggingNode
    {
        get
        {
            return playerDraggingNode;
        }
        set
        {
            playerDraggingNode = value;
        }
    }

    // Rotation activation
    public bool OnRotation()
    {
        if (playerDraggingNode)
            return false;

#if UNITY_IOS
        bool value = (Input.touchCount > 0) && (Input.GetTouch(0).phase == TouchPhase.Moved);
#else
        bool value = Input.GetMouseButton(0);
#endif

        return value;
    }

    // Rotation along the Y-axis
    public float RotationX()
    {
#if UNITY_IOS
        float value = Input.GetTouch(0).deltaPosition.y * inverseScreenHalfWidth;
#else
        float value = Input.GetAxis("Mouse Y");
#endif

        return value;
    }

    // Rotation along the Y-axis
    public float RotationY()
    {
#if UNITY_IOS
        float value = Input.GetTouch(0).deltaPosition.x * inverseScreenHalfHeight;
#else
        float value = Input.GetAxis("Mouse X");
#endif

        return value;
    }

    // Move activation
    public bool OnZoom()
    {
        if (playerDraggingNode)
            return false;

#if UNITY_IOS
        bool value = (Input.touchCount > 1) && (Input.GetTouch(0).phase == TouchPhase.Moved) && (Input.GetTouch(1).phase == TouchPhase.Moved);
#else
        bool value = Input.GetMouseButton(1);
#endif

        return value;
    }

    // Move along the Y-axis
    public float ZoomScale()
    {
#if UNITY_IOS
        Vector2 curr = Input.GetTouch(0).position - Input.GetTouch(1).position;
        Vector2 prev = (Input.GetTouch(0).position - Input.GetTouch(0).deltaPosition) - (Input.GetTouch(1).position - Input.GetTouch(1).deltaPosition);
        float value = (curr.magnitude - prev.magnitude) * inverseHalfScreenDiagonal;
#else
        float value = Input.GetAxis("Mouse Y");
#endif

        return value;
    }

    // Rotation activation
    public Camera PlayerCamera()
    {
        return Camera.main;
    }

    public Vector3 MousePosition()
    {
#if UNITY_IOS
        return Input.GetTouch(0).position;
#else
        return Input.mousePosition;
#endif
    }

    void Awake()
    {
        graphControl = GetComponent<GraphController>();
        gameUI = GetComponent<GameUI>();
        gameCtrlHelper = GetComponent<GameCtrlHelper>();
    }
}