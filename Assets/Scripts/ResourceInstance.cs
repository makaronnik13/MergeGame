

using com.armatur.common.flags;
using com.armatur.common.serialization;

[System.Serializable]
public class ResourceInstance
{
    [SerializeData("ResType", FieldRequired.False)]
    [Savable]
    private GenericFlag<int> resId = new GenericFlag<int>("resId", -1);

    public GameResource ResType
    {
        get
        {
            return DefaultRessources.GetRes(resId.Value);
        }
        set
        {
            resId.SetState(DefaultRessources.GetResId(value));
        }
    }

    [SerializeData("ResVal", FieldRequired.False)]
    [Savable]
    public CounterWithMax ResVal = new CounterWithMax("ResVal", 0);

    public ResourceInstance()
    {

    }

    public ResourceInstance(GameResource res, long curr = 0, long max = long.MaxValue)
    {
        if (max == 0)
        {
            max = long.MaxValue;
        }
        ResType = res;
        ResVal.SetState(curr);
        ResVal.SetMax(max);
    }
}