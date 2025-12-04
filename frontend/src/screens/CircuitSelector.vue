<template>
    <button @click="NewCircuit">New Circuit</button>
    <CircuitList/>
</template>

<script setup>
    import CircuitList from '@/components/CircuitList.vue';
    import { circuit } from '@/circuit';
    import { useRouter } from 'vue-router';

    const router = useRouter();

    async function NewCircuit(){
        const Id = window.prompt("Enter Circuit Id");
        const Name = window.prompt("Enter Circuit Name");

        const CircuitInfo = {
            circuitId: Id,
            name: Name,
            components: [],
            wires: []
        }
        
        try {
            await fetch(
                'http://localhost:5107/api/circuit/create', 
                {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(CircuitInfo),
                },
            );
        }
        catch(err){
            alert(err);
        }
        
        circuit.circuitId = Id;
        circuit.name = Name;
        router.push("/builder");
    }
</script>

<style scoped></style>