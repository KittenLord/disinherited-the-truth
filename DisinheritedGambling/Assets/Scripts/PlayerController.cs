using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Main { get; private set; }

    [SerializeField] public float MovementSpeed;
    [SerializeField] public float JumpForce;

    [SerializeField] private Transform WeaponHolder;
    public IWeapon Weapon;
    public void SetWeapon(IWeapon weapon)
    {
        if(WeaponHolder.childCount != 0) Destroy(WeaponHolder.GetChild(0).gameObject);
        Weapon = weapon;
        if(weapon.Name == "") return;
        var go = Resources.Load<GameObject>($"Prefabs/{weapon.Name}");
        var ins = Instantiate(go, WeaponHolder);
        ins.GetComponent<SpriteRenderer>().sortingOrder = 1;
        // ins.transform.position = Vector3.zero;
    }

    private Rigidbody2D rb;
    private Animator animator;
    private Camera cam;

    private Vector3 direction;

    public void Damage(float h)
    {
        if(!GodMode)
        {
            Audio.Play("playerHurt");
            Health -= h;
        }
    }

    public void Heal()
    {
        Health = MaxHealth;
        Audio.Play("playerHeal");
    }

    public float Health = 100;
    public float MaxHealth = 100;
    public bool GodMode = false;

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

        SetWeapon(RegularWeapon.Default);
        // SetWeapon(FirearmWeapon.WeaponW2);

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        cam = Camera.main;

        animator.Play("Idle");
    }

    void Reset()
    {
        direction = Vector2.zero;
        moveJump = false;
    }

    private bool DeadAnimation = false;

    private float ShootingDelay = 0;
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(UI.Main.MenuOpened) UI.Main.CloseMenu();
            else UI.Main.OpenMenu();
        }

        ShootingDelay -= Time.deltaTime;
        UI.Main.SetHealthBar(Health / MaxHealth);

        if(Health <= 0) { DeadAnimation = true; animator.Play("Attack"); transform.rotation = Quaternion.Euler(0, 0, -90); Reset(); return; }
        if(DeadAnimation) { DeadAnimation = false; animator.Play("Idle"); transform.rotation = Quaternion.identity; }

        if(!canInteract) { Reset(); return; }
        if(attacking && Weapon.Block) { return; }

        if(Weapon.Block)
        {
            if(Input.GetMouseButtonDown(0) && !attacking && !UI.Main.MenuOpened)
            {
                var boingNumber = Random.Range(0, 5);
                Audio.Play($"Jumps/boing{boingNumber + 1}");
                attacking = true;
                rb.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
                StartHitting();
                animator.Play("Attack");
            }
        }
        else
        {
            if(Input.GetMouseButton(0) && ShootingDelay <= 0 && !UI.Main.MenuOpened)
            {
                var volume = UnityEngine.Random.Range(0.7f, 1);
                var bulletIndex = UnityEngine.Random.Range(0, 3);
                Audio.Play($"bullet{bulletIndex + 1}", volume);

                var w = Weapon as FirearmWeapon;
                ShootingDelay = w.Delay + UnityEngine.Random.Range(-Time.deltaTime, 2*Time.deltaTime);

                var bulletPrefab = Resources.Load<BulletProjectile>("Prefabs/bullet");
                var bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

                bullet.Spread = w.Spread;
                bullet.Damage = w.Damage;
                bullet.Reverse = transform.localScale.x < 0;
                bullet.Speed = w.Speed;
            }
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
        rb.velocity = new Vector2(direction.x * MovementSpeed, rb.velocity.y);
        if(touchesGround && moveJump)
        {
            var boingNumber = Random.Range(0, 5);
            Audio.Play($"Jumps/boing{boingNumber + 1}");

            rb.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
            if(jumpState = !jumpState)
                 animator.Play("Jump1");
            else animator.Play("Jump2");

            touchesGround = false;
        }
    }

    private IEnumerator Hit()
    {
        // yield return new WaitForSeconds(0.55f);

        while(rb.velocity.y >= 0) yield return null; // You start hitting immediately when falling
        Audio.Play($"playerSwish");
        yield return new WaitForSeconds(0.1f); // or maybe not immediately lol
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
        if(col.transform.tag == "Ground") 
        {
            touchesGround = true;
            attacking = false;
        }
    }

    void OnCollisionStay2D(Collision2D col)
    {
        if(col.transform.tag == "Wall")
        {
            rb.MovePosition(transform.position - Vector3.up*0.1f);
        }
    }
}
