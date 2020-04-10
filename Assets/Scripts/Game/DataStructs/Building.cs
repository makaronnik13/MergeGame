using UnityEngine;
using com.armatur.common.flags;
using com.armatur.common.serialization;
using System.Collections.Generic;
using System.Linq;
using System;

[SerializeField]
public class Building
{

    public bool InHand = false;

    [Savable]
    [SerializeData(FieldOmitName.False, FieldRequired.False)]
    public GenericFlag<int> Level = new GenericFlag<int>("Level", 0);

    [Savable]
    [SerializeData(FieldOmitName.False, FieldRequired.False)]
    public GenericFlag<int> StateId = new GenericFlag<int>("StateId", -1);

    public CellState State
    {
        get
        {
            return DefaultRessources.GetState(StateId.Value);
        }
        set
        {
            if (value==null)
            {
                StateId.SetState(-1);
            }
            else
            {
                StateId.SetState(DefaultRessources.GetStateId(value));
            }
        }
    }

    public Dictionary<Cell, CellBuff> ActiveBuffs(CellState state, int lvl = -1)
    {
        if (state == null)
        {
            state = State;
        }
        if (lvl == -1)
        {
            lvl = Level.Value;
        }
            Dictionary<Cell, CellBuff> buffs = new Dictionary<Cell, CellBuff>();
            if (state.Initial)
            {
                return buffs;
            }
            foreach (Block b in BlocksField.Instance.Blocks)
            {
                if (b.Cell.Building == this)
                {
                    continue;
                }
                foreach (CellBuff buff in b.Cell.State.buffs)
                {
                    if (buff.ChechCondition(b, BlocksField.Instance.Blocks.FirstOrDefault(bl=>bl.Cell.Building == this),b.Cell.State, state, b.Level.Value, lvl))
                    {
                        buffs.Add(b.Cell, buff);
                    }
                }
            }
          
            return buffs;

    }

    public  List<Condition> Conditions(CellState state = null)
    {
        if (state == null)
        {
            state = State;
        }

        return state.conditions.ToList();
    }

    private List<Inkome> Income(CellState state = null)
    {
        if (state)
        {
            return state.income.ToList();
        }
   
            return State.income.ToList();
    }

    private List<Inkome> currentIncome = new List<Inkome>();
    public List<Inkome> CurrentIncome(CellState state = null, int lvl = -1)
    {

        if (state)
        {
            RecalculateInkome(state, lvl);
            List<Inkome> inc = currentIncome;
            RecalculateInkome();
            return inc;
        }
        if (lvl == -1)
        {
            lvl = Level.Value;
        }

        RecalculateInkome();
        return currentIncome;
    }

