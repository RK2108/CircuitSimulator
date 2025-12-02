<template>
    <div class="container">
        <Pallete @select="tool = $event"/>
        <Canvas ref="canvas"></Canvas>
        <DataPanel/>
    </div>
</template>

<script setup>
    import Pallete from '@/components/Pallete.vue';
    import Canvas from '@/components/Canvas.vue';
    import DataPanel from '@/components/DataPanel.vue';
    import { ref, watch } from 'vue';

    const tool = ref(null);
    const canvas = ref(null);

    watch (tool, (newVal) => {
        if (canvas.value){
            canvas.value.selectedTool = newVal;
        }
    });

</script>

<style scoped>
    .container {
        color: #111827;
        font-family: 'Inter', sans-serif;
        display: flex;
        gap: 0;
    }
</style>