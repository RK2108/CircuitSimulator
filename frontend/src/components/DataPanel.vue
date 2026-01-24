<template>
    <div class="data">
        <h3>Circuit Data</h3>
        <button class="solve-btn" @click="SimulateCircuit">Simulate</button>
        <button class="solve-btn" @click="SaveCircuit">Save</button>
        <h3>Input Values</h3>
        <div v-if="comp" class="result-box">
            <p>ComponentId: {{ comp.componentId }}</p>
            <label v-if="comp.componentType === 'Battery'">Voltage: <input required type="number" v-model="voltageval"></label>
            <label v-else-if="comp.componentType === 'Resistor'">Resistance: <input required type="number" v-model="resval"></label>
            <label v-else-if="comp.componentType === 'Lamp'">Power: <input required type="number" v-model="powerval"></label>
        </div>
        <h3 v-if="result">Output Values</h3>
        <div v-if="result" class="result-box">
            <p>Component Id: {{ comp.componentId }}</p>
            <p>Voltage: {{ result.solvedComponents[comp.componentId - 1].voltage }}</p>
            <p>Resistance: {{ result.solvedComponents[comp.componentId - 1].resistance }}</p>
            <p>Current: {{ result.solvedComponents[comp.componentId - 1].current}}</p>
        </div>
    </div>
</template>

<script setup>
    import { toRaw, ref } from 'vue';
    import { watch } from 'vue';
    import { circuit } from '@/circuit';

    const result = ref(null);
    const comp = ref(null);

    const resval = ref(0);
    const powerval = ref(0);
    const voltageval = ref(0);

    watch(resval, (newval) => {
        const index = circuit.components.findIndex(c => c.componentId === comp.value.componentId)
        console.log(index);

        circuit.components[index].resistance = newval;
    });

    watch(powerval, (newval) =>{
        const index = circuit.components.findIndex(c => c.componentId == comp.value.componentId)
        console.log(index);
        circuit.components[index].power = newval;
    })

    watch(voltageval, (newval) =>{
        const index = circuit.components.findIndex(c => c.componentId == comp.value.componentId)
        console.log(index);
        circuit.components[index].voltage = newval;
    })

    async function SimulateCircuit(){
      try {
          const RawCircuit = toRaw(circuit);
          
          const FormattedComponents = RawCircuit.components?.map((c) => {

              const component = {
                  id: c.componentId, 
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
            WireId: w.wireId,
            StartId: w.startId,
            EndId: w.endId,

          })) ?? [];

          const FormattedCircuit = {
              CircuitId: circuit.circuitId,
              name: circuit.name ?? 'Untitled Circuit',
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

    async function SaveCircuit(){
      try {
          const RawCircuit = toRaw(circuit);
          
          const FormattedComponents = RawCircuit.components?.map((c) => {

              const component = {
                  id: c.componentId, 
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
            WireId: w.wireId,
            StartId: w.startId,
            EndId: w.endId,

          })) ?? [];

          const FormattedCircuit = {
              CircuitId: circuit.circuitId,
              name: circuit.name ?? 'Untitled Circuit',
              components: FormattedComponents,
              wires: FormattedWires,
          };

          const response = await fetch(
              'http://localhost:5107/api/circuit/save', 
              {
                  method: 'POST',
                  headers: { 'Content-Type': 'application/json' },
                  body: JSON.stringify(FormattedCircuit),
              },
          );

          alert(await response.text());

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

    defineExpose({comp});
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