using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttacker : MonoBehaviour
{
    void Start()
    {

    }

    void OnTriggerStay2D(Collider2D other)
    {
        if(other.tag == "Player") return;
        if(PlayerController.Main.hitting)
        {
            var entity = other.GetComponent<IEntity>();
            if(entity is null) return;

            // wanted to do polymorphically, but fuck it we ball
            if(PlayerController.Main.Weapon is RegularWeapon rw)
            {
                PlayerController.Main.hitting = false;
                entity.OnAttack(rw.Damage);
            }
        }
    }
}
