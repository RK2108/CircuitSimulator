public class Resistor : Component
{
    public double Resistance
    {
        get;
        set;
    }

    public Resistor(int componentId, double resistance, int x, int y) : base(componentId, "Resistor", x, y)
    {
        Resistance = resistance;
    }

    public double GetResistance()
    {
        return Resistance;
    }
}