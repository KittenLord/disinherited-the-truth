using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// I'm fairly sure I could've reused the BoneProjectile, but naaah
public class BulletProjectile : MonoBehaviour
{
    public float Spread;
    public float Damage;
    public bool Reverse;
    public float Speed;

    private float Sign => Reverse ? -1 : 1;
    private Vector3 Direction;

    void Start()
    {
        var v = transform.localScale;
        v.x *= -Sign;
        transform.localScale = v;

        // Direction = new Vector3(Sign, Random.Range(-Spread, Spread), 0).normalized;
        Direction = Vector3.right * Sign;
        transform.rotation = Quaternion.Euler(0, 0, Random.Range(-Spread, Spread));
    }

    private float Lifetime = 0;
    void Update()
    {
        if(Lifetime > 20) Destroy(this.gameObject);
        Lifetime += Time.deltaTime;

        transform.Translate(Direction * Speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player") return;

        var entity = other.GetComponent<IEntity>();
        if(entity is null) return;

        entity.OnAttack(Damage);
        Destroy(this.gameObject);
    }
}
