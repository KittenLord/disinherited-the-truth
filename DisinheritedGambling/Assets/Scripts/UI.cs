using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour
{
    public static UI Main { get; private set; }

    [SerializeField] private GameObject GambaMenu;
    [SerializeField] private GameObject InteractTip_;
    [SerializeField] private CanvasGroup HitLeverTip_;
    [SerializeField] private Transform HealthBar;

    void Start()
    {
        if(Main != null) Destroy(Main);
        Main = this;
    }

    public void OpenGambaMenu()
    {
        GambaMenu.SetActive(true);
    }

    public void InteractTip(bool t)
    {
        InteractTip_.SetActive(t);
    }

    public void ConfirmGambaMenu()
    {
        PlayerController.Main.Health -= PlayerController.Main.MaxHealth * 0.5f;
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
        PlayerController.Main.canInteract = true;
        GambaMenu.SetActive(false);
        PlayerController.Main.transform.Translate(Vector3.right * 0.01f);
        PlayerController.Main.transform.Translate(Vector3.right *-0.01f);
    }

    public void SetHealthBar(float value)
    {
        value = Mathf.Clamp01(value);
        HealthBar.localScale = new Vector3(1, value, 1);
    }
}
