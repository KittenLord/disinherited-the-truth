using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedWeapon : MonoBehaviour
{
    public IWeapon Weapon;
    public void SetWeapon(IWeapon weapon)
    {
        Weapon = weapon;
        var go = Resources.Load<GameObject>($"Prefabs/{weapon.Name}");
        var ins = Instantiate(go, this.transform);
        // ins.transform.position = Vector3.zero;
    }

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
            // TODO: Sound effect
            PlayerController.Main.canInteract = false;
            PlayerController.Main.SetWeapon(Weapon);
            StartCoroutine(UnlockPlayer());
        }
    }

    private IEnumerator UnlockPlayer()
    {
        yield return new WaitForSeconds(0.1f);
        PlayerController.Main.canInteract = true;
        Destroy(this.gameObject);
    }
}
