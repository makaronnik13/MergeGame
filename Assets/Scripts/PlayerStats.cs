using com.armatur.common.flags;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public Bank Bank;
    public PerksStorage Perks;

    public static PlayerStats Instance;

    public int ClickForce = 10;


    /*
    public void BuyBlock()
    {
        Bank.ChangeResource(ResType.Money, -NewBlocksCost);
        Bank.ChangeResource(ResType.OpenedFields, 1);
    }*/

    public AnimationCurve BlocksCost;

    public bool CanBuyBlock
    {
        get
        {
            return Bank.GetResource(ResType.OpenedFields).ResVal.CurrentValue > BlocksField.Instance.Blocks.Where(b=>!b.Cell.Blocked.Value).Count();
        }
    }



    private void Awake()
    {

        Bank = new Bank();
        Bank.Init();

        Perks = new PerksStorage();
     

        Bank.GetResource(ResType.OpenedFields).ResVal.AddListener(UpdateMaxBuildPoints);
        Bank.GetResource(ResType.BuildPoints).ResVal.AddListener(OnBPChanged);
        Bank.GetResource(ResType.RedMana).ResVal.AddListener(UpdateMaxBuildPoints);
        Bank.GetResource(ResType.PurpleMana).ResVal.AddListener(UpdateMaxBuildPoints);
        Bank.GetResource(ResType.YellowMana).ResVal.AddListener(UpdateMaxBuildPoints);
        Instance = this;
    }

    private void UpdateMaxBuildPoints(long v)
    {
        //long mana = Bank.GetResource(ResType.RedMana).ResVal.CurrentValue + Bank.GetResource(ResType.GreenMana).ResVal.CurrentValue + Bank.GetResource(ResType.BlueMana).ResVal.CurrentValue;
        int lvl = 0;
        foreach (Block b in BlocksField.Instance.Blocks)
        {
            lvl += b.Level.Value;
        }

        Bank.GetResource(ResType.BuildPoints).ResVal.SetMax(Mathf.RoundToInt(1000f+100* (float)Math.Exp(lvl/20f))); //+100*(float)Math.Exp(mana / 5f)
    }

    public ResourceInstance GetMana(CombineModel.Skills mType)
    {
        switch (mType)
        {
            case CombineModel.Skills.Purple:
                return Bank.GetResource(ResType.PurpleMana);
            case CombineModel.Skills.Yellow:
                return Bank.GetResource(ResType.YellowMana);
            case CombineModel.Skills.Red:
                return Bank.GetResource(ResType.RedMana);
        }

        return null;
    }

    private void OnBPChanged(long obj)
    {
        if (Bank.GetResource(ResType.BuildPoints).ResVal.CurrentValue >= Bank.GetResource(ResType.BuildPoints).ResVal.MaxValue)
        {
            Bank.GetResource(ResType.BuildPoints).ResVal.SetState(0);
            GenerateMana();
        }
    }

    private void GenerateMana()
    {
        Bank.GetResource(ResType.BuildPoints).ResVal.SetState(0);
        switch (ServerFake.Instance.GetRes())
        {
            case 0:
                Bank.ChangeResource(ResType.RedMana, 1);
                break;
            case 1:
                Bank.ChangeResource(ResType.YellowMana, 1);
                break;
            case 2:
                Bank.ChangeResource(ResType.PurpleMana, 1);
                break;
        }
    }

    private void Start()
    {
        StartCoroutine(GenerateBuildPoints());
    }

    private IEnumerator GenerateBuildPoints()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.01f);
            if (Bank.GetResource(ResType.YellowMana).ResVal.CurrentValue + Bank.GetResource(ResType.PurpleMana).ResVal.CurrentValue + Bank.GetResource(ResType.RedMana).ResVal.CurrentValue<Bank.GetResource(ResType.MaxMana).ResVal.CurrentValue)
            {
                Bank.ChangeResource(ResType.BuildPoints, Mathf.RoundToInt(Bank.GetResource(ResType.BuildSpeed).ResVal.CurrentValue * Time.timeScale));
            }
        }
    }

    private void Update()
    {
        /*
        long l = -10;
        long l2 = 0;
        Debug.Log("!"+(l-l2));
        */
    }
}