    public void RecalculateInkome(CellState state = null, int lvl = -1)
    {
        if (lvl == -1)
        {
            lvl = Level.Value;
        }
        if (state == null)
        {
            state = State;
        }

        currentIncome = new List<Inkome>();

        if (!state)
        {
            return;
        }


        foreach (Inkome inc in Income(state))
        {
            Inkome newInkome = new Inkome(inc);
            newInkome.resource = inc.resource;
            newInkome.value = Mathf.RoundToInt(inc.value * DefaultRessources.Settings.IncomeCurve.Evaluate(lvl));
            currentIncome.Add(newInkome);
        }

        
        foreach (Condition c in Conditions(state))
        {
            foreach (Inkome inc in c.ActualIncome(BlocksField.Instance.Cells.FirstOrDefault(cc => cc.Building == this), state, lvl))
            {
                Inkome inkome = currentIncome.FirstOrDefault(cc => cc.resource == inc.resource);
                if (inkome != null)
                {
                    inkome.value += Mathf.RoundToInt(inc.value);
                }
                else
                {
                    currentIncome.Add(new Inkome(inkome.resource, inkome.value));
                }
            }

        }
        
        
        foreach (KeyValuePair<Cell, CellBuff> buff in ActiveBuffs(state, lvl))
        {
            List<Inkome> ink = new List<Inkome>();

            foreach (Inkome inc in buff.Value.GetInkome(state, buff.Key.Building.Level.Value))
            {

                if (currentIncome.FirstOrDefault(cc => cc.resource == inc.resource) != null)
                {
                    currentIncome.FirstOrDefault(cc => cc.resource == inc.resource).value += inc.value;
                }
                else
                {
                    currentIncome.Add(new Inkome(inc.resource,inc.value));
                }
            }
        }

        foreach (Inkome inc in currentIncome)
        {
            inc.value = Convert.ToInt64(inc.value* (1 + 100*PlayerStats.Instance.Bank.GetResource((ResType)(State.Id + 21)).ResVal.CurrentValue/10000f));
        }

        foreach (Inkome inc in currentIncome)
        {
            switch (state.Biom)
            {
                case CombineModel.Biom.Forest:
                    inc.value = Convert.ToInt64(inc.value * (1 + 100 * PlayerStats.Instance.Bank.GetResource(ResType.GroundIncome).ResVal.CurrentValue / 10000f));
                    break;
                case CombineModel.Biom.Mountains:
                    inc.value = Convert.ToInt64(inc.value * (1 + 100 * PlayerStats.Instance.Bank.GetResource(ResType.MountainsIncome).ResVal.CurrentValue / 10000f));
                    break;
                case CombineModel.Biom.Water:
                    inc.value = Convert.ToInt64(inc.value * (1 + 100 * PlayerStats.Instance.Bank.GetResource(ResType.WaterIncome).ResVal.CurrentValue / 10000f));
                    break;
            }
        }

        foreach (Inkome inc in currentIncome)
        {
            switch (state.Color)
            {
                case CombineModel.Skills.Purple:
                    inc.value = Convert.ToInt64(inc.value * (1 + 100 * PlayerStats.Instance.Bank.GetResource(ResType.BlueIncome).ResVal.CurrentValue / 10000f));
                    break;
                case CombineModel.Skills.Red:
                    inc.value = Convert.ToInt64(inc.value * (1 + 100 * PlayerStats.Instance.Bank.GetResource(ResType.RedIncome).ResVal.CurrentValue / 10000f));
                    break;
                case CombineModel.Skills.Yellow:
                    inc.value = Convert.ToInt64(inc.value * (1 + 100 * PlayerStats.Instance.Bank.GetResource(ResType.GreenIncome).ResVal.CurrentValue / 10000f));
                    break;
            }
        }

        foreach (Inkome inc in currentIncome)
        {
            switch (inc.resource.ResType)
            {
                case ResType.Money:
                    inc.value = Convert.ToInt64(inc.value * (1 + 100 * PlayerStats.Instance.Bank.GetResource(ResType.MoneyIncome).ResVal.CurrentValue / 10000f));
                    break;
                case ResType.Knowleges:
                    inc.value = Convert.ToInt64(inc.value * (1 + 100 * PlayerStats.Instance.Bank.GetResource(ResType.KnowlegesIncome).ResVal.CurrentValue / 10000f));
                    break;
                case ResType.Votes:
                    inc.value = Convert.ToInt64(inc.value * (1 + 100 * PlayerStats.Instance.Bank.GetResource(ResType.VotesInkome).ResVal.CurrentValue / 10000f));
                    break;
                case ResType.Crystalls:
                    inc.value = Convert.ToInt64(inc.value * (1 + 100 * PlayerStats.Instance.Bank.GetResource(ResType.CristallsIncome).ResVal.CurrentValue / 10000f));
                    break;
            }
        }
    }
    public Building(CellState state)
    {
        if (state.Initial)
        {
            Level.SetState(0);
        }
        else
        {
            Level.SetState(1);
        }
        StateId.SetState(DefaultRessources.GetStateId(state));
    }

    public Building(Building building)
    {
        this.State = building.State;
        this.Level.SetState(building.Level.Value);
    }
}