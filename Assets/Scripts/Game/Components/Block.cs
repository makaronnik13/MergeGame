using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using com.armatur.common.flags;

public class Block : MonoBehaviour
{
    public BlockCanvas BCanvas;

    public GenericFlag<int> Level
    {
        get
        {
            return Cell.Building.Level;
        }
    }

    public GenericFlag<bool> Blocked
    {
        get
        {
            return Cell.Blocked;
        }
    }

    [SerializeField]
    private TempModel TempModel;

	private CellModel cellModel;
	private CellModel CellModel
	{
		get
		{
			if(cellModel == null)
			{
				cellModel = GetComponentInChildren<CellModel> ();
			}

			return cellModel;
		}
	}

	private CellHighlighter highLighter;
	private CellHighlighter Highlighter
	{
		get
		{
			if(highLighter == null)
			{
				highLighter = GetComponentInChildren<CellHighlighter> ();
			}

			return highLighter;
		}
	}

    private Coroutine resGeneration;

    public static Block aimingBlock;

	private ModelReplacer GroundReplacer
    {
        get
        {
            return GetComponentInChildren<ModelReplacer>();
        }
    }

    public void GenerateRessource()
    {
        if (Cell.Building.Level.Value==0)
        {
            return;
        }
        foreach (Inkome ink in Cell.Building.CurrentIncome())
        {
                PlayerStats.Instance.Bank.ChangeResource(ink.resource.ResType, ink.value);
        }

        InkomeEmmiter.Instance.Emmit(Cell.Building.CurrentIncome(), CellModel.transform.position+Vector3.up*2);
    }

   

    private Cell cell;
    public Cell Cell
    {
        get
        {
            return cell;
        }
    }

    public int Radius
    {
        get
        {
			if (Cell.State.radiusType ==  CellState.RadiusType.Simple) {
				return Cell.State.radius;
			} else {
				return  Mathf.FloorToInt(Cell.Building.CurrentIncome().Find (i=>i.resource == Cell.State.radiusResource).value);
				}
        }
    }

    private Coroutine MouseClickCoroutine;
    private Vector2 mPos;

    #region lifecycle
    void Start()
    {
        BCanvas.Init(this);
        Blocked.AddListener((v) => { GetComponentInChildren<ModelReplacer>().SetBlocked(v, (int)Cell.Biom.Value);});
        GetComponent<MeshCollider>().enabled = false;
        Animator anim = GetComponent<Animator>();
        float randomIdleStart = UnityEngine.Random.Range(0, anim.GetCurrentAnimatorStateInfo(0).length/4f);
        anim.Play("CellFlowingIn", 0, randomIdleStart);
        StartCoroutine(SetupAnimation(2));
        Cell.Building.StateId.AddListener(StateChanged);
        Cell.Building.Level.AddListener(LevelChanged);
    }

    private void LevelChanged(int lvl)
    {
        if (resGeneration!=null)
        {
            StopCoroutine(resGeneration);
        }

        resGeneration = StartCoroutine(InkomeGeneration());
    }

    private void StateChanged(int id)
    {
        if (resGeneration != null)
        {
            StopCoroutine(resGeneration);
        }

        resGeneration = StartCoroutine(InkomeGeneration());
    }

    private IEnumerator InkomeGeneration()
    {
        while (true)
        {
            yield return new WaitForSeconds(DefaultRessources.Settings.GenTime);
            GenerateRessource();
        }
    }

    private IEnumerator SetupAnimation(int v)
    {
        yield return new WaitForSeconds(2);
        GetComponent<MeshCollider>().enabled = true;
        Animator anim = GetComponent<Animator>();
        anim.speed = UnityEngine.Random.Range(0.3f, 0.6f);
    }

    #endregion

    #region publicMethods
    public void UseMana(int value)
    {
        TempModel.HideTemp();
        BCanvas.Clear();
        Highlighter.Set(false, true);
        
        bool combined = Cell.Combine((CombineModel.Skills)(value));

        CounterWithMax mana = PlayerStats.Instance.GetMana((CombineModel.Skills)ManaPlayer.Instance.PlayingMana.Value).ResVal;

        if (combined)
        {
            mana.SetState(mana.CurrentValue-1);
            Cell.Building.Level.SetState(1);
        }
        else
        {
            //use several mana

            if (ManaPlayer.Instance.PlayingMana.Value == (int)Cell.State.Color)
            {
                int needMana = Mathf.RoundToInt(Mathf.Pow(2, Level.Value - 1));
      
                if (mana.CurrentValue>=needMana)
                {
                    mana.SetState(mana.CurrentValue - needMana);
                    Level.SetState(Level.Value + 1);
                    Debug.Log("Up");
                }
            }
        }
        ManaPlayer.Instance.PlayingMana.SetState(-1);
        
    }

