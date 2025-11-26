public class CircuitDTO
{
    public int CircuitId
    {
        get;
        set;
    }

    public required string Name
    {
        get;
        set;
    }

    public required List<ComponentDTO> Components
    {
        get;
        set;
    }

    public required List<WireDTO> Wires
    {
        get;
        set;
    }
}