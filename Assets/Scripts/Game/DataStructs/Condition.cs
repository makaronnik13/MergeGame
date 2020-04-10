using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

[CreateAssetMenu(menuName = "Sinergy", fileName = "Sinergy")]
public class Condition: ScriptableObject, ISinergy
{
	public enum BuffType
	{
		ForEvery,
        More,
        Less,
        Equal
    }

    public List<Inkome> InkomeChange;
    public int radius;
    public bool onlyDifferent = false;

    public CombineModel.Skills Color;
    public CombineModel.Biom Biom;

    public BuffType ComparationLevel;

    public CellState[] States;

    public BuffType Comparation;

    public int Count;

    public List<Inkome> ActualIncome(Cell c)
    {
        List<Inkome> ink = new List<Inkome>();

        foreach (Inkome inc in InkomeChange)
        {
            ink.Add(new Inkome(inc.resource,inc.value));
        }

        //if building on field
        if (c != null)
        {
            foreach (Inkome inc in ink)
            {
                inc.value *= Mathf.RoundToInt(DefaultRessources.Settings.IncomeCurve.Evaluate(c.Building.Level.Value));
            }

            foreach (Inkome inc in ink)
            {
                switch (Comparation)
                {
                    case BuffType.ForEvery:
                        inc.value *= ChechCondition(BlocksField.Instance.Cells.First(cell => cell.Building == c.Building));
                        break;
                    case BuffType.More:
                        if (ChechCondition(BlocksField.Instance.Cells.First(cell => cell.Building == c.Building)) > Count)
                        {
                            return ink;
                        }
                        else
                        {
                            return new List<Inkome>();
                        }
                    case BuffType.Less:
                        if (ChechCondition(BlocksField.Instance.Cells.First(cell => cell.Building == c.Building)) < Count)
                        {
                            return ink;
                        }
                        else
                        {
                            return new List<Inkome>();
                        }
                    case BuffType.Equal:
                        if (ChechCondition(BlocksField.Instance.Cells.First(cell => cell.Building == c.Building)) == Count)
                        {
                            return ink;
                        }
                        else
                        {
                            return new List<Inkome>();
                        }
                }

            }
        }

        return ink;
    }

    public List<Inkome> ActualIncome(Cell cell, CellState state, int lvl)
    {
        Block block = BlocksField.Instance.Blocks.FirstOrDefault(b=>b.Cell == cell);
        List<Inkome> ink = new List<Inkome>();

        foreach (Inkome inc in InkomeChange)
        {
            ink.Add(new Inkome(inc.resource, inc.value));
        }

        //if building on field
        if (cell != null)
        {
            foreach (Inkome inc in ink)
            {
                inc.value *= Mathf.RoundToInt(DefaultRessources.Settings.IncomeCurve.Evaluate(lvl));
            }

            foreach (Inkome inc in ink)
            {
                switch (Comparation)
                {
                    case BuffType.ForEvery:
                        inc.value *= ChechCondition(block,state, lvl).Count;
                        break;
                    case BuffType.More:
                        if (ChechCondition(block, state, lvl).Count > Count)
                        {
                            return ink;
                        }
                        else
                        {
                            return new List<Inkome>();
                        }
                    case BuffType.Less:
                        if (ChechCondition(block, state, lvl).Count < Count)
                        {
                            return ink;
                        }
                        else
                        {
                            return new List<Inkome>();
                        }
                    case BuffType.Equal:
                        if (ChechCondition(block, state, lvl).Count == Count)
                        {
                            return ink;
                        }
                        else
                        {
                            return new List<Inkome>();
                        }
                }

            }
        }

        return ink;
    }

    public int ChechCondition(Cell cell)
    {
        Block b = BlocksField.Instance.Blocks.FirstOrDefault(bb=>bb.Cell == cell);
        List<CellState> states = ChechCondition(b, cell.State, b.Level.Value).Select(bb=>bb.Cell.State).ToList();
        if (onlyDifferent)
        {
            states = states.Distinct().ToList();
        }
        return states.Count;
    }

