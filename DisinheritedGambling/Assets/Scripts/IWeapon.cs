using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeapon
{
    public string Name { get; }
}

// TODO: revisit, looks bad
public class RegularWeapon : IWeapon
{
    public static RegularWeapon Default => new RegularWeapon("", 10);
    public static RegularWeapon Weapon1 => new RegularWeapon("Reward1", 30);
    public static RegularWeapon Weapon2 => new RegularWeapon("Reward2", 60);
    public static RegularWeapon Weapon3 => new RegularWeapon("Reward3", 99999);

    // HACK: Remove later
    public static RegularWeapon WeaponW1 => new RegularWeapon("Weapon1", 30);
    public static RegularWeapon WeaponW2 => new RegularWeapon("Weapon2", 60);
    public static RegularWeapon WeaponW3 => new RegularWeapon("Weapon3", 99999);

    public float Damage;
    public string Name { get; private set; }
    public RegularWeapon(string name, float damage) { Name = name; Damage = damage; } 
}