    public void SetBuilding(Building db)
    {
        Cell.State = db.State;
        Cell.Building.Level.SetState(db.Level.Value);
    }



    /*
    public bool IsAwaliable(Card card)
    {
        if (!card)
        {
            return false;
        }

        if (card.CardEffects.Count() == 0)
        {
            return false;
        }

        CardEffect ce = card.CardEffects.FirstOrDefault(cardEffect => cardEffect.cardAim == CardEffect.CardAim.Cell);

        if (ce != null)
        {
            if (ce.cellOwnership == CardEffect.CellOwnership.Neutral)
            {
                return false;
            }
            if (ce.cellOwnership == CardEffect.CellOwnership.Player)
            {
                return false;
            }
            if (ce.cellOwnership == CardEffect.CellOwnership.Oponent)
            {
                return false;
            }
            if (ce.cellOwnership == CardEffect.CellOwnership.PlayerAndNeutral)
            {
                return false;
            }

            if (ce.cellOwnership == CardEffect.CellOwnership.OponentAndNeutral)
            {
                return false;
            }

            if (ce.biomsFilter.Contains(Biom))
            {
                return false;
            }

            if (ce.statesFilter.Contains(State))
            {
                return false;
            }

            if (ce.cellActionType == CardEffect.CellActionType.Evolve)
            {
                if (State == null)
                {
                    return false;
                }
                foreach (Combination comb in State.Combinations)
                {
                    if (comb.skill == ce.EvolveType && comb.skillLevel == ce.EvolveLevel)
                    {
                        return true;
                    }
                }
                return false;
            }

            return true;
        }

        return false;
    }
    public void Highlight(Card card, bool v)
    {
        Highlighter.Set(v && IsAwaliable(card), false);
    }
    */
    public void HighlightSimple(bool v)
    {
        BCanvas.Clear();

        Highlighter.Set(v, false);
        if (!Blocked.Value)
        {
            if (BuildingsDragManager.Instance.building == null)
            {
                if (ManaPlayer.Instance.PlayingMana.Value==-1)
                {
                    BCanvas.SetAimed(v);
                }
                BCanvas.Clear();
            }
            else
            {
                BCanvas.SetTempBuilding(BuildingsDragManager.Instance.building.State, BuildingsDragManager.Instance.building.Level.Value);
            }
        }
      
    }
    #endregion

