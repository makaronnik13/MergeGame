using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Aura", fileName = "Aura")]
public class CellBuff: ScriptableObject, ISinergy
{
    public List<Inkome> InkomeChange;
	
    public CombineModel.Biom Biom;
    public CombineModel.Skills Color = CombineModel.Skills.None;

    public Condition.BuffType ComparationLevel;

    public CellState[] States;

    public int Radius;

    public List<Inkome> GetInkome(Cell emmitor)
    {
        List<Inkome> inkomes = new List<Inkome>();
        foreach (Inkome ink in InkomeChange)
        {
            inkomes.Add(new Inkome(ink.resource, Mathf.RoundToInt(ink.value * DefaultRessources.Settings.IncomeCurve.Evaluate(emmitor.Building.Level.Value))));
        }
        return inkomes;
    }

    public List<Inkome> GetInkome(CellState state, int lvl)
    {
        List<Inkome> inkomes = new List<Inkome>();
        foreach (Inkome ink in InkomeChange)
        {
            inkomes.Add(new Inkome(ink.resource, Mathf.RoundToInt(ink.value * DefaultRessources.Settings.IncomeCurve.Evaluate(lvl))));
        }
        return inkomes;
    }

    public bool ChechCondition(Block emmitor, Block aim, CellState state1, CellState state2, int lvl1, int lvl2)
    {
        if (emmitor == aim)
        {
            return false;
        }


        if (lvl2 == 0)
        {
            return false;
        }

        if (!BlocksField.Instance.GetBlocksInRadius(emmitor, Radius).Contains(aim))
        {
            return false;
        }

        if (ComparationLevel == Condition.BuffType.Equal)
        {
            return lvl1 == lvl2;
        }

        if (ComparationLevel == Condition.BuffType.More)
        {
            return lvl1 < lvl2;
        }

        if (ComparationLevel == Condition.BuffType.Less)
        {
            return lvl1 > lvl2;
        }

        if (States.Length > 0)
        {
            return States.ToList().Contains(state2);
        }

        if (Color != CombineModel.Skills.None)
        {
            return Color == state2.Color;
        }


        return Biom == state2.Biom;
    }

    public bool ChechCondition(Block emmitor, Block aim)
    {
        return ChechCondition(emmitor, aim, emmitor.Cell.State, aim.Cell.State, emmitor.Level.Value, aim.Level.Value);
    }



    public string Description(int lvl)
    {
        string s = "";

        if (Radius == 1)
        {
            s += "Соседние здания";
        }
        else 
        {
            s += "Здания в радиусе " + Radius;
        }

        if (States.Count()>0)
        {
            foreach (CellState cs in States)
            {
                s += " "+cs.StateName;
            }
        }
        else if(Color != CombineModel.Skills.None)
        {
            s += "<sprite name=" + Color.ToString() + ">";
        }
        else if (ComparationLevel!= Condition.BuffType.ForEvery)
        {
            switch (ComparationLevel)
            {
                case Condition.BuffType.Equal:
                    s += " такого же уровня";
                    break;
                case Condition.BuffType.Less:
                    s += " меньшего уровня";
                    break;
                case Condition.BuffType.More:
                    s += " большего уровня";
                    break;
            }
        }
        else if(Biom!= CombineModel.Biom.None)
        {
         s += " <sprite name=" + Biom.ToString() + ">";
        }

        s += "\n"+"получают";
        foreach (Inkome ink in InkomeChange)
        {     
            s += " +" + Mathf.RoundToInt(DefaultRessources.Settings.IncomeCurve.Evaluate(lvl)*ink.value) + " " + " <sprite name=" + ink.resource.ResType.ToString() + ">";
        }

        return s;
    }
}
