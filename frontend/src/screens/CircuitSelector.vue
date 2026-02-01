<template>
    <v-app>
        <v-app-bar color="primary" dark>
            <v-toolbar-title>Circuit Simulator</v-toolbar-title>
            <v-spacer></v-spacer>
            <v-btn @click="router.push('/builder');" prepend-icon="mdi-plus-circle">
                New Circuit
            </v-btn>
        </v-app-bar>

        <v-main>
            <v-container>
                <CircuitList/>
            </v-container>
        </v-main>
  </v-app>
</template>

<script setup>
    import CircuitList from '@/components/CircuitList.vue';
    import { useRouter } from 'vue-router';

    const router = useRouter();
    
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