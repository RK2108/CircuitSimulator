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
        circuit.components = [];
        circuit.wires = [];
        router.push("/builder");
    }
</script>

<style scoped>
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