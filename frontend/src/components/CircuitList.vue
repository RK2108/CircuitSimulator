<template>
    <v-data-table :headers="headers" :items="result" :items-per-page="10" class="elevation-1">
        <template #item.actions="{ item }">
            <v-btn size="small" icon color="primary" @click="router.push(`/builder/${item.circuitId}`)">
                <v-icon>mdi-open-in-app</v-icon>
            </v-btn>
            <v-btn size="small" icon color="red" class="ml-2" @click="DeleteCircuit(item.circuitId)">
                <v-icon>mdi-delete</v-icon>
            </v-btn>
        </template>
    </v-data-table>
    <v-snackbar v-model="Alert" :color="AlertColor" :timeout="3000">
        {{ AlertText }}
        <template v-slot:actions>
            <v-btn variant="text" @click="Alert = false">Close</v-btn>
        </template>
    </v-snackbar>
</template>

<script setup>
    import { onMounted, ref } from 'vue';
    import { useRouter } from 'vue-router';

    const router = useRouter();

    const result = ref([]); 

    const headers = [
        { title: 'Circuit Id', key: 'circuitId'},
        { title: 'Name', key: 'name'},
        { title: 'Actions', key: 'actions', sortable: false}
    ];

    onMounted(() => GetAllCircuits());

    async function GetAllCircuits(){
        try {
            const response = await fetch('http://localhost:5107/api/circuit/GetAllCircuits');
            result.value = await response.json();
        }
        catch(err){
            ShowMessage(err, "error")
        }
    }

    /// Alerts ///
    const Alert = ref(false);
    const AlertText = ref('');
    const AlertColor = ref('success');

    function ShowMessage(text, color = 'success'){
        AlertText.value = text;
        AlertColor.value = color;
        Alert.value = true;
    }

    /// DELETING CIRCUITS ///

    async function DeleteCircuit(id){
        try {

            const response = await fetch('http://localhost:5107/api/circuit/delete', 
            {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(id),
            });

            const message = await response.text();

            GetAllCircuits();

            ShowMessage(message, "success");
        }
        catch (err){
            ShowMessage(err, "error");
        }
    }

</script>

<style scoped>
</style>