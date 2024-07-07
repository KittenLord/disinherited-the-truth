using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public static UI Main { get; private set; }

    [SerializeField] private GameObject GambaMenu;
    [SerializeField] private GameObject Menu;
    [SerializeField] private GameObject CheatingMenu;

    [SerializeField] private GameObject InteractTip_;
    [SerializeField] private CanvasGroup HitLeverTip_;
    [SerializeField] private Transform HealthBar;

    [SerializeField] private GameObject RigTick;
    [SerializeField] private GameObject GodModeTick;

    [SerializeField] private Image ResetGambaButton;

    [SerializeField] private Image[] RigBoxes;

    private int[] WheelIndexes = new int[3] { 0, 0, 0 };
    [SerializeField] private Sprite[] WheelSprites;
    private GambaWheelValue[] WheelValues = new GambaWheelValue[] { 
        GambaWheelValue.Skull,
        GambaWheelValue.Heart,
        GambaWheelValue.Gold,
        GambaWheelValue.Sword,
        GambaWheelValue.Bone,
        GambaWheelValue.Magic,
    };

    void Start()
    {
        if(Main != null) Destroy(Main);
        Main = this;
    }

    void Update()
    {
        RigTick.SetActive(GambaController.Main.UseRigged);
        GodModeTick.SetActive(PlayerController.Main.GodMode);

        RigBoxes[0].sprite = WheelSprites[WheelIndexes[0]];
        RigBoxes[1].sprite = WheelSprites[WheelIndexes[1]];
        RigBoxes[2].sprite = WheelSprites[WheelIndexes[2]];

        GambaController.Main.RiggedValues[0] = WheelValues[WheelIndexes[0]];
        GambaController.Main.RiggedValues[1] = WheelValues[WheelIndexes[1]];
        GambaController.Main.RiggedValues[2] = WheelValues[WheelIndexes[2]];

        ResetGambaButton.color = GambaController.Main.CanReset ? new Color(1, 1, 1) : new Color(0.5f, 0.5f, 0.5f);
        ResetGambaButton.GetComponent<Button>().enabled = GambaController.Main.CanReset;
    }

    public void TryReset()
    {
        if(!GambaController.Main.CanReset) return;
        Audio.Play("click1");
        GambaController.Main.Reset();
    }

    public void OpenGambaMenu()
    {
        Audio.Play("click1");
        GambaMenu.SetActive(true);
    }

    public void InteractTip(bool t)
    {
        InteractTip_.SetActive(t);
    }

    public void ConfirmGambaMenu()
    {
        Audio.Play("click1");

        PlayerController.Main.Damage(PlayerController.Main.MaxHealth * 0.5f);
        GambaController.Main.GambaBodyTrigger.SetActive(false);
        GambaController.Main.GambaLeverTrigger.SetActive(true);
        CloseGambaMenu();
        StartCoroutine(HitLeverTip());
    }

    private IEnumerator HitLeverTip()
    {
        float time = 0;
        float max = 2;

        while(time < max)
        {
            time += Time.deltaTime;
            HitLeverTip_.alpha = time / max;
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        while(time > 0)
        {
            time -= Time.deltaTime*1.5f;
            HitLeverTip_.alpha = time / max;
            yield return null;
        }
    }

    public void CloseGambaMenu()
    {
        Audio.Play("click1");

        PlayerController.Main.canInteract = true;
        GambaMenu.SetActive(false);
        PlayerController.Main.transform.Translate(Vector3.right * 0.01f);
        PlayerController.Main.transform.Translate(Vector3.right *-0.01f);
    }

    public bool MenuOpened => Menu.activeSelf || CheatingMenu.activeSelf;

    public void OpenMenu() { Menu.SetActive(true); }
    public void CloseMenu() { Menu.SetActive(false); CheatingMenu.SetActive(false); }

    public void OpenCheatingMenu()
    {
        Audio.Play("click1");
        Menu.SetActive(false);
        CheatingMenu.SetActive(true);
    }

    public void SetHealthBar(float value)
    {
        value = Mathf.Clamp01(value);
        HealthBar.localScale = new Vector3(1, value, 1);
    }

    public void CloseGame() { Application.Quit(); }

    public void HealPlayer()
    {
        Audio.Play("click1");
        PlayerController.Main.Heal();
        // PlayerController.Main.Health = PlayerController.Main.MaxHealth;
    }

    public void FlipGodMode() { PlayerController.Main.GodMode = !PlayerController.Main.GodMode; Audio.Play("click1"); }
    public void FlipRig() { GambaController.Main.UseRigged = !GambaController.Main.UseRigged;Audio.Play("click1");}

    public void AdvanceRig(int index)
    {
        Audio.Play("click1");
        WheelIndexes[index] = (WheelIndexes[index] + 1) % 6;
    }

    public void SpawnEnemy(int enemyNum)
    {
        Audio.Play("click1");
        string name = "BoneEnemy";
        if(enemyNum == 1) name = "SkullEnemy";
        else if(enemyNum == 2) name = "MothEnemy";

        var prefab = Resources.Load("Enemies/" + name);
        var x = UnityEngine.Random.Range(4.0f, 8.0f);
        if((int)Random.Range(0, 2) == 0) x = -x;

        Instantiate(prefab, new Vector3(x, 6.5f, 0), Quaternion.identity);
    }
}
