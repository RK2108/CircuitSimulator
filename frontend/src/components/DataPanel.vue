<template>
    <div class="data">
        <h3>Circuit Data</h3>
        <button class="solve-btn" @click="simulateCircuit">Simulate</button>
        <button class="solve-btn">Save</button>
        <div v-if="result" class="result-box">
            <p>{{ result }}</p>
        </div>
    </div>
</template>

<script setup>
    // import { toRaw, ref } from 'vue';
    // import { circuit } from '@/circuit';

    // const result = ref(null);

    // async function simulateCircuit(){
    //     result.value = null;
    //     try {
    //         const RawCircuit = toRaw(circuit);
            
    //         const FormattedComponents = RawCircuit.components?.map((c) => {
    //             const base = {id: c.id, type: c.componentType, x: c.x, y: c.y };
    //             if (c.type === 'Resistor'){
    //                 base.resistance = c.resistance;
    //             }
    //             else if (c.type === 'Battery'){
    //                 base.voltage = c.voltage;
    //             }
    //             else if (c.type === 'Lamp'){
    //                 base.power = c.power;
    //             }
                
    //             return base;
    //         }) ?? [];

    //         const FormattedWires = RawCircuit.wires?.map((w) => ({
    //             StartId: w.startId,
    //             EndId: w.endId,
    //         })) ?? [];

    //         const FormattedCircuit = {
    //             CircuitId: RawCircuit.CircuitId,
    //             name: RawCircuit.name ?? 'Untitled Circuit',
    //             components: FormattedComponents,
    //             wires: FormattedWires,
    //         };

    //         const response = await fetch(
    //             'http://localhost:5014/api/circuit/simulate', 
    //             {
    //                 method: 'POST',
    //                 headers: { 'Content-Type': 'application/json' },
    //                 body: JSON.stringify(FormattedCircuit),
    //             },
    //         );

    //         if (!response.ok){
    //             const errorMessage = await response.text();
    //             alert(errorMessage);
    //             return;
    //         }
    //     }
    //     catch (err){
    //         alert("Something went wrong");
    //     }
    // }
</script>

<style scoped></style>