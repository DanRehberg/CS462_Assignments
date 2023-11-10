using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Student : MonoBehaviour
{
    public float radius;

    private Book book = null;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public Book getBook()
    {
        Book temp = book;
        book = null;
        return temp;
    }

    public Vector3 getPosition()
    {
        return this.transform.position;
    }

    public float getRadius()
    {
        return radius;
    }

    public bool setBook(Book val)
    {
        if (book != null) return false;
        book = val;
        return true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        //Prevent student from going into the floor
        Vector3 pos = this.transform.position;
        pos.y = radius;
        this.transform.position = pos;
        if (book != null)
        {
            book.transform.position = this.transform.position +
                this.transform.forward * -radius;
        }
    }
}
