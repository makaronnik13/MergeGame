using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestBustBtn : MonoBehaviour
{
    [SerializeField]
    private float BustTime = 36f, BustForce = 100f;

    private float t;

    [SerializeField]
    private Image fillImg;

    public void Bust()
    {
        if (t<=0)
        {
            t = BustTime;
            Time.timeScale = BustForce;
        }
        else
        {
            Time.timeScale = 0;
        }
    }

    private void Update()
    {
        fillImg.fillAmount = t / BustTime;
        if (t>0)
        {
            t -= Time.deltaTime/BustForce;
        }
        else
        {
            t = 0;
            Time.timeScale = 1;
        }
    }
}
