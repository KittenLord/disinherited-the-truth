using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour
{
    public static UI Main { get; private set; }

    [SerializeField] private GameObject GambaMenu;
    [SerializeField] private GameObject InteractTip_;

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
        CloseGambaMenu();
    }

    public void CloseGambaMenu()
    {
        PlayerController.Main.canInteract = true;
        GambaMenu.SetActive(false);
        PlayerController.Main.transform.Translate(Vector3.right * 0.01f);
        PlayerController.Main.transform.Translate(Vector3.right *-0.01f);
    }
}
