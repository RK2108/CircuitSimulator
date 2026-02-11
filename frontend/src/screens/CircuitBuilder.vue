<template>
    <v-app>
        <v-app-bar color="primary" dark>
            <v-btn icon @click="router.push('/')">
                <v-icon>mdi-arrow-left</v-icon>
            </v-btn>

            <v-toolbar-title>CircuitBuilder</v-toolbar-title>

            <v-spacer></v-spacer>

            <v-btn @click="SimulateCircuit" prepend-icon="mdi-play-circle">Simulate</v-btn>
            <v-btn @click="ConfirmSave" prepend-icon="mdi-content-save">Save</v-btn>
            <v-btn @click="help = true" prepend-icon="mdi-help">Help</v-btn>
        </v-app-bar>

        <v-main>
            <div class="builder">
                <Pallete @select="tool = $event" :circuit="circuit"/>
                <Canvas @component="comp = $event" :circuit="circuit" ref="canvas"></Canvas>
                <DataPanel :circuit="circuit" ref="data"/>
            </div>
            <HelpModal v-if="help" @close="help = false"></HelpModal>
        </v-main>

        <v-dialog v-model="NameDialog" max-width="400">
            <v-card>
                <v-card-title>Enter chosen circuit name</v-card-title>
                <v-card-text>
                    <v-text-field v-model="CircuitName" 
                                label="Circuit Name" 
                                autofocus 
                                @keyup.enter="SaveCircuit">
                    </v-text-field>
                </v-card-text>
                <v-card-actions>
                    <v-spacer></v-spacer>
                    <v-btn @click="SaveCircuit">Confirm</v-btn>
                </v-card-actions>
            </v-card>
        </v-dialog>

        <v-snackbar v-model="Alert" :color="AlertColor" :timeout="3000">
            {{ AlertText }}
            <template v-slot:actions>
                <v-btn variant="text" @click="Alert = false">Close</v-btn>
            </template>
        </v-snackbar>
    </v-app>
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

    /// Alerts ///
    const Alert = ref(false);
    const AlertText = ref('');
    const AlertColor = ref('success');

    function ShowMessage(text, color = 'success'){
        AlertText.value = text;
        AlertColor.value = color;
        Alert.value = true;
    }

    /// Routing ///

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

            if (!response.ok){
                const message = await response.text();
                ShowMessage(message, 'error');
                return;
            };

            const data = await response.json();

            circuit.value.circuitId = data.circuitId;
            circuit.value.components = data.components;
            circuit.value.name = data.name;
            circuit.value.wires = data.wires;
        }
    });

    watch (tool, (newVal) => {
        if (canvas.value){
            canvas.value.SelectedTool = newVal;
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

    function FormatComponents(){
        const FormattedComponents = [];

        for(var comp of circuit.value.components){
    
            const Component = {
                Id: comp.componentId,
                Type: comp.componentType,
                Resistance: comp.resistance,
                Voltage: comp.voltage,
                Power: comp.power,
                X: comp.x,
                Y: comp.y
            };

            FormattedComponents.push(Component);
        }

        return FormattedComponents;
    }

    function FormatWires(){
        const FormattedWires = [];
          
        for(var wire of circuit.value.wires){

            const Wire = {
                WireId: wire.wireId,
                StartId: wire.startId,
                EndId: wire.endId,
            }

            FormattedWires.push(Wire);
        }

        return FormattedWires;
    }

    function FormatCircuit(){
        
        const FormattedComponents = FormatComponents();
        const FormattedWires = FormatWires();
        
        const FormattedCircuit = {
            CircuitId: circuit.value.circuitId,
            name: circuit.value.name ?? 'Untitled Circuit',
            components: FormattedComponents,
            wires: FormattedWires,
        };

        return FormattedCircuit;
    }

    /// SIMULATING CIRCUITS ///

    async function SimulateCircuit(){

        const Circuit = FormatCircuit();

        try {
            const response = await fetch(
                'http://localhost:5107/api/circuit/simulate', 
                {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(Circuit),
                },
            );

            if (!response.ok){
                const message = await response.text();
                throw new Error(message)
            }

            result.value = await response.json();

            ShowMessage("Simulated successfully", "success");
        }
        catch (err){
            ShowMessage(err.message, "error");
        }
    }

    /// SAVING CIRCUITS ///

    const CircuitName = ref(null);
    const NameDialog = ref(false);

    function ConfirmSave(){
        if (!id){
            NameDialog.value = true;
        }
        else {
            SaveCircuit();
        }
    }

    async function SaveCircuit(){
        
        const Circuit = FormatCircuit();

        try {

            if (id){
                Circuit.CircuitId = id;
                const response = await fetch(
                'http://localhost:5107/api/circuit/save', 
                {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(Circuit),
                });

                const message = await response.text();

                if (!response.ok){
                    throw new Error(message);
                }
                
                ShowMessage(message, "success");
            }
            else {

                Circuit.name = CircuitName.value;
                NameDialog.value = false
                const response = await fetch('http://localhost:5107/api/circuit/create', 
                {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(Circuit),
                });

                if (!response.ok){
                    const message = await response.text();
                    throw new Error(message);
                }

                const NewId = await response.json();


                id = Number(NewId);
                router.replace(`/builder/${NewId}`);

                ShowMessage("Created successfully", "success");
                
            }
        }
        catch (err){
            ShowMessage(err, "error");
        }
    }
</script>

<style scoped>
.builder {
    color: #111827;
    font-family: 'Inter', sans-serif;
    display: flex;
    gap: 0;
    height: calc(100vh - 64px);
}
</style>