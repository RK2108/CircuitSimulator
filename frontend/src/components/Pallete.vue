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
    .palette {
    width: 180px;
    background: #f9fafb;
    border-right: 1px solid #e5e7eb;
    padding: 1rem;
    box-shadow: inset -2px 0 4px rgba(0, 0, 0, 0.03);
    }

    .palette h3 {
    font-weight: 600;
    color: #374151;
    margin-bottom: 1rem;
    text-transform: uppercase;
    letter-spacing: 0.5px;
    font-size: 0.9rem;
    }

    .tool-options label {
    display: flex;
    align-items: center;
    gap: 8px;
    padding: 8px 10px;
    margin-bottom: 6px;
    cursor: pointer;
    border: 1px solid #d1d5db;
    border-radius: 6px;
    transition: all 0.2s ease;
    background: #ffffff;
    font-size: 0.9rem;
    }

    .tool-options input[type='radio'] {
    accent-color: #3b82f6;
    }

    .tool-options label:hover {
    background: #eef2ff;
    border-color: #6366f1;
    transform: translateX(3px);
    }

    .tool-options input:checked + span,
    .tool-options input:checked {
    background-color: #3b82f6;
    color: white;
    border-color: #3b82f6;
    }
</style>