using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ModelReplacer : MonoBehaviour
{
    private Dictionary<MeshRenderer, List<Material>> savedMaterial = new Dictionary<MeshRenderer, List<Material>>();
    public Material[] BlockedMaterials;

    private bool blocked = false;

    public void SetBlocked(bool v, int mat)
    {
        blocked = v;
        if (blocked)
        {
            foreach (KeyValuePair<MeshRenderer, List<Material>> pair in savedMaterial)
            {
                List<Material> newMat = new List<Material>();
                foreach (Material m in pair.Value)
                {
                    newMat.Add(BlockedMaterials[mat]);
                }
                pair.Key.materials = newMat.ToArray();
            }
        }
        else
        {
            foreach (KeyValuePair<MeshRenderer, List<Material>> pair in savedMaterial)
            {
                pair.Key.materials = pair.Value.ToArray();
            }
        }
    }

    private void Awake()
    {
        foreach (MeshRenderer mr in GetComponentsInChildren<MeshRenderer>())
        {
            savedMaterial.Add(mr, mr.materials.ToList());
        }
    }

    public void SetModel(int id)
	{
		int i = 0;
		foreach(Transform t in transform)
		{
			t.gameObject.SetActive (i == id);
			i++;
		}
	}
}
