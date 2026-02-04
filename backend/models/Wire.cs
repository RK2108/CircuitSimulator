namespace backend.models
{
    public class Wire
    {
        public Guid WireId
        {
            get;
            set;
        }

        public Guid StartId
        {
            get;
            set;
        }

        public Guid EndId
        {
            get;
            set;
        }

        public int CircuitId
        {
            get;
            set;
        }

        public Wire(Guid wireId, Guid startId, Guid endId)
        {
            this.WireId = wireId;
            StartId = startId;
            EndId = endId;
        }
    }
}