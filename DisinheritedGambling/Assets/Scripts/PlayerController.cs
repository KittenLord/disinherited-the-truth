using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Main { get; private set; }

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
    private bool hitting = false;
    public bool canInteract = true;

    void Start()
    {
        if(Main != null) Destroy(Main);
        Main = this;

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        cam = Camera.main;
    }

    void Update()
    {
        if(!canInteract) { direction = Vector2.zero; moveJump = false; return; }
        if(attacking) { return; }

        if(Input.GetMouseButtonDown(0) && !attacking)
        {
            attacking = true;
            StartHitting();
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
        else if(touchesGround) transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x) * mouseDirection, transform.localScale.y);
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

    private IEnumerator Hit()
    {
        yield return new WaitForSeconds(0.3f);
        hitting = true;
        yield return new WaitForSeconds(0.2f);
        hitting = false;
    }

    private Coroutine HitCoroutine;
    private void StartHitting()
    {
        if(HitCoroutine != null) StopCoroutine(HitCoroutine);
        HitCoroutine = StartCoroutine(Hit());
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

    void OnTriggerStay2D(Collider2D other)
    {
        if(canInteract && other.transform.tag == "GambaBody")
        {
            UI.Main.InteractTip(true);
        }
        else
        {
            UI.Main.InteractTip(false);
        }

        if(canInteract && other.transform.tag == "GambaBody" && Input.GetKey(KeyCode.E))
        {
            canInteract = false;
            UI.Main.OpenGambaMenu();
        }
    }
}
