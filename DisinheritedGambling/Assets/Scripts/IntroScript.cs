using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class IntroScript : MonoBehaviour
{
    [SerializeField] private SpriteRenderer[] Parts;

    private int Step = 0;

    void Start()
    {
        StartCoroutine(DoStep(0));
    }

    void Update()
    {
        if(Input.anyKeyDown)
        {
            Step++;
            StartCoroutine(DoStep(Step));
        }
    }

    private IEnumerator DoStep(int index)
    {
        if(index >= Parts.Length)
        {
            SceneManager.LoadScene("GameScene");
            yield break;
        }

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
