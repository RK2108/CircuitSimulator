namespace backend.DTOs
{
    public class ComponentDTO
    {
        public required string Type
        {
            get;
            set;
        }

        public int Id
        {
            get;
            set;
        }

        public required int X
        {
            get;
            set;
        }

        public required int Y
        {
            get;
            set;
        }

        public double Resistance
        {
            get;
            set;
        }

        public double Voltage
        {
            get;
            set;
        }

        public double Power
        {
            get;
            set;
        }
    }
}