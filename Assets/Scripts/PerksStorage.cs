using System;
using System.Collections.Generic;
using System.Linq;

public class PerksStorage
{
    public Dictionary<Perk, int> PerksChoose = new Dictionary<Perk, int>();
    public Dictionary<CellState, Dictionary<Perk, int>> BuildingsPerksChoose = new Dictionary<CellState, Dictionary<Perk, int>>();

    public PerksStorage()
    {
        PerksChoose = new Dictionary<Perk, int>();
        foreach (Perk p in DefaultRessources.Settings.Perks)
        {
            PerksChoose.Add(p, -1);
        }

        foreach (CellState cs in DefaultRessources.CellStates.Where(s=>!s.Initial))
        {
            Dictionary<Perk, int> statePerks = new Dictionary<Perk, int>();
            foreach (Perk p in cs.Perks)
            {
                statePerks.Add(p, -1);
            }
            BuildingsPerksChoose.Add(cs, statePerks);
        }
    } 

    public void BuyPerk(Perk perk, int variant = 0)
    {
        foreach (Inkome cost in perk.cost)
        {
            PlayerStats.Instance.Bank.ChangeResource(cost.resource.ResType, -cost.value);
        }
        ActivatePerk(perk, variant);
    }

    public void ActivatePerk(Perk perk, int variant = 0)
    {
        if (perk.Subperks.Count == 0)
        {
            UsePerk(perk);
        }
        else
        {
            UsePerk(perk.Subperks[variant]);
        }

        if (PerksChoose.ContainsKey(perk))
        {          
            PerksChoose[perk] = variant;
        }
        else
        {
            BuildingsPerksChoose.FirstOrDefault(k=>k.Value.ContainsKey(perk)).Value[perk] = variant;
        }
    }

    private void UsePerk(Perk perk)
    {
        switch (perk.PType)
        {
            case Perk.PerkType.ModifyRes:
                PlayerStats.Instance.Bank.ChangeResource(perk.Resource.ResType, perk.Value);
                break;
        }
    }

    public int PerkLvl(Perk perk)
    {
        if (PerksChoose.ContainsKey(perk))
        {
            return PerksChoose[perk];
        }
        else
        {
            return BuildingsPerksChoose.FirstOrDefault(k => k.Value.ContainsKey(perk)).Value[perk];
        }

        return -1;
    }
}