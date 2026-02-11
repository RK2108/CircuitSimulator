<template>
    <div class="container">
        <svg class="canvas" @click.self="DisplayComponent">
            <line
				v-for="wire in circuit.wires"
				:key="wire.wireId"
				:x1="GetComponent(wire.startId)?.x + 30"
				:y1="GetComponent(wire.startId)?.y + 15"
				:x2="GetComponent(wire.endId)?.x + 30"
				:y2="GetComponent(wire.endId)?.y + 15"
				stroke="black"
				stroke-width="2"
				@click="DeleteWire(wire.wireId)"
                class="wire" />
        </svg>

        <div
            v-for="comp in circuit.components"
            :key="comp.componentId"
            class="component-group"
            :style="{ left: comp.x + 'px', top: comp.y + 'px' }"
            @contextmenu.prevent="ConnectComponents(comp.componentId)"
            @click.left="DeleteComponent(comp.componentId)"
            @click="HighlightEmitted(comp)">
            <v-card
                class="component"
                elevation="4"
                rounded="lg"
                :class="{ selected: selectedComp === comp.componentId , highlighted: HighlightedComp === comp.componentId }"
                @mousedown="e => StartDrag(e, comp)">
                
                <v-icon size="28">
                {{ ComponentIcon(comp.componentType) }}
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

    function ComponentIcon(type) {
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

    const SelectedTool = ref(null);
    const SelectedComp = ref(null);
    
    const emit = defineEmits(['component']);

    const PlaceableTypes = ['Resistor', 'Battery', 'Lamp'];

    function DisplayComponent(event){
        
        for (var type of PlaceableTypes){
            if (type == SelectedTool.value){
                circuit.components.push({
                    componentId: uuidv4(),
                    componentType: SelectedTool.value,
                    resistance: 0,
                    voltage: 0,
                    power: 0,
                    x: event.offsetX - 30,
                    y: event.offsetY - 20,
                });
            }
        }
    }

    function DeleteComponent(id){
        if (SelectedTool.value == 'Delete'){
            const index = circuit.components.findIndex((c) => c.componentId === id);
            if (index !== -1){
                circuit.components.splice(index, 1);
                circuit.wires = circuit.wires.filter((w) => w.startId !== id && w.endId !== id);
            }
        }
    }

    function DeleteWire(id) {
        if (SelectedTool.value == 'Delete') {
            const index = circuit.wires.findIndex((w) => w.wireId === id);
            if (index !== -1){
                circuit.wires.splice(index, 1);
            }
        }
	}

	function ConnectComponents(id) {
		if (!SelectedComp.value) {
			SelectedComp.value = id;
		} 
        else {
            const wireId = uuidv4();
			const startId = SelectedComp.value;
			const endId = id;

			if (startId === endId) {
				SelectedComp.value = null;
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

			SelectedComp.value = null;
		}
	}

	function GetComponent(id) {
		return circuit.components.find((c) => c.componentId === id);
	}

    // Highlights for selected components

    const HighlightedComp = ref(null);

    function HighlightEmitted(comp){
        emit("component", comp)
        HighlightedComp.value = comp.componentId;
    }

    defineExpose({ SelectedTool });
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
        outline: 3px solid #ef4444;
    }

    .highlighted {
        outline: 3px solid #179650
    }
</style>