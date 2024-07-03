using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeapon
{
    void OnClick();
}

// TODO: revisit, looks bad
public class RegularWeapon : IWeapon
{
    public float Damage;
    public void OnClick() { }
}
