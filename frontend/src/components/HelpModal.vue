<template>
    <v-dialog :model-value="true" @update:model-value="$emit('close')" max-width="600">
        <v-card>
        <v-card-title class="d-flex justify-space-between align-center">
            <span>Help & Instructions</span>
            <v-btn icon variant="text" @click="$emit('close')">
            <v-icon>mdi-close</v-icon>
            </v-btn>
        </v-card-title>

        <v-divider></v-divider>

        <v-card-text>
            <v-list>
            <v-list-subheader>CONTROLS</v-list-subheader>

            <v-list-item>
                <template v-slot:prepend>
                <v-icon color="primary">mdi-cursor-default</v-icon>
                </template>
                <v-list-item-title>Left Click</v-list-item-title>
                <v-list-item-subtitle>Place component or delete (if delete tool selected)</v-list-item-subtitle>
            </v-list-item>

            <v-list-item>
                <template v-slot:prepend>
                <v-icon color="primary">mdi-cursor-default-click</v-icon>
                </template>
                <v-list-item-title>Right Click</v-list-item-title>
                <v-list-item-subtitle>Connect components with wires</v-list-item-subtitle>
            </v-list-item>
            </v-list>
        </v-card-text>

        <v-divider></v-divider>

        <v-card-actions>
            <v-spacer></v-spacer>
            <v-btn color="primary" @click="$emit('close')">Got it!</v-btn>
        </v-card-actions>
        </v-card>
  </v-dialog>
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