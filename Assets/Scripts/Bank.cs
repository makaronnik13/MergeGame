using com.armatur.common.serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Bank
{

    public Action<Inkome> OnRessourceChanged = (c) => { };

    [Savable]
    [SerializeCollection("Resources", FieldRequired.False)]
    public List<ResourceInstance> ResourcesList = new List<ResourceInstance>();

    public void Init()
    {
        foreach (GameResource rType in DefaultRessources.GameRessources)
        {
            if (ResourcesList.FirstOrDefault(r => r.ResType==rType) == null)
            {
                    ResourcesList.Add(new ResourceInstance(rType, rType.StartingValue, rType.MaxValue));
            }
        }
    }

    public ResourceInstance GetResource(ResType rType)
    {
        return ResourcesList.FirstOrDefault(r => r.ResType.Id == (int)rType);
    }

    public static string GetResName(ResType resType)
    {
        return DefaultRessources.GetRes((int)resType).ResName;
    }

    public ResourceInstance GetResource(GameResource res)
    {
        return ResourcesList.FirstOrDefault(r => r.ResType == res);
    }

    public void ChangeResource(Inkome cost)
    {
        GetResource(cost.resource).ResVal.Change(cost.value);
        //return new Inkome(cost.resource, change);
    }

    public void ChangeResource(ResType rType, long val)
    {
        GetResource(rType).ResVal.Change(val);
    }


    public void SetResource(ResType rType, long val)
    {
        GetResource(rType).ResVal.SetState(val);
    }


}