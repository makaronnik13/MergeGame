using System;
using System.Collections.Generic;
using UnityEngine;

public class InkomeEmmiter: Singleton<InkomeEmmiter>
{

    [SerializeField]
    private GameObject PopupPrefab;

    public void Emmit(List<Inkome> ink, Vector3 pos)
    {
        GameObject newInk = Instantiate(PopupPrefab);
        newInk.transform.SetParent(transform);
        newInk.transform.position = pos;
        newInk.transform.localScale = Vector3.one;
        newInk.transform.rotation = Quaternion.identity;
        //newInk.GetComponent<InkomeView>().ShowSign = true;
        newInk.GetComponent<InkomeView>().Init(ink);
    
    }

}