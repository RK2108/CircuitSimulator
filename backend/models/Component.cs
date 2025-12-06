public abstract class Component
{
    public int ComponentId
    {
        get;
        set;
    }

    public string ComponentType
    {
        get;
        set;
    }

    public int X
    {
        get;
        set;
    }

    public int Y
    {
        get;
        set;
    }

    public Component(int componentId, string componentType, int x, int y)
    {
        ComponentType = componentType;
        ComponentId = componentId;
        X = x;
        Y = y;
    }
}