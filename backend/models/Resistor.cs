public class Resistor : Component
{
    public double Resistance
    {
        get;
        set;
    }

    public Resistor(int id, double resistance, int x, int y) : base(id, "Resistor", x, y)
    {
        Resistance = resistance;
    }

    public double GetResistance()
    {
        return Resistance;
    }
}