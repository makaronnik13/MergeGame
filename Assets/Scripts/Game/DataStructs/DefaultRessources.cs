using UnityEngine;
using com.armatur.common.flags;
using com.armatur.common.serialization;
using System;
using System.Collections.Generic;
using System.Linq;

public static class DefaultRessources
{
    private static GameSettings settings;
    public static GameSettings Settings
    {
        get
        {
            if (settings == null)
            {
                settings = Resources.Load<GameSettings>("Assets/Settings");
            }
            return settings;
        }
    }

    private static List<CellState> cellstates;
    public static List<CellState> CellStates
    {
        get
        {
            if (cellstates == null)
            {
                cellstates = Resources.LoadAll<CellState>("Assets/Cells").ToList();
            }
            return cellstates;
        }
    }

    private static List<GameResource> gameRessources;
    public static List<GameResource> GameRessources
    {
        get
        {
            if (gameRessources == null)
            {
                gameRessources = Resources.LoadAll<GameResource>("Assets/GameResources").ToList();
            }
            return gameRessources;
        }
    }


    public static int GetResId(GameResource value)
    {
        return value.Id;
    }

    public static GameResource GetRes(int Id)
    {
        return GameRessources.FirstOrDefault(r=>r.Id == Id);
    }


    public static CellState GetState(int id)
    {
        if (id>=0 && id<CellStates.Count)
        {
            return CellStates[id];
        }
        return null;
    }

    public static int GetStateId(CellState state)
    {
        if (state == null)
        {
            return -1;
        }
        return cellstates.IndexOf(state);
    }
}