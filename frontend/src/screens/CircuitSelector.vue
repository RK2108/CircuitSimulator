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
</style>