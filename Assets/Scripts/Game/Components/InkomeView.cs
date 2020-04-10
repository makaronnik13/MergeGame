using System;
using System.Collections.Generic;
using UnityEngine;

public class InkomeView: MonoBehaviour
{
    [SerializeField]
    private TMPro.TextMeshProUGUI Text;

    public bool Showing = true;

    public bool ShowSign = false;

    public void Init(List<Inkome> ink)
    {
        Text.text = "";
        foreach (Inkome i in ink)
        {
            if (ShowSign)
            {
                if (i.value>0)
                {
                    Text.text += "+";
                }
                if (i.value<0)
                {
                    Text.text += "-";
                }
            }
           Text.text += StaticTools.Format(i.value)+" <sprite name="+i.resource.ResType.ToString()+">";
            if (i!=ink[ink.Count-1])
            {
                Text.text += "\n";
            }
        }
    }

    public void Update()
    {
        if (Showing)
        {
            Text.color = Color.Lerp(Text.color, new Color(Text.color.r, Text.color.g, Text.color.b, 1), Time.deltaTime*2f);
        }
        else
        {
            Text.color = Color.Lerp(Text.color, new Color(Text.color.r, Text.color.g, Text.color.b, 0), Time.deltaTime * 2f);
        }
    }
}