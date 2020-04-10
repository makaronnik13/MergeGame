using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempModel : MonoBehaviour
{
    [SerializeField]
    private Material tempMaterial;

    private GameObject tempModel;

    public void ShowTemp(CellState state)
    {
        tempModel = Instantiate(state.prefab);
        tempModel.transform.SetParent(transform);
        tempModel.transform.localPosition = Vector3.zero;
        tempModel.transform.localRotation = Quaternion.identity;
        tempModel.transform.localScale = Vector3.one;

        foreach (MeshRenderer mr in tempModel.GetComponentsInChildren<MeshRenderer>())
        {
            List<Material> tm = new List<Material>();
            foreach (Material m in mr.materials)
            {
                tm.Add(tempMaterial);
            }
            mr.materials = tm.ToArray();
        }

       
    }

    public void HideTemp()
    {
        if (tempModel)
        {
            Destroy(tempModel);
        }
    }
}
