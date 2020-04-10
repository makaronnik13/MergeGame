using UnityEngine;

public class LineInfo
{
    public Vector3 From, To;
    public BuffLineInfo lineIfo;

    public LineInfo(Vector3 from, Vector3 to, BuffLineInfo bli)
    {
        lineIfo = bli;
        From = from;
        To = to;
    }
}