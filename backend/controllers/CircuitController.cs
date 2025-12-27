using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class CircuitController : ControllerBase
{
    private readonly CircuitDbContext database;

    public CircuitController(CircuitDbContext db)
    {
        database = db;
    }

    [HttpPost("simulate")]
    public IActionResult Simulate([FromBody] CircuitDTO circuitDto) // Method for simulating a circuit and returning key values
    {
        Circuit circuit = ConvertFromDTO(circuitDto);

        if (circuit.Components.Count < 2)
        {
            return BadRequest("Invalid Circuit: Add more components");
        }
        
        int batteryCount = 0;
        foreach (var component in circuit.Components)
        {
            if (component.ComponentType == "Battery")
            {
                batteryCount++;
            }
        }

        if (batteryCount == 0)
        {
            return BadRequest("Invalid Circuit: Add a battery");
        }

        var SolvedCircuit = circuit.SolveCircuit();

        return Ok(SolvedCircuit);
    }

    public Circuit ConvertFromDTO(CircuitDTO circuitDTO) // Helper method for converting circuits sent from frontend to C# objects
    {
        Circuit circuit = new Circuit(circuitDTO.CircuitId, circuitDTO.Name);

        foreach (var compDTO in circuitDTO.Components)
        {
            Component comp = null;

            if (compDTO.type == "Resistor")
            {
                comp = new Resistor(compDTO.id, compDTO.resistance, compDTO.x, compDTO.y);
            }
            else if (compDTO.type == "Battery")
            {
                comp = new Battery(compDTO.id, compDTO.voltage, compDTO.x, compDTO.y);
            }
            else if (compDTO.type == "Lamp")
            {
                comp = new Lamp(compDTO.id, compDTO.power, compDTO.x, compDTO.y);
            }

            if (comp != null)
            {
                circuit.AddComponent(comp);
            }
        }

        foreach (var wireDTO in circuitDTO.Wires)
        {
            int start = wireDTO.StartId;
            int end = wireDTO.EndId;

            if (start == end)
            {
                continue;
            }

            bool startExists = circuit.Components.Any(c => c.ComponentId == start);
            bool endExists = circuit.Components.Any(c => c.ComponentId == end);

            if (startExists && endExists)
            {
                
                Wire w = new Wire(wireDTO.Id, wireDTO.StartId, wireDTO.EndId);
                circuit.AddWire(w);
            }
        }

        return circuit;
    }


    [HttpPost("save")]
    public async Task<IActionResult> SaveCircuit([FromBody] CircuitDTO circuitDto) // Method for further saves of a circuit
    {
        Circuit payload = ConvertFromDTO(circuitDto);

        var circuit = database.Circuits.Include(c => c.Components) // checks if the circuit is already stored in the database (failsafe)
                                        .Include(c => c.Wires)
                                        .FirstOrDefault(c => c.CircuitId == payload.CircuitId);

        if (circuit != null) // if already stored, it is removed
        {
            database.Wires.RemoveRange(circuit.Wires);
            database.Components.RemoveRange(circuit.Components);
            database.Circuits.Remove(circuit);
            await database.SaveChangesAsync();
        }

        await database.Circuits.AddAsync(payload); // regardless if it is already stored, the updated circuit will be saved
        await database.SaveChangesAsync();

        return Ok("Circuit has been saved");
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateCircuit([FromBody] CircuitDTO circuitDto) // Method for creating a circuit and storing it initially
    {
        Circuit circuit = ConvertFromDTO(circuitDto);

        if (database.Circuits.Contains(circuit))
        {
            return BadRequest("Circuit already exists with this id/name");
        }
        else
        {
            await database.Circuits.AddAsync(circuit);
            await database.SaveChangesAsync();

            return Ok("Circuit Created");
        }
    }

    [HttpGet("GetAllCircuits")]
    public async Task<IActionResult> GetAllCircuits() // Method for returning a list of all stored circuits
    {
        var AllCircuits = database.Circuits.ToList();
        return Ok(AllCircuits);
    }

    [HttpPost("load")]
    public async Task<IActionResult> LoadCircuit([FromBody] int id) // Method for loading a circuit, whilst mapping components correctly
    {
        int CircuitId = 0;
        string Name = "";

        foreach (var circuit in database.Circuits)
        {
            if (circuit.CircuitId == id)
            {
                CircuitId = id;
                Name = circuit.Name;
            }
        }

        var components = new List<object>();

        foreach (var comp in database.Components.Where(c => c.CircuitId == id))
        {
            double? resistance;     
            double? voltage;        // nullable variables account for null values in database
            double? power;

            if (comp is Resistor r)
            {
                resistance = r.Resistance;

                var component = new
                {
                    comp.ComponentId,
                    comp.ComponentType,
                    comp.X,
                    comp.Y,
                    Resistance = resistance, // if resistor, then resistance is needed
                    Voltage = 0,
                    Power = 0
                };

                components.Add(component);
            }
            else if (comp is Battery b)
            {
                voltage = b.Emf;

                var component = new
                {
                    comp.ComponentId,
                    comp.ComponentType,
                    comp.X,
                    comp.Y,
                    Resistance = 0,
                    Voltage = voltage, // if battery, then voltage is needed
                    Power = 0
                };

                components.Add(component);
            }
            else if (comp is Lamp l)
            {
                power = l.Power;

                var component = new
                {
                    comp.ComponentId,
                    comp.ComponentType,
                    comp.X,
                    comp.Y,
                    Resistance = 0,
                    Voltage = 0,
                    Power = power // if lamp, then power is needed
                };

                components.Add(component);
            }
        }

        var wires = database.Wires;

        return Ok(new   // returns full circuit
        {
            CircuitId,
            Name,
            components,
            wires
        }); 
    }
}