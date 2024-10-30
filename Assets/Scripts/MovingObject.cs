using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public abstract class MovingObject : MonoBehaviour
{

    public float moveTime = 0.1f;
    public LayerMask blockingLayer;
    

    private BoxCollider2D boxCollider;
    private Rigidbody2D rb2D;
    private bool isMoving;
    private float inverseMoveTime;

    // Gets references for boxcollider and rigidbody of moving object. Adapted from code written by Matt Schell (2022).

    protected virtual void Start()
    {

        boxCollider = GetComponent<BoxCollider2D>();

        rb2D = GetComponent<Rigidbody2D>();

        inverseMoveTime = 1f / moveTime;
        
    }

    // Checks if there is a collision using RaycastHit and takes the x and y position to move object into.

    protected bool Move (int xDir, int yDir, out RaycastHit2D hit)


    {
       

        Vector2 start = transform.position;

        Vector2 end = start + new Vector2(xDir, yDir);

        hit = Physics2D.Linecast(start, end, blockingLayer);

        Debug.DrawLine(start,end,Color.red,3f);

        if (hit.transform == null && !isMoving && gameObject.activeSelf)

        {

            StartCoroutine (Actuator(end));

            return true;
        }

        return false;



    }


    // Co-routine called by above function to make sure object moves to destination and prevents any other movement of objects. Adapted from code written by Matt Schell (2022).
    protected IEnumerator Actuator (Vector2 end)
    {
        isMoving = true;

        float sqrRemainingDistance = ((Vector2)transform.position - end).sqrMagnitude;

        while(sqrRemainingDistance > float.Epsilon)
        {
            Vector2 newPosition = Vector2.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);

            rb2D.MovePosition(newPosition);

            sqrRemainingDistance = ((Vector2)transform.position - end).sqrMagnitude;

            yield return null;

        }

        rb2D.MovePosition(end);

        isMoving = false;

    }

    // Checks if the object has hit anything and that will the parameter T. Adapted from code written by Matt Schell (2022).

    protected virtual void AttemptMove<T>(int xDir, int yDir)
        where T : Component
    {
        RaycastHit2D hit;

        bool canMove = Move(xDir, yDir, out hit);

        if (hit.transform == null)
        {
            return;
        }

        T hitComponent = hit.transform.GetComponent<T>();

        if(!canMove && hitComponent != null)
        {
            onCantMove(hitComponent);
        }


    }

    protected abstract void onCantMove<T>(T component)
        where T : Component;

}

// Code from Matt Schell (2022) can be accessed from: https://learn.unity.com/project/2d-roguelike-tutorial?uv=5.x
