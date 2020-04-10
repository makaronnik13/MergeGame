using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Card")]
public class Card : ScriptableObject
{
    public string CardName;

    public string CardDescription;

	public Sprite cardSprite;
	public List<Inkome> Cost = new List<Inkome>();

	public bool WinCard = false;

	public bool DestroyAfterPlay;

	public List<CardEffect> CardEffects = new List<CardEffect>();
}