    #region mouseEvents
    void OnMouseDown()
    {
        mPos = Input.mousePosition;
        MouseClickCoroutine = StartCoroutine(MouseClickCor());
        if (Blocked.Value && PlayerStats.Instance.CanBuyBlock)
        {
            Cell.Blocked.SetState(false);
            PlayerStats.Instance.Bank.GetResource(ResType.OpenedFields).ResVal.RaiseEvents();
        }
    }
    void OnMouseUp()
    {
        BCanvas.Clear();


        TempModel.HideTemp();

        if (MouseClickCoroutine != null)
        {
            StopCoroutine(MouseClickCoroutine);
        }

        /*
        if (aimingBlock == null)
        {
            BuildingsDragManager.Instance.Release();
            return;
        }
        else
        {
            Building db = BuildingsDragManager.Instance.building;

            if (db == null)
            {
                return;
            }
            if (db.State.Biom != aimingBlock.Cell.Biom.Value)
            {
                BuildingsDragManager.Instance.Release();
                return;
            }

     
            if (aimingBlock.Cell.Building.Level.Value == 0 && !aimingBlock.Blocked.Value)
            {
                aimingBlock.Cell.State = db.State;
                aimingBlock.Cell.Building.Level.SetState(db.Level.Value);
                BuildingsDragManager.Instance.Clear();
            }
            else if (db.State == aimingBlock.Cell.State && aimingBlock.Cell.Building.Level.Value == db.Level.Value && aimingBlock.Cell.Building.Level.Value < 10)
            {
                aimingBlock.Cell.Building.Level.SetState(aimingBlock.Cell.Building.Level.Value + 1);
                BuildingsDragManager.Instance.Clear();
            }
            else
            {
                BuildingsDragManager.Instance.Release();
            }
        }   */

        BuildingsDragManager.Instance.MouseUp();
    }
    void OnMouseEnter()
    {
        aimingBlock = this;

        if (ManaPlayer.Instance.PlayingMana.Value != -1)
        {
            CellState resultState = Cell.State.CombinationResult((CombineModel.Skills)(ManaPlayer.Instance.PlayingMana.Value), 1);
            if (!Blocked.Value && resultState != null)
            {
                TempModel.ShowTemp(resultState);
                BCanvas.SetTempBuilding(resultState);
                Highlighter.Set(true, true);
            }
        }

        HighlightSimple(true);

        CameraController.Instance.AimedBlockChanged(this);

        if (!Blocked.Value)
        {
            ManaPlayer.Instance.AimingBlock.SetState(this);
        }
     
    }
    void OnMouseExit()
    {
        BCanvas.Clear();
        TempModel.HideTemp();
        aimingBlock = null;
        if (MouseClickCoroutine != null)
        {
            StopCoroutine(MouseClickCoroutine);
        }
        if (!Blocked.Value)
        {
            ManaPlayer.Instance.AimingBlock.SetState(null);
        }

        HighlightSimple(false);
        Highlighter.Set(false, true);

        bool shouldDehighlight = true;

        /*if (CardsPlayer.Instance.ActiveCard) {
			CardEffect cardEffect = CardsPlayer.Instance.ActiveCard.CardAsset.CardEffects.FirstOrDefault (ce => ce.cardAim == CardEffect.CardAim.Player);
			if (cardEffect != null) {
				if (cardEffect.playerAimType == CardEffect.PlayerAimType.All || cardEffect.playerAimType == CardEffect.PlayerAimType.Enemies || cardEffect.playerAimType == CardEffect.PlayerAimType.You) {
					shouldDehighlight = false;
				}
			}

			cardEffect = CardsPlayer.Instance.ActiveCard.CardAsset.CardEffects.FirstOrDefault (ce=>ce.cardAim == CardEffect.CardAim.Cell);
			if(cardEffect!=null)
			{
				if(cardEffect.cellAimType == CardEffect.CellAimType.All  || cardEffect.cellAimType == CardEffect.CellAimType.Random)
				{
					shouldDehighlight = false;
				}
			}
		}
			*/
        if (shouldDehighlight)
        {
            //CardsPlayer.Instance.SelectAim (null);
        }

        // InformationPanel.Instance.ShowInfo(null);
    }
    void OnMouseDrag()
    {
        if (BuildingsDragManager.Instance.building == null)
        {
            BCanvas.Clear();
        }

        TempModel.HideTemp();
        if (!Cell.State.Initial && Vector2.Distance(mPos, Input.mousePosition) > 20)
        {
            Building b = new Building(cell.Building);
            Cell.Clear();
            BuildingsDragManager.Instance.StartDrag(this, b, CellModel.transform.position);
            if (MouseClickCoroutine != null)
            {
                StopCoroutine(MouseClickCoroutine);
            }
        }
    }
    private IEnumerator MouseClickCor()
    {
        yield return new WaitForSeconds(0.3f);
        

        if (Cell.Building.Level.Value>0)
        {
            InformationPanel.Instance.ShowInfo(Cell.Building);
        }
    }
    #endregion


    #region PRCMethods
    public void InitBlock(Vector3 localPos, Cell cell)
    {
        this.cell = cell;
        transform.SetParent(BlocksField.Instance.transform);
        transform.localScale = Vector3.one;
        transform.localRotation = Quaternion.identity;
        transform.localPosition = localPos;
        Cell.Biom.AddListener(BiomChanged);
        cell.Building.StateId.AddListener(StateIdChanged);
    }

    private void StateIdChanged(int id)
    {
        CellModel.SetCell(DefaultRessources.GetState(id));
    }

    private void BiomChanged(CombineModel.Biom biom)
    {
        GroundReplacer.SetModel((int)biom);
    }
    #endregion
}
