using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using com.armatur.common.flags;

public class InformationPanel : Singleton<InformationPanel>
{
    public Image icon;
	public TMPro.TextMeshProUGUI blockName;
    public TMPro.TextMeshProUGUI symbiosys;
    public TMPro.TextMeshProUGUI lvl;
    public InkomeView incomeView;
    public GameObject View;

	private Building showingBuilding;
	
    public void Close()
    {
        View.SetActive(false);
    }

    public void ShowInfo(Building building)
    {
        View.SetActive(true);

        lvl.text = building.Level.Value.ToString();
           
            blockName.text = building.State.StateName;

            icon.sprite = building.State.Sprite;


            incomeView.Init(building.CurrentIncome());


            showingBuilding = building;
            symbiosys.text = ApplyTags();

    }

	private string ApplyTags()
	{
        string s = "";

        Block block = BlocksField.Instance.Blocks.FirstOrDefault(b => b.Cell.Building == showingBuilding);
        Cell cell = null;

        if (block)
        {
            cell = block.Cell;
        }

        //int i = 0;
        foreach (Condition c in  showingBuilding.Conditions())
        {
            s += c.Description(cell)+"\n";
            //ReplaceNextConditionTag(ref s, i, c.ChechCondition(BlocksField.Instance.Cells.FirstOrDefault(b=>b.Building == showingBuilding))>0);
            //i++;
        }

        foreach (CellBuff buff in showingBuilding.State.buffs)
        {     
            s += buff.Description(showingBuilding.Level.Value) + "\n";
        }
		return s;
	}

    private void ReplaceNextConditionTag(ref string s, int i, bool condition)
    {
        if (s.Contains("<if" + i + ">"))
        {
            if (condition)
            {
                s = s.Replace("</if" + i + ">", "</color></b>");
                s = s.Replace("<if" + i + ">", "<b><color=#FFFFFF>");
            }
            else {
                s = s.Replace("</if" + i + ">", "</color>");
                s = s.Replace("<if" + i + ">", "<color=#a9a9ad>");
            }
        }
    }

    private string GetNextResourceName(ref string s)
	{
		
		string result = "";

		if(s.Contains("["))
		{
			int first = s.IndexOf ("[");
			int second = s.IndexOf ("]");

			result = s.Substring (first, second-first+1);


            string resName = result.Substring(1, result.Length-2);

            List<GameResource> resourcesList = new List<GameResource>();


			s = s.Replace (result, "<sprite index="+ resourcesList.IndexOf(resourcesList.Find(r=>r.name == resName)) +">");
			result = result.Substring (1, result.Length-2);
		}

		return result;
	}

    public void Destroy()
    {
        int returningMana = Mathf.RoundToInt(Mathf.Pow(2, showingBuilding.Level.Value-2));


        CounterWithMax mana = PlayerStats.Instance.GetMana((CombineModel.Skills)showingBuilding.State.Color).ResVal;


        if (showingBuilding.InHand)
        {
            FindObjectsOfType<BuildingHandSlot>().FirstOrDefault(s => s.Building == showingBuilding).Clear();
        }
        else
        {
            BlocksField.Instance.Cells.FirstOrDefault(c=>c.Building == showingBuilding).Clear();
        }

        mana.SetState(mana.CurrentValue+returningMana);
        Close();
    }
}
