using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using oti.AI;

public class GraphController : MonoBehaviour
{

    [SerializeField]
    private static bool verbose = false;

    private static GameController gameControl;
    private static GameUI gameUI;
    private static GameCtrlHelper gameCtrlHelper;

    [SerializeField]
    private bool allStatic = false;
    [SerializeField]
    private bool paintMode = false;
    [SerializeField]
    private bool repulseActive = true;

    [SerializeField]
    public GameObject nodePrefab; 
    [SerializeField] 
    public GameObject linkPrefab;

    [SerializeField]
    private float nodeVectorGenRange = 50F;

    [SerializeField]
    private float globalGravityPhysX = 10f;
    [SerializeField]
    private float repulseForceStrength = 0.1f;
    [SerializeField]
    private float nodePhysXForceSphereRadius = 50F;
    [SerializeField]
    private float linkForceStrength = 6F;
    [SerializeField]
    private float linkIntendedLinkLength = 5F;

    private static int nodeCount;
    private static int linkCount;

    private static float nodePhysXForceSphereRadiusSquare;

    // Wolfram Model
    private GameObject wolframModel;
    private Transform wolframModelTransform;
    private WorldMonitors worldMonitors;

    public GameObject Model
    {
        get
        {
            return wolframModel;
        }
    }

    public bool AllStatic
    {
        get
        {
            return allStatic;
        }
        set
        {
            allStatic = value;
        }
    }

    public bool PaintMode
    {
        get
        {
            return paintMode;
        }
        set
        {
            paintMode = value;
        }
    }

    public bool RepulseActive
    {
        get
        {
            return repulseActive;
        }
        set
        {
            repulseActive = value;
        }
    }

    public float GlobalGravityPhysX
    {
        get
        {
            return globalGravityPhysX;
        }
        set
        {
            globalGravityPhysX = value;
        }
    }

    public float RepulseForceStrength
    {
        get
        {
            return repulseForceStrength;
        }
        private set
        {
            repulseForceStrength = value;
        }
    }

    public float NodePhysXForceSphereRadius
    {
        get
        {
            return nodePhysXForceSphereRadius;
        }
        set
        {
            nodePhysXForceSphereRadius = value;
        }
    }

    public float NodePhysXForceSphereRadiusSquare
    {
        get
        {
            return nodePhysXForceSphereRadiusSquare;
        }
    }

    public float LinkForceStrength
    {
        get
        {
            return linkForceStrength;
        }
        private set
        {
            linkForceStrength = value;
        }
    }

    public float LinkIntendedLinkLength
    {
        get
        {
            return linkIntendedLinkLength;
        }
        set
        {
            linkIntendedLinkLength = value;
        }
    }

    public int NodeCount
    {
        get
        {
            return nodeCount;
        }
        set
        {
            nodeCount = value;
        }
    }

    public int LinkCount
    {
        get
        {
            return linkCount;
        }
        set
        {
            linkCount = value;
        }
    }

    public void ResetWorld()
    {
        foreach (GameObject destroyTarget in GameObject.FindGameObjectsWithTag("link"))
        {
            Destroy(destroyTarget);
            LinkCount -= 1;
            // gameUI.PanelStatusLinkCountTxt.text = "Linkcount: " + LinkCount;
        }

        foreach (GameObject destroyTarget in GameObject.FindGameObjectsWithTag("node"))
        {
            Destroy(destroyTarget);
            NodeCount -= 1;
            // gameUI.PanelStatusNodeCountTxt.text = "Nodecount: " + NodeCount;
        }

        foreach (GameObject destroyTarget in GameObject.FindGameObjectsWithTag("debug"))
        {
            Destroy(destroyTarget);
        }

    }

    private GameObject InstObj(Vector3 createPos)
    {

        GameObject go = Instantiate(nodePrefab, createPos, Quaternion.identity, wolframModelTransform) as GameObject;
        if (WorldMonitor.Instance.FreeSpace)
        {
            WorldMonitor.Instance.InsertNewTrackedObject(go, worldMonitors, "Agent_" + nodeCount.ToString(), nodePhysXForceSphereRadius);
        }
        return go;
    }

