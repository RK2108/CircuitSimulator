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
import Pallete from '@/components/Pallete.vue';
import DataPanel from '@/components/DataPanel.vue';

    const tool = ref(null);
    const canvas = ref(null);

    watch (tool, (newVal) => {
        if (canvas.value){
            canvas.value.selectedTool = newVal;
        }
    });

</script>

<style scoped>
</style>