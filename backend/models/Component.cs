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

    public Component(int id, string type, int x, int y)
    {
        ComponentType = type;
        ComponentId = id;
        X = x;
        Y = y;
    }
}