    public GameObject GenerateNode()
    {
        // Method for creating a Node on random coordinates, e.g. when spawning multiple new nodes

        GameObject nodeCreated = null;

        Vector3 createPos = new Vector3(UnityEngine.Random.Range(0, nodeVectorGenRange), UnityEngine.Random.Range(0, nodeVectorGenRange), UnityEngine.Random.Range(0, nodeVectorGenRange));

        nodeCreated = InstObj(createPos);

        if (nodeCreated != null)
        {
            nodeCreated.name = "node_" + nodeCount;
            nodeCount++;
            // gameUI.PanelStatusNodeCountTxt.text = "Nodecount: " + NodeCount;

            if (verbose)
                Debug.Log(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + ": Node created: " + nodeCreated.gameObject.name);

        }
        else
        {
            if (verbose)
                Debug.Log(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + ": Something went wrong, did not get a Node Object returned.");
        }

        return nodeCreated.gameObject;
    }

    public GameObject GenerateNode(Vector3 createPos)
    {
        // Method for creating a Node on specific coordinates, e.g. in Paintmode when a node is created at the end of a paintedLink

        GameObject nodeCreated = null;

        //nodeCreated = Instantiate(nodePrefabBullet, createPos, Quaternion.identity) as Node;
        nodeCreated = InstObj(createPos);

        if (nodeCreated != null)
        {
            nodeCreated.name = "node_" + nodeCount;
            nodeCount++;
            // gameUI.PanelStatusNodeCountTxt.text = "Nodecount: " + NodeCount;

            if (verbose)
                Debug.Log(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + ": Node created: " + nodeCreated.gameObject.name);
        }
        else
        {
            if (verbose)
                Debug.Log(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + ": Something went wrong, did not get a Node Object returned.");
        }

        return nodeCreated.gameObject;
    }

    public GameObject GenerateNode(string id, string type)
    {
        // Method for creating a Node on random coordinates, but with defined labels. E.g. when loaded from a file which contains these label.

        GameObject nodeCreated = null;

        Vector3 createPos = new Vector3(UnityEngine.Random.Range(0, nodeVectorGenRange), UnityEngine.Random.Range(0, nodeVectorGenRange), UnityEngine.Random.Range(80.0f, 80.0f + nodeVectorGenRange));

        //nodeCreated = Instantiate(nodePrefabBullet, createPos, Quaternion.identity) as Node;
        nodeCreated = InstObj(createPos);

        if (nodeCreated != null)
        {
            Node nodeNode = nodeCreated.GetComponent<Node>();
            nodeNode.name = id;
            nodeNode.Text = id;
            nodeNode.Type = type;

            nodeCount++;

            if (verbose)
                Debug.Log(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + ": Node created: " + nodeCreated.gameObject.name);
        }
        else
        {
            if (verbose)
                Debug.Log(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + ": Something went wrong, no node created.");
        }

        return nodeCreated.gameObject;
    }

    public bool CreateLink(GameObject source, GameObject target)
    {
        if (source == null || target == null)
        {
            if (verbose)
            {
                Debug.Log(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + ": source or target does not exist. Link not created.");
            }
            return false;
        }
        else
        {
            if (source != target)
            {
                bool alreadyExists = false;
                foreach (GameObject checkObj in GameObject.FindGameObjectsWithTag("link"))
                {
                    Link checkLink = checkObj.GetComponent<Link>();
                    if (checkLink.source == source && checkLink.target == target)
                    {
                        alreadyExists = true;
                        break;
                    }
                }

                if (!alreadyExists)
                {
                    GameObject linkObject = Instantiate(linkPrefab, new Vector3(0, 0, 0), Quaternion.identity, wolframModelTransform) as GameObject;
                    Link link = linkObject.GetComponent<Link>();
                    linkObject.name = "link_" + linkCount;
                    link.source = source;
                    link.target = target;
                    linkCount++;

                    return true;
                }
                else
                {
                    if (verbose)
                    {
                        Debug.Log(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + ": Link between source " + source.name + " and target " + target.name + " already exists. Link not created.");
                    }
                    return false;
                }
            }
            else
            {
                if (verbose)
                {
                    Debug.Log(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + ": source " + source.name + " and target " + target.name + " are the same. Link not created.");
                }
                return false;
            }
        }
    }

