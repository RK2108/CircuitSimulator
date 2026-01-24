<template>
    <div v-for="res in result" :key="res.circuitId" @click="router.push(`/builder/${res.circuitId}`)">
        <p>Circuit Id: {{ res.circuitId }}</p>
        <p>Name: {{ res.name }}</p>
    </div>
</template>

<script setup>
import { onMounted, ref } from 'vue';
import { useRouter } from 'vue-router';

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

</script>

<style scoped>
    div {
        border: 1px solid black;
        border-radius: 10px;
        padding: 6px;
    }

</style>