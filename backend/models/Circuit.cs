

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

    public List<List<int>> GetCircuitStructure() // returns the overall structure of the circuit, modelled as a graph
    {
        var FullStructure = new List<List<int>>();
        var visited = new List<int>();
        var queue = new Queue<int>();

        foreach (var comp in Components)
        {
            if (GetConnectionCount(comp.Id) != 2)
            {
                queue.Enqueue(comp.Id);
            }
        }

        while (queue.Count > 0)
        {
            var section = new List<int>();

            int currentNode = queue.Dequeue();
            section.Add(currentNode);
        


        }

        return FullStructure;
    }

    public List<int> GetNeighbours(int currentNode)
    {
        var neighbours = new List<int>();

        foreach (var wire in Wires)
        {
            if (wire.StartId == currentNode)
            {
                neighbours.Add(wire.EndId);
            }
            else if (wire.EndId == currentNode)
            {
                neighbours.Add(wire.StartId);
            }
        }

        return neighbours;
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

    public List<int> GetSeriesSection(int startId, int currentNode, List<int> visitedNodes)
    {
        var section = new List<int>();
        section.Add(startId);

        int previous = startId;
        int current = currentNode;

        while (GetConnectionCount(current) == 2)
        {
            section.Add(current);
            visitedNodes.Add(current);

            var neighbours = GetNeighbours(current);

            int next = neighbours.First(n => n != previous);

            previous = current;
            current = next;
        }

        section.Add(current);

        return section;
    }

    public List<List<int>> GetAllSections()
    {
        var queue = new Queue<int>();
        var visitedNodes = new List<int>();
        var visitedBoundaries = new List<int>();
        var AllSections = new List<List<int>>();

        foreach (var comp in Components)
        {
            if (GetConnectionCount(comp.Id) != 2)
            {
                queue.Enqueue(comp.Id);
                visitedBoundaries.Add(comp.Id);
            }
        }

        while (queue.Count > 0)
        {
            int start = queue.Dequeue();
            
            var neighbours = GetNeighbours(start);

            foreach (var neighbour in neighbours)
            {
                if (visitedNodes.Contains(neighbour) == true){
                    continue;
                }

                var section = GetSeriesSection(start, neighbour, visitedNodes);

                AllSections.Add(section);

                int end = section.Last();

                if (visitedBoundaries.Contains(end) == false)
                {
                    queue.Enqueue(end);
                    visitedBoundaries.Add(end);
                }
            }
        }

        return AllSections;
    }

    public List<List<int>> GetParallelSections(List<List<int>> AllSections)
    {
        var parallelgroups = new List<List<int>>();
        var checkedgroups = new List<int>();

        for(int i = 0; i < AllSections.Count - 1; i++)
        {
            if (checkedgroups.Contains(i) == true)
            {
                continue;
            }

            var section1 = AllSections[i];

            var currentgroup = new List<int>();

            for(int j = 0; j < AllSections.Count - 1; j++)
            {
                var section2 = AllSections[j];

                if (section1[0] == section2[0] && section1.Last() == section2.Last())
                {
                    currentgroup = section1;
                    checkedgroups.Add(j);
                }
            }

            if (currentgroup.Count > 1)
            {
                parallelgroups.Add(currentgroup);
            }
        }

        return parallelgroups;
    }


    public double CalculateResistance()
    {
        var SeriesSections = GetAllSections();
        var ParallelSections = GetParallelSections(SeriesSections);
        double seriesResistance = 0;

        foreach (var section in SeriesSections)
        {
            if (ParallelSections.Contains(section))
            {
                SeriesSections.Remove(section);
            }
        }

        if (SeriesSections != null)
        {
            foreach (var section in SeriesSections)
            {
                double SectionResistance = 0;
                foreach (var component in section)
                {
                    foreach (var comp in Components)
                    {
                        if (comp.Id == component)
                        {
                            if (comp is Resistor)
                            {
                                Resistor r = (Resistor)comp;
                                SectionResistance += r.GetResistance();
                            }
                        }
                    }
                }

                seriesResistance += SectionResistance;
            }    
        }

        double inverseResistance = 0;
        foreach (var section in ParallelSections)
        {
            double SectionResistance = 0;
            foreach (var component in section)
            {
                foreach (var comp in Components)
                {
                    if (comp.Id == component)
                    {
                        if (comp is Resistor)
                        {
                            Resistor r = (Resistor)comp;
                            SectionResistance += r.GetResistance();
                        }
                    }
                }
            }

            inverseResistance += 1 / SectionResistance;
        }

        double totalResistance = seriesResistance + (1 / inverseResistance);

        return totalResistance;
    }
}