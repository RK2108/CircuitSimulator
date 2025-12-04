<template>
    <div class="container">
        <svg class="canvas" @click="displayComponent">
            <line
				v-for="wire in circuit.wires"
				:key="wire.wireId"
				:x1="getComponent(wire.startId)?.x + 30"
				:y1="getComponent(wire.startId)?.y + 15"
				:x2="getComponent(wire.endId)?.x + 30"
				:y2="getComponent(wire.endId)?.y + 15"
				stroke="black"
				stroke-width="2"
				@click="deleteWire(wire.wireId)" />
            <g 
                v-for="comp in circuit.components"
                :key="comp.componentId"
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
                    {{ comp.componentType }} {{ comp.componentId }}
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
    const nextWireId = ref(0);
    const count = ref(0);

    function displayComponent(event){
        if (selectedTool.value !== null && selectedTool.value !== 'Wire'){
            if (selectedTool.value == 'Resistor'){
                circuit.components.push({
                    componentId: count.value,
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
                    componentId: count.value,
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
                    componentId: count.value,
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
            const index = circuit.components.findIndex((c) => c.componentId === id);
            circuit.components.splice(index, 1);
            circuit.wires = circuit.wires.filter((w) => w.startId !== id && w.endId !== id,);
        }
    }

    function deleteWire(id) {
        if (selectedTool.value == 'Delete') {
            const index = circuit.wires.findIndex((w) => w.wireId === id);
            circuit.wires.splice(index, 1);
        }
	}

    function handleClicks(comp){
        if (selectedTool.value == 'Delete'){
            deleteComponent(comp.componentId);
        }
        else if (selectedTool.value === 'Wire') {
			connectComponents(comp.componentId);
		}
    }

    
	function connectComponents(id) {
		const wireId = nextWireId.value++;
		if (!selectedComp.value) {
			selectedComp.value = id;
		} else {
			const startId = selectedComp.value;
			const endId = id;

			if (startId === endId) {
				selectedComp.value = null;
				return;
			}

			const duplicate = circuit.wires.some(
				(w) =>
					(w.startId === startId && w.endId === endId) ||
					(w.startId === endId && w.endId === startId),
			);

			if (!duplicate) {
				circuit.wires.push({ wireId, startId, endId });
			}

			selectedComp.value = null;
		}
	}

	function getComponent(id) {
		return circuit.components.find((c) => c.componentId === id);
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
		width: 1000px;
		height: 550px;
		border: 2px solid black;
		border-radius: 10px;
		background:
			linear-gradient(#5097dd 1px, transparent 1px),
			linear-gradient(90deg, #5097dd 1px, transparent 1px);
		background-size: 25px 25px;
		box-shadow: 10px 10px 10px 10px rgba(0, 0, 0, 0.05);
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