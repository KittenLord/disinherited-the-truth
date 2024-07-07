using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeapon
{
    public string Name { get; }
    public bool Block { get; }
}

// TODO: revisit, looks bad
public class RegularWeapon : IWeapon
{
    public static RegularWeapon Default => new RegularWeapon("", 10);
    public static RegularWeapon Weapon1 => new RegularWeapon("Reward1", 30);
    public static RegularWeapon Weapon2 => new RegularWeapon("Reward2", 60);
    public static RegularWeapon Weapon3 => new RegularWeapon("Reward3", 99999);

    public bool Block => true;

    public float Damage;
    public string Name { get; private set; }
    public RegularWeapon(string name, float damage) { Name = name; Damage = damage; } 
}

public class FirearmWeapon : IWeapon
{
    public static FirearmWeapon WeaponW1 => new FirearmWeapon("Weapon1", 15, 3f, 0.6f, 20);
    public static FirearmWeapon WeaponW2 => new FirearmWeapon("Weapon2", 10, 5f, 0.1f, 30);
    public static FirearmWeapon WeaponW3 => new FirearmWeapon("Weapon3", 7, 5f, 0.03f, 50);

    public bool Block => false;

    public float Damage;
    public float Spread;
    public float Delay;
    public float Speed;
    public string Name { get; private set; }
    public FirearmWeapon(string name, float damage, float spread, float delay, float speed) { Name = name; Damage = damage; Spread = spread; Delay = delay; Speed = speed; } 
}
