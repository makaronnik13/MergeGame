using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopBtn: MonoBehaviour
{
    private Action onClick;
    private Perk perk;

    [SerializeField]
    private InkomeView InkView;

    [SerializeField]
    private Transform subPerks;

    [SerializeField]
    private TMPro.TextMeshProUGUI PerkName;

    public void Init(Perk key, int value, Action updateShopBtns)
    {
        perk = key;

        onClick = updateShopBtns;
        foreach (Inkome ink in perk.cost)
        {

            PlayerStats.Instance.Bank.GetResource(ink.resource).ResVal.AddListener(ResChanged);
        }

        //GetComponent<Button>().enabled = perk.Subperks.Count == 0;

        foreach (Transform t in subPerks)
        {
            t.gameObject.SetActive(perk.Subperks.Count>0);
        }

        int i = 0;
        foreach (Perk p in perk.Subperks)
        {
            subPerks.GetChild(i).GetComponentInChildren<TMPro.TextMeshProUGUI>().text = p.Description;
            i++;
        }

        PerkName.text = perk.Description;
        if (value!=-1 && perk.Subperks.Count>0)
        {
            PerkName.text = perk.Subperks[value].Description;
        }
        PerkName.enabled = perk.Subperks.Count == 0 || value!=-1;
        subPerks.gameObject.SetActive(perk.Subperks.Count>0 && value == -1);
        InkView.Init(perk.cost);

        if (value>-1)
        {
            Color color;
            ColorUtility.TryParseHtmlString("#FCFC66FC", out color);
            GetComponent<Image>().color = color;
        }
        else
        {
            GetComponent<Image>().color = Color.white;
        }
    }

    public void Purchase(int i)
    {
        if (perk.Subperks.Count != 0 && i == 0)
        {
            return;
        }
        if (perk.Subperks.Count != 0)
        {
            i--;
        }

        Debug.Log(i);
        PlayerStats.Instance.Perks.BuyPerk(perk, i);

        onClick();
    }

    private void OnDestroy()
    {
        if (perk == null)
        {
            return;
        }
        foreach (Inkome ink in perk.cost)
        {
            PlayerStats.Instance.Bank.GetResource(ink.resource).ResVal.RemoveListener(ResChanged);
        }
    }

    private void ResChanged(long v)
    {
        GetComponent<Button>().interactable = perk.CanBeBought;
        foreach (Button b in subPerks.GetComponentsInChildren<Button>())
        {
            b.interactable = perk.CanBeBought;
        }
    }
}