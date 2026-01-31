<template>
    <div class="data">
        <h3>Circuit Data</h3>
        <h3>Input Values</h3>
        <div v-if="comp" class="result-box">
            <p>ComponentId: {{ comp.componentId }}</p>
            <label v-if="comp.componentType === 'Battery'">Voltage: <input :value="comp.voltage" @input="voltageval = $event.target.value" required type="number"></label>
            <label v-else-if="comp.componentType === 'Resistor'">Resistance: <input :value="comp.resistance" @input="resval = $event.target.value" required type="number"></label>
            <label v-else-if="comp.componentType === 'Lamp'">Power: <input :value="comp.power" @input="powerval = $event.target.value" required type="number"></label>
        </div>
        <h3 v-if="result">Output Values</h3>
        <div v-if="result" class="result-box">
            <p>Component Id: {{ result.solvedComponents[SolvedComp].componentId }}</p>
            <p>Voltage: {{ result.solvedComponents[SolvedComp].voltage }}</p>
            <p>Resistance: {{ result.solvedComponents[SolvedComp].resistance }}</p>
            <p>Current: {{ result.solvedComponents[SolvedComp].current}}</p>
        </div>
    </div>
</template>

<script setup>
    import { ref } from 'vue';
    import { computed, watch } from 'vue';

    const result = ref(null);
    const comp = ref(null);

    const SolvedComp = computed(() => {
        if (result.value?.solvedComponents && comp.value){
          return result.value.solvedComponents.findIndex(c => c.componentId == comp.value.componentId);
        }
    });

    const resval = ref(0);
    const powerval = ref(0);
    const voltageval = ref(0);

    const { circuit } = defineProps({
        circuit: {
            type: Object,
            required: true
        }
    });

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

    defineExpose({comp, result});
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