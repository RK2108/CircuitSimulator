using System.Data.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// used namespaces
using backend.DTOs;
using backend.models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace backend.Controllers
{
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

                ValidateCircuit(circuit);

                var SolvedCircuit = circuit.SolveCircuit();

                return Ok(SolvedCircuit); 
            }
            catch (InvalidOperationException err)
            {
                return BadRequest($"{err.Message}");
            }
            catch (DivideByZeroException)
            {
                return BadRequest("Infinite Resistance: check component input values");        
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

                    if (compDTO.Type == "Resistor")
                    {
                        comp = new Resistor(compDTO.Id, compDTO.Resistance, compDTO.X, compDTO.Y);
                    }
                    else if (compDTO.Type == "Battery")
                    {
                        comp = new Battery(compDTO.Id, compDTO.Voltage, compDTO.X, compDTO.Y);
                    }
                    else if (compDTO.Type == "Lamp")
                    {
                        comp = new Lamp(compDTO.Id, compDTO.Power, compDTO.X, compDTO.Y);
                    }
                    else
                    {
                        throw new InvalidOperationException($"Unsupported component type: {compDTO.Type}");
                    }

                    if (comp != null)
                    {
                        comp.CircuitId = circuitDTO.CircuitId;
                        circuit.AddComponent(comp);
                    }
                }

                foreach (var wireDTO in circuitDTO.Wires)
                {
                    Guid start = wireDTO.StartId;
                    Guid end = wireDTO.EndId;

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
            await using var transacation = await database.Database.BeginTransactionAsync();

            try
            {
                Circuit payload = ConvertFromDTO(circuitDto);

                ValidateCircuit(payload);

                var circuit = await database.Circuits.Include(c => c.Components) // checks if the circuit is already stored in the database (failsafe)
                                                .Include(c => c.Wires)
                                                .FirstOrDefaultAsync(c => c.CircuitId == payload.CircuitId);

                if (circuit == null)
                {
                    throw new DbUpdateException();
                }

                ValidateCircuit(circuit);

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
                        circuit.Components.Add(NewComp);
                    }
                    // Editing Saved Components //
                    else
                    {
                        if (ExistingComp.GetType() != component.GetType())
                        {
                            return BadRequest("Changing component type requires deleting and recreating the component");
                        }
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
                }

                // Deleting Removed Components //

                var PayloadCompIDs = new List<Guid>();

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
                        circuit.Wires.Add(wire);
                    }
                    else
                    {
                        ExistingWire.StartId = wire.StartId;
                        ExistingWire.EndId = wire.EndId;
                    }
                }

                var PayloadWireIDs = new List<Guid>();

                foreach (var wire in payload.Wires)
                {
                    PayloadWireIDs.Add(wire.WireId);
                }

                var WiresToRemove = circuit.Wires.Where(w => !PayloadWireIDs.Contains(w.WireId));

                database.RemoveRange(WiresToRemove);

                await database.SaveChangesAsync();
                await transacation.CommitAsync();

                return Ok("Circuit saved successfully");
            }
            catch (DbUpdateException)
            {
                await transacation.RollbackAsync();
                return StatusCode(500, "Failed to save");
            }
            catch (Exception)
            {
                await transacation.RollbackAsync();
                return StatusCode(500, "Unexpected error");
            }
        }
        

        [HttpPost("create")]
        public async Task<IActionResult> CreateCircuit([FromBody] CircuitDTO circuitDto) // Method for creating a circuit and storing it initially
        {
            await using var transacation = await database.Database.BeginTransactionAsync();

            try
            {
                Circuit circuit = ConvertFromDTO(circuitDto);

                var ExistingCircuit = await database.Circuits.AnyAsync(c => c.Name == circuit.Name);

                if (ExistingCircuit)
                {
                    return BadRequest($"Circuit name {circuit.Name} already exists");
                }

                if (await database.Circuits.ContainsAsync(circuit))
                {
                    await transacation.DisposeAsync();
                    return BadRequest("Circuit already exists with this id/name");
                }
                else
                {
                    await database.Circuits.AddAsync(circuit);
                    
                    await database.SaveChangesAsync();
                    await transacation.CommitAsync();

                    var id = circuit.CircuitId;

                    return Ok(id);
                }
            }
            catch (Exception)
            {
                await transacation.RollbackAsync();
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
                var circuit = await database.Circuits.FirstOrDefaultAsync(c => c.CircuitId == id);

                if (circuit == null)
                {
                    return NotFound($"Circuit with ID {id} not found");
                }
                
                int CircuitId = circuit.CircuitId;
                string Name = circuit.Name;

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

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteCircuit([FromBody] int id)
        {
            await using var transacation = await database.Database.BeginTransactionAsync();

            try
            {
                var circuit = await database.Circuits.FirstOrDefaultAsync(c => c.CircuitId == id);
                
                if (circuit == null)
                {
                    return NotFound($"Circuit with ID {id} not found");
                }

                database.Remove(circuit);
                    
                await database.SaveChangesAsync();
                await transacation.CommitAsync();

                return Ok("Circuit has been deleted");
            }
            catch (DbException)
            {
                await transacation.RollbackAsync();

                return StatusCode(500, "Couldn't delete circuit");
            }
            catch (Exception)
            {
                await transacation.RollbackAsync();

                return BadRequest("Something went wrong...");
            }
        }

        public void ValidateCircuit(Circuit circuit)
        {
            if (circuit.Components.Count < 1)
            {
                throw new InvalidOperationException("Invalid Circuit: Add more components");
            }
            else if (circuit.Components.Count > 50)
            {
                throw new InvalidOperationException("Too many components: MAX 50");
            }
            else if (circuit.Wires.Count > 100)
            {
                throw new InvalidOperationException("Too many wires: MAX 100");
            }
            
            int batteryCount = 0;

            foreach (var component in circuit.Components)
            {
                if (component.ComponentType == "Battery")
                {
                    batteryCount++;
                }

            }

            foreach (var comp in circuit.Components)
            {
                if (comp is Resistor r)
                {
                    if (r.Resistance <= 0)
                    {
                        throw new InvalidOperationException("Error: invalid component values");
                    }
                }
                else if (comp is Battery b)
                {
                    if (b.Emf <= 0)
                    {
                        throw new InvalidOperationException("Error: invalid component values");
                    }
                }
                else if (comp is Lamp l)
                {
                    if (l.Power <= 0)
                    {
                        throw new InvalidOperationException("Error: invalid component values");
                    }
                }
            }

            if (batteryCount == 0)
            {
                throw new InvalidOperationException("Invalid Circuit: Add a battery");
            }
        }

    }
}

