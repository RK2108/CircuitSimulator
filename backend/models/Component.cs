public abstract class Component
{
    public int Id
    {
        get;
        set;
    }

    public string ComponentType
    {
        get;
        set;
    }

    public int x
    {
        get;
        set;
    }

    public int y
    {
        get;
        set;
    }

    public Component(int id, string type, int x, int y)
    {
        ComponentType = type;
        Id = id;
        this.x = x;
        this.y = y;
    }
}