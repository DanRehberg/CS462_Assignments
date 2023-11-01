using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColTester : MonoBehaviour
{
    static Vector3 minA = new Vector3(0f,0f,0f);
    static Vector3 maxA = new Vector3(5f,5f,5f);
    static Vector3 minB = new Vector3(1f,1f,1f);
    static Vector3 maxB = new Vector3(9f,9f,9f);
    static Vector3 minC = new Vector3(6f,6f,6f);
    static Vector3 maxC = new Vector3(8f,8f,8f);

    public static bool solution0 = true;//a and b box test
    public static bool solution1 = true;//c and b box test
    public static bool solution2 = false;//a and c box test

    static Vector3 sphere = new Vector3(10.0f, 0.0f, 0.0f);
    static float radA = 64.0f;
    static float radB = 8.0f;
    static Vector3 testPoint = new Vector3(17.0f, 0.0f, 0.0f);

    public static bool solution3 = true;//sphere with 64 sqr radius
    public static bool solution4 = false;//sphere with 8 sqr radius

    public static bool[] result0;
    public static bool[] result1;
    public static bool[] result2;
    public static bool[] result3;
    public static bool[] result4;
    public static bool[] errThrown;


    public static void runTests(int size)
    {
        result0 = new bool[size];
        result1 = new bool[size];
        result2 = new bool[size];
        result3 = new bool[size];
        result4 = new bool[size];
        errThrown = new bool[size];
        //Provide incorrect results by default
        //  as bad answers are flags for manual review
        for (int i = 0; i < size; ++i)
        {
            result0[i] = !solution0;
            result1[i] = !solution1;
            result2[i] = !solution2;
            result3[i] = !solution3;
            result4[i] = !solution4;
            errThrown[i] = false;
        }
        //DO NOT DELETE THE COMMENT BELOW!!!
        //  THIS COMMENT IS USED TO WRITE NEW CODE IN THIS FILE AFTER GOING THROUGH SUBMISSION FILES!!!

        
		
		//New Line



    }

}
