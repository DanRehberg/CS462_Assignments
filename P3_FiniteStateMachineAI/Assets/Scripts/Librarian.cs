using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Librarian : MonoBehaviour
{
    public TMPro.TextMeshProUGUI text;
    public float radius;
    public SpaceLayout environment;
    public Material calmMat;
    public Material fastMat;

    private int state = 0;
    private float selfTimer = 0.0f;
    private float[] timers;
    private Student[] students;
    private int studentID = -1;
    private Book book = null;
    private bool noBook = false;
    private bool returnedBook = false;

    private const float CHECKOUT_DURATION = 30.0f;
    private const float RESET_TIME = 5.0f;
    private bool setupOnce = false;

    private const float CALM_SPEED = 1f;
    private const float FAST_SPEED = 2f;

    private int booksCheckedOut = 0;
    private int booksReturned = 0;
    private int booksOverdue = 0;

    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Renderer>().material = calmMat;
    }

    public void setBookCount(int size)
    {
        if (!setupOnce)
        {
            setupOnce = true;
            timers = new float[size];
            students = new Student[size];
            for (int i = 0; i < size; ++i)
            {
                timers[i] = 0.0f;
                students[i] = null;
            }
        }
    }

    private void addStudent(Student cur)
    {
        for (int i = 0; i < timers.Length; ++i)
        {
            if (students[i] == null)
            {
                students[i] = cur;
                timers[i] = 0.0f;
                break;
            }
        }
    }

    private bool atDestination(Vector3 startPos, Vector3 endPos, Vector3 goalPos)
    {
        Vector3 traveled = endPos - startPos;
        Vector3 remaining = goalPos - endPos;
        float epsilon = 0.02f;
        if ((traveled.magnitude + radius) + epsilon >= remaining.magnitude)
        {
            return true;
        }
        return false;
    }

    private void removeStudent(Student cur)
    {
        for (int i = 0; i < timers.Length; ++i)
        {
            if (students[i] != null)
            {
                if (students[i].gameObject == cur.gameObject)
                {
                    students[i] = null;
                    timers[i] = 0.0f;
                    break;
                }
            }
        }
    }

    private Vector3 velocity(float speed, Vector3 goalPosition)
    {
        //TASK 1: 
        //  Generate a vector from the librarian (this object) to the goalPosition
        //  Normalize the vector and then scale it by the speed to produce the
        //      velocity to return

        Vector3 librarianPosition = this.transform.position;

        //Your code here, replace the zero vector returned with the
        //  computed velocity.


        return Vector3.zero;
    }

    private bool waitForTime(float max)
    {
        //TASK 2:
        //  Add the current elapsed time (Time.deltaTime) to selfTimer
        //  Test if selfTimer is greater than the wait time (max argument passed in function)
        //      If it is, reset selfTimer to 0.0f and return true.
        //      Else, return false.
        
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < timers.Length; ++i)
        {
            //Update timers only for books that have been checked out
            if (students[i] != null) timers[i] += Time.deltaTime;
        }
        //Using a switch statement as a basic finite state machine
        switch (state)
        {
            case 0://Waiting at counter state, if timer exceeds maximum then pursue student with the book
                {
                    string stats = "Books checked out: " + booksCheckedOut.ToString() +
                        "\nBooks returned: " + booksReturned.ToString() +
                        "\nBooks overdue: " + booksOverdue.ToString();
                    text.text = stats;
                    this.transform.position = environment.getCounterPosition() + this.transform.up * radius;
                    //Order of precendence is:
                    //  Timer exceeded
                    //  Student returning a book
                    //  Student wants a book
                    for (int i = 0; i < timers.Length; ++i)
                    {
                        if (timers[i] > CHECKOUT_DURATION)
                        {
                            this.GetComponent<Renderer>().material = fastMat;
                            studentID = i;
                            state = 7;
                            return;//One break statment will get us out of loop, but we want to skip checking out new books
                        }
                    }
                    Student student = environment.studentAtCounter();
                    if (student != null)
                    {
                        book = student.getBook();
                        if (book != null)
                        {
                            //Remove this student from the listing of checked out books
                            removeStudent(student);
                        }
                        state = 1;
                    }
                    break;
                }
            case 1://Go to the bookshelf
                {
                    text.text = "Moving to bookshelf";
                    Vector3 initialPosition = this.transform.position;
                    Vector3 v = velocity(CALM_SPEED, environment.getShelfPosition());
                    this.transform.position += Time.deltaTime * v;
                    if (atDestination(initialPosition, this.transform.position, environment.getShelfPosition()))
                    {
                        //Arrived at destination
                        this.transform.position = environment.getShelfPosition() + this.transform.up * radius;
                        state = 2;
                    }
                    break;
                }
            case 2://Arrived at bookshelf, look for book, or return a book
                {
                    //Options, and order of precedence
                    //If holding book, wait 5 seconds, return the book and walk back to the counter
                    //If book available, grab it and take it to the counter
                    //Else, walk to counter and announce no book available
                    if (book != null)
                    {
                        text.text = "Returning a book.";
                        if (waitForTime(RESET_TIME))
                        {
                            text.text = "";
                            book.returnBook();
                            booksReturned += 1;
                            book = null;
                            returnedBook = true;
                            state = 3;
                        }
                    }
                    else
                    {
                        text.text = "Finding a book.";
                        if (waitForTime(RESET_TIME))
                        {
                            text.text = "";
                            book = environment.getBook();
                            if (book == null)
                            {
                                noBook = true;
                            }
                            state = 3;
                        }
                    }
                    break;
                }
            case 3://Walk book back to counter
                {
                    Vector3 initialPosition = this.transform.position;
                    Vector3 v = velocity(CALM_SPEED, environment.getCounterPosition());
                    this.transform.position += Time.deltaTime * v;
                    if (atDestination(initialPosition, this.transform.position, environment.getCounterPosition()))
                    {
                        //Arrived at destination
                        this.transform.position = environment.getCounterPosition() + this.transform.up * radius;
                        if (noBook)
                        {
                            //Attempted to get a book for a student, but none available
                            noBook = false;
                            state = 5;
                        }
                        else if (returnedBook)
                        {
                            //Just returned a book, take a break
                            returnedBook = false;
                            state = 6;
                        }
                        else
                        {
                            //Found a book for a student
                            state = 4;
                        }
                    }
                    break;
                }
            case 4://Attempt to give book to student, if just returned a book, then go to reset period
                {
                    //If student not here, enter book return process
                    Student student = environment.studentAtCounter();
                    if (student == null)
                    {
                        if (book != null)
                        {
                            booksReturned -= 1;
                            state = 1;//Return the book
                        }
                    }
                    else
                    {
                        //Checkout the book to the student, then take a break
                        addStudent(student);
                        if (!student.setBook(book))
                        {
                            //Student already has a book, so return the book
                            booksReturned -= 1;
                            state = 1;
                            break;
                        }
                        book = null;
                        booksCheckedOut += 1;
                        state = 6;
                    }
                    break;
                }
            case 5://Walk back to counter to say no book available
                {
                    //If student not present, skip saying anything and go to cool down state
                    //else, talk for 5 seconds or until student leaves
                    text.text = "Sorry, all books have been checked out.";
                    if (environment.studentAtCounter() == null || waitForTime(RESET_TIME))
                    {
                        text.text = "";
                        state = 6;
                    }
                    break;
                }
            case 6://Cool down state, let's students leave if they just received a book
                {
                    text.text = "Taking a break.";
                    if (waitForTime(RESET_TIME))
                    {
                        text.text = "";
                        state = 0;
                    }
                    break;
                }
            case 7://Hunt down an overdue book
                {
                    text.text = "OVERDUE BOOK!!!!!";
                    Vector3 initialPosition = this.transform.position;
                    Vector3 v = velocity(FAST_SPEED, students[studentID].getPosition());
                    this.transform.position += Time.deltaTime * v;
                    if (atDestination(initialPosition, this.transform.position, students[studentID].getPosition()))
                    {
                        this.GetComponent<Renderer>().material = calmMat;
                        text.text = "";
                        book = students[studentID].getBook();
                        removeStudent(students[studentID]);
                        state = 1;
                        booksOverdue += 1;
                        studentID = -1;
                    }
                    break;
                }

        }//End switch statement
    }

    private void LateUpdate()
    {
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
