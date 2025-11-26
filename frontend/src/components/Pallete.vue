<template>
    <div class="pallete">
        <h3>Tools</h3>
        <div class="options">
            <label>Battery</label>
            <input type="radio" value="Battery" v-model="selected"></input>
            <label>Resistor</label>
            <input type="radio" value="Resistor" v-model="selected"></input>
            <label>Lamp</label>
            <input type="radio" value="Lamp" v-model="selected"></input>
            <label>Wire</label>
            <input type="radio" value="Wire" v-model="selected"></input>
            <label>Delete</label>
            <input type="radio" value="Delete" v-model="selected"></input>
        </div>
    </div>
</template>

<script setup>
    import { ref, watch } from 'vue';

    const selected = ref(null);

    watch(selected, (newVal) => {
        if (newVal){
            emit('select', newVal);
        }
    })

    const emit = defineEmits(['select']);
</script>

<style scoped>

</style>