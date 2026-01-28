<template>
    <div class="container" @click.self="emit('close')">
        <div class="modal">
            <div class="info">
                <h1>Information to assist you:</h1>
                <h3>Component Related</h3>
                    <p>1. Use the buttons on the left to choose the type of component</p>
                    <p>2. For connecting with wires, right click on the components</p>
                    <p>3. For deleting, select the delete button on the left and click</p>
                <h3>Simulating Circuits</h3>
                    <p>1. Right click on components to edit values</p>
                    <p>2. Click the Simulate button to simulate the circuit</p>
                    <p>3. For a simulation to work, there needs to be at least 3 components and all should have values</p>
                <h3>Saving Circuits</h3>
                    <p>1. Click the Save button to save the current state of the circuit</p>
                    <p>2. These circuits can later be re-opened by going back to circuit selection</p>
            </div>
            <button class="close" @click="emit('close')">Close</button>
        </div>
    </div>
</template>

<script setup>

    const emit = defineEmits(['close'])

</script>

<style scoped>
    .container {
        position: fixed;
        inset: 0;
        background: rgba(0, 0, 0, 0.4);
        display: grid;
        place-items: center;
    }


    .modal {
        background: white;
        padding: 1rem;
        border-radius: 8px;
        min-width: 300px;
    }


    .close {
        margin-top: 1rem;
    }

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