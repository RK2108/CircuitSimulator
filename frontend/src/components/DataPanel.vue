<template>
  <v-navigation-drawer location="right" permanent width="280">
    <v-list>
      <v-list-subheader>CIRCUIT DATA</v-list-subheader>
    </v-list>

    <v-container>
      <v-card v-if="comp" class="mb-4" elevation="2">
        <v-card-title class="text-subtitle-1">Input Values</v-card-title>
        <v-card-text>
          <v-chip class="mb-3" size="small">
            Component ID: {{ comp.componentId }}
          </v-chip>

          <v-text-field
            v-if="comp.componentType === 'Battery'"
            label="Voltage (V)"
            type="number"
            :model-value="comp.voltage"
            @update:model-value="voltageval = $event"
            variant="outlined"
            density="compact"
            prepend-icon="mdi-flash">
          </v-text-field>

          <v-text-field
            v-else-if="comp.componentType === 'Resistor'"
            label="Resistance (Ω)"
            type="number"
            :model-value="comp.resistance"
            @update:model-value="resval = $event"
            variant="outlined"
            density="compact"
            prepend-icon="mdi-resistor">
          </v-text-field>

          <v-text-field
            v-else-if="comp.componentType === 'Lamp'"
            label="Power (W)"
            type="number"
            :model-value="comp.power"
            @update:model-value="powerval = $event"
            variant="outlined"
            density="compact"
            prepend-icon="mdi-lightbulb">
          </v-text-field>
        </v-card-text>
      </v-card>

      <v-card v-if="result" elevation="2">
        <v-card-title class="text-subtitle-1">Output Values</v-card-title>
        <v-card-text>
          <v-chip class="mb-3" color="success" size="small">
            Component ID: {{ comp.componentId }}
          </v-chip>

          <v-list density="compact">
            <v-list-item>
              <template v-slot:prepend>
                <v-icon>mdi-flash</v-icon>
              </template>
              <v-list-item-title>Voltage</v-list-item-title>
              <v-list-item-subtitle>
                {{ result.solvedComponents[SolvedComp].voltage }} V
              </v-list-item-subtitle>
            </v-list-item>

            <v-list-item>
              <template v-slot:prepend>
                <v-icon>mdi-resistor</v-icon>
              </template>
              <v-list-item-title>Resistance</v-list-item-title>
              <v-list-item-subtitle>
                {{ result.solvedComponents[SolvedComp].resistance }} Ω
              </v-list-item-subtitle>
            </v-list-item>

            <v-list-item>
              <template v-slot:prepend>
                <v-icon>mdi-current-ac</v-icon>
              </template>
              <v-list-item-title>Current</v-list-item-title>
              <v-list-item-subtitle>
                {{ result.solvedComponents[SolvedComp].current }} A
              </v-list-item-subtitle>
            </v-list-item>
          </v-list>
        </v-card-text>
      </v-card>
    </v-container>
  </v-navigation-drawer>
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
        circuit.components[index].resistance = newval;
    });

    watch(powerval, (newval) =>{
        const index = circuit.components.findIndex(c => c.componentId == comp.value.componentId)
        circuit.components[index].power = newval;
    })

    watch(voltageval, (newval) =>{
        const index = circuit.components.findIndex(c => c.componentId == comp.value.componentId)
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