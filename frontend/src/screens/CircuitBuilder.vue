<template>
    <nav>
        <button @click="router.push('/')">Back</button>
        <button @click="SimulateCircuit">Simulate</button>
        <button @click="SaveCircuit">Save</button>
        <button @click="help = true">Help</button>
    </nav>
    <div class="container">
        <div class="builder">
            <Pallete @select="tool = $event" :circuit="circuit"/>
            <Canvas @component="comp = $event" :circuit="circuit" ref="canvas"></Canvas>
            <DataPanel :circuit="circuit" ref="data"/>
        </div>
        <HelpModal v-if="help" @close="help = false"></HelpModal>
    </div>
</template>

<script setup>
    import Pallete from '@/components/Pallete.vue';
    import Canvas from '@/components/Canvas.vue';
    import DataPanel from '@/components/DataPanel.vue';
    import HelpModal from '@/components/HelpModal.vue';
    import { onBeforeMount, ref, watch } from 'vue';
    import { useRoute, useRouter } from 'vue-router';

    const tool = ref(null);
    const comp = ref(null);
    const canvas = ref(null);
    const data = ref(null);
    const result = ref(null);

    const help = ref(false);

    const router = useRouter();
    const route = useRoute();

    let id = Number(route.params.id)

    const circuit = ref({
        circuitId: 0,
        name: "",
        components: [],
        wires: []
    });

    onBeforeMount(async () => {
        if (id){
            const response = await fetch('http://localhost:5107/api/circuit/load', 
                {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(id),
                },
            );

            const data = await response.json();

            circuit.value.circuitId = data.circuitId
            circuit.value.components = data.components
            circuit.value.name = data.name
            circuit.value.wires = data.wires
        }
    });

    watch (tool, (newVal) => {
        if (canvas.value){
            canvas.value.selectedTool = newVal;
        }
    });

    watch (comp, (newVal) => {
        if (data.value){
            data.value.comp = newVal;
        }
    });

    watch (result, (newVal) => {
        if (data.value){
            data.value.result = newVal;
        }
    })

    async function SimulateCircuit(){
        
        const FormattedComponents = circuit.value.components?.map((c) => {

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

        const FormattedWires = circuit.value.wires?.map((w) => ({
            
            WireId: w.wireId,
            StartId: w.startId,
            EndId: w.endId,

        })) ?? [];

        const FormattedCircuit = {
            CircuitId: circuit.value.circuitId,
            name: circuit.value.name ?? 'Untitled Circuit',
            components: FormattedComponents,
            wires: FormattedWires,
        };

        try {
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

        const FormattedComponents = circuit.value.components?.map((c) => {

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

        const FormattedWires = circuit.value.wires?.map((w) => ({
            
            WireId: w.wireId,
            StartId: w.startId,
            EndId: w.endId,

        })) ?? [];

        const FormattedCircuit = {
            CircuitId: circuit.value.circuitId,
            name: circuit.value.name ?? 'Untitled Circuit',
            components: FormattedComponents,
            wires: FormattedWires,
        };

        try {

            if (id){
                FormattedCircuit.CircuitId = id;
                const response = await fetch(
                'http://localhost:5107/api/circuit/save', 
                {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(FormattedCircuit),
                });

                alert(await response.text());

                if (!response.ok){
                    const errorMessage = await response.text();
                    alert(errorMessage);
                    return;
                }
            }
            else {

                let name = window.prompt("Enter Name");
                FormattedCircuit.name = name;
                const response = await fetch('http://localhost:5107/api/circuit/create', 
                {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(FormattedCircuit),
                });

                const NewId = await response.json();


                id = Number(NewId);
                router.replace(`/builder/${NewId}`);

                
            }
        }
        catch (err){
            alert(err);
        }
    }
</script>

<style scoped>
    .builder {
        color: #111827;
        font-family: 'Inter', sans-serif;
        display: flex;
        gap: 0;
    }

    button {
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

    button:hover {
        background-color: #2563eb;
        box-shadow: 0 3px 8px rgba(59, 130, 246, 0.3);
        transform: translateY(-1px);
    }
</style>