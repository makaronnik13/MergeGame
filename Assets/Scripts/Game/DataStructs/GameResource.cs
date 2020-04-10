using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Grids/Resource")]
public class GameResource : ScriptableObject {
   
    public int Id
    {
        get
        {
            return (int)ResType;
        }
    }

    public ResType ResType;

    public Sprite sprite;

    public long StartingValue = 0;
    public long MaxValue = 0;
    public string ResName;
}
