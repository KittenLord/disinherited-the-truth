using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float MovementSpeed;
    [SerializeField] private float JumpForce;

    private Rigidbody2D rb;
    private Animator animator;
    private Camera cam;

    private Vector3 direction;

    private bool moveJump = false;
    private bool jumpState = false;
    private bool touchesGround = true;
    private bool attacking = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        cam = Camera.main;
    }

    void Update()
    {
        if(attacking) { return; }

        if(Input.GetMouseButtonDown(0) && !attacking)
        {
            attacking = true;
            rb.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
            animator.Play("Attack");
        }


        var axis = Input.GetAxisRaw("Horizontal");
        direction = new Vector2(axis, 0);
        moveJump = axis != 0;
        if(!moveJump)
        {
            animator.Play("Idle");
        }

        var mouseDirection = Mathf.Sign((cam.ScreenToWorldPoint(Input.mousePosition) - transform.position).x);
        if(axis != 0) transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x) * axis, transform.localScale.y);
        else transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x) * mouseDirection, transform.localScale.y);
    }

    void FixedUpdate() 
    {
        rb.velocity = new Vector2(direction.x, rb.velocity.y);
        if(touchesGround && moveJump)
        {
            rb.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
            if(jumpState = !jumpState)
                 animator.Play("Jump1");
            else animator.Play("Jump2");

            touchesGround = false;
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        UnityEngine.Debug.Log($"ground");
        if(col.transform.tag == "Ground") 
        {
            touchesGround = true;
            attacking = false;
        }
    }
}
