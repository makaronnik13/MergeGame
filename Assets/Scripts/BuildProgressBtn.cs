using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildProgressBtn : MonoBehaviour
{
    [SerializeField]
    private TMPro.TextMeshProUGUI Text;

    [SerializeField]
    private Image Fill;

    // Start is called before the first frame update
    void Start()
    {
        PlayerStats.Instance.Bank.GetResource(ResType.BuildPoints).ResVal.Current.AddListener(UpdateView);
        PlayerStats.Instance.Bank.GetResource(ResType.BuildPoints).ResVal.Max.AddListener(UpdateView);
    }

    private void UpdateView(long v)
    {
 
        Fill.fillAmount = (float)PlayerStats.Instance.Bank.GetResource(ResType.BuildPoints).ResVal.CurrentValue / PlayerStats.Instance.Bank.GetResource(ResType.BuildPoints).ResVal.MaxValue;
        Text.text = Mathf.RoundToInt(PlayerStats.Instance.Bank.GetResource(ResType.BuildPoints).ResVal.CurrentValue /100f).ToString();
    }

    public void Click()
    {
        if (PlayerStats.Instance.Bank.GetResource(ResType.YellowMana).ResVal.CurrentValue + PlayerStats.Instance.Bank.GetResource(ResType.PurpleMana).ResVal.CurrentValue + PlayerStats.Instance.Bank.GetResource(ResType.RedMana).ResVal.CurrentValue < PlayerStats.Instance.Bank.GetResource(ResType.MaxMana).ResVal.CurrentValue)
        {
            PlayerStats.Instance.Bank.ChangeResource(ResType.BuildPoints, PlayerStats.Instance.Bank.GetResource(ResType.ClickForce).ResVal.CurrentValue);
        }
    }
}
