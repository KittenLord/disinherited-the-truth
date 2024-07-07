using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class IntroScript : MonoBehaviour
{
    [SerializeField] private Image Tip;
    [SerializeField] private Image StartButton;

    [SerializeField] private Image[] Parts;

    private int Step = 0;

    void Start()
    {
        GetComponent<AudioSource>().Play();
        StartCoroutine(DoStep(0));
    }

    private bool TipActivated = false;
    void Update()
    {
        if(Time.time > 5 && !TipActivated && Step == 0)
        {
            StartCoroutine(SetImage(Tip, true));
        }

        if(Input.anyKeyDown)
        {
            Step++;
            StartCoroutine(DoStep(Step));
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    private IEnumerator SetImage(Image img, bool value)
    {
        if(value) img.gameObject.SetActive(true);
        if(img.color.a > 0 && value) yield break; 
        if(img.color.a < 1 && !value) yield break; 

        float time = value ? 0 : 1;
        while(value ? time < 1 : time > 0)
        {
            time += Time.deltaTime * (value ? 1 : -1);
            img.color = new Color(1, 1, 1, time);
            yield return null;
        }

        img.color = new Color(1, 1, 1, value ? 1 : 0);
    }

    private IEnumerator DoStep(int index)
    {
        StartCoroutine(SetImage(Tip, false));
        if(index >= Parts.Length) { StartCoroutine(SetImage(StartButton, true)); yield break; }

        float time = 0;
        while(time < 2)
        {
            time += Time.deltaTime;
            Parts[index].color = new Color(1, 1, 1, time/2);
            yield return null;
        }

        Parts[index].color = new Color(1, 1, 1, 1);
    }
}
