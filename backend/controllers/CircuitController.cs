using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class CircuitController : ControllerBase
{
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

        var simulation = new
        {
            name = circuit.Name,
            resistance = circuit.CalculateResistance(),
            voltage = circuit.GetVoltage(),
            current = circuit.GetCurrent()
        };


        return Ok(simulation);
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

            bool startExists = circuit.Components.Any(c => c.Id == start);
            bool endExists = circuit.Components.Any(c => c.Id == end);

            if (startExists && endExists)
            {
                
                Wire w = new Wire(wireDTO.StartId, wireDTO.EndId);
                circuit.AddWire(w);
            }
        }

        return circuit;
    }
}