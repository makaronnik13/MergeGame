using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ManaView : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    [SerializeField]
    public CombineModel.Skills MType;

    [SerializeField]
    private TMPro.TextMeshProUGUI Text;

    private void Start()
    {
        switch (MType)
        {
            case CombineModel.Skills.Purple:
                PlayerStats.Instance.Bank.GetResource(ResType.PurpleMana).ResVal.AddListener(ManaChanged);
                break;
            case CombineModel.Skills.Yellow:
                PlayerStats.Instance.Bank.GetResource(ResType.YellowMana).ResVal.AddListener(ManaChanged);
                break;
            case CombineModel.Skills.Red:
                PlayerStats.Instance.Bank.GetResource(ResType.RedMana).ResVal.AddListener(ManaChanged);
                break;
        }
    }

    private void ManaChanged(long v)
    {
        Text.text = v.ToString();
        GetComponent<Image>().enabled = v > 0;
        Text.enabled = v > 0;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (PlayerStats.Instance.GetMana(MType).ResVal.CurrentValue > 0)
        {
            ManaPlayer.Instance.PlayingMana.SetState((int)MType);
        }
       
    }

    public void OnDrag(PointerEventData eventData)
    {
        
    }
}
