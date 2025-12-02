public class Lamp : Component
{
    public double Power
    {
        get;
        set;
    }

    public Lamp(int componentId, double power, int x, int y) : base(componentId, "Lamp", x, y)
    {
        Power = power;
    }

    public double CalculateResistance(double voltage)
    {
        return Math.Pow(voltage, 2) / Power;
    }
}