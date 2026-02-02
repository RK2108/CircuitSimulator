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
</style>