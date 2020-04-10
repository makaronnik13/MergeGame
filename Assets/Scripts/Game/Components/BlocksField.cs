using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class BlocksField : MonoBehaviour
{
    public GameObject BlockPrefab;
    public static BlocksField Instance;
	public int Size = 4;
	public float CellSize = 2;
	private Dictionary<Cell, Block> cellsAndBlocks = new Dictionary<Cell, Block> ();
    public List<Cell> Cells
    {
        get
        {
            return cellsAndBlocks.Keys.ToList();
        }
    }
    public List<Block> Blocks
    {
        get
        {
            return cellsAndBlocks.Values.ToList();
        }
    }
    public List<CellState> baseStates;

    private void Awake()
    {
        Instance = this;
        GenerateField();
    }

    public List<Inkome> GetInkomes(Block block = null, CellState state = null, int lvl1 = 1, Block block2 = null, CellState state2 = null, int lvl2 = 1)
    {
        if (block == block2)
        {
            block2 = null;
        }

        CellState b1State = null, b2State = null;
        int b1Lvl = 0, b2Lvl = 0;

        
        if (block!=null)
        {
            b1State = block.Cell.State;
            b1Lvl = block.Cell.Building.Level.Value;
            block.SetBuilding(new Building(state));
            block.Cell.Building.Level.SetState(lvl1);
        }

        if (block2!=null)
        {
            b2State = block2.Cell.State;
            b2Lvl = block2.Cell.Building.Level.Value;
            block2.SetBuilding(new Building(state2));
            block2.Cell.Building.Level.SetState(lvl2);
        }

    

            List<Inkome> incs = new List<Inkome>();
            foreach (Block b in Blocks)
            {
                b.Cell.Building.RecalculateInkome();
                foreach (Inkome i in b.Cell.Building.CurrentIncome())
                {
                    if (incs.FirstOrDefault(inc=>inc.resource == i.resource)!=null)
                    {
                        incs.FirstOrDefault(inc => inc.resource == i.resource).value += i.value;
                    }
                    else
                    {
                        incs.Add(i);
                    }
                }
            }

        if (b1State != null)
        {
            block.SetBuilding(new Building(b1State));
            block.Cell.Building.Level.SetState(b1Lvl);
        }

        if (b2State != null)
        {
            block2.SetBuilding(new Building(b2State));
            block2.Cell.Building.Level.SetState(b2Lvl);
        }

        return incs;
    }

   


    public List<Block> GetBlocksInRadius(Block block, int r)
	{
		List<Block> blocks = new List<Block> ();
		if(block == null)
		{
			return blocks;
		}

		Vector2 blockPos = cellsAndBlocks.FirstOrDefault (x => x.Value == block).Key.Position;

		foreach(Cell c in Cells)
		{
            if (block.Cell==c)
            {
                continue;
            }
			float deltaX = Mathf.Abs (c.Position.x - blockPos.x);
			float deltaY = Mathf.Abs (c.Position.y - blockPos.y); 
			if(deltaX<=r && deltaY<=r && c.Position.x - blockPos.x+ c.Position.y - blockPos.y<=r && -c.Position.x + blockPos.x- c.Position.y + blockPos.y<=r)
			{
					blocks.Add(cellsAndBlocks[c]);
			}
		}

		return blocks;
    }
		
	public void GenerateField()
	{
		cellsAndBlocks.Clear ();
		foreach(Transform t in transform)
		{
            Lean.Pool.LeanPool.Despawn(t.gameObject);
		}

        List<CellState> states = new List<CellState>();
        for (int i = 0; i < Mathf.CeilToInt(RecalculateHexes().Count()/3f); i++)
        {
            states.Add(DefaultRessources.CellStates.Where(c => c.Initial).ToList()[0]);
            states.Add(DefaultRessources.CellStates.Where(c => c.Initial).ToList()[1]);
            states.Add(DefaultRessources.CellStates.Where(c => c.Initial).ToList()[2]);
        }
        states = states.OrderBy(c=>Guid.NewGuid()).ToList();

        //Instantiate blocks
        int j = 0;
		foreach(KeyValuePair<Vector3, Vector2> v in RecalculateHexes())
		{
            GameObject newCellView = Instantiate (BlockPrefab);
            Block cb = newCellView.GetComponent<Block>();
            Cell newCell = new Cell(states[j], new Vector2Int(Mathf.RoundToInt(v.Value.x), Mathf.RoundToInt(v.Value.y)));
            cb.InitBlock(v.Key, newCell);
            cellsAndBlocks.Add(newCell, cb);
            j++;
		}

        Block startingBlock = cellsAndBlocks.FirstOrDefault(c=>c.Key.Position == Vector2Int.zero).Value;
        startingBlock.Blocked.SetState(false);

        for (int i = 0; i < 4; i++)
        {
            GetBlockFromSide(startingBlock, i).Blocked.SetState(false);
        }

        /*
        //InitBlocks
		foreach(KeyValuePair<Vector2, Block> pair in cells)
		{
            int randomRotation = Mathf.RoundToInt(UnityEngine.Random.Range(0, 6));
            int state = Random.Range(0, 4);
            Vector3 worldPos = CellCoordToWorld(pair.Key);
            float[] posArray = new float[] {worldPos.x, worldPos.y, worldPos.z};
            //pair.Value.GetComponent<PhotonView>().RPC("InitBlock", PhotonTargets.All, new object[] {posArray, randomRotation, state});	
		}
        */
	}

    public Block GetBlock(Cell cell)
    {
        return cellsAndBlocks[cell];
    }

    public Dictionary<Vector3, Vector2> RecalculateHexes ()
	{
		List<Vector2> cellsCoordinates = new List<Vector2> ();
		Dictionary<Vector3, Vector2> result = new Dictionary<Vector3, Vector2> ();
		cellsCoordinates.Clear ();

		for (int i = -Size * 3; i < Size * 3; i++) {
			for (int j = -Size * 3; j < Size * 3; j++) {
				if (UnityEngine.Random.Range (0, 10) >= 0 && Mathf.Abs (i + j) < Size && Mathf.Abs (j) < Size && Mathf.Abs (i) < Size) {
					cellsCoordinates.Add (new Vector2 (i, j));
				}
			}
		}

		foreach (Vector2 c in cellsCoordinates) {
			Vector2 cell2DCoord = CellCoordToWorld (c);
			Vector3 cellPosition = RotatePointAroundPivot (new Vector3 (cell2DCoord.x, transform.position.y, cell2DCoord.y), transform.position, transform.rotation.eulerAngles);
			result.Add (cellPosition, c);
		}

		return result;
	}

	private Vector2 CellCoordToWorld (Vector2 cellCoord)
	{
		float x =  CellSize * (float)Mathf.Sqrt (3) * (cellCoord.x + cellCoord.y / 2);
		float y =  CellSize * 3 / 2 * -cellCoord.y;
		Vector3 pos = transform.position + new Vector3 (x, 0, y) * 0.6f;
		return new Vector2 (pos.x, pos.z);
	}

	private Vector3 RotatePointAroundPivot (Vector3 point, Vector3 pivot, Vector3 angles)
	{
		Vector3 dir = point - pivot; // get point direction relative to pivot
		dir = Quaternion.Euler (angles) * dir; // rotate it
		point = dir + pivot; // calculate rotated point
		return point; // return it
	}

	public Block GetBlockFromSide(Block block, int i)
	{
		try
		{
            Vector2 pos = cellsAndBlocks.FirstOrDefault(cc=>cc.Value == block).Key.Position;

			switch(i)
			{
			case 0:
				return cellsAndBlocks[Cells.FirstOrDefault(c=>c.Position == new Vector2Int(Mathf.RoundToInt(pos.x - 1), Mathf.RoundToInt(pos.y)))];
			case 1:
                return cellsAndBlocks[Cells.FirstOrDefault(c => c.Position == new Vector2Int(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y-1)))];
			case 2:
                return cellsAndBlocks[Cells.FirstOrDefault(c => c.Position == new Vector2Int(Mathf.RoundToInt(pos.x + 1), Mathf.RoundToInt(pos.y-1)))];
			case 3:
                return cellsAndBlocks[Cells.FirstOrDefault(c => c.Position == new Vector2Int(Mathf.RoundToInt(pos.x + 1), Mathf.RoundToInt(pos.y)))];
			case 4:
                return cellsAndBlocks[Cells.FirstOrDefault(c => c.Position == new Vector2Int(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y+1)))];
			case 5:
                return cellsAndBlocks[Cells.FirstOrDefault(c => c.Position == new Vector2Int(Mathf.RoundToInt(pos.x - 1), Mathf.RoundToInt(pos.y+1)))];
			}
		}
		catch
		{
			return null;
		}

		return null;
	}
}
