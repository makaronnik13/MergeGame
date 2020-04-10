using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockCanvas : MonoBehaviour
{
    [SerializeField]
    public CellInkomeView InkomeView;

    [SerializeField]
    private GameObject BuyImage;

    [SerializeField]
    private RectTransform Level;

    [SerializeField]
    private TMPro.TextMeshProUGUI LevelText;

    private Block Block;

    // Start is called before the first frame update
    public void Init(Block b)
    {
        Block = b;
        Block.Blocked.AddListener(BlockedStateChanged);
        PlayerStats.Instance.Bank.GetResource(ResType.OpenedFields).ResVal.AddListener(OpenedFieldsChanged);
        b.Cell.Building.Level.AddListener(LevelChanged);
        InkomeView.Init(b.Cell);

    }

    private void LevelChanged(int v)
    {
        if (v<5)
        {
            Level.sizeDelta = new Vector2(512 * v, Level.sizeDelta.y);
        }
        else
        {
            Level.sizeDelta = new Vector2(512, Level.sizeDelta.y);
        }
        LevelText.text = v.ToString();
        LevelText.gameObject.SetActive(v>=5);
    }

    private void OpenedFieldsChanged(long v)
    {
        UpdateBuyIcon();
    }


    private void BlockedStateChanged(bool obj)
    {
        UpdateBuyIcon();
    }

    private void UpdateBuyIcon()
    {
        BuyImage.SetActive(Block.Blocked.Value && PlayerStats.Instance.CanBuyBlock);
    }

    public void SetAimed(bool v)
    {
        InkomeView.SetAimed(v);
    }

    public void SetTempBuilding(CellState state, int lvl = 1)
    {
        InkomeView.SetTemp(Block, state, lvl);
    }

    public void Clear()
    {
        InkomeView.Clear();
    }
}
