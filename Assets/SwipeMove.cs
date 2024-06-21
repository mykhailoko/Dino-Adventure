using UnityEngine;
using DG.Tweening;
using System.Collections;

public class SwipeControl : MonoBehaviour
{
    private Vector2 startTouchPosition, endTouchPosition;
    private float swipeThreshold = 50f;

    public GameManager gm;
    public Rigidbody rb;

    public float runSpeed = 500f;
    public float strafeSpeed = 500f;
    public float jumpForce = 15f;

    private bool strafeLeft = false;
    private bool strafeRight = false;
    private bool doJump = false;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Obstacle")
        {
            gm.EndGame();
            Debug.Log("Game Over!");
        }
    }

    void Update()
    {
        // Отслеживание касаний для свайпов
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    startTouchPosition = touch.position;
                    break;

                case TouchPhase.Ended:
                    endTouchPosition = touch.position;
                    DetectSwipe();
                    break;
            }
        }

        if (transform.position.y < -5f)
        {
            gm.EndGame();
            Debug.Log("Game Over!");
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(transform.position + Vector3.forward * runSpeed * Time.deltaTime);

        if (strafeLeft)
        {
            rb.AddForce(-strafeSpeed * Time.deltaTime, 0, 0, ForceMode.VelocityChange);
        }

        if (strafeRight)
        {
            rb.AddForce(strafeSpeed * Time.deltaTime, 0, 0, ForceMode.VelocityChange);
        }

        if (doJump)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            doJump = false;
        }
    }

    private void DetectSwipe()
    {
        if (Vector2.Distance(startTouchPosition, endTouchPosition) >= swipeThreshold)
        {
            Vector2 direction = endTouchPosition - startTouchPosition;
            Vector2 swipeDirection = Vector2.zero;

            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                // Горизонтальный свайп
                swipeDirection = direction.x > 0 ? Vector2.right : Vector2.left;
            }
            else
            {
                // Вертикальный свайп
                swipeDirection = direction.y > 0 ? Vector2.up : Vector2.down;
            }

            PerformAction(swipeDirection);
        }
    }

    private void PerformAction(Vector2 swipeDirection)
    {
        if (swipeDirection == Vector2.up)
        {
            Jump();
        }
        else if (swipeDirection == Vector2.left)
        {
            StrafeLeft();
        }
        else if (swipeDirection == Vector2.right)
        {
            StrafeRight();
        }
    }

    private void Jump()
    {
        Debug.Log("Jump");
        doJump = true;
    }

    private void StrafeLeft()
    {
        Debug.Log("Strafe Left");
        strafeLeft = true;
        StartCoroutine(ResetStrafe());
    }

    private void StrafeRight()
    {
        Debug.Log("Strafe Right");
        strafeRight = true;
        StartCoroutine(ResetStrafe());
    }

    private IEnumerator ResetStrafe()
    {
        yield return new WaitForSeconds(0.1f);
        strafeLeft = false;
        strafeRight = false;
    }
}