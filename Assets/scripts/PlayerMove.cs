using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] float runSpeed;
    [SerializeField] float jumpSpeed;
    [SerializeField] float maxJumpHeight;
    [SerializeField] float timeToMaxHeight;
    [SerializeField] float gravityScale;
    [SerializeField] float descendSpeed;
    Vector2 moveInput;
    Rigidbody2D mrigidbody2D;
    public float maxJum;
    bool isJumping = false;

    void Start()
    {
        mrigidbody2D = GetComponent<Rigidbody2D>();
        mrigidbody2D.gravityScale = gravityScale;
    }

    void Update()
    {
        Run();
        FlipPlayer();
    }

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void OnJum(InputValue value)
    {
        if (value.isPressed)
        {
            
                mrigidbody2D.velocity += new Vector2(mrigidbody2D.velocity.x, jumpSpeed);
            
        }

    }

    void OnFly(InputValue value)
    {
        mrigidbody2D.velocity += new Vector2(mrigidbody2D.velocity.x, jumpSpeed);
        Debug.Log(value);
    }
    IEnumerator JumpCoroutine()
    {
        float timer = 0f;
        while (isJumping && timer < timeToMaxHeight)
        {
            float normalizedTime = timer / timeToMaxHeight;
            float jumpVelocity = Mathf.Lerp(0f, maxJumpHeight, normalizedTime);
            mrigidbody2D.velocity = new Vector2(mrigidbody2D.velocity.x, jumpVelocity);
            timer += Time.deltaTime;
            yield return null;
        }

        while (isJumping)
        {
            mrigidbody2D.velocity = new Vector2(mrigidbody2D.velocity.x, -descendSpeed);
            yield return null;
        }
    }

    void Run()
    {
        Vector2 playerMove = new Vector2(moveInput.x * runSpeed, mrigidbody2D.velocity.y);
        mrigidbody2D.velocity = playerMove;
    }

    void FlipPlayer()
    {
        bool playerHasHor = Mathf.Abs(mrigidbody2D.velocity.x) > Mathf.Epsilon;
        if (playerHasHor)
        {
            transform.localScale = new Vector2(Mathf.Sign(mrigidbody2D.velocity.x), 1f);
        }
    }
}
