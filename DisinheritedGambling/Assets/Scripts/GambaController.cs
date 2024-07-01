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

    private Dictionary<GambaWheelValue, Sprite[]> WheelValues = new();
    private Sprite[][] WheelFrames = { new Sprite[6], new Sprite[6], new Sprite[6] };

    // Start is called before the first frame update
    void Start()
    {
        if(Main != null) Destroy(Main);
        Main = this;

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

    private Coroutine SpinningCoroutine;
    private Coroutine[] WheelCoroutines = new Coroutine[3];

    private IEnumerator Spinning()
    {
        float time = 0;
        WheelCoroutines[0] = StartCoroutine(WheelSpinning(0, 3.0f));
        WheelCoroutines[1] = StartCoroutine(WheelSpinning(1, 4.8f));
        WheelCoroutines[2] = StartCoroutine(WheelSpinning(2, 7.2f));

        while(time <= Mathf.PI*2)
        {
            GambaBody.localScale = new Vector2(GambaBody.localScale.x, 1 + Mathf.Sin(time*4)*0.1f);
            GambaBody.rotation = Quaternion.Euler(0, 0, Mathf.Sin(time*5)*5);

            time += Time.deltaTime;
            yield return null;
        }

        GambaBody.localScale = Vector3.one;
        GambaBody.rotation = Quaternion.Euler(0, 0, 0);

        yield return WheelCoroutines[2];

        UnityEngine.Debug.Log($"done");
    }

    private IEnumerator WheelSpinning(int i, float time)
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

        Wheels[i].sprite = WheelValues[values[r.Next(0, values.Count)]][i];
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

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.U)) SetRandom();
        if(Input.GetKeyDown(KeyCode.O)) Spin();
    }
}
