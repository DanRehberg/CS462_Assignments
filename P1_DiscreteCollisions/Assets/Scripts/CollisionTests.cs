using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionTests : MonoBehaviour
{
    public bool motionOn = false;
    public GameObject boundary;

    private static bool integrate = false;
    private BroadShape[] shapes;
    private long frame = 0;

    private float sqrRadiusBounds;
    private Vector3 centerBounds;

    // Start is called before the first frame update
    void Start()
    {
        shapes = this.transform.GetComponentsInChildren<BroadShape>();
        //Setup the boundaries of the playspace
        sqrRadiusBounds = 0.5f * boundary.transform.localScale.x;
        sqrRadiusBounds = sqrRadiusBounds * sqrRadiusBounds;
        centerBounds = boundary.transform.position;
        //Generate random initial velocities
        foreach (BroadShape s in shapes)
        {
            changeVelocities(s);
        }
        //Setup test cases
        bool testing = collisionAABB(new Vector3(0f, 0f, 0f), new Vector3(5f, 1f, 1f), new Vector3(2f, -1f, -1f), new Vector3(6f, 2f, 2f));
        print("AABB Test 1: result is " + testing + " expected True");

        testing = collisionAABB(new Vector3(-9f, 0f, 0f), new Vector3(-5f, 1f, 1f), new Vector3(2f, -1f, -1f), new Vector3(6f, 2f, 2f));
        print("AABB Test 2: result is " + testing + " expected False");

        testing = collisionAABB(new Vector3(13f, -6f, -12f), new Vector3(15f, -4f, -2f), new Vector3(14f, 1f, -6f), new Vector3(6f, 2f, 2f));
        print("AABB Test 3: result is " + testing + " expected False");

        testing = collisionAABB(new Vector3(-9f, -4f, -4f), new Vector3(-5f, -1f, -1f), new Vector3(-7f, -3f, -3f), new Vector3(-4f, -2f, -2f));
        print("AABB Test 4: result is " + testing + " expected True");

        testing = collisionAABB(new Vector3(-9f, -4f, -4f), new Vector3(-5f, -1f, -1f), new Vector3(-7f, -3f, -30f), new Vector3(-4f, -2f, -20f));
        print("AABB Test 5: result is " + testing + " expected False");

        testing = collisionVertexSphere(new Vector3(0f, 0f, 0f), 5f * 5f, new Vector3(8f, 0f, 0f));
        print("Sphere Test 1: result is " + testing + " expected False");

        testing = collisionVertexSphere(new Vector3(0f, 0f, 0f), 5f * 5f, new Vector3(4f, 0f, 0f));
        print("Sphere Test 2: result is " + testing + " expected True");

        testing = collisionVertexSphere(new Vector3(10f, 2f, 50f), 8f * 8f, new Vector3(8f, 4f, 46f));
        print("Sphere Test 3: result is " + testing + " expected True");

        testing = collisionVertexSphere(new Vector3(-60f, -10f, -5f), 2f * 2f, new Vector3(-59f, -9f, -5f));
        print("Sphere Test 4: result is " + testing + " expected True");

        testing = collisionVertexSphere(new Vector3(-30f, -120f, -60f), 9f * 9f, new Vector3(18f, 20f, 19f));
        print("Sphere Test 5: result is " + testing + " expected False");
    }

    private void changeVelocities(BroadShape a)
    {
        //Same as below, but velocity vectors dictated by the direction from the center of the playspace bounds
        Vector3 dir = 2.0f * (centerBounds - a.getPosition()).normalized;
        a.setAngularVelocity(dir * 4f);
        a.setVelocity(dir);
    }

    private void changeVelocities(BroadShape a, BroadShape b)
    {
        //Emphasize a vector that separates the two cubes, but add a random direction to
        //  also provide some amount of variance in motion.
        //  For mechanics simulation, this would normally be applying an impulse between two bodies.
        Vector3 dir = 2.0f * (Random.insideUnitSphere + (a.getPosition() - b.getPosition()) * 2.0f).normalized;
        a.setAngularVelocity(dir * 4f);
        a.setVelocity(dir);
        b.setAngularVelocity(dir * -4f);
        b.setVelocity(-dir);
    }

    public static bool getIntegrate()
    {
        return integrate;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //TASK 1: Complete the collision test for a vertex and
    //  a sphere
    public bool collisionVertexSphere(Vector3 sphereCenter, float sqrRadius, Vector3 testVert)
    {
        // The sphere exists at sphereCenter
        // Its squared radius is sqrRadius (i.e., radius * radius, so we can avoid sqrt operations)
        // The vertex to test with the sphere is testVert
        // Expected behavior is to compare the squared distance from sphereCenter to testVert
        //      against the sqrRadius

        //Feel free to use a few local variables to assist in finishing the if statement's condition



        //if ()
        {
            return true;
        }

        return false;
    }

    //TASK 2: Finish the conditions for the Axis-Aligned Bounding Box
    public bool collisionAABB(Vector3 min0, Vector3 max0, Vector3 min1, Vector3 max1)
    {
        //Feel free to use a few local variables to assist in finishing the if statements' condition


        //if ()// Test one axis for intersection
        {
            //if ()// Test another axis for intersection
            {
                //if ()// Test the last axis for intersection
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void insidePlayspace(BroadShape s)
    {
        // Radius Test
        Vector3[] verts = s.getAllVertices();
        Vector3 vel = s.getVelocity();
        foreach (Vector3 v in verts)
        {
            if (collisionVertexSphere(centerBounds, sqrRadiusBounds, v) == false)
            {
                changeVelocities(s);
                break;// Do not need to test other vertices if one violation found
            }
        }
    }

    private void broadCollision()
    {
        //AABB Collisions
        for (int i = 0; i < shapes.Length; ++i)
        {
            Vector3[] aabb0 = shapes[i].getAABB();
            for (int j = 0; j < shapes.Length; ++j)
            {
                if (i <= j) continue;
                Vector3[] aabb1 = shapes[j].getAABB();
                //Finish testing axis-aligned separating axis thereom
                if (collisionAABB(aabb0[0], aabb0[1], aabb1[0], aabb1[1]))
                {
                    if (integrate)
                    {
                        changeVelocities(shapes[i], shapes[j]);
                    }
                    else print("Frame: " + frame + " objects: " + i + " and " + j + " are colliding.");
                }
            }
            //Regardless of collision with another cube,
            //  move this cube back into the playspace if outside
            //  of the spherical bounds.
            insidePlayspace(shapes[i]);
        }
    }

    private void LateUpdate()
    {
        //Syncing things..
        integrate = motionOn;
        
        broadCollision();

        frame += 1;
    }
}
