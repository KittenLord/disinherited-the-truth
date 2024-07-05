using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GambaWheelValue
{
    Skull,
    Heart,
    Gold,
    Sword,
    Bone,
    Magic
}

public class GambaController : MonoBehaviour
{
    public static GambaController Main { get; private set; }

    [SerializeField] private SpriteRenderer[] Wheels;
    [SerializeField] private Transform GambaBody;

    [SerializeField] public GameObject GambaBodyTrigger;
    [SerializeField] public GameObject GambaLeverTrigger;

    [SerializeField] private GameObject GambaLeverOn;
    [SerializeField] private GameObject GambaLeverOff;

    [SerializeField] private Animator CraneAnimator;
    [SerializeField] private Animator GambaAnimator;

    [SerializeField] private Transform Weapon1Holder;
    [SerializeField] private Transform Weapon2Holder;
    [SerializeField] private Transform BoneOrigin;

    public void SetLever(bool value)
    {
        GambaLeverOn.SetActive(value);
        GambaLeverOff.SetActive(!value);
    }

    private Dictionary<GambaWheelValue, Sprite[]> WheelValues = new();
    private Sprite[][] WheelFrames = { new Sprite[6], new Sprite[6], new Sprite[6] };

    void StopAnimating()
    {
        GambaAnimator.Play("Idle");
        GambaAnimator.enabled = false;
    }

    void StartAnimating()
    {
        GambaAnimator.enabled = true;
        GambaAnimator.Play("Idle");
    }

    void Start()
    {
        if(Main != null) Destroy(Main);
        Main = this;

        CanReset = false;

        StopAnimating();

        GambaLeverTrigger.SetActive(false);
        GambaBodyTrigger.SetActive(true);

        foreach(var type in Enum.GetValues(typeof(GambaWheelValue)).Cast<GambaWheelValue>())
        {
            var str = type.ToString().ToLower();
            WheelValues[type] = new Sprite[3];
            for(int i = 0; i < 3; i++)
            {
                WheelValues[type][i] = Resources.Load<Sprite>($"Wheel/w{i+1}{str}");
            }
        }

        for(int frame = 0; frame < 6; frame++)
        {
            for(int i = 0; i < 3; i++)
            {
                WheelFrames[i][frame] = Resources.Load<Sprite>($"Wheel/w{i+1}_{frame+1}");
            }
        }

        SetRandom();
    }

    void SetRandom()
    {
        var values = Enum.GetValues(typeof(GambaWheelValue)).Cast<GambaWheelValue>().ToList();
        var rn = new System.Random();
        for(int i = 0; i < 3; i++)
        {
            var r = rn.Next(0, values.Count);
            Wheels[i].sprite = WheelValues[values[r]][i];
        }
    }

    public void Reset()
    {
        CanReset = false;
        GambaAnimator.gameObject.SetActive(true);
        SetLever(false);

        GambaLeverTrigger.SetActive(false);
        GambaBodyTrigger.SetActive(true);
    }

    public bool UseRigged = false;
    public GambaWheelValue[] RiggedValues = { GambaWheelValue.Bone, GambaWheelValue.Bone, GambaWheelValue.Bone };
    
    public GambaWheelValue[] ResultValues = { GambaWheelValue.Bone, GambaWheelValue.Bone, GambaWheelValue.Bone };

    private Coroutine SpinningCoroutine;
    private Coroutine[] WheelCoroutines = new Coroutine[3];

    private IEnumerator Spinning()
    {
        float time = 0;
        WheelCoroutines[0] = StartCoroutine(WheelSpinning(0, 3.0f, UseRigged));
        WheelCoroutines[1] = StartCoroutine(WheelSpinning(1, 4.8f, UseRigged));
        WheelCoroutines[2] = StartCoroutine(WheelSpinning(2, 7.2f, UseRigged));
        UseRigged = false;

        while(time <= Mathf.PI*2)
        {
            // GambaBody.localScale = new Vector2(GambaBody.localScale.x, 1 + Mathf.Sin(time*4)*0.1f);
            GambaBody.localScale = new Vector2(1 - Mathf.Sin(time*4)*0.1f, 1 + Mathf.Sin(time*4)*0.1f);
            GambaBody.rotation = Quaternion.Euler(0, 0, Mathf.Sin(time*5)*5);

            time += Time.deltaTime;
            yield return null;
        }

        GambaBody.localScale = Vector3.one;
        GambaBody.rotation = Quaternion.Euler(0, 0, 0);

        yield return WheelCoroutines[2];

        StartCoroutine(AnimateCrane());

        var magicCount = ResultValues.Where(r => r == GambaWheelValue.Magic).Count();
        if(magicCount > 0)
        {
            // TODO: Sound effect
            PlayerController.Main.Health = PlayerController.Main.MaxHealth;
        }
        StartCoroutine(DamageEffect(ResultValues.Where(r => r == GambaWheelValue.Heart).Count()));
    }

    public bool CanReset = false;

