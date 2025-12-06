public class Wire
{
    public int WireId
    {
        get;
        set;
    }

    public int StartId
    {
        get;
        set;
    }

    public int EndId
    {
        get;
        set;
    }

    public Wire(int wireId, int startId, int endId)
    {
        WireId = wireId;
        StartId = startId;
        EndId = endId;
    }
}