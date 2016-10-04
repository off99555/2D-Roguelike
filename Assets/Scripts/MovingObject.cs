using System;
using System.Collections;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour
{
    public LayerMask blockingLayer;

    private BoxCollider2D boxCollider;
    public float moveTime = 0.1f;
    private Rigidbody2D rb2D;
    private float speed;

    // for x value only 1 = right, -1 = left
    protected virtual int FacingDirectionOnStart
    {
        get { throw new NotImplementedException(); }
    }

    protected virtual void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();
        speed = 1f/moveTime;
    }

    protected bool Move(Vector2 displacement, out RaycastHit2D hit)
    {
        // turn around 
        var xScale = Mathf.Sign(displacement.x)*FacingDirectionOnStart;
        var localScale = transform.localScale;
        localScale.x = xScale;
        transform.localScale = localScale;

        Vector2 start = transform.position;
        var end = start + displacement;
        end.x = Mathf.Round(end.x);
        end.y = Mathf.Round(end.y);

        boxCollider.enabled = false;
        hit = Physics2D.Linecast(start, end, blockingLayer);
        boxCollider.enabled = true;

        var canMove = hit.transform == null;
        if (canMove)
        {
            StartCoroutine(SmoothMovement(end));
        }
        return canMove;
    }

    protected IEnumerator SmoothMovement(Vector3 end)
    {
        var sqrDistance = (transform.position - end).sqrMagnitude;
        while (sqrDistance > float.Epsilon)
        {
            var newPos = Vector3.MoveTowards(rb2D.position, end, speed*Time.deltaTime);
            rb2D.MovePosition(newPos);
            sqrDistance = (transform.position - end).sqrMagnitude;
            yield return null;
        }
    }

    protected virtual void AttemptMove<T>(Vector2 displacement) where T : Component
    {
        RaycastHit2D hit;
        var canMove = Move(displacement, out hit);

        if (!canMove)
        {
            var hitComponent = hit.transform.GetComponent<T>();
            if (hitComponent != null)
                OnCantMove(hitComponent);
        }
    }

    protected abstract void OnCantMove<T>(T component) where T : Component;
}