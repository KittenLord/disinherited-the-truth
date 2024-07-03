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
        StartCoroutine(DamageEffect(ResultValues.Where(r => r == GambaWheelValue.Heart).Count()));

        UnityEngine.Debug.Log($"done");
    }

    private IEnumerator AnimateCrane()
    {
        StartAnimating();
        GambaAnimator.Play("Move");
        CraneAnimator.Play("Move");

        yield return new WaitForSeconds(20);

        CraneAnimator.Play("Idle");
        GambaAnimator.Play("Idle");

        // ;-; unity doesnt like having object disabled while still animating it
        yield return null;

        StopAnimating();
        GambaAnimator.gameObject.SetActive(false);
    }

    private IEnumerator DamageEffect(int count)
    {
        yield return new WaitForSeconds(1);
        while(count > 0)
        {
            count--;
            PlayerController.Main.Damage(PlayerController.Main.MaxHealth * 0.2f);
            yield return new WaitForSeconds(0.8f);
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

    // I swear I'm not dumb, when I call Spin() from another gameobject it halves the framerate for no reason, I have no idea why
    private bool StartSpin = false;
    public void CallSpin() => StartSpin = true;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.U)) SetRandom();
        if(Input.GetKeyDown(KeyCode.O)) Spin();

        if(StartSpin) { StartSpin = false; Spin(); }
    }
}
