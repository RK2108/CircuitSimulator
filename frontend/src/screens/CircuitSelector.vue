<template>
    <button @click="NewCircuit">New Circuit</button>
    <CircuitList/>
</template>

<script setup>
    import CircuitList from '@/components/CircuitList.vue';

    async function NewCircuit(){
        const Id = window.prompt("Enter Circuit Id");
        const Name = window.prompt("Enter Circuit Name");

        const CircuitInfo = {
            circuitId: Id,
            name: Name
        }
        
        try {
            const response = await fetch(
                'http://localhost:5107/api/circuit/create', 
                {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(CircuitInfo),
                },
            );

            alert(await response.json());
        }
        catch(err){
            alert(err);
        }
    }
</script>

<style scoped></style>