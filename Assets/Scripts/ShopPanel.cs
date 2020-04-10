using com.armatur.common.flags;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ShopPanel : Singleton<ShopPanel>
{
    [SerializeField]
    private GameObject View;

    [SerializeField]
    private GameObject PerkViewPrefab;

    [SerializeField]
    private Transform typeTabs, colotTabs, biomTabs;

    [SerializeField]
    private Transform generalContent, buildingContent, buildingView;

    private Dictionary<ShopBtn, Perk> Btns  = new Dictionary<ShopBtn, Perk>();

    public enum UpgradeType
    {
        General = 0,
        Building = 1
    }

    private GenericFlag<UpgradeType> UpType = new GenericFlag<UpgradeType>("upType", UpgradeType.General);
    private GenericFlag<CombineModel.Biom> Biom = new GenericFlag<CombineModel.Biom>("biom", CombineModel.Biom.Mountains);
    private GenericFlag<CombineModel.Skills> Skill = new GenericFlag<CombineModel.Skills>("skill", CombineModel.Skills.Red);

    private CellState State
    {
        get
        {
            return DefaultRessources.CellStates.FirstOrDefault(s => s.Color == Skill.Value && !s.Initial && s.Biom == Biom.Value);
        }
    }


    private void ColorChanged(CombineModel.Skills col)
    {
        for (int i = 0; i < 3; i++)
        {
            Image img = colotTabs.GetComponentsInChildren<Button>()[i].GetComponent<Image>();
            img.color = new Color(img.color.r, img.color.g, img.color.b, 0.2f);
            if ((i == 0 && col == CombineModel.Skills.Red)|| (i == 1 && col == CombineModel.Skills.Yellow)|| (i == 2 && col == CombineModel.Skills.Purple))
            {
                img.color = new Color(img.color.r, img.color.g, img.color.b, 1f);
            }
        }
        UpdateShopBtns();
    }

    private void BiomChanged(CombineModel.Biom biom)
    {
        for (int i = 0; i < 3; i++)
        {
            Image img = biomTabs.GetComponentsInChildren<Button>()[i].GetComponent<Image>();
            img.color = new Color(img.color.r, img.color.g, img.color.b, 0.2f);
            if ((i == 0 && biom == CombineModel.Biom.Mountains) || (i == 1 && biom == CombineModel.Biom.Forest) || (i == 2 && biom == CombineModel.Biom.Water))
            {
                img.color = new Color(img.color.r, img.color.g, img.color.b, 1f);
            }
        }
        UpdateShopBtns();
    }

    private void UpTypeChanged(UpgradeType ut)
    {
        for (int i = 0; i < 2; i++)
        {
            Image img = typeTabs.GetComponentsInChildren<Button>()[i].GetComponent<Image>();
            if (i == (int)ut)
            {
                img.color = new Color(img.color.r, img.color.g, img.color.b, 1);
            }
            else
            {
                img.color = new Color(img.color.r, img.color.g, img.color.b, 0.2f);
            }
            
        }
        generalContent.gameObject.SetActive(ut == UpgradeType.General);
        buildingView.gameObject.SetActive(ut == UpgradeType.Building);
        UpdateShopBtns();
    }

    public void Show(CellState state)
    {
        UpType.SetState(UpgradeType.Building);
        Biom.SetState(state.Biom);
        Skill.SetState(state.Color);
        Show();
    }

    public void Show()
    {
        View.SetActive(true);

        UpdateShopBtns();

        UpType.AddListener(UpTypeChanged);
        Biom.AddListener(BiomChanged);
        Skill.AddListener(ColorChanged);
    }

    private void Start()
    {
        foreach (KeyValuePair<Perk, int> pair in PlayerStats.Instance.Perks.PerksChoose.OrderBy(p => p.Key.cost[0].value))
        {
            CreateBtn(pair.Key, pair.Value);
        }
        foreach (KeyValuePair<Perk, int> pair in PlayerStats.Instance.Perks.BuildingsPerksChoose[State].OrderBy(p => p.Key.cost[0].value))
        {
            CreateBtn(pair.Key, pair.Value);
        }
    }

    private void UpdateShopBtns()
    {
        foreach (KeyValuePair<ShopBtn, Perk> pair in Btns)
        {
            if (pair.Value.Building)
            {
                pair.Key.Init(pair.Value, PlayerStats.Instance.Perks.BuildingsPerksChoose[pair.Value.Building][pair.Value], UpdateShopBtns);
            }
            else
            {
                pair.Key.Init(pair.Value, PlayerStats.Instance.Perks.PerksChoose[pair.Value], UpdateShopBtns);
            }
        }

     
    }


    private void CreateBtn(Perk key, int value)
    {
        GameObject newBtn = Instantiate(PerkViewPrefab);
        if (key.Building)
        {   
                newBtn.transform.SetParent(buildingContent.GetChild((int)key.Building.Color-1).GetChild((int)key.Building.Biom).GetComponentInChildren<ScrollRect>().content);
        }
        else
        {
            newBtn.transform.SetParent(generalContent.GetComponent<ScrollRect>().content);
        }
        newBtn.transform.localScale = Vector3.one;
        newBtn.transform.localPosition = Vector3.zero;
        newBtn.transform.localRotation = Quaternion.identity;
        Btns.Add(newBtn.GetComponent<ShopBtn>(), key);
        UpdateShopBtns();
    }

    public void Hide()
    {
        View.SetActive(false);

        UpType.RemoveListener(UpTypeChanged);
        Biom.RemoveListener(BiomChanged);
        Skill.AddListener(ColorChanged);
    }

    public void SetMode(int i)
    {
        UpType.SetState((UpgradeType)i);
    }

    public void SetBiom(int biom)
    {
        Biom.SetState((CombineModel.Biom)biom);
    }

    public void SetColor(int color)
    {
        Skill.SetState((CombineModel.Skills)color);
    }
}
