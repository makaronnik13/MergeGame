using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IncomeCounter : MonoBehaviour
{
    [SerializeField]
    private TMPro.TextMeshProUGUI Text;


    // Update is called once per frame
    void Update()
    {
        Inkome inc = BlocksField.Instance.GetInkomes().FirstOrDefault(i => i.resource.Id == (int)ResType.Money);
        if (inc!=null && inc.value>0)
        {
            Text.text = "+" + StaticTools.Format(Mathf.RoundToInt(inc.value/DefaultRessources.Settings.GenTime)) + " <sprite name=\"Money\">"+"/sec";
            Text.enabled = true;
        }
        else
        {
            Text.enabled = false;
        }
    }
}