    private IEnumerator AnimateCrane()
    {
        yield return new WaitForSeconds(2);

        StartAnimating();
        GambaAnimator.Play("Move");
        CraneAnimator.Play("Move");

        yield return new WaitForSeconds(7);

        var goldCount = ResultValues.Where(r => r == GambaWheelValue.Gold).Count();
        var weaponCount = ResultValues.Where(r => r == GambaWheelValue.Sword).Count();

        IWeapon goldWeapon = RegularWeapon.Default;
        IWeapon weaponWeapon = RegularWeapon.Default; // naming be like lmao

        if(goldCount == 1) goldWeapon = RegularWeapon.Weapon1;
        if(goldCount == 2) goldWeapon = RegularWeapon.Weapon2;
        if(goldCount == 3) goldWeapon = RegularWeapon.Weapon3;

        if(weaponCount == 1) weaponWeapon = FirearmWeapon.WeaponW1;
        if(weaponCount == 2) weaponWeapon = FirearmWeapon.WeaponW2;
        if(weaponCount == 3) weaponWeapon = FirearmWeapon.WeaponW3;

        if(goldCount != 0) 
        {
            var weapon = Instantiate<DroppedWeapon>(Resources.Load<DroppedWeapon>("Prefabs/DroppedWeapon"), Weapon1Holder);
            weapon.SetWeapon(goldWeapon);
        }

        if(weaponCount != 0) 
        {
            var weapon = Instantiate<DroppedWeapon>(Resources.Load<DroppedWeapon>("Prefabs/DroppedWeapon"), Weapon2Holder);
            weapon.SetWeapon(weaponWeapon);
        }

        var boneCount = ResultValues.Where(r => r == GambaWheelValue.Bone).Count();
        if(boneCount == 1) boneCount = 5;
        else if(boneCount == 2) boneCount = 20;
        else if(boneCount == 3) boneCount = 500;

        var rng = new System.Random();
        for(int i = 0; i < boneCount; i++)
        {
            var type = rng.Next(0, 3);
            var obj = Resources.Load<Rigidbody2D>($"Prefabs/bone{type+1}");
            var rb = Instantiate(obj, BoneOrigin.position, Quaternion.Euler(0, 0, rng.Next(0, 360)));
            rb.AddForce(new Vector2(UnityEngine.Random.Range(-12.0f, 12.0f), UnityEngine.Random.Range(1.0f, 9.0f)), ForceMode2D.Impulse);
        }

        var skullCount = ResultValues.Where(r => r == GambaWheelValue.Skull).Count();
        List<EnemyType> enemies = new();

        if(skullCount == 1) { 
            for(int i = 0; i < 1; i++) enemies.Add(EnemyType.Bone); 
            for(int i = 0; i < 1; i++) enemies.Add(EnemyType.Skull); 
        }
        else if(skullCount == 2) { 
            for(int i = 0; i < 2; i++) enemies.Add(EnemyType.Bone); 
            for(int i = 0; i < 1; i++) enemies.Add(EnemyType.Skull); 
            for(int i = 0; i < 1; i++) enemies.Add(EnemyType.Moth); 
        }
        else if(skullCount == 3) { 
            for(int i = 0; i < 5; i++) enemies.Add(EnemyType.Bone); 
            for(int i = 0; i < 5; i++) enemies.Add(EnemyType.Skull); 
            for(int i = 0; i < 5; i++) enemies.Add(EnemyType.Moth); 
        }

        foreach(var enemy in enemies)
        {
            string name = "BoneEnemy";
            if(enemy == EnemyType.Skull) name = "SkullEnemy";
            else if(enemy == EnemyType.Moth) name = "MothEnemy";

            var prefab = Resources.Load("Enemies/" + name);
            var x = UnityEngine.Random.Range(4.0f, 8.0f);
            if(rng.Next(0, 2) == 0) x = -x;

            Instantiate(prefab, new Vector3(x, 6.5f, 0), Quaternion.identity);
        }

        yield return new WaitForSeconds(13);

        CraneAnimator.Play("Idle");
        GambaAnimator.Play("Idle");

        // ;-; unity doesnt like having object disabled while still animating it
        yield return null;
        StopAnimating();

        GambaAnimator.gameObject.SetActive(false);

        CanReset = true;
    }

    private IEnumerator DamageEffect(int count)
    {
        yield return new WaitForSeconds(0.6f);
        while(count > 0)
        {
            count--;
            PlayerController.Main.Damage(PlayerController.Main.MaxHealth * 0.2f);
            yield return new WaitForSeconds(0.6f);
        }
    }

    private IEnumerator WheelSpinning(int i, float time, bool useRigged)
    {
        var r = new System.Random();
        var dt = Time.deltaTime * 4;
        while(time > 0)
        {
            time -= dt;
            Wheels[i].sprite = WheelFrames[i][r.Next(0, 6)];
            yield return new WaitForSeconds(dt);
        }

        var values = Enum.GetValues(typeof(GambaWheelValue)).Cast<GambaWheelValue>().ToList();
        var value = values[r.Next(0, values.Count)];
        if(useRigged) value = RiggedValues[i];

        ResultValues[i] = value;
        Wheels[i].sprite = WheelValues[value][i];
    }

    public void Spin()
    {
        if(SpinningCoroutine != null)
        {
            StopCoroutine(SpinningCoroutine);
        }

        foreach(var wc in WheelCoroutines) 
        {
            if(wc != null)
            {
                StopCoroutine(wc);
            }
        }

        SpinningCoroutine = StartCoroutine(Spinning());
    }

    // I swear I'm not dumb, when I call Spin() directly from another gameobject it halves the framerate for no reason, I have no idea why
    private bool StartSpin = false;
    public void CallSpin() => StartSpin = true;

    void Update()
    {
        // if(Input.GetKeyDown(KeyCode.U)) SetRandom();
        // if(Input.GetKeyDown(KeyCode.O)) Spin();
        // if(Input.GetKeyDown(KeyCode.M)) Reset();

        if(StartSpin) { StartSpin = false; Spin(); }
    }
}
