using UnityEngine;
using com.armatur.common.flags;
using com.armatur.common.serialization;
using System.Collections.Generic;
using System.Linq;
using System;

[SerializeField]
public class Cell
{
    [Savable]
    [SerializeData(FieldOmitName.False, FieldRequired.False)]
    public Vector2 Position;

    [Savable]
    [SerializeData(FieldOmitName.False, FieldRequired.False)]
    public GenericFlag<CombineModel.Biom> Biom = new GenericFlag<CombineModel.Biom>("Biom", CombineModel.Biom.None);

    [Savable]
    [SerializeData(FieldOmitName.False, FieldRequired.False)]
    private Building building;
    public Building Building
    {
        get
        {
            return building;
        }
        set
        {
            building = value;
            building.StateId.SetState(value.StateId.Value);
        }
    }

    public GenericFlag<bool> Blocked = new GenericFlag<bool>("Blocked", true);

    public CombineModel.Skills Color
    {
        get
        {
            return State.Color;
        }
    }

    public CellState State
    {
        get
        {
            return building.State;
        }
        set
        {
            building.State = value;
        }
    }


    public Cell(CellState state, Vector2 pos)
    {
        Position = pos;
        building = new Building(state);
        Biom.SetState(state.Biom);

    }

    public void Clear()
    {
        State = DefaultRessources.CellStates.FirstOrDefault(c => c.Biom == State.Biom && c.Initial);
        Building.Level.SetState(0);
    }

    public bool Combine(CombineModel.Skills skill)
    {
        CellState resultState = State.CombinationResult(skill, 1);
        if (resultState)
        {
            State = resultState;
            return true;
        }
        else
        {
            return false;
        }
    }

  
}
