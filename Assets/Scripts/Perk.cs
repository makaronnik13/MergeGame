using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Perk", fileName = "Perk")]
public class Perk : ScriptableObject
{

   public enum PerkType
    {
        ModifyRes = 0,
        PerkForBuilding = 1,
        PerkForBiome = 2,
        PerkForColor = 3
    }

    public string PerkName;
    public PerkType PType;
    public CombineModel.Biom Biom = CombineModel.Biom.None;
    public CellState Building;
    public CombineModel.Skills Color;
    public long Value;
    public GameResource Resource;

    public long NeedKnolwge;

    public List<Perk> Subperks;

    public List<Inkome> cost;

    public string Description
    {
        get
        {
            switch (PType)
            {
                case PerkType.ModifyRes:
                    string s = "Увеличивает "+ Bank.GetResName(Resource.ResType);
                    s += " на " + Value;
                    if (Resource.ResType != ResType.ClickForce && Resource.ResType != ResType.BuildSpeed && Resource.ResType != ResType.OpenedSlots && Resource.ResType!= ResType.OpenedFields)
                    {
                        s += "%";
                    }
                    return s;
            }
            return name;
        }
    }

    public bool CanBeBought
    {
        get
        {
            bool res = true;

            foreach (Inkome c in cost)
            {
                if (PlayerStats.Instance.Bank.GetResource(c.resource).ResVal.CurrentValue < c.value)
                {
                    res = false;
                }
            }

            res = res && PlayerStats.Instance.Perks.PerkLvl(this) == -1;

            return res;
        }
    }

    public List<Inkome> Cost()
    {
        return cost;
    }
}
