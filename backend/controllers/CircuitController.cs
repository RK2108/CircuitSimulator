using System.Data;
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
    public IActionResult Simulate([FromBody] CircuitDTO circuitDto)
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

    public Circuit ConvertFromDTO(CircuitDTO circuitDTO)
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
    public async Task<IActionResult> SaveCircuit([FromBody] CircuitDTO circuitDto)
    {
        Circuit payload = ConvertFromDTO(circuitDto);

        var circuit = database.Circuits.Include(c => c.Components).Include(c => c.Wires).FirstOrDefault(c => c.CircuitId == payload.CircuitId);

        if (circuit != null)
        {
            database.Wires.RemoveRange(circuit.Wires);
            database.Components.RemoveRange(circuit.Components);
            database.Circuits.Remove(circuit);
            await database.SaveChangesAsync();
        }

        await database.Circuits.AddAsync(payload);  
        await database.SaveChangesAsync();

        return Ok("Circuit has been saved");
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateCircuit([FromBody] CircuitDTO circuitDto)
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
    public async Task<IActionResult> GetAllCircuits()
    {
        var AllCircuits = database.Circuits.ToList();
        return Ok(AllCircuits);
    }

    [HttpPost("load")]
    public async Task<IActionResult> LoadCircuit([FromBody] int id)
    {
        return Ok(database.Circuits.Where(c => c.CircuitId == id).Include(c => c.Components).Include(c => c.Wires).ToList());
    }
}