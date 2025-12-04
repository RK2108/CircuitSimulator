<template>
    <div v-for="circuit in result">
        <p>Circuit Id: {{ circuit.circuitId }}</p>
        <p>Name: {{ circuit.name }}</p>
    </div>
</template>

<script>
import { onMounted, ref } from 'vue';

const result = ref(null);

onMounted(async () => {
    const response = await fetch('http://localhost:5107/api/circuit/GetAllCircuits');
    result.value = await response.json();
});

</script>

<style scoped></style>