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

            count.value++;
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
    .container {
		flex: 1;
		display: flex;
		justify-content: center;
		align-items: center;
		background: #ffffff;
		position: relative;
	}

	.canvas {
		width: 900px;
		height: 550px;
		border: 2px solid #d1d5db;
		border-radius: 10px;
		background:
			linear-gradient(#f9fafb 1px, transparent 1px),
			linear-gradient(90deg, #f9fafb 1px, transparent 1px);
		background-size: 25px 25px;
		box-shadow: 0 2px 10px rgba(0, 0, 0, 0.05);
		cursor: crosshair;
	}

	line {
		stroke: #1f2937;
		stroke-width: 3;
		transition: stroke 0.2s ease;
	}

	line:hover {
		stroke: #3b82f6;
		stroke-width: 3.5;
	}

	.component {
		stroke-width: 2;
		rx: 8;
		transition: all 0.2s ease;
	}

	.comp-text {
		font-size: 12px;
		font-weight: 600;
		fill: #1f2937;
		paint-order: stroke;
		stroke: white;
		stroke-width: 0.5px;
	}
</style>