using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CellInkomeView : MonoBehaviour
{
    [SerializeField]
    private InkomeView CurrentInkomes, ExtraInkomes;

    public void SetAimed(bool v)
    {
        CurrentInkomes.Showing = v;
    }

    private Cell cell = null;

    public void Init(Cell cell)
    {
        this.cell = cell;
        ExtraInkomes.ShowSign = true;
    }

    private void Update()
    {
        if (cell!=null)
        {
            CurrentInkomes.Init(cell.Building.CurrentIncome());
        }
    }

    public void SetTemp(Block block, CellState state, int lvl = 1)
    {
        PlayerStats.Instance.Bank.SetResource(ResType.MoneyChangeInc, 0);

        foreach (Block b in BlocksField.Instance.Blocks)
        {
            b.BCanvas.InkomeView.ShowExtra(new List<Inkome>());
        }
      
        if (state == null)
        {
            ExtraInkomes.Init(new List<Inkome>());
            SynergyLineDrawer.Instance.Hide();
            foreach (Block b in BlocksField.Instance.Blocks)
            {
                b.BCanvas.InkomeView.ShowExtra(new List<Inkome>());
            }
        }
        else
        {
            ExtraInkomes.Init(new List<Inkome>());

            if (block.Cell.State.Biom!=state.Biom || block.Blocked.Value)
            {
                return;
            }
            List<LineInfo> lines = new List<LineInfo>();
            List<BuffLineInfo> lineInfo = new List<BuffLineInfo>();

            //sending buffs
            int buffid = 0;

            foreach (CellBuff cb in state.buffs)
            {
                Dictionary<Block, List<Inkome>> bb = new Dictionary<Block, List<Inkome>>();

                foreach (Block b in BlocksField.Instance.Blocks)
                {
                    if (cb.ChechCondition(block, b, state, b.Cell.State, lvl, b.Level.Value))
                    {
                        bb.Add(b, cb.GetInkome(state, lvl));
                    }
                }
                if (bb.Count>0)
                {
                    lineInfo.Add(new BuffLineInfo(buffid, true, bb));
                    buffid++;
                }
            }

            //recievng buffs2          
                foreach (Block b in BlocksField.Instance.Blocks)
                {
                    foreach (CellBuff cb in b.Cell.State.buffs)
                    {
                    Dictionary<Block, List<Inkome>> bb = new Dictionary<Block, List<Inkome>>();

                    if (cb.ChechCondition(b, block, b.Cell.State, state, b.Level.Value, lvl))
                        {
                            bb.Add(b, cb.GetInkome(state, b.Cell.Building.Level.Value));
                        }
                        if (bb.Count > 0)
                        {
                            lineInfo.Add(new BuffLineInfo(0,false, bb));
                        }
                }
               
            }

            //conditions
            foreach (Condition cb in state.conditions)
            {
                foreach (Block b in cb.ChechCondition(block, state, lvl))
                {
                    Dictionary<Block, List<Inkome>> cond = new Dictionary<Block, List<Inkome>>();
                    cond.Add(b, cb.ActualIncome(block.Cell, state, lvl));
                    lineInfo.Add(new BuffLineInfo(buffid, false, cond, true));
                }
            }

            buffid = 0;
            //input conditions
            foreach (Block b in BlocksField.Instance.Blocks)
            {
                foreach (Condition cb in b.Cell.State.conditions)
                {
                    if (cb.ChechCondition(b, b.Cell.State, b.Level.Value).Contains(block))
                    {
                        Dictionary<Block, List<Inkome>> cond = new Dictionary<Block, List<Inkome>>();
                        cond.Add(b, cb.ActualIncome(b.Cell, b.Cell.State,b.Level.Value));
                        lineInfo.Add(new BuffLineInfo(buffid, true, cond,true));
                        buffid++;
                    }
                }
            }


            foreach (BuffLineInfo bli in lineInfo)
            {
                foreach (KeyValuePair<Block, List<Inkome>> pair in bli.buffs)
                {
                    lines.Add(new LineInfo(pair.Key.transform.position, GetComponentInParent<Block>().transform.position, bli));
                }
                
            }

            SynergyLineDrawer.Instance.Show(lines);
            ExtraInkomes.Init(cell.Building.CurrentIncome(state, lvl));
            ExtraInkomes.Showing = true;

            Dictionary<Block, List<Inkome>> extraIncomes = new Dictionary<Block, List<Inkome>>();
            foreach (BuffLineInfo bli in lineInfo)
            {
                if (bli.emmiting == true)
                {
                    foreach (KeyValuePair<Block, List<Inkome>> bb in bli.buffs)
                    {
                        if (extraIncomes.ContainsKey(bb.Key))
                        {
                            foreach (Inkome ink in bb.Value)
                            {
                                if (extraIncomes[bb.Key].FirstOrDefault(i=>i.resource == ink.resource)!=null)
                                {
                                    extraIncomes[bb.Key].FirstOrDefault(i => i.resource == ink.resource).value += ink.value;
                                }
                                else
                                {
                                    extraIncomes[bb.Key].Add(ink);
                                }
                            }
                        }
                        else
                        {
                            extraIncomes.Add(bb.Key, bb.Value);
                        }
                    }
                }
            }

            foreach (KeyValuePair<Block,List<Inkome>> pair in extraIncomes)
            {
              //  if (pair.Key!=block)
               // {
                    pair.Key.BCanvas.InkomeView.ShowExtra(pair.Value);
               // }
            }

            List<Inkome> changes = BlocksField.Instance.GetInkomes();


            Block fromBlock = null;
            CellState fromState = null;
            int fromLvl = 0;

            if (BuildingsDragManager.Instance.building!=null)
            {

                fromBlock = BuildingsDragManager.Instance.fromBlock;
                fromState = BuildingsDragManager.Instance.building.State;
                fromLvl = BuildingsDragManager.Instance.building.Level.Value;
                if (block.Cell.Building.Level.Value==0 && BuildingsDragManager.Instance.fromBlock)
                {
                    fromLvl = BuildingsDragManager.Instance.fromBlock.Level.Value;
                    fromState = BuildingsDragManager.Instance.fromBlock.Cell.State;
                }
            }

            foreach (Inkome ink in BlocksField.Instance.GetInkomes(block, state, lvl, fromBlock, fromState, fromLvl))
            {
                if (changes.FirstOrDefault(i=>i.resource == ink.resource)!=null)
                {
                    changes.FirstOrDefault(i => i.resource == ink.resource).value = ink.value;
                }
                else
                {
                    changes.Add(new Inkome(ink.resource, ink.value));
                }
            }

            if (changes.FirstOrDefault(i => i.resource.ResType == ResType.Money)!=null)
            {
                PlayerStats.Instance.Bank.SetResource(ResType.MoneyChangeInc, (long)(changes.FirstOrDefault(i => i.resource.ResType == ResType.Money).value/DefaultRessources.Settings.GenTime));
            }
            else
            {
                PlayerStats.Instance.Bank.SetResource(ResType.MoneyChangeInc,0);
            }
        }
    }

    public void Clear()
    {
        SynergyLineDrawer.Instance.Hide();
        foreach (Block b in BlocksField.Instance.Blocks)
        {
            b.BCanvas.InkomeView.SetAimed(false);
            b.BCanvas.InkomeView.ShowExtra(new List<Inkome>());
        }
        PlayerStats.Instance.Bank.SetResource(ResType.MoneyChangeInc, 0);
    }

    private void ShowExtra(List<Inkome> value)
    {
        ExtraInkomes.Init(value);
        ExtraInkomes.ShowSign = true;
        ExtraInkomes.Showing = value.Count>0;
    }
}