    public string Description(Cell cell)
    {
        if (cell == null)
        {
            return "";
        }

        string descr = "";
        string colTag = "";
        string biomTag = "";
        string LvlTag = "";
        string rangeTag = "";
        string statesTag = "";

        if (radius == 1)
        {
            rangeTag = "рядом ";
        }
        else if (radius>=10)
        {
            rangeTag = "";
        }
        else
        {
            rangeTag = "в радиусе " + radius+" ";
        }

        switch (ComparationLevel)
        {
            case BuffType.ForEvery:
                break;
            case BuffType.Equal:
                LvlTag ="такого же уровня ";
                break;
            case BuffType.Less:
                LvlTag = "меньшего уровня ";
                break;
            case BuffType.More:
                LvlTag = "большего уровня ";
                break;
        }

        if (Biom!= CombineModel.Biom.None)
        {
                biomTag = "<sprite name=" + Biom.ToString() + "> ";
        }

        biomTag += LvlTag;

        foreach (CellState cs in States)
        {
            statesTag += cs.StateName;
            if (States.Count()>1 && cs!=States[States.Count()-1])
            {
                if (States.Count()>1 && cs != States[States.Count() - 2])
                {
                    statesTag += " и ";
                }
                else
                {
                    statesTag += ", ";
                }
            }
            else
            {
                statesTag += " ";
            }
        }


        biomTag = statesTag + biomTag;

        if (Color!= CombineModel.Skills.None)
        {
            colTag = "<sprite name=" + Color.ToString() + "> ";        
        }

        foreach (Inkome ink in InkomeChange)
        {
            descr += "+" + Mathf.RoundToInt(ink.value * DefaultRessources.Settings.IncomeCurve.Evaluate(cell.Building.Level.Value)) + " " + " <sprite name=" + ink.resource.ResType.ToString() + ">" + " ";
        }

        switch (Comparation)
        {
            case BuffType.ForEvery:
                descr += "за каждое ";
                if (onlyDifferent)
                {
                    descr += "уникальное ";
                }
                descr += colTag + biomTag;
                if (States.Count() == 0)
                {
                    descr += " здание ";
                }
                descr += rangeTag;
                break;
            case BuffType.Equal:
                descr += "если ";
                if (onlyDifferent)
                {
                    descr += "уникальных ";
                }
                descr += colTag + biomTag;
                if (States.Count()==0)
                {
                    descr+="зданий ";
                }
                descr+=rangeTag + Count+" ";
                break;
            case BuffType.Less:
                descr += "если ";
                if (onlyDifferent)
                {
                    descr += "уникальных ";
                }
                descr += colTag+ biomTag+"зданий "+ rangeTag +"меньше " + Count + " ";
                break;
            case BuffType.More:
                descr += "если ";
                if (onlyDifferent)
                {
                    descr += "уникальных ";
                }
                descr += colTag + biomTag + "зданий "+ rangeTag +"больше " + Count + " ";
                break;
        }

       

       

      

        if (ActualIncome(cell).Count()==0)
        {
            descr = "<alpha=#AA>" + descr + "<alpha=#AA>";
        }

        return descr;
    }

    public List<Block> ChechCondition(Block b, CellState state, int lvl)
    {
        List<Block> blocks = new List<Block>();

        blocks = BlocksField.Instance.GetBlocksInRadius(b, b.Radius).Where(bb=>!bb.Cell.Building.State.Initial).ToList();

        if (States.Count() != 0)
        {
           blocks = blocks.Where(bb => States.Contains(bb.Cell.State)).ToList();
        }
        else if (Color != CombineModel.Skills.None)
        {
            blocks = blocks.Where(bb => bb.Cell.Color == Color).ToList();
        }
        else if (Biom != CombineModel.Biom.None)
        {
            blocks = blocks.Where(bb => bb.Cell.Biom.Value == Biom).ToList();
        }

        switch (ComparationLevel)
        {
            case BuffType.Equal:
                blocks = blocks.Where(cc=>cc.Cell.Building.Level.Value == lvl).ToList();
                break;
            case BuffType.ForEvery:
                break;
            case BuffType.Less:
                blocks = blocks.Where(cc => cc.Cell.Building.Level.Value < lvl).ToList();
                break;
            case BuffType.More:
                blocks = blocks.Where(cc => cc.Cell.Building.Level.Value > lvl).ToList();
                break;
        }

        return blocks;
    }
}

