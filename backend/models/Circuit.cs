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

    public int GetConnectionCount(int id) // Method that finds the number of components the component is connected to
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

    public List<int> GetNeighbours(int currentNode) // Method that gets the all components connected to the paramaterised component
    {
        var neighbours = new List<int>();

        foreach (var wire in Wires)
        {
            if (wire.StartId == currentNode) // checks if they share a wire
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

    public List<int> TraverseBranch(int id, List<int> visited) // Method for finding a branch of components (for parallel circuits)
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

    public List<List<int>> GetCircuitStructure(List<int> TerminalNodes) // Main BFS method for traversing a full circuit
    {
        var CircuitStructure = new List<List<int>>();
        var visited = new List<int>();
        var queue = new Queue<int>(); // A queue is used to traverse all components

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
                        queue.Enqueue(neighbour); // adds all neighbours of the specified component to the queue to be traversed
                    }
                }
            }
        }
        
        return CircuitStructure;
    }

    public double BranchResistance(List<int> branch) // Method for returning the total resistance of a branch of components
    {
        double resistance = 0;

        foreach (var id in branch)
        {
            var comp = Components.First(c => c.ComponentId == id);
            if (comp is Resistor r)
            {
                resistance += r.Resistance;
            }
            
        }

        return resistance;
    }

    public double ParallelResistance(List<double> resistances) // Method for calculating the total resistance in the circuit
    {
        double inverse = 0;

        foreach (var resistance in resistances)
        {
            if (resistance != 0)
            {
                inverse += 1 / resistance;
            }
        }

        return 1 / inverse;
    }

    public double GetVoltage() // Method for returning the total voltage of a circuit
    {
        double voltage = 0;

        foreach (var component in Components)
        {
            if (component is Battery)
            {
                Battery b = (Battery)component;
                voltage += b.Emf;
            }
        }

        return voltage;
    }

    public object SolveCircuit() // Main method to solve a circuit and create an object containing components and their key values
    {
        double CircuitVoltage = GetVoltage();

        var TerminalNodes = new List<int>();
        foreach (var comp in Components)
        {
            if (GetConnectionCount(comp.ComponentId) >= 2)
            {
                if (TerminalNodes.Contains(comp.ComponentId) == false)
                {
                    TerminalNodes.Add(comp.ComponentId);
                }
            }
        }

        var branches = GetCircuitStructure(TerminalNodes);

        var BranchResistances = new List<double>();

        foreach (var branch in branches)
        {
            BranchResistances.Add(BranchResistance(branch));
        }

        double TotalResistance = ParallelResistance(BranchResistances);

        double TotalCurrent = CircuitVoltage / TotalResistance;

        var SolvedComponents = new List<object>();

        for (int i = 0; i < branches.Count; i++)
        {
            var branch = branches[i];

            double resistance = BranchResistances[i];
            double voltage = CircuitVoltage;
            double current = voltage / resistance;

            foreach (var id in branch)
            {
                var comp = Components.First(c => c.ComponentId == id);

                double CompRes = 0;
                if (comp is Resistor r)
                {
                    CompRes = r.Resistance;
                }
                else if (comp is Lamp l)
                {
                    CompRes = l.CalculateResistance(voltage);                  
                }

                double CompVoltage = 0;
                
                if (comp is Battery b)
                {
                    CompVoltage = b.Emf;
                }
                else
                {
                    CompVoltage = CompRes / resistance * voltage;
                }

                double CompCurrent = current;

                var component = new
                {
                    componentId = comp.ComponentId,
                    voltage = CompVoltage,
                    current = CompCurrent,
                    resistance = CompRes
                };

                SolvedComponents.Add(component);
            }
        }

        var circuit = new
        {
            CircuitVoltage,
            SolvedComponents,
        };

        return circuit;
    }
}