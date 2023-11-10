using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceLayout : MonoBehaviour
{
    //Environment information
    public GameObject bookshelf;
    public GameObject counter;
    public GameObject queue;
    public Book[] books;
    public Student[] students;
    public Librarian librarian;

    //Radius of desitnations
    public float radius;

    private Student counterStudent = null;

    private const float epsilon = 0.05f;

    // Start is called before the first frame update
    void Start()
    {
        librarian.setBookCount(books.Length);
    }

    public Book getBook()
    {
        //Find an available book by its return location
        foreach (Book b in books)
        {
            Vector3 separation = b.transform.position - b.getReturnPosition();
            if (Vector3.Dot(separation, separation) <= epsilon)
            {
                //Return the first found book
                return b;
            }
        }
        //No books available
        return null;
    }

    public Vector3 getCounterPosition()
    {
        return counter.transform.position;
    }

    public float getFloorRadius()
    {
        return radius;
    }

    public Vector3 getShelfPosition()
    {
        return bookshelf.transform.position;
    }

    public Student studentAtCounter()
    {
        return counterStudent;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static bool atCounter(Vector3 a, Vector3 b, float radA, float radB)
    {
        Vector3 test = a - b;
        float radSum = (radA + radB);
        if (Vector3.Dot(test, test) < (radSum * radSum))
        {
            return true;
        }
        return false;
    }

    private void LateUpdate()
    {
        counterStudent = null;
        foreach (Student s in students)
        {
            if (atCounter(getCounterPosition(), s.getPosition(), radius, s.getRadius()))
                counterStudent = s;
        }
    }
}
