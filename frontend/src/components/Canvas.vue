<template>
    <div class="container">
        <svg class="canvas" @click.self="DisplayComponent">
            <line
				v-for="wire in circuit.wires"
				:key="wire.wireId"
				:x1="getComponent(wire.startId)?.x + 30"
				:y1="getComponent(wire.startId)?.y + 15"
				:x2="getComponent(wire.endId)?.x + 30"
				:y2="getComponent(wire.endId)?.y + 15"
				stroke="black"
				stroke-width="2"
				@click="deleteWire(wire.wireId)"
                class="wire" />
        </svg>

        <div
            v-for="comp in circuit.components"
            :key="comp.componentId"
            class="component-group"
            :style="{ left: comp.x + 'px', top: comp.y + 'px' }"
            @contextmenu.prevent="connectComponents(comp.componentId)"
            @click.left="deleteComponent(comp.componentId)"
            @click="emit('component', comp)">
            <v-card
                class="component"
                elevation="4"
                rounded="lg"
                :class="{ selected: selectedComp === comp.componentId }"
                @mousedown="e => StartDrag(e, comp)">
                
                <v-icon size="28">
                {{ componentIcon(comp.componentType) }}
                </v-icon>
            </v-card>
        </div>
    </div>

    <v-snackbar v-model="Alert" :color="AlertColor" :timeout="3000">
        {{ AlertText }}
        <template v-slot:actions>
            <v-btn variant="text" @click="Alert = false">Close</v-btn>
        </template>
    </v-snackbar>
</template>

<script setup>
    import { onBeforeUnmount, ref } from 'vue';
    import { v4 as uuidv4 } from 'uuid';

    const { circuit } = defineProps({
        circuit: {
            type: Object,
            required: true
        }
    });

    function componentIcon(type) {
        switch (type) {
            case 'Resistor':
                return 'mdi-resistor';
            case 'Battery':
                return 'mdi-battery';
            case 'Lamp':
                return 'mdi-lightbulb-on-outline';
            default:
                return 'mdi-help-circle-outline';
        }
    }

    /// Drag & Drop ///

    const DraggingId = ref(null);
    let offsetX = 0;
    let offsetY = 0;

    function StartDrag(event, component){
        DraggingId.value = component.componentId;
        
        offsetX = event.clientX - component.x;
        offsetY = event.clientY - component.y;

        window.addEventListener('mousemove', DuringDragging)
        window.addEventListener('mouseup', StopDragging)
    }

    function DuringDragging(event){
        if (!DraggingId.value){
            return;
        }

        const component = circuit.components.find(c => c.componentId === DraggingId.value);

        component.x = event.clientX - offsetX;
        component.y = event.clientY - offsetY;
    }

    function StopDragging(){
        DraggingId.value = null;

        window.removeEventListener('mousemove', DuringDragging)
        window.removeEventListener('mouseup', StopDragging)
    }

    onBeforeUnmount(() => {
        if (DraggingId == null){
                window.removeEventListener('mousemove', DuringDragging)
                window.removeEventListener('mouseup', StopDragging)
        }
    })

    /// Alerts ///
    const Alert = ref(false);
    const AlertText = ref('');
    const AlertColor = ref('success');

    function ShowMessage(text, color = 'success'){
        AlertText.value = text;
        AlertColor.value = color;
        Alert.value = true;
    }

    const selectedTool = ref(null);
    const selectedComp = ref(null);
    

    const emit = defineEmits(['component']);

    function DisplayComponent(event){
        if (selectedTool.value !== null){
            if (selectedTool.value == 'Resistor'){
                circuit.components.push({
                    componentId: uuidv4(),
                    componentType: selectedTool.value,
                    resistance: 0,
                    voltage: 0,
                    power: 0,
                    x: event.offsetX - 30,
                    y: event.offsetY - 20,
                });
            }
            else if (selectedTool.value == 'Battery'){
                circuit.components.push({
                    componentId: uuidv4(),
                    componentType: selectedTool.value,
                    resistance: 0,
                    voltage: 0,
                    power: 0,
                    x: event.offsetX - 30,
                    y: event.offsetY - 20,
                });
            }
            else if (selectedTool.value == 'Lamp'){
                circuit.components.push({
                    componentId: uuidv4(),
                    componentType: selectedTool.value,
                    resistance: 0,
                    voltage: 0,
                    power: 0,
                    x: event.offsetX - 30,
                    y: event.offsetY - 20,
                });
            }
        }
    }

    function deleteComponent(id){
        if (selectedTool.value == 'Delete'){
            const index = circuit.components.findIndex((c) => c.componentId === id);
            if (index !== -1){
                circuit.components.splice(index, 1);
                circuit.wires = circuit.wires.filter((w) => w.startId !== id && w.endId !== id);
            }
        }
    }

    function deleteWire(id) {
        if (selectedTool.value == 'Delete') {
            const index = circuit.wires.findIndex((w) => w.wireId === id);
            circuit.wires.splice(index, 1);
        }
	}

	function connectComponents(id) {
		if (!selectedComp.value) {
			selectedComp.value = id;
		} 
        else {
            const wireId = uuidv4();
			const startId = selectedComp.value;
			const endId = id;

			if (startId === endId) {
				selectedComp.value = null;
                ShowMessage("Cannot loop components", "error");
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
            else{
                ShowMessage("Cannot connect components more than once", "error");
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
        padding: 20px;
		justify-content: center;
		align-items: center;
		background: #ffffff;
		position: relative;
	}

    .canvas {
        position: relative;
        width: 1000px;
        height: 550px;
        border-radius: 12px;
        background:
            linear-gradient(#5097dd 1px, transparent 1px),
            linear-gradient(90deg, #5097dd 1px, transparent 1px);
        background-size: 25px 25px;
    }

    .canvas-svg {
        position: absolute;
        inset: 0;
        pointer-events: none;
    }

    .wire {
        stroke: #1f2937;
        stroke-width: 3;
        pointer-events: stroke;
    }

    .component-group {
        position: absolute;
        width: 80px;
        height: 60px;
    }

    .component {
        width: 100%;
        height: 100%;
        display: flex;
        align-items: center;
        justify-content: center;
        cursor: pointer;
    }

    .component:hover {
        transform: translateY(-2px);
    }

    .selected {
        outline: 2px solid #ef4444;
    }
</style>