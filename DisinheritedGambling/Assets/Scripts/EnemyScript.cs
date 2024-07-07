using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType
{
    Bone,
    Skull,
    Moth
}

public class EnemyScript : MonoBehaviour, IEntity
{
    public EnemyType Type;

    public float Health;
    public float Damage;
    public float AttackRadius;
    public bool LongRange;
    public float Speed;
    public float AttackDelay;

    public Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void OnAttack(float damage)
    {
        Health -= damage;
        var soundIndex = UnityEngine.Random.Range(0, 3);
        Audio.Play($"enemyDamage{soundIndex+1}");
        if(Health <= 0) Destroy(this.gameObject);
    }

    private Vector3 direction;

    private IEnumerator Attack()
    {
        isAttacking = true;

        if(!LongRange)
        {
            yield return new WaitForSeconds(AttackDelay);
            isHitting = true;
            // TODO: Sound effect
            yield return new WaitForSeconds(0.3f);
            isHitting = false;
            yield return new WaitForSeconds(0.3f);
        }
        else
        {
            yield return new WaitForSeconds(0.3f);
            var bonePrefab = Resources.Load<BoneProjectile>("Bones/BoneProjectile");
            var bone = Instantiate(bonePrefab, transform.position - Vector3.up*0.5f, Quaternion.identity);
            var direction = PlayerController.Main.transform.position - transform.position;
            bone.Direction = direction.normalized;
            bone.Damage = this.Damage;
            yield return new WaitForSeconds(AttackDelay);
        }

        isAttacking = false;
    }

    private bool isAttacking = false;
    public bool isHitting = false;

    void Update()
    {
        if(isAttacking) { direction = Vector3.zero; return; }

        var delta = PlayerController.Main.transform.position - transform.position;
        direction = delta.normalized;

        var sc = transform.localScale;
        sc.x = Mathf.Abs(sc.x) * Mathf.Sign(direction.x);
        transform.localScale = sc;

        if(delta.magnitude <= AttackRadius)
        {
            StartCoroutine(Attack());
        }
    }

    void FixedUpdate()
    {
        rb.velocity = new Vector2(direction.x * Speed, rb.velocity.y);
    }
}
