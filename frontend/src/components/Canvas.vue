<template>
    <div class="container">
        <svg class="canvas" @click="displayComponent">
            <g 
            v-for="comp in circuit.components"
            :key="comp.id"
            @click="handleClicks(comp)">
                <rect 
                    class="component"
                    :x="comp.x"
                    :y="comp.y"
                    width="60"
                    height="30"
                    fill="lightgray"
                    stroke="black"/>
                <text :x="comp.x + 30" :y="comp.y + 20" text-anchor="middle" class="text">
                    {{ comp.componentType }} {{ comp.id }}
                </text>
            </g>
        </svg>
    </div>
</template>

<script setup>
    import { ref } from 'vue';
    import { circuit } from '@/circuit';

    const selectedTool = ref(null);
    const selectedComp = ref(null);
    const count = ref(0);

    function displayComponent(event){
        if (selectedTool.value !== null && selectedTool.value !== 'Wire'){
            if (selectedTool.value == 'Resistor'){
                circuit.components.push({
                    id: count.value,
                    componentType: selectedTool.value,
                    resistance: 10,
                    voltage: 0,
                    power: 0,
                    x: event.offsetX - 30,
                    y: event.offsetY - 20,
                });
            }
            else if (selectedTool.value == 'Battery'){
                circuit.components.push({
                    id: count.value,
                    componentType: selectedTool.value,
                    resistance: 0,
                    voltage: 9,
                    power: 0,
                    x: event.offsetX - 30,
                    y: event.offsetY - 20,
                });
            }
            else if (selectedTool.value == 'Lamp'){
                circuit.components.push({
                    id: count.value,
                    componentType: selectedTool.value,
                    resistance: 0,
                    voltage: 0,
                    power: 12,
                    x: event.offsetX - 30,
                    y: event.offsetY - 20,
                });
            }
        }
    }

    function deleteComponent(id){
        if (selectedTool.value == 'Delete'){
            const index = circuit.components.findIndex((c) => c.id === id);
            circuit.components.splice(index, 1);
            circuit.wires = circuit.wires.filter((w) => w.startId !== id && w.endId !== id,);
        }
    }

    function handleClicks(comp){
        if (selectedTool.value == 'Delete'){
            deleteComponent(comp.id);
        }
    }

    defineExpose({ selectedTool });
</script>

<style scoped>
</style>