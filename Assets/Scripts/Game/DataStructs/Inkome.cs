
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Inkome
{
	public GameResource resource;

    public long value;

    public Inkome(Inkome inc)
    {
        resource = inc.resource;
        value = inc.value;
    }

    public Inkome(GameResource res, long v)
    {
        resource =res;
        value = v;
    }
}
