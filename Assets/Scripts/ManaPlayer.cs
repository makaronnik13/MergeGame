using com.armatur.common.flags;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ManaPlayer : MonoBehaviour
{
    public static ManaPlayer Instance;



    public GenericFlag<Block> AimingBlock = new GenericFlag<Block>("AimingBlock", null);
    public GenericFlag<int> PlayingMana = new GenericFlag<int>("PlayingMana", -1);

    private void Awake()
    {
        Instance = this;
    }

    public Transform GetManaTransform()
    {
        return FindObjectsOfType<ManaView>().FirstOrDefault(v=>(int)v.MType == PlayingMana.Value).transform;
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (PlayingMana.Value!=-1 && AimingBlock.Value && !AimingBlock.Value.Blocked.Value)
            {
                AimingBlock.Value.UseMana(PlayingMana.Value);
            }
            PlayingMana.SetState(-1);
        }
    }
}
