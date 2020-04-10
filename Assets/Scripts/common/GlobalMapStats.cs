using com.armatur.common.serialization;
using System;

[Serializable]
public class GlobalMapStats
{
    [Savable]
    [SerializeData("PP_X", FieldRequired.False)]
    public float PlayerPos_x;
    [Savable]
    [SerializeData("PP_Y", FieldRequired.False)]
    public float PlayerPos_y;
    [Savable]
    [SerializeData("EP_", FieldRequired.False)]
    public float EnemyPos_x;
    [Savable]
    [SerializeData("EP_Y", FieldRequired.False)]
    public float EnemyPos_y;

    [Savable]
    [SerializeData("FromNodeId", FieldRequired.False)]
    public string FomNodeId;
    [Savable]
    [SerializeData("ToNodeId", FieldRequired.False)]
    public string ToNodeId;

    [Savable]
    [SerializeData("Chasing", FieldRequired.False)]
    public bool Chasing;

    [Savable]
    [SerializeData("Fighting", FieldRequired.False)]
    public bool Fighting;


    public bool Moving
    {
        get
        {
            return ToNodeId != null;
        }
    }
}