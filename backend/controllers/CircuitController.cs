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

        double resistance = circuit.CalculateResistance();

        if (resistance is double.PositiveInfinity || resistance is double.NegativeInfinity || resistance is double.NaN)
        {
            resistance = 0;
        }

        double voltage = circuit.GetVoltage();

        if (voltage is double.PositiveInfinity || voltage is double.NegativeInfinity || voltage is double.NaN)
        {
            voltage = 0;
        }

        double current = circuit.GetCurrent(voltage, resistance);

        if (current is double.PositiveInfinity || current is double.NegativeInfinity || current is double.NaN)
        {
            current = 0;
        }

        var simulation = new
        {
            name = circuit.Name,
            resistance,
            voltage,
            current
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