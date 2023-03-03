using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMove : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] float runSpeed;
    [SerializeField] float jumSpeed;
    Vector2 moveInput;
    Rigidbody2D mrigidbody2D;
    BoxCollider2D mBox;
    void Start()
    {
        mrigidbody2D = GetComponent<Rigidbody2D>();
        mBox = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Run();
        FlipPlayer();
    }

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
        Debug.Log(moveInput);
    }
    void OnJum(InputValue value)
    {
        if (!mBox.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            return;
        }
        if (value.isPressed)
        {
            mrigidbody2D.velocity += new Vector2(0f, jumSpeed);
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
