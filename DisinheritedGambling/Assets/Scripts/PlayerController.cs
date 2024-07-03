using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Main { get; private set; }

    [SerializeField] private float MovementSpeed;
    [SerializeField] private float JumpForce;

    [SerializeField] private Transform WeaponHolder;
    public IWeapon Weapon;

    private Rigidbody2D rb;
    private Animator animator;
    private Camera cam;

    private Vector3 direction;

    public void Damage(float h)
    {
        Health -= h;
    }

    public float Health = 100;
    public float MaxHealth = 100;

    private bool moveJump = false;
    private bool jumpState = false;
    private bool touchesGround = true;
    private bool attacking = false;
    public bool hitting = false;
    public bool canInteract = true;

    void Start()
    {
        if(Main != null) Destroy(Main);
        Main = this;

        Weapon = new RegularWeapon() { Damage = 15 };

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        cam = Camera.main;
    }

    void Reset()
    {
        direction = Vector2.zero;
        moveJump = false;
    }

    void Update()
    {
        UI.Main.SetHealthBar(Health / MaxHealth);
        // if(Input.GetKeyDown(KeyCode.K)) Health -= 10;

        // TODO: Death animation
        if(Health <= 0) { Reset(); return; }

        if(!canInteract) { Reset(); return; }
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
        yield return new WaitForSeconds(0.5f);
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

}
