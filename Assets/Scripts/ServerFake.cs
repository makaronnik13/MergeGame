using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ServerFake : MonoBehaviour
{
    public static ServerFake Instance;

    private Queue<int> resources;

    public int GetRes()
    {
        if (resources.Count == 0)
        {
            FillQueue();
        }
        return resources.Dequeue();
    }

    private void FillQueue()
    {
        resources = new Queue<int>();
        for (int i = 0; i < 100; i++)
        {
            resources.Enqueue(0);
            resources.Enqueue(1);
            resources.Enqueue(2);
        }
    }

    private void Awake()
    {
        Instance = this;
        FillQueue();
        resources = new Queue<int>(resources.ToList().OrderBy(g=>System.Guid.NewGuid()));
    }
}
