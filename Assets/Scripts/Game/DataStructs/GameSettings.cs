using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GameSettings", fileName = "Settings")]
public class GameSettings: ScriptableObject
{
    public AnimationCurve IncomeCurve;

    public List<Perk> Perks;
    public float GenTime = 3f;
}