

using System.Runtime.InteropServices.Marshalling;
using Microsoft.OpenApi.Services;
using Microsoft.VisualBasic;

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

    public List<int> GetConnectedComponents(List<int> TerminalNodes)
    {
        var visited = new List<int>();
        var queue = new Queue<int>();

        foreach (var id in TerminalNodes)
        {
            if (visited.Contains(id))
            {
                continue;
            }

            int startNode = Components.FindIndex(c => c.Id == id);

            queue.Enqueue(startNode);
            visited.Add(startNode);

            while (queue.Count > 0)
            {
                int currentNode = queue.Dequeue();
                var neighbours = GetNeighbours(currentNode);

                foreach (var neighbour in neighbours)
                {
                    if (visited.Contains(neighbour) == false)
                    {
                        visited.Add(neighbour);
                        queue.Enqueue(neighbour);
                    }
                }
            }
        }



        return visited;
    }

    public double CalculateResistance()
    {
        var TerminalNodes = new List<int>();
        foreach (var comp in Components)
        {
            if (GetConnectionCount(comp.Id) < 3)
            {
                if (TerminalNodes.Contains(comp.Id) == false)
                {
                    TerminalNodes.Add(comp.Id);
                }
            }
        }

        var ConnectedComponents = GetConnectedComponents(TerminalNodes);

        double resistance = 0;

        foreach (var id in ConnectedComponents)
        {
            foreach(var comp in Components)
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

    public double GetCurrent()
    {
        double voltage = GetVoltage();
        double resistance = CalculateResistance();

        double current = voltage / resistance;

        return current;
    }
}