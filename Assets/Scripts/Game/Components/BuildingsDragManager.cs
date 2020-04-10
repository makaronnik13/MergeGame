using System;
using UnityEngine;

public class BuildingsDragManager : Singleton<BuildingsDragManager>
{
    public Building building = null;
    
    public GameObject go;

    public Block fromBlock;
    private BuildingHandSlot fromSlot;


    public void StartDrag(Block block, Building building, Vector3 position)
    {
        if (this.building!=null)
        {
            return;
        }

        fromBlock = block;
        this.building = new Building(building);
        go = Instantiate(building.State.prefab);
    }
    public void StartDrag(BuildingHandSlot buildingHandSlot, Building building, Vector3 position)
    {
        if (this.building != null)
        {
            return;
        }

        fromSlot = buildingHandSlot;
        this.building = new Building(building);
        go = Instantiate(building.State.prefab);
    }

    private void Update()
    {

        if (go)
        {
            var v3 = Input.mousePosition;
            v3.z = 10.0f;
            v3 = Camera.main.ScreenToWorldPoint(v3);

            go.transform.position = v3;
        }

    }


    public void Release()
    {
        if (BuildingHandSlot.AimingSlot != null)
        {
            if (fromSlot || building.State.Biom== BuildingHandSlot.AimingSlot.Building.State.Biom)
            {
                Building b = BuildingHandSlot.AimingSlot.Building;
                Debug.Log(b.State.StateName);
                BuildingHandSlot.AimingSlot.SetBuilding(building);
                Debug.Log(b.State.StateName);
                if (fromBlock)
                {
                    fromBlock.SetBuilding(b);
                }
                else if (fromSlot)
                {
                    fromSlot.SetBuilding(b);
                }
                Clear();
                return;
            }
        }

        if (Block.aimingBlock!=null)
        {
            if (!Block.aimingBlock.Blocked.Value && building.State.Biom == Block.aimingBlock.Cell.Biom.Value)
            {
                Building b = new Building(Block.aimingBlock.Cell.Building);
                Block.aimingBlock.SetBuilding(building);
                if (fromBlock)
                {
                    fromBlock.SetBuilding(b);
                }
                else if (fromSlot)
                {
                    fromSlot.SetBuilding(b);
                }
                Clear();
                return;
            }
        }
        /*if (BuildingHandSlot.AimingSlot!=null && fromSlot==null)
        {
            BuildingHandSlot.AimingSlot.SetBuilding(building);
        }*/

        if (fromBlock)
        {
            fromBlock.Cell.State = building.State;
            Debug.Log("+");
            fromBlock.Cell.Building.Level.SetState(building.Level.Value);
        }

        if (fromSlot)
        {
            fromSlot.SetBuilding(building);
        }
        Clear();
    }

    public void MouseUp()
    {
        if (building==null)
        {
            return;
        }

        if (Block.aimingBlock == null)
        {
            if (BuildingHandSlot.AimingSlot != null)
            {
                if (BuildingHandSlot.AimingSlot.Building == null)
                {
                    BuildingHandSlot.AimingSlot.SetBuilding(building);
                    Clear();
                    return;
                }
                else if (BuildingHandSlot.AimingSlot &&
                    BuildingHandSlot.AimingSlot.Building != null &&
                    BuildingHandSlot.AimingSlot.Building.Level.Value < 10 &&
                    BuildingHandSlot.AimingSlot.Building.State == building.State &&
                    BuildingHandSlot.AimingSlot.Building.Level.Value == building.Level.Value
                  )
                {
                    BuildingHandSlot.AimingSlot.Building.Level.SetState(BuildingHandSlot.AimingSlot.Building.Level.Value + 1);
                    Clear();
                    return;
                }
                else
                {
                    Release();
                    return;
                }
            }
            else
            {
                Release();
                return;
            }
        }
        else
        {
            Building db = building;

            if (db == null)
            {
                return;
            }

            if (db.State.Biom != Block.aimingBlock.Cell.Biom.Value)
            {
                Release();
                return;
            }

            if (Block.aimingBlock.Cell.Building.Level.Value == 0 && !Block.aimingBlock.Blocked.Value)
            {
                Block.aimingBlock.SetBuilding(db);
                Clear();
            }
            else if (Block.aimingBlock!=fromBlock && db.State == Block.aimingBlock.Cell.State && Block.aimingBlock.Cell.Building.Level.Value == db.Level.Value && Block.aimingBlock.Cell.Building.Level.Value < 10)
            {
                Block.aimingBlock.Cell.Building.Level.SetState(Block.aimingBlock.Cell.Building.Level.Value + 1);
                Clear();
            }

            else
            {
                Release();
            }
        }
    }

    public void Clear()
    {
        if (go)
        {
            Destroy(go);
        }
        fromSlot = null;
        fromBlock = null;
        building = null;
    }

}