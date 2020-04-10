using System.Collections.Generic;

public class BuffLineInfo
{
    public int buffid;
    public Dictionary<Block, List<Inkome>> buffs;
    public bool emmiting;
    public bool combination;

    public BuffLineInfo(int buffid, bool emmiting, Dictionary<Block, List<Inkome>> bb, bool combination = false)
    {
        this.combination = combination;
        this.buffid = buffid;
        this.buffs = bb;
        this.emmiting = emmiting;
    }
}