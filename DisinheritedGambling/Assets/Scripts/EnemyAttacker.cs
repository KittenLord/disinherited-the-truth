using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttacker : MonoBehaviour
{
    private EnemyScript Enemy;

    void Start()
    {
        Enemy = transform.parent.GetComponent<EnemyScript>();
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if(!Enemy.isHitting) return;
        if(other.tag != "Player") return;
        Enemy.isHitting = false;

        other.GetComponent<PlayerController>().Damage(Enemy.Damage);
    }
}
