public class Circuit
{
    public string Name
    {
        get;
        set;
    }

    public int CircuitId
    {
        get;
        set;
    }

    public List<Component> Components
    {
        get;
        set;
    } = new();

    public List<Wire> Wires
    {
        get;
        set;
    } = new();

    public Circuit(int CircuitId, string Name)
    {
        this.CircuitId = CircuitId;
        this.Name = Name;
    }

    public void AddComponent(Component comp)
    {
        Components.Add(comp);
    }

    public void AddWire(Wire w)
    {
        Wires.Add(w);
    }

    public void RemoveComponent(Component comp)
    {
        Components.Remove(comp);
    }

    public int GetConnectionCount(int id) // finds the number of components the component is connected to
    {
        int count = 0;

        foreach (var wire in Wires)
        {
            if (wire.StartId == id || wire.EndId == id)
            {
                count++;
            }
        }

        return count;
    }

    public List<int> GetNeighbours(int currentNode)
    {
        var neighbours = new List<int>();

        foreach (var wire in Wires)
        {
            if (wire.StartId == currentNode)
            {
                if (neighbours.Contains(wire.EndId) == false)
                {
                    neighbours.Add(wire.EndId);
                }
            }
            else if (wire.EndId == currentNode)
            {
                if (neighbours.Contains(wire.StartId) == false)
                {
                    neighbours.Add(wire.StartId);
                }
            }
        }

        return neighbours;
    }

    public List<int> TraverseBranch(int id, List<int> visited)
    {
        var branch = new List<int>();
        int currentNode = id;

        while (visited.Contains(currentNode) == false)
        {
            visited.Add(currentNode);
            branch.Add(currentNode);

            var neighbours = GetNeighbours(currentNode);

            for (int i=0; i < neighbours.Count - 1; i++)
            {
                if (visited.Contains(neighbours[i]))
                {
                    neighbours.Remove(neighbours[i]);
                }
            }

            if (neighbours.Count == 0)
            {
                break;
            }

            currentNode = neighbours[0];
        }

        return branch;
    }

    public bool DetectLoop(List<int> branch)
    {
        if (branch.Count > 1 && branch.First() == branch.Last())
        {
            return true;
        }

        return false;
    }

    public string ClassifyCircuit(List<List<int>> branches)
    {
        var loops = new List<List<int>>();

        foreach (var branch in branches)
        {
            if (DetectLoop(branch))
            {
                loops.Add(branch);
            }
        }

        if (loops.Count == 1)
        {
            return "Series Loop";
        }
        else if (loops.Count == 2)
        {
            if (loops[0].First() == loops[1].First() && loops[0].Last() == loops[1].Last())
            {
                return "Parallel Loops";
            }
        }
        
        return "Spider";
    }

    public List<List<int>> GetCircuitStructure(List<int> TerminalNodes)
    {
        var CircuitStructure = new List<List<int>>();
        var visited = new List<int>();
        var queue = new Queue<int>();

        foreach (var id in TerminalNodes)
        {
            if (visited.Contains(id))
            {
                continue;
            }

            queue.Enqueue(id);

            while (queue.Count > 0)
            {
                int currentNode = queue.Dequeue();

                if (visited.Contains(currentNode))
                {
                    continue;
                }

                var branch = TraverseBranch(currentNode, visited);
                CircuitStructure.Add(branch);
                
                foreach (var neighbour in GetNeighbours(currentNode))
                {
                    if (visited.Contains(neighbour) == false)
                    {
                        queue.Enqueue(neighbour);
                    }
                }
            }
        }
        
        return CircuitStructure;
    }

    public double BranchResistance(List<int> branch)
    {
        double resistance = 0;

        foreach (var id in branch)
        {
            foreach (var comp in Components)
            {
                if (comp.Id == id)
                {
                    if (comp is Resistor)
                    {
                        Resistor r = (Resistor)comp;
                        resistance += r.Resistance;
                    }
                }
            }
        }

        return resistance;
    }

    public double ParallelResistance(List<double> resistances)
    {
        double inverse = 0;

        foreach (var resistance in resistances)
        {
            inverse += 1 / resistance;
        }

        return 1 / inverse;
    }

    public double CalculateResistance()
    {
        var TerminalNodes = new List<int>();
        foreach (var comp in Components)
        {
            if (GetConnectionCount(comp.Id) >= 2)
            {
                if (TerminalNodes.Contains(comp.Id) == false)
                {
                    TerminalNodes.Add(comp.Id);
                }
            }
        }

        var CircuitStructure = GetCircuitStructure(TerminalNodes);

        string type = ClassifyCircuit(CircuitStructure);

        var BranchResistances = new List<double>();

        foreach (var branch in CircuitStructure)
        {
            double res = BranchResistance(branch);
            BranchResistances.Add(res);
        }

        double totalResistance = 0;

        if (type == "Series Loop")
        {
            totalResistance = BranchResistances[0];
        }
        else if (type == "Parallel Loops" || type == "Spider")
        {
            totalResistance = ParallelResistance(BranchResistances);
        }

        return totalResistance;
    }

    public double GetVoltage()
    {
        double voltage = 0;

        foreach (var component in Components)
        {
            if (component is Battery)
            {
                Battery b = (Battery)component;
                voltage += b.EMF;
            }
        }

        return voltage;
    }

    public double GetCurrent(double voltage, double resistance)
    {
        double current = voltage / resistance;

        return current;
    }
}