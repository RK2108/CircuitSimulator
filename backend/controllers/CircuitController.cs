using System.Data.Common;
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
        try
        {
            Circuit circuit = ConvertFromDTO(circuitDto);

            if (circuit.Components.Count < 2)
            {
                throw new Exception("Invalid Circuit: Add more components");
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
                throw new Exception("Invalid Circuit: Add a battery");
            }

            var SolvedCircuit = circuit.SolveCircuit();

            return Ok(SolvedCircuit); 
        }
        catch (InvalidOperationException)
        {
            return BadRequest("Infinite Values: check component input values");        
        }
        catch (ArithmeticException)
        {
            return BadRequest("Math Error");
        }
        catch (Exception)
        {
            return BadRequest("Something went wrong...");
        }
    }

    public Circuit ConvertFromDTO(CircuitDTO circuitDTO) // Helper method for converting circuits sent from frontend to C# objects
    {
        try
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
                    comp.CircuitId = circuitDTO.CircuitId;
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
                    
                    Wire w = new Wire(wireDTO.WireId, wireDTO.StartId, wireDTO.EndId);
                    w.CircuitId = circuitDTO.CircuitId;
                    circuit.AddWire(w);
                }
            }

            return circuit;
        }
        catch (Exception)
        {
            throw new Exception("Couldn't process circuit");
        }
    }


    [HttpPost("save")]
    public async Task<IActionResult> SaveCircuit([FromBody] CircuitDTO circuitDto) // Method for further saves of a circuit
    {
        try
        {
            Circuit payload = ConvertFromDTO(circuitDto);

            var circuit = await database.Circuits.Include(c => c.Components) // checks if the circuit is already stored in the database (failsafe)
                                            .Include(c => c.Wires)
                                            .FirstOrDefaultAsync(c => c.CircuitId == payload.CircuitId);

            // PATCH Components //

            // Adding New Components //

            circuit.Name = payload.Name;

            foreach (var component in payload.Components)
            {
                var ExistingComp = circuit.Components.FirstOrDefault(c => c.ComponentId == component.ComponentId);

                if (ExistingComp == null)
                {
                    Component NewComp;

                    switch (component)
                    {
                        case Resistor r:
                            NewComp = new Resistor(r.ComponentId, r.Resistance, r.X, r.Y);
                            break;
                        case Battery b:
                            NewComp = new Battery(b.ComponentId, b.Emf, b.X, b.Y);
                            break;
                        case Lamp l:
                            NewComp = new Lamp(l.ComponentId, l.Power, l.X, l.Y);
                            break;
                        default:
                            continue;
                    }

                    NewComp.CircuitId = payload.CircuitId;

                await database.Components.AddAsync(NewComp);
                }
                // Editing Saved Components //
                else
                {
                    ExistingComp.X = component.X;
                    ExistingComp.Y = component.Y;

                    switch (ExistingComp)
                    {
                        case Resistor r:
                            r.Resistance = ((Resistor)component).Resistance;
                            break;
                        case Battery b:
                            b.Emf = ((Battery)component).Emf;
                            break;
                        case Lamp l:
                            l.Power = ((Lamp)component).Power;
                            break;
                    }
                }
            }

            // Deleting Removed Components //

            var PayloadCompIDs = new List<int>();

            foreach (var comp in payload.Components)
            {
                PayloadCompIDs.Add(comp.ComponentId);
            }

            var ComponentsToRemove = circuit.Components.Where(c => !PayloadCompIDs.Contains(c.ComponentId));

            database.RemoveRange(ComponentsToRemove);

            // PATCH Wires //

            foreach (var wire in payload.Wires)
            {
                var ExistingWire = circuit.Wires.FirstOrDefault(w => w.WireId == wire.WireId);

                if (ExistingWire == null)
                {
                    var NewWire = new Wire(wire.WireId, wire.StartId, wire.EndId);
                    NewWire.CircuitId = payload.CircuitId;
                    circuit.Wires.Add(NewWire);
                }
            }

            var PayloadWireIDs = new List<int>();

            foreach (var wire in payload.Wires)
            {
                PayloadWireIDs.Add(wire.WireId);
            }

            var WiresToRemove = circuit.Wires.Where(w => !PayloadWireIDs.Contains(w.WireId));

            database.RemoveRange(WiresToRemove);

            await database.SaveChangesAsync();

            return Ok("Circuit has been saved");
        }
        catch (DbException)
        {
            return StatusCode(500, "Database error");
        }
        catch (Exception)
        {
            return BadRequest("Error");
        }
        
    }
    

    [HttpPost("create")]
    public async Task<IActionResult> CreateCircuit([FromBody] CircuitDTO circuitDto) // Method for creating a circuit and storing it initially
    {
        try
        {
            Circuit circuit = ConvertFromDTO(circuitDto);

            if (await database.Circuits.ContainsAsync(circuit))
            {
                return BadRequest("Circuit already exists with this id/name");
            }
            else
            {
                await database.Circuits.AddAsync(circuit);
                await database.SaveChangesAsync();

                var id = circuit.CircuitId;

                return Ok(id);
            }
        }
        catch (Exception)
        {
            return BadRequest("Something went wrong...");
        }
    }

    [HttpGet("GetAllCircuits")]
    public async Task<IActionResult> GetAllCircuits() // Method for returning a list of all stored circuits
    {
        try
        {
            var AllCircuits = await database.Circuits.ToListAsync();
            return Ok(AllCircuits);
        }
        catch (DbException)
        {
            return StatusCode(500, "Couldn't retrive circuits");
        }
        catch (Exception)
        {
            return BadRequest("Something went wrong...");
        }
    }

    [HttpPost("load")]
    public async Task<IActionResult> LoadCircuit([FromBody] int id) // Method for loading a circuit, whilst mapping components correctly
    {
        try
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

            var wires = database.Wires.Where(w => w.CircuitId == CircuitId);

            return Ok(new   // returns full circuit
            {
                CircuitId,
                Name,
                components,
                wires
            }); 
        }
        catch (DbException)
        {
            return StatusCode(500, "Database Error");
        }
        catch (Exception)
        {
            return BadRequest("Something went wrong...");
        }
    }
}