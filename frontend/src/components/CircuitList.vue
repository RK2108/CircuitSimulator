<template>
    <div v-for="res in result" :key="res.circuitId" @click="LoadCircuit(res.circuitId)">
        <p>Circuit Id: {{ res.circuitId }}</p>
        <p>Name: {{ res.name }}</p>
    </div>
</template>

<script setup>
import { onMounted, ref } from 'vue';
import { useRouter } from 'vue-router';
import { circuit } from '@/circuit';

const router = useRouter();

const result = ref(null);

const emit = defineEmits(['nextid']);

onMounted(async () => {
    try {
        const response = await fetch('http://localhost:5107/api/circuit/GetAllCircuits');
        result.value = await response.json();
        
        let next = 1;
        for (var res in result.value){
            next++;
        }

        emit("nextid", next);
    }
    catch(err){
        alert(err);
    }
});

async function LoadCircuit(id){
    const response = await fetch(
              'http://localhost:5107/api/circuit/load', 
              {
                  method: 'POST',
                  headers: { 'Content-Type': 'application/json' },
                  body: JSON.stringify(id),
              },
          );

    const data = await response.json();

    circuit.circuitId = data.circuitId
    circuit.components = data.components
    circuit.name = data.name
    circuit.wires = data.wires 

    router.push('/builder');
}

</script>

<style scoped>
    div {
        border: 1px solid black;
        border-radius: 10px;
        padding: 6px;
    }

</style>