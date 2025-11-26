public class Circuit
{
    public string Name
    {
        get;
        set;
    }

    public int CircuitId
    {
        get;
        set;
    }

    public List<Component> Components
    {
        get;
        set;
    } = new();

    public List<Wire> Wires
    {
        get;
        set;
    } = new();

    public Circuit(int CircuitId, string Name)
    {
        this.CircuitId = CircuitId;
        this.Name = Name;
    }

    public void AddComponent(Component comp)
    {
        Components.Add(comp);
    }

    public void AddWire(Wire w)
    {
        Wires.Add(w);
    }

    public void RemoveComponent(Component comp)
    {
        Components.Remove(comp);
    }

}