public class Battery : Component
{
    public double EMF
    {
        get;
        set;
    }

    public Battery(int id, double emf, int x, int y) : base(id, "Battery", x, y)
    {
        EMF = emf;
    }
}