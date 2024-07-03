using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedWeapon : MonoBehaviour
{
    public IWeapon Weapon;

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.transform.tag != "Player") return;
        UI.Main.InteractTip(false);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if(other.transform.tag != "Player") return;
        UI.Main.InteractTip(PlayerController.Main.canInteract);
        if(PlayerController.Main.canInteract && Input.GetKey(KeyCode.E))
        {
            PlayerController.Main.Weapon = Weapon;
            Destroy(this.gameObject);
        }
    }
}
