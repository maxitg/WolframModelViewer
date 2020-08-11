using UnityEngine;
using System.Collections;

public class Link : MonoBehaviour
{
    public GameObject source;
    public GameObject target;

    private Component sourceRb;
    private Component targetRb;

    public static float intendedLinkLength;
    public static float forceStrength;

    private static GameController gameControl;
    private static GraphController graphControl;

    private float intendedLinkLengthSqr;
    private float distSqrNorm;


    void doAttraction()
    {
        Vector3 forceDirection = sourceRb.transform.position - targetRb.transform.position;
        float distSqr = forceDirection.sqrMagnitude;

        if (distSqr > intendedLinkLengthSqr)
        {
            //Debug.Log("(Link.FixedUpdate) distSqr: " + distSqr + "/ intendedLinkLengthSqr: " + intendedLinkLengthSqr + " = distSqrNorm: " + distSqrNorm);
            distSqrNorm = distSqr / intendedLinkLengthSqr;

            Vector3 targetRbImpulse = forceDirection.normalized * forceStrength * distSqrNorm;
            Vector3 sourceRbImpulse = forceDirection.normalized * -1 * forceStrength * distSqrNorm;

            //Debug.Log("(Link.FixedUpdate) targetRb: " + targetRb + ". forceDirection.normalized: " + forceDirection.normalized + ". distSqrNorm: " + distSqrNorm + ". Applying Impulse: " + targetRbImpulse);
            ((Rigidbody)targetRb as Rigidbody).AddForce(targetRbImpulse);
            //Debug.Log("(Link.FixedUpdate) targetRb: " + sourceRb + ". forceDirection.normalized: " + forceDirection.normalized + "  * -1 * distSqrNorm: " + distSqrNorm + ". Applying Impulse: " + sourceRbImpulse);
            ((Rigidbody)sourceRb as Rigidbody).AddForce(sourceRbImpulse);

        }
    }

    // Use this for initialization
    void Start()
    {
        gameControl = FindObjectOfType<GameController>();
        graphControl = FindObjectOfType<GraphController>();

        sourceRb = source.GetComponent<Rigidbody>();
        targetRb = target.GetComponent<Rigidbody>();

        intendedLinkLengthSqr = intendedLinkLength * intendedLinkLength;
    }


    // Update is called once per frame
    void Update()
    {
        // moved from Start() in Update(), otherwise it won't see runtime updates of intendedLinkLength
        intendedLinkLengthSqr = intendedLinkLength * intendedLinkLength;

        Vector3 p = source.transform.position;
        Vector3 q = target.transform.position;

        Vector3 pos = Vector3.Lerp(p, q, (float)0.5);
        double halftheight = Vector3.Distance(p, q) / 2.0;
        transform.localScale = new Vector3(transform.localScale.x, (float)halftheight, transform.localScale.z);
        transform.position = pos;
        transform.up = q - p;
    }

    void FixedUpdate()
    {
        if (!graphControl.AllStatic)
            doAttraction();
    }
}