    public void GenerateLink(string mode)
    {
        if (mode == "random")
        {
            bool success = false;
            int tryCounter = 0;
            int tryLimit = nodeCount * 5;

            while (!success && tryCounter < tryLimit)
            {
                tryCounter++;

                int sourceRnd = UnityEngine.Random.Range(0, nodeCount);
                int targetRnd = UnityEngine.Random.Range(0, nodeCount);

                GameObject source = GameObject.Find("node_" + sourceRnd);
                GameObject target = GameObject.Find("node_" + targetRnd);

                success = CreateLink(source, target);
            }
            if (!success)
                if (verbose)
                    Debug.Log(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + ": Too many unsuccessful tries, limit reached. Bailing out of GenerateLink run with mode=random. TryCounter: " + tryCounter + " Limit: " + nodeCount * 5);
        }
    }

    public void GenerateLink(string mode, GameObject source, GameObject target)
    {
        if (mode == "specific_src_tgt")
        {
            bool success = false;

            success = CreateLink(source, target);

            if (!success)
                if (verbose)
                    Debug.Log(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + ": Problem with creating link. Link not created.");
        }
    }

    public void GenNodes(int count)
    {
        for (int i = 0; i < count; i++)
        {
            // Create a node on random Coordinates
            GenerateNode();
        }
    }

    public void GenLinks(int count)
    {
        for (int i = 0; i < count; i++)
        {
            // Create a link on random Coordinates
            GenerateLink("random");
        }
    }

    private void entererListener(GameObject trackedObject, GameObject[] neighborObjects, string[] conflictingTypes)
    {
        trackedObject.GetComponent<Node>().NeighborhoodNodes = neighborObjects;
    }

    private void leaverListener(GameObject trackedObject, GameObject[] neighborObjects, string[] conflictingTypes)
    {

    }

    private void endListener(GameObject trackedObject, GameObject[] neighborObjects, string[] conflictingTypes)
    {
        trackedObject.GetComponent<Node>().NeighborhoodNodes = Array.Empty<GameObject>();
    }

    public Vector3 ModelCentroid()
    {
        Vector3 centroid = new Vector3(0, 0, 0);

        foreach (Transform child in wolframModelTransform)
        {
            centroid += child.position;
        }
        return (centroid / wolframModelTransform.childCount);
    }

    void Awake()
    {
        gameControl = GetComponent<GameController>();
        gameUI = GetComponent<GameUI>();
        gameCtrlHelper = GetComponent<GameCtrlHelper>();

        wolframModel = GameObject.Find("Wolfram Model");
        wolframModelTransform = wolframModel.GetComponent<Transform>();

        nodeCount = 0;
        linkCount = 0;

        RepulseForceStrength = 5f;
        GlobalGravityPhysX = 10f;
        // NodePhysXForceSphereRadius = 35f;
        nodePhysXForceSphereRadius = 250f;
        LinkForceStrength = 5f;
        LinkIntendedLinkLength = 3f;

        // Subscribe to delegate for tracked object information
        worldMonitors = wolframModel.GetComponent<WorldMonitors>();
        worldMonitors.ConflictEnterers += entererListener;
        worldMonitors.ConflictLeavers += leaverListener;
        worldMonitors.ConflictEnd += endListener;
    }

    void Start()
    {
        // Node
        nodePhysXForceSphereRadiusSquare = nodePhysXForceSphereRadius * nodePhysXForceSphereRadius;

        // Link
        Link.intendedLinkLength = linkIntendedLinkLength;
        Link.forceStrength = linkForceStrength;
    }

    private void OnDestroy()
    {
        worldMonitors.ConflictEnterers -= entererListener;
        worldMonitors.ConflictLeavers -= leaverListener;
        worldMonitors.ConflictEnd -= endListener;
    }

}
