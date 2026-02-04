namespace backend.models
{
    public abstract class Component
    {
        public Guid ComponentId
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

        public int CircuitId
        {
            get;
            set;
        }

        public Component(Guid componentId, string componentType, int x, int y)
        {
            ComponentType = componentType;
            ComponentId = componentId;
            X = x;
            Y = y;
        }
    }
}