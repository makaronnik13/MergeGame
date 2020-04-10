using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyCounter : MonoBehaviour
{
    [SerializeField]
    private TMPro.TextMeshProUGUI text;

    // Start is called before the first frame update
    void Start()
    {
        PlayerStats.Instance.Bank.GetResource(ResType.Money).ResVal.Current.AddListener(MoneyChanged);   
    }

    private void MoneyChanged(long v)
    {
        text.text = StaticTools.Format(v)  +" <sprite name=\"Money\">";
    }
}
