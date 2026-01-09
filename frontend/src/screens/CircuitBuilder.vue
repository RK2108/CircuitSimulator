<template>
    <div class="container">
        <Pallete @select="tool = $event"/>
        <Canvas @component="comp = $event" ref="canvas"></Canvas>
        <DataPanel ref="data"/>
    </div>
</template>

<script setup>
    import Pallete from '@/components/Pallete.vue';
    import Canvas from '@/components/Canvas.vue';
    import DataPanel from '@/components/DataPanel.vue';
    import { ref, watch } from 'vue';

    const tool = ref(null);
    const comp = ref(null);
    const canvas = ref(null);
    const data = ref(null);

    watch (tool, (newVal) => {
        if (canvas.value){
            canvas.value.selectedTool = newVal;
        }
    });

    watch (comp, (newVal) => {
        if (data.value){
            data.value.comp = newVal;
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