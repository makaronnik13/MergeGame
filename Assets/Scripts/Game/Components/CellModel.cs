using com.armatur.common.flags;
using UnityEngine;


public class CellModel : MonoBehaviour
{
	private GameObject model;
    public GenericFlag<int> Level = new GenericFlag<int>("Level", 0);
    public CellState State;

	public void SetColor(Color color)
	{
        foreach (MeshRenderer mr in GetComponentsInChildren<MeshRenderer>())
		{
			foreach(Material material in mr.materials)
			{
                if (material.shader == Shader.Find("Shader Forge/PlayerShader"))
				{
					material.color = new Color (color.r, color.g, color.b, material.color.a);
				}
			}
		}
	}

	public void SetCell(CellState state)
	{
        State = state;
		if(model)
		{
            Destroy(model);
            //Lean.Pool.LeanPool.Despawn(model);
		}


        if (state && state.prefab) 
		{
            model = Instantiate(state.prefab);// Lean.Pool.LeanPool.Spawn(state.prefab);
			model.transform.SetParent (transform);
			model.transform.localRotation = Quaternion.identity;
			model.transform.localPosition = Vector3.zero;
			model.transform.localScale = Vector3.one;
		}
		//FindObjectOfType<Block>().GetComponentInChildren<TextMeshProUGUI> ().text = state.StateName;
	}
}
