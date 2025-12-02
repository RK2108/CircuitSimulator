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

    public Wire(int startId, int endId)
    {
        StartId = startId;
        EndId = endId;
    }
}