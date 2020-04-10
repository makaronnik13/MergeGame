using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceView : MonoBehaviour
{
    [SerializeField]
    private ResType Res;
    [SerializeField]
    private TMPro.TextMeshProUGUI Text;
    [SerializeField]
    private bool ShowSign;
    [SerializeField]
    private bool Colorize;


    // Start is called before the first frame update
    void Start()
    {
        PlayerStats.Instance.Bank.GetResource(Res).ResVal.AddListener(ValChanged);
    }

    private void ValChanged(long v)
    {
        Text.text = "";
        if (ShowSign)
        {
            if (v>0)
            {
                Text.text += "+";
            }
        }
        Text.text += v.ToString();
        Text.enabled = v != 0;

        if (Colorize)
        {
            if (v > 0)
            {
                Text.color = Color.green;
            }
            else
            {
                Text.color = Color.red;
            }
        }

        string icon = Res.ToString();
        if (Res == ResType.MoneyChangeInc)
        {
            icon = ResType.Money.ToString();
        }
        Text.text += "<sprite name=\"" + icon+"\">";
    }
}
