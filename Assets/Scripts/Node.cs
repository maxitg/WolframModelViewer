using UnityEngine;
using System;
using System.Collections;

public class Node : MonoBehaviour
{
    private static GraphController graphControl;
    private static GameHID gameHID;

    private GameObject[] neighborhoodNodes;
    private Rigidbody thisRigidbody;

    private string id;
    private string text;
    private string type;

    [SerializeField]
    protected static bool verbose = true;

    // Drag node
    private float mZCoord;
    private Vector3 mOffset;

    public GameObject[] NeighborhoodNodes
    {
        get
        {
            return neighborhoodNodes;
        }
        set
        {
            neighborhoodNodes = value;
        }
    }

    public string Id
    {
        get
        {
            return id;
        }
        set
        {
            id = value;
        }
    }

    public string Text
    {
        get
        {
            return text;
        }
        set
        {
            text = value;
        }
    }

    public string Type
    {
        get
        {
            return type;
        }
        set
        {
            type = value;
        }
    }

    // 
    //
    //
    //

    void Start()
    {
        graphControl = FindObjectOfType<GraphController>();
        gameHID = FindObjectOfType<GameHID>();

        thisRigidbody = this.GetComponent<Rigidbody>();
        neighborhoodNodes = Array.Empty<GameObject>();

        // freeze node rotation
        thisRigidbody.freezeRotation = true;
    }

    // 
    //
    //
    //

    void FixedUpdate()
    {
        if (!graphControl.AllStatic && graphControl.RepulseActive)
            doRepulse();

        if (!graphControl.AllStatic)
            doGravity();
    }

    private void doGravity()
    {
        // Apply global gravity pulling node towards center of universe
        Vector3 dirToCenter = -this.transform.position;
        Vector3 impulse = dirToCenter.normalized * thisRigidbody.mass * graphControl.GlobalGravityPhysX;
        thisRigidbody.AddForce(impulse);
    }

    private void doRepulse()
    {
        // only apply force to nodes within forceSphere, with Falloff towards the boundary of the Sphere and no force if outside Sphere.
        foreach (GameObject go in neighborhoodNodes)
        {
            Collider hitCollider = go.GetComponent<Node>().GetComponent<Collider>();
            Rigidbody hitRb = hitCollider.attachedRigidbody;

            if (hitRb != null && hitRb != thisRigidbody)
            {
                Vector3 forceDirection = hitCollider.transform.position - this.transform.position;
                float distSqr = forceDirection.sqrMagnitude;

                // Normalize the distance from forceSphere Center to node into 0..1
                float impulseExpoFalloffByDist = Mathf.Clamp(1 - (distSqr / graphControl.NodePhysXForceSphereRadiusSquare), 0, 1);

                Vector3 nodeRbImpulse = forceDirection.normalized * graphControl.RepulseForceStrength * impulseExpoFalloffByDist;
                hitRb.AddForce(nodeRbImpulse);
            }
        }
    }

    // Drag node
    //
    //
    //

    void OnMouseDown()
    {
        //
        gameHID.PlayerDraggingNode = true;

        //
        mZCoord = gameHID.PlayerCamera().WorldToScreenPoint(gameObject.transform.position).z;

        // 
        mOffset = gameObject.transform.position - GetMouseWorldPosition();
    }

    void OnMouseDrag()
    {
        transform.position = GetMouseWorldPosition() + mOffset;
    }

    void OnMouseUp()
    {
        gameHID.PlayerDraggingNode = false;
    }

    private Vector3 GetMouseWorldPosition()
    {
        // pixel coordinates (x, y)
        Vector3 mousePoint = gameHID.MousePosition();

        // z-coordinate of game object on screen
        mousePoint.z = mZCoord;

        return gameHID.PlayerCamera().ScreenToWorldPoint(mousePoint);
    }

}