using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GambaLever : MonoBehaviour, IEntity
{
    public void OnAttack(float damage)
    {
        // TODO: Sound effect
        GambaController.Main.SetLever(true);
        this.gameObject.SetActive(false);

        GambaController.Main.CallSpin();
    }
}
