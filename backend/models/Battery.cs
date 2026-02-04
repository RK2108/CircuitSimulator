namespace backend.models
{
    public class Battery : Component
    {
        public double Emf
        {
            get;
            set;
        }

        public Battery(Guid componentId, double emf, int x, int y) : base(componentId, "Battery", x, y)
        {
            Emf = emf;
        }
    }
}