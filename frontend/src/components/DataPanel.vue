<template>
    <div class="data">
        <h3>Circuit Data</h3>
        <button class="solve-btn" @click="simulateCircuit">Simulate</button>
        <button class="solve-btn">Save</button>
        <p>Circuit Id: {{ circuit.circuitId }}</p>
        <div v-if="result" class="result-box">
            <p>Resistance: {{ result.resistance }}</p>
            <p>Voltage: {{ result.voltage }}</p>
            <p>Current: {{ result.current }}</p>
        </div>
    </div>
</template>

<script setup>
    import { toRaw, ref } from 'vue';
    import { circuit } from '@/circuit';

    const result = ref(null);

    async function simulateCircuit(){
        try {
            const RawCircuit = toRaw(circuit);
            
            const FormattedComponents = RawCircuit.components?.map((c) => {

                const component = {
                    id: c.id, 
                    type: c.componentType,
                    resistance: c.resistance,
                    voltage: c.voltage,
                    power: c.power, 
                    x: c.x, 
                    y: c.y 
                };
                
                return component;

            }) ?? [];

            const FormattedWires = RawCircuit.wires?.map((w) => ({

                StartId: w.startId,
                EndId: w.endId,

            })) ?? [];

            const name = window.prompt("Enter name");

            const FormattedCircuit = {
                CircuitId: circuit.circuitId,
                name: name ?? 'Untitled Circuit',
                components: FormattedComponents,
                wires: FormattedWires,
            };

            const response = await fetch(
                'http://localhost:5107/api/circuit/simulate', 
                {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(FormattedCircuit),
                },
            );

            result.value = await response.json();

            if (!response.ok){
                const errorMessage = await response.text();
                alert(errorMessage);
                return;
            }
        }
        catch (err){
            alert(err);
        }
    }
</script>

<style scoped>
.data{
  width: 260px;
  background: #f3f4f6;
  border-left: 1px solid #e5e7eb;
  padding: 1rem;
  overflow-y: auto;
  box-shadow: inset 2px 0 4px rgba(0, 0, 0, 0.03);
}

.data h3 {
  font-weight: 600;
  color: #374151;
  margin-bottom: 1rem;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  font-size: 0.9rem;
}

.solve-btn {
  display: inline-block;
  background-color: #3b82f6;
  color: white;
  padding: 10px 14px;
  border-radius: 6px;
  border: none;
  cursor: pointer;
  font-weight: 600;
  margin-bottom: 1rem;
  transition: all 0.2s ease;
  box-shadow: 0 2px 4px rgba(59, 130, 246, 0.2);
}

.solve-btn:hover {
  background-color: #2563eb;
  box-shadow: 0 3px 8px rgba(59, 130, 246, 0.3);
  transform: translateY(-1px);
}

.result-box {
  background: white;
  padding: 10px 12px;
  border-radius: 8px;
  border: 1px solid #d1d5db;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.05);
}

.result-box p {
  margin: 6px 0;
  font-size: 13px;
  color: #1f2937;
}

.result-box strong {
  color: #111827;
}
</style>