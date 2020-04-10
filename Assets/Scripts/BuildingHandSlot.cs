using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingHandSlot : MonoBehaviour
{
    private Coroutine MouseClickCoroutine;
    private Vector2 mPos;
    private Building building;
    public Building Building
    {
        get
        {
            return building;
        }
        private set
        {
            if (building!=null)
            {
                building.Level.RemoveListener(LevelChanged);
            }
            building = value;
            if (building!=null)
            {
                building.Level.AddListener(LevelChanged);
            }
        }
    }

    private int Lvl;

    [SerializeField]
    private RectTransform Level;

    [SerializeField]
    private TMPro.TextMeshProUGUI LevelText;


    private GameObject buildingGo;

    public static BuildingHandSlot AimingSlot;

    private void Start()
    {
        PlayerStats.Instance.Bank.GetResource(ResType.OpenedSlots).ResVal.AddListener(HandSlotsChanged);
    }

    private void HandSlotsChanged(long v)
    {
        gameObject.SetActive(transform.GetSiblingIndex()<v);
    }

    private void OnMouseDown()
    {
        mPos = Input.mousePosition;
        if (building!=null)
        {
            MouseClickCoroutine = StartCoroutine(MouseClickCor());
        }
    }

    private void OnMouseEnter()
    {
        AimingSlot = this;
    }

    void OnMouseUp()
    {
        if (MouseClickCoroutine != null)
        {
            StopCoroutine(MouseClickCoroutine);
        }

        BuildingsDragManager.Instance.MouseUp();
       
        AimingSlot = null;

    }

    public void SetBuilding(Building building)
    {
        Clear();

        if (building!=null)
        {
                building.InHand = true;
                buildingGo = Instantiate(building.State.prefab);
                buildingGo.transform.SetParent(transform);
                buildingGo.transform.localPosition = Vector3.zero;
                buildingGo.transform.localRotation = Quaternion.identity;
                buildingGo.transform.localScale = Vector3.one;
                StaticTools.SetTag(buildingGo, "UI");

                //BuildingsDragManager.Instance.go = null;
                //state = BuildingsDragManager.Instance.state;
                //BuildingsDragManager.Instance.Clear();
                LevelChanged(building.Level.Value);
        }

        Building = building;
    }

    void OnMouseExit()
    {
        AimingSlot = null;
        if (MouseClickCoroutine != null)
        {
            StopCoroutine(MouseClickCoroutine);
        }
    }

    void OnMouseDrag()
    {
        if (Building == null)
        {
            return;
        }

        if (Vector2.Distance(mPos, Input.mousePosition) > 20)
        {
            Building.InHand = false;
            BuildingsDragManager.Instance.StartDrag(this, Building, transform.position);
            SetBuilding(null);
            if (MouseClickCoroutine != null)
            {
                StopCoroutine(MouseClickCoroutine);
            }
        }
    }

    private IEnumerator MouseClickCor()
    {
        yield return new WaitForSeconds(0.5f);

        if (Building!=null)
        {
            InformationPanel.Instance.ShowInfo(Building);
        }
    }

    private void LevelChanged(int v)
    {
        Level.sizeDelta = new Vector2(512 * v, Level.sizeDelta.y);
        LevelText.text = v.ToString();

        Level.gameObject.SetActive(v < 5);
        LevelText.transform.parent.gameObject.SetActive(v >= 5);
    }

    public void Clear()
    {
        Destroy(buildingGo);
        LevelChanged(0);
        Building = null;
    }

}
