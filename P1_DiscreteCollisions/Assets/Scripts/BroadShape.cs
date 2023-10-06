using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BroadShape : MonoBehaviour
{
    //Original bounds are in local space (i.e., no translation)
    public Vector3 min, max;

    private Vector3[] currentBounds = new Vector3[2];
    private Vector3[] currentExtrema = new Vector3[8];
    private Vector3[] extrema = new Vector3[8];
    private Vector3 velocity = new Vector3(0.0f, 0.0f, 0.0f);
    private Vector3 angularVelocity = new Vector3(0.0f, 0.0f, 0.0f);
    private float priorMin, priorMax;
    private const float epsilonChange = 0.001f;

    // Start is called before the first frame update
    void Start()
    {
        buildExtrema();
    }

    private void buildExtrema()
    {
        //Build out the extrema from the min and max
        //  Written explicitly..
        extrema[0] = new Vector3(min.x, min.y, min.z);
        extrema[1] = new Vector3(min.x, max.y, min.z);
        extrema[2] = new Vector3(max.x, min.y, min.z);
        extrema[3] = new Vector3(max.x, max.y, min.z);
        extrema[4] = new Vector3(min.x, min.y, max.z);
        extrema[5] = new Vector3(max.x, max.y, max.z);
        extrema[6] = new Vector3(min.x, max.y, max.z);
        extrema[7] = new Vector3(max.x, min.y, max.z);
        priorMax = Vector3.Dot(max, max);
        priorMin = Vector3.Dot(min, min);
    }

    public Vector3[] getAABB()
    {
        return currentBounds;
    }

    public Vector3[] getAllVertices()
    {
        return currentExtrema;
    }

    public Vector3 getPosition()
    {
        return this.transform.position;
    }

    public Vector3 getVelocity()
    {
        return velocity;
    }

    public void setAngularVelocity(Vector3 w)
    {
        angularVelocity = w;
    }

    public void setVelocity(Vector3 v)
    {
        velocity = v;
    }

    // Update is called once per frame
    void Update()
    {
        if (CollisionTests.getIntegrate())
        {
            if (velocity != null && angularVelocity != null)
            {
                transform.position += Time.deltaTime * velocity;
                transform.rotation = Quaternion.Euler(Time.deltaTime * angularVelocity) * transform.rotation;
            }
        }

        //Check to see if the external extrema values were changed
        float curMin = Vector3.Dot(min, min);
        float curMax = Vector3.Dot(max, max);

        //Quick test in the event that public min/max values are changed in real-time
        //  Ignoring a few corner cases, but fine for demonstration
        if (Mathf.Abs(curMin - priorMin) >= epsilonChange || Mathf.Abs(curMax - priorMax) >= epsilonChange)
        {
            buildExtrema();
        }

        float minX = 99999.9f, minY = 99999.9f, minZ = 99999.9f,
            maxX = -99999.9f, maxY = -99999.9f, maxZ = -99999.9f;

        for (int i = 0; i < 8; ++i)
        {
            Vector3 rotated = this.transform.rotation * extrema[i];
            if (rotated.x < minX) minX = rotated.x;
            else if (rotated.x > maxX) maxX = rotated.x;
            if (rotated.y < minY) minY = rotated.y;
            else if (rotated.y > maxY) maxY = rotated.y;
            if (rotated.z < minZ) minZ = rotated.z;
            else if (rotated.z > maxZ) maxZ = rotated.z;
            currentExtrema[i] = rotated + this.transform.position;
        }
        currentBounds[0] = new Vector3(minX, minY, minZ) + this.transform.position;
        currentBounds[1] = new Vector3(maxX, maxY, maxZ) + this.transform.position;
    }
}
