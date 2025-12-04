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

onMounted(async () => {
    try {
        const response = await fetch('http://localhost:5107/api/circuit/GetAllCircuits');
        result.value = await response.json();
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

    circuit.circuitId = data[0].circuitId
    circuit.components = data[0].components
    circuit.name = data[0].name
    circuit.wires = data[0].wires 

    router.push('/builder');
}

</script>

<style scoped></style>