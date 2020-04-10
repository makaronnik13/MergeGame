using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Popup : MonoBehaviour
{
    [SerializeField]
    private float lifetime = 2f;

    [SerializeField]
    private float flowSpeed = 1f;

    private float t = 0;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    // Update is called once per frame
    void Update()
    {
        foreach (TMPro.TextMeshProUGUI te in GetComponentsInChildren<TMPro.TextMeshProUGUI>())
        {
            te.color = Color.Lerp(Color.white, new Color(1,1,1,0), t/lifetime);
        }
        transform.Translate(Vector3.up*Time.deltaTime*flowSpeed);
        t += Time.deltaTime;
    }
}
