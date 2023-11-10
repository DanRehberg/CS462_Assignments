using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Book : MonoBehaviour
{
    private Vector3 returnedLocation;
    // Start is called before the first frame update
    void Start()
    {
        returnedLocation = this.transform.position;
    }

    public Vector3 getReturnPosition()
    {
        return returnedLocation;
    }

    public void returnBook()
    {
        this.transform.position = returnedLocation;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
