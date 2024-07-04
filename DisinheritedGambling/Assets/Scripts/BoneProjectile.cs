using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoneProjectile : MonoBehaviour
{
    void Start()
    {
        var sr = transform.GetChild(0).GetComponent<SpriteRenderer>();
        int i = new System.Random().Next(0, 3);
        var sprite = Resources.Load<Sprite>($"Bones/bone{i+1}");
        sr.sprite = sprite;
    }

    public Vector3 Direction;
    public float Damage;

    private float dr;
    private float timeAlive;
    void Update()
    {
        timeAlive += Time.deltaTime;
        if(timeAlive > 20) { Destroy(this.gameObject); return; }

        dr += Time.deltaTime * 1400;
        transform.GetChild(0).rotation = Quaternion.Euler(0, 0, dr);
        transform.Translate(Direction * Time.deltaTime * 3f);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag != "Player") return;
        other.GetComponent<PlayerController>().Damage(Damage);
        Destroy(this.gameObject);
    }
}
