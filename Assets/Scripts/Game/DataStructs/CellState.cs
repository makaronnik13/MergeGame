using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


[CreateAssetMenu(menuName = "Grids/CellState")]
public class CellState: ScriptableObject
{
    public int Id;
    public bool Initial = false;
    public string StateName;
    public CombineModel.Biom Biom;
    
    [HideInInspector]
	[SerializeField]
	public float X, Y;

	public void Drag(Vector2 p)
	{
		X = p.x;
		Y = p.y;
	}

    public enum RadiusType
    {
        Simple,
        FromStat
    }
    public RadiusType radiusType = RadiusType.Simple;
    
    public int radius = 1;


    public GameResource radiusResource;

    private bool RadiusSimple()
    {
        return radiusType == RadiusType.Simple;
    }

    private bool RadiusFromStat()
    {
        return radiusType == RadiusType.FromStat;
    }

 
	public string description;


	public GameObject prefab;

   
    public Sprite Sprite;

  
    public Inkome[] income;

	public Combination[] Combinations
	{
		get
		{
			if(combinations == null)
			{
				combinations = new Combination[0];
			}
			return combinations;
		}
		set
		{
			combinations = value;
		}
	}

    public CombineModel.Skills Color
    {
        get
        {
            if (Initial)
            {
                return CombineModel.Skills.None;
            }

            return DefaultRessources.CellStates.FirstOrDefault(s=>s.Initial && s.Biom == Biom).combinations.FirstOrDefault(c=>c.ResultState == this).skill;
        }
    }

    [SerializeField]
	private Combination[] combinations;


    public Condition[] conditions; 

	public CellBuff[] buffs;

    public List<Perk> Perks;

    public bool HasCombination(CombineModel.Skills skill)
	{
		foreach(Combination comb in Combinations)
		{
			if(comb.skill == skill)
			{
				return true;
			}
		}
		return false;
	}

	public CellState CombinationResult(CombineModel.Skills skill, int skillLevel = 0)
	{
		foreach(Combination comb in Combinations)
		{
			if(comb.skill == skill && skillLevel == comb.skillLevel)
			{
				return comb.ResultState;
			}
		}
		return null;
	}

	public void AddCombination()
	{
		Combination c = new Combination ();
		List<Combination> comb = Combinations.ToList ();
		comb.Add(c);
		Combinations = comb.ToArray ();
	}


	public void RemoveCombination(int i)
	{
		List<Combination> comb = Combinations.ToList ();
		comb.RemoveAt (i);
		Combinations = comb.ToArray ();
	}

		
	public void RemoveIncome(int i)
	{
		List<Inkome> incomes = income.ToList ();
		incomes.RemoveAt (i);
		income = incomes.ToArray ();
	}

	public void AddBuff()
	{
		List<Condition> ink = conditions.ToList ();
		ink.Add(new Condition());
		conditions = ink.ToArray ();
	}

	public void RemoveBuff(int i)
	{
		List<Condition> incomes = conditions.ToList ();
		incomes.RemoveAt (i);
		conditions = incomes.ToArray ();
	}

}
