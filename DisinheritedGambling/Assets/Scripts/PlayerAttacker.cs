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
        if(PlayerController.Main.hitting)
        {
            var entity = other.GetComponent<IEntity>();
            if(entity is null) return;

            PlayerController.Main.hitting = false;
            entity.OnAttack(15);
        }
    }
}